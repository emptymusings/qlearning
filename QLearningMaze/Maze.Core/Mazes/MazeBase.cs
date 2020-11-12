using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace QLearningMaze.Core.Mazes
{
    public enum Actions
    {
        Stay = 0,
        MoveUp = 1,
        MoveRight = 2,
        MoveDown = 3,
        MoveLeft = 4,
        GetCustomReward = 5
    }

    public abstract partial class MazeBase : IMaze
    {
        protected Random _random = new Random();
        protected int _goalValue = 200;
        protected double _movementValue = -1;
        protected double _start_decay = 1;
        protected double _end_decay;
        protected double _epsilon_decay_value;
        protected bool _observationSpaceInitialized = false;
        protected int _numberOfActions = 5;
        protected int _backtrackPunishment = -3;
        private List<AdditionalReward> _tempAdditionalRewards;
        
        protected int _additionalRewardsReceived = 0;

        public MazeBase(
            int rows,
            int columns,
            int startPosition,
            int goalPosition,
            double discountRate,
            double learningRate,
            int maxEpisodes)
        {
            this.Rows = rows;
            this.Columns = columns;
            this.StartPosition = startPosition;
            this.GoalPosition = goalPosition;
            this.DiscountRate = discountRate;
            this.LearningRate = learningRate;
            this.MaxEpisodes = maxEpisodes;
        }

        /// <summary>
        /// Gets the total number of states for the Q-Table (Rows * Columns * Additional Rewards + 1/custom reward for whether it's been picked up)
        /// </summary>
        public int NumberOfStates 
        { 
            get
            {
                return Rows * Columns * (AdditionalRewards.Count > 0 ? AdditionalRewards.Count + 1 : 1);
            }
        }

        public int TotalSpaces
        {
            get
            {
                return Rows * Columns;
            }
        }
        /// <summary>
        /// Gets or Sets the number of rows in the maze
        /// </summary>
        public int Rows { get; set; } = 4;
        /// <summary>
        /// Gets or Sets the number of columns in the maze 
        /// </summary>
        public int Columns { get; set; } = 4;
        /// <summary>
        /// Gets or Sets the Exit/Goal of the Maze (win condition)
        /// </summary>
        public int GoalPosition { get; set; } = 0;
        /// <summary>
        /// Gets or Sets the Agent's start position
        /// </summary>
        public int StartPosition { get; set; } = 0;
        /// <summary>
        /// (aka Gamma) Decimal value between 0 and 1 that determines how much long term reward is weighted vs immediate.  Higher values reflect more regard to long term rewards
        /// </summary>
        public double DiscountRate { get; set; } = 0.5;
        /// <summary>
        /// (aka Alpha) Determines to what extent newly acquired information overrides old information. A factor of 0 makes the agent learn nothing (exclusively exploiting prior knowledge),
        /// while a factor of 1 makes the agent consider only the most recent information (ignoring prior knowledge to explore possibilities).
        /// </summary>
        public double LearningRate { get; set; } = 0.5;
        /// <summary>
        /// Gets or Sets the total number of episodes (or episodes) to train for
        /// </summary>
        public int MaxEpisodes { get; set; } = 1000;
        /// <summary>
        /// Observation Space, where the first dimension is state, the second is actions, 
        /// and the value is binary (0 or 1) to determine if the action is allowed
        /// </summary>
        public int[][] ObservationSpace { get; set; }
        /// <summary>
        /// Rewards table, where the first dimension is state, the second is movement to the next state (action), and the value is the reward
        /// </summary>
        public double[][] Rewards { get; set; }
        /// <summary>
        /// The Q-Table, where the first dimesion is state, the second is movement to the next state (action), and the value is the result of the quality algorithm
        /// </summary>
        public double[][] Quality { get; set; }
        /// <summary>
        /// Gets or Sets a list of obstructions (walls) to avoid 
        /// </summary>
        public List<MazeObstruction> Obstructions { get; set; } = new List<MazeObstruction>();
        /// <summary>
        /// Gets or Sets any custom rewards
        /// </summary>
        public virtual List<AdditionalReward> AdditionalRewards { get; set; } = new List<AdditionalReward>();
        /// <summary>
        /// Gets the total rewards received during a training or maze run session
        /// </summary>
        public double TotalRewards { get; set; }
        /// <summary>
        /// Gets or Sets the episode interval between saving quality and run information during training
        /// </summary>
        public int SaveEpisodes { get; set; } = 100;
        /// <summary>
        /// Gets the count of actions available for the observation space
        /// </summary>
        protected virtual int GetActionsCount()
        {
            return Enum.GetNames(typeof(Actions)).Length;
        }

        /// <summary>
        /// Gets the maze position, regardless of rewards collected, in the state space
        /// </summary>
        /// <param name="state">The state to be evaluated</param>
        protected virtual int GetPosition(int state)
        {
            return state % TotalSpaces;
        }

        /// <summary>
        /// Creates the maze matrix.  Anything with a value of 1 indicates the ability of free movement between 2 spaces (needs to be assigned bi-directionally).  A value of 0 (zero)
        /// indicates a blocked path (also bi-directional)
        /// </summary>
        protected virtual void CreateObservationSpace()
        {
            OnObservationSpaceCreatingEventHandler();

            Console.WriteLine("Creating Maze States (Observation Space)");

            int[][] observationSpaceActions = new int[NumberOfStates][];

            for (int i = 0; i < NumberOfStates; ++i)
            {
                observationSpaceActions[i] = new int[GetActionsCount()];

                int position = GetPosition(i);

                if (position >= Columns) // Can't move up unless we're in at least the second row
                {
                    observationSpaceActions[i][(int)Actions.MoveUp] = 1;

                    if (position < TotalSpaces - Columns)  // Can't move down if we're in the last row
                    {
                        observationSpaceActions[i][(int)Actions.MoveDown] = 1;
                    }
                }
                else
                {
                    observationSpaceActions[i][(int)Actions.MoveDown] = 1;  // Can move down from the first row
                }

                if (position < TotalSpaces - 1 &&
                    (position + 1) % Columns != 0)  // Ensure we're not on the right edge of the maze
                {
                    observationSpaceActions[i][(int)Actions.MoveRight] = 1;
                }

                if (position > 0 &&
                    position % Columns != 0)        // Ensure we're not on the left edge of the maze
                {
                    observationSpaceActions[i][(int)Actions.MoveLeft] = 1;
                }

                // Only allow the path to the goal in the last possible state
                // TEMP - Changing this so that access to the goal is allowed from any state, hitting goal takes priority over getting rewards
                if (position == GoalPosition) 
                    //&& i + TotalSpaces > NumberOfStates)       // Don't allow movement out of the goal
                {
                    observationSpaceActions[i][(int)Actions.Stay] = 1;
                    observationSpaceActions[i][(int)Actions.MoveUp] = 0;
                    observationSpaceActions[i][(int)Actions.MoveRight] = 0;
                    observationSpaceActions[i][(int)Actions.MoveDown] = 0;
                    observationSpaceActions[i][(int)Actions.MoveLeft] = 0;
                }

            }

            OnObservationSpaceCreatedEventhHandler();
            
            ObservationSpace = observationSpaceActions;
            _observationSpaceInitialized = true;

            foreach (var wall in Obstructions)
            {
                AddWall(wall.BetweenSpace, wall.AndSpace);
            }
        }

        protected virtual void CreateRewards()
        {
            Console.WriteLine("Creating Reward States");
            double[][] reward = new double[NumberOfStates][];
            int rewardPriority = 0;
            List<int> assignedRewards = new List<int>();

            if (NumberOfStates != ObservationSpace.Length)
            {
                CreateObservationSpace();
            }

            // Order custom rewards.  This will be necessary to assign position
            var orderedRewards = AdditionalRewards
                .OrderByDescending(proximity => (
                    (proximity.Position > StartPosition ? proximity.Position - StartPosition : StartPosition - proximity.Position) * _movementValue) + proximity.Value
                );

            for (int i = 0; i < NumberOfStates; ++i)
            {
                reward[i] = new double[ObservationSpace[i].Length];
                
                int position = GetPosition(i);

                for (int j = 0; j < ObservationSpace[i].Length; ++j)
                {
                    
                    var customReward = orderedRewards.Where(r => r.Position == GetPosition(i)).FirstOrDefault();

                    if (ObservationSpace[i][j] > 0 || customReward != null)
                    {
                        if (position == GoalPosition && i + TotalSpaces > NumberOfStates)
                        {
                            reward[i][j] = _goalValue;
                        }                        
                        else
                        {                            
                            if (j != (int)Actions.GetCustomReward)
                            {
                                reward[i][j] = _movementValue;
                            }
                        }
                    }
                }
            }
            
            foreach (var prioritizedReward in orderedRewards)
            {
                ObservationSpace[prioritizedReward.Position + rewardPriority][(int)Actions.GetCustomReward] = 1;
                reward[prioritizedReward.Position + rewardPriority][(int)Actions.GetCustomReward] = prioritizedReward.Value;
                rewardPriority += TotalSpaces;
            }

            Rewards = reward;
            OnRewardsCreated();
        }

        public virtual void AddCustomReward(int position, double reward)
        {
            // Determine if the reward already exists
            var exists = AdditionalRewards.Where(x => x.Position == GetPosition(position)).FirstOrDefault();

            if (exists != null)
            {
                // Is the value the same, too?  If so, this is a redundant action
                if (exists.Value == reward)
                    return;

                // Assign the new value
                exists.Value = reward;
            }
            else
            {
                AdditionalRewards.Add(new AdditionalReward {  Position = position, Value = reward });

                // Since we've just changed the number of possible states, recreate the Observation Space and Rewards table
                CreateObservationSpace();
            }

        }

        public virtual void RemoveCustomReward(int position)
        {
            var exists = AdditionalRewards.Where(x => x.Position == GetPosition(position)).FirstOrDefault();

            if (exists == null) return;

            AdditionalRewards.Remove(exists);
        }

        public IEnumerable<AdditionalReward> GetAdditionalRewards()
        {
            return this.AdditionalRewards;
        }

        /// <summary>
        /// Creates the Q-Table matrix
        /// </summary>
        protected virtual void InitializeQualityTable()
        {
            Console.WriteLine("Creating Quality of States");

            double[][] quality = new double[NumberOfStates][];

            for (int i = 0; i < NumberOfStates; ++i)
            {
                quality[i] = new double[ObservationSpace[i].Length];
            }
            
            Quality = quality;
            OnQualityCreated();
        }

        public virtual void AddWall(int betweenSpace, int andSpace)
        {
            if (!_observationSpaceInitialized)
            {
                CreateObservationSpace();
            }

            var actions = GetActionBetweenSpaces(betweenSpace, andSpace);
            var action = actions.action;
            var reverseAction = actions.reverseAction;

            ObservationSpace[betweenSpace][action] = 0;
            ObservationSpace[andSpace][reverseAction] = 0;

            int addedBetween = betweenSpace + TotalSpaces;
            int addedAnd = andSpace + TotalSpaces;

            while (addedBetween < NumberOfStates)
            {
                ObservationSpace[addedBetween][action] = 0;
                ObservationSpace[addedAnd][reverseAction] = 0;

                addedBetween += TotalSpaces;
                addedAnd += TotalSpaces;
            }            

            var wall = GetObstructionFromList(betweenSpace, andSpace);

            if (wall == null)
            {
                var obstruction = new MazeObstruction
                {
                    BetweenSpace = betweenSpace,
                    AndSpace = andSpace
                };

                Obstructions.Add(obstruction);
                OnObstructionAdded(obstruction);
            }                
        }

        public virtual void RemoveWall(int betweenSpace, int andSpace)
        {
            if (!_observationSpaceInitialized)
            {
                CreateObservationSpace();
            }

            var actions = GetActionBetweenSpaces(betweenSpace, andSpace);
            var action = actions.action;
            var reverseAction = actions.reverseAction;

            ObservationSpace[betweenSpace][action] = 1;
            ObservationSpace[andSpace][reverseAction] = 1;

            int addedBetween = betweenSpace + TotalSpaces;
            int addedAnd = andSpace + TotalSpaces;

            while (addedBetween < NumberOfStates)
            {
                ObservationSpace[addedBetween][action] = 1;
                ObservationSpace[addedAnd][reverseAction] = 1;

                addedBetween += TotalSpaces;
                addedAnd += TotalSpaces;
            }

            var wall = GetObstructionFromList(betweenSpace, andSpace);

            if (wall != null)
            {
                Obstructions.Remove(wall);
                OnObstructionRemoved(wall);
            }
        }

        protected virtual (int action, int reverseAction) GetActionBetweenSpaces(int betweenSpace, int andSpace)
        {
            int differential = betweenSpace - andSpace;
            int action = 0;
            int reverseAction = 0;

            if (differential == 1)
            {
                action = (int)Actions.MoveLeft;
                reverseAction = (int)Actions.MoveRight;
            }

            if (differential == -1)
            {
                action = (int)Actions.MoveRight;
                reverseAction = (int)Actions.MoveLeft;
            }

            if (differential == Columns * -1)
            {
                action = (int)Actions.MoveDown;
                reverseAction = (int)Actions.MoveUp;
            }

            if (differential == Columns)
            {
                action = (int)Actions.MoveUp;
                reverseAction = (int)Actions.MoveDown;
            }

            return (action, reverseAction);
        }

        protected virtual MazeObstruction GetObstructionFromList(int betweenSpace, int andSpace)
        {
            return Obstructions.Where(x => (x.BetweenSpace == betweenSpace && x.AndSpace == andSpace) || (x.AndSpace == betweenSpace && x.BetweenSpace == andSpace)).FirstOrDefault();
        }

        protected virtual List<int> GetPossibleNextActions(int currentState)
        {
            List<int> result = new List<int>();
            int actionCount = GetActionsCount();

            for (int i = 0; i < actionCount; ++i)
            {
                if (ObservationSpace[currentState][i] > 0)
                {
                    result.Add(i);
                }
            }

            return result;
        }

        protected virtual int GetRandomNextAction(int currentState)
        {
            var position = GetPosition(currentState);
            List<int> possibleNextStates = GetPossibleNextActions(currentState);

            int count = possibleNextStates.Count;
            int index = _random.Next(0, count);

            if (possibleNextStates.Count > 0)
                return possibleNextStates[index];
            else
                return -1;
        }

        public virtual void Train()
        {            
            int moves;

            CreateObservationSpace();
            CreateRewards();
            InitializeQualityTable();

            if (!Directory.Exists(MazeUtilities.TRAINING_SESSIONS_DIRECTORY))
            {
                Directory.CreateDirectory(MazeUtilities.TRAINING_SESSIONS_DIRECTORY);
            }

            foreach(var file in Directory.GetFiles(MazeUtilities.TRAINING_SESSIONS_DIRECTORY))
            {
                File.Delete(file);
            }

            double epsilon = 1;
            _end_decay = (int)MaxEpisodes / 2;
            _epsilon_decay_value = epsilon / (_end_decay - _start_decay);

            Console.WriteLine("Please wait while I learn the maze");
            
            OnTrainingStatusChanged(true);
            
            for (int episode = 0; episode < MaxEpisodes; ++episode)
            {

                moves = 0;
                TotalRewards = 0;
                Console.Write($"Runnging through episode {(episode + 1).ToString("#,##0")} of {MaxEpisodes.ToString("#,##0")}\r");

                int currentState = _random.Next(0, NumberOfStates);
                //int currState = StartPosition;
                bool done = false;

                while (!done)
                {
                    moves++;
                    
                    int nextAction = TrainingDetermineNextAction(currentState, epsilon);   

                    if (nextAction < 0)
                    {
                        // Something went wrong, or the agent can't move (walled in), and an invalid next state was returned, start over with a new random current state
                        currentState = _random.Next(0, Rewards.Length);
                        continue;
                    }

                    CalculateQValue(currentState, nextAction);

                    currentState = GetNextState(currentState, nextAction);

                    if (GetPosition(currentState) == GoalPosition ||
                        moves > 1000)
                    {
                        done = true;
                    }
                }

                if (_end_decay >= episode &&
                    episode >= _start_decay)
                {
                    epsilon -= _epsilon_decay_value;
                }

                int currentPosition = GetPosition(currentState);

                if (currentPosition == GoalPosition && 
                    (1 + episode) % SaveEpisodes == 0 &&
                    episode > 0)
                {
                    SaveQuality(episode + 1, moves);
                }

                OnTrainingEpisodeCompleted(new TrainingEpisodeCompletedEventArgs(episode + 1, MaxEpisodes, moves, TotalRewards, currentPosition == GoalPosition));
            }

            OnTrainingStatusChanged(false);
            
            CreateObservationSpace();
            CreateRewards();
            //Quality = CopyQuality(bestQuality);
            Console.WriteLine();
            Console.WriteLine("I'm done learning");
        }

        private void SaveQuality(int episode, int moves)
        {
            MazeUtilities.SaveObject($@"{MazeUtilities.TRAINING_SESSIONS_DIRECTORY}\Episode_{episode}_{DateTime.Now.ToString("HH_mm_ss")}.json", new TrainingSession
            {
                Episode = episode,
                Moves = moves,
                Score = TotalRewards,
                Quality = Quality
            });
        }

        protected virtual int TrainingDetermineNextAction(int currentState, double epsilon)
        {
            double randRand = _random.NextDouble();
            int nextAction = -1;

            if (randRand > epsilon)
            {
                int preferredNext = -1;
                double max = double.MinValue;

                for (int i = 0; i < this.Quality[currentState].Length; ++i)
                {
                    if (this.Quality[currentState][i] > max &&
                        Quality[currentState][i] != 0)
                    {
                        max = this.Quality[currentState][i];
                        preferredNext = i;
                    }
                }

                if (preferredNext >= 0)
                {
                    nextAction = preferredNext;
                }
            }
            else
            {
                nextAction = GetRandomNextAction(currentState);
            }

            while (nextAction < 0)
                nextAction = GetRandomNextAction(currentState);

            return nextAction;
        }

        protected virtual void CalculateQValue(int currentState, int action)
        {
            double maxQ = double.MinValue;
            bool isBackTrack = false;
            int nextState = GetNextState(currentState, action);
            List<int> possNextNextActions = GetPossibleNextActions(nextState);
            double oldtempQuality = Quality[currentState][action];
            int selectedNextPosition = -1;

            for (int j = 0; j < possNextNextActions.Count; ++j)
            {
                int futureNextAction = possNextNextActions[j];  // short alias

                double futureQuality = Quality[nextState][futureNextAction];
                int currentPosition = GetPosition(currentState);
                int nextPosition = GetNextState(nextState, futureNextAction);
                nextPosition = GetPosition(nextPosition);

                if (futureQuality > maxQ)
                {
                    maxQ = futureQuality;
                    selectedNextPosition = nextPosition;
                }                
            }

            if (GetPosition(currentState) == selectedNextPosition)
            {
                isBackTrack = true;
            }

            var r = Rewards[currentState][action] + (isBackTrack ? _backtrackPunishment : 0);
            var p = GetPosition(currentState);

            //TotalRewards = Rewards[currentState][action] + (isBackTrack ? (AdditionalRewards.Count > 0 ? GetAdditionalRewards().Max(x => x.Value) : 0) - _backtrackPunishment : 0);
            //TotalRewards = Rewards[currentState][action];
            
            Quality[currentState][action] = ((1 - LearningRate) * Quality[currentState][action]) + (LearningRate * (r + (DiscountRate * maxQ)));
            TotalRewards += r;
            isBackTrack = false;
            OnTrainingAgentStateChanging(action, currentState, GetPosition(currentState), oldtempQuality, Quality[currentState][action]);
        }

        public virtual int GetNextState(int currentState, int action, bool isTraining = true)
        {
            return GetNextState(currentState, (Actions)action, isTraining);
        }

        public virtual int GetNextState(int currentState, Actions action, bool isTraining)
        {
            switch(action)
            {
                case Actions.MoveUp:
                    return currentState - Columns;
                case Actions.MoveRight:
                    return currentState + 1;
                case Actions.MoveDown:
                    return currentState + Columns;
                case Actions.MoveLeft:
                    return currentState - 1;
                case Actions.Stay:
                    return currentState;
                case Actions.GetCustomReward:
                    return GetRewardState(currentState, isTraining);
                default:
                    return -1;
            }
        }

        private int GetRewardState(int currentState, bool isTraining)
        {
            if (currentState + TotalSpaces > NumberOfStates)
            {
                ObservationSpace[currentState][(int)Actions.GetCustomReward] = 0;
                return currentState;
            }
            int leveled = currentState + TotalSpaces;

            while (leveled < NumberOfStates)
            {
                ObservationSpace[leveled][(int)Actions.GetCustomReward] = 0;
                leveled += TotalSpaces;
            }

            return (currentState + TotalSpaces > NumberOfStates ? currentState : currentState + TotalSpaces);

        }

        
        public virtual void RunMaze()
        {
            int curr = StartPosition; 
            int action;
            int nextState;
            int moves = 0;
            int previousState = -1;
            int backtrackTimes = 0;
            TotalRewards = 0;
            
            while (GetPosition(curr) != GoalPosition)
            {
                if (Quality == null)
                    Train();

                action = GetBestAction(curr, previousState, ref backtrackTimes);

                if (ObservationSpace[curr][action] < 1)
                {
                    string message = "I guess I didn't learn very  well, I just tried an illegal move.  Check the learning rate and discount rate and try again";
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine(message);
                    throw new InvalidOperationException(message);
                }
                
                TotalRewards += Rewards[curr][action];
                moves++;
                Console.Write(action + "->");
                nextState = GetNextState(curr, action);

                int nextPosition = GetPosition(nextState);
                int previousPosition = GetPosition(previousState);
                int currentPosition = GetPosition(curr);

                if (((nextPosition == currentPosition ||
                    nextPosition == previousPosition) &&
                    backtrackTimes > 3) ||
                    moves > NumberOfStates) // This condition should never happen, but sometimes I make bugs
                {

                    string message = "I'm a greedy idiot, and am backtracking to get more rewards.  Try adjusting the Discount and/or Learning Rate to stop me from doing this.";
                    throw new InvalidOperationException(message);
                }


                previousState = curr;
                curr = nextState;
                OnAgentStateChanged(GetPosition(curr), curr, TotalRewards, moves);
            }

            OnAgentCompletedMaze(new AgentCompletedMazeEventArgs(moves, TotalRewards));

            Console.WriteLine("done");
        }

        protected virtual int GetBestAction(int currentState, int previousState, ref int backtrackTimes)
        {
            int action = GetArgMax(currentState, (ObservationSpace[currentState][(int)Actions.GetCustomReward] == 0 ? (int)Actions.GetCustomReward : -1));
            int nextPosition = GetPosition(GetNextState(currentState, action, false));
            int previousPosition = GetPosition(previousState);

            if (nextPosition == previousPosition)
            {
                // We'll allow a singular backtrack, but anything more will cause the agent to select
                // a different action to prevent infinitely going back and forth
                if (backtrackTimes > 1)
                {
                    action = GetArgMax(currentState, action);
                    backtrackTimes = 0;
                }
                else
                {
                    backtrackTimes++;
                }
            }
            else
            {
                // We've moved to a new state, so reset the backtrack counter
                backtrackTimes = 0;
            }

            return action;
        }

        protected virtual int GetArgMax(int currentState, int excludeAction = -1)
        {
            double[] vector = Quality[currentState];
            double maxVal = double.MinValue; 
            int idx = 0;

            for (int i = 0; i < vector.Length; ++i)
            {
                if (i == excludeAction) 
                    continue;

                if (vector[i] != 0 && 
                    vector[i] > maxVal)
                    //&&
                    //ObservationSpace[currentState][i] != 0
                {
                    maxVal = vector[i]; 
                    idx = i;
                }
            }

            return idx;
        }

    }
}
