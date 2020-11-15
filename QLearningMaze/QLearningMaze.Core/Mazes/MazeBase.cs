using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
        protected double _movementValue = -1;
        protected double _start_decay = 1;
        protected double _end_decay;
        protected double _epsilon_decay_value;
        protected bool _observationSpaceInitialized = false;
        protected int _numberOfActions = 5;
        protected int _backtrackPunishment = -3;
        
        protected int _additionalRewardsReceived = 0;

        public MazeBase(
            int rows,
            int columns,
            int startPosition,
            int goalPosition,
            double discountRate,
            double learningRate,
            int maxEpisodes,
            double goalReward = 200)
        {
            this.Rows = rows;
            this.Columns = columns;
            this.StartPosition = startPosition;
            this.GoalPosition = goalPosition;
            this.DiscountRate = discountRate;
            this.LearningRate = learningRate;
            this.MaxEpisodes = maxEpisodes;
            this.GoalReward = goalReward;
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
        /// <summary>
        /// The total number of spaces in the maze (Rows * Columns)
        /// </summary>
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
        /// Gets or Sets the reward for reaching the final objective
        /// </summary>
        public double GoalReward { get; set; } = 200;
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
        public int SaveQualityFrequency { get; set; } = 10;
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
        protected virtual void InitializeObservationSpace()
        {
            OnObservationSpaceCreating();

            Console.WriteLine("Creating Maze States (Observation Space)");

            ObservationSpace = new int[NumberOfStates][];

            for (int i = 0; i < NumberOfStates; ++i)
            {
                ObservationSpace[i] = new int[GetActionsCount()];

                int position = GetPosition(i);

                SetObservationSpaceStateActions(i, position);

                // Only allow the path to the goal in the last possible state
                // TEMP - Changing this so that access to the goal is allowed from any state, hitting goal takes priority over getting rewards
                if (position == GoalPosition) 
                {
                    SetObservationSpaceGoalActions(i);
                }
            }

            _observationSpaceInitialized = true;

            SetObservationSpaceObstructions();

            OnObservationSpaceCreated();
        }

        /// <summary>
        /// Sets the available actions for a given state
        /// </summary>
        /// <param name="state">The state for which to set available actions</param>
        /// <param name="position">The phase position (state without phase)</param>
        protected virtual void SetObservationSpaceStateActions(int state, int position)
        {
            if (position >= Columns) // Can't move up unless we're in at least the second row
            {
                ObservationSpace[state][(int)Actions.MoveUp] = 1;

                if (position < TotalSpaces - Columns)  // Can't move down if we're in the last row
                {
                    ObservationSpace[state][(int)Actions.MoveDown] = 1;
                }
            }
            else
            {
                ObservationSpace[state][(int)Actions.MoveDown] = 1;  // Can move down from the first row
            }

            if (position < TotalSpaces - 1 &&
                (position + 1) % Columns != 0)  // Ensure we're not on the right edge of the maze
            {
                ObservationSpace[state][(int)Actions.MoveRight] = 1;
            }

            if (position > 0 &&
                position % Columns != 0)        // Ensure we're not on the left edge of the maze
            {
                ObservationSpace[state][(int)Actions.MoveLeft] = 1;
            }
        }

        /// <summary>
        /// Sets the actions for the goal position
        /// </summary>
        /// <param name="state"></param>
        protected virtual void SetObservationSpaceGoalActions(int state)
        {
            ObservationSpace[state][(int)Actions.Stay] = 1;
            ObservationSpace[state][(int)Actions.MoveUp] = 0;
            ObservationSpace[state][(int)Actions.MoveRight] = 0;
            ObservationSpace[state][(int)Actions.MoveDown] = 0;
            ObservationSpace[state][(int)Actions.MoveLeft] = 0;
        }

        /// <summary>
        /// Sets the actions for the goal position(s) for each phase
        /// </summary>
        protected virtual void SetObservationSpaceObstructions()
        {
            foreach (var wall in Obstructions)
            {
                AddObstruction(wall.BetweenSpace, wall.AndSpace);
            }
        }

        /// <summary>
        /// Initializes the Rewards table
        /// </summary>
        protected virtual void InitializeRewards()
        {
            Console.WriteLine("Creating Reward States");
            List<int> assignedRewards = new List<int>();

            Rewards = new double[NumberOfStates][];

            if (NumberOfStates != ObservationSpace.Length)
            {
                InitializeObservationSpace();
            }

            // Order custom rewards.  This will be necessary to assign reward value to a single state to prevent improper/repeated assignment 
            // of quality in the Q-Table
            var orderedRewards = GetPrioritizedRewards();

            ApplyGeneralRewardValues(orderedRewards);
            ApplyCustomRewardValues(orderedRewards);
            
            OnRewardsCreated();
        }

        /// <summary>
        /// Prioritize custom rewards
        /// </summary>
        /// <returns></returns>
        protected virtual IOrderedEnumerable<AdditionalReward> GetPrioritizedRewards()
        {
            return AdditionalRewards
                .OrderByDescending(proximity => (
                    (proximity.Position > StartPosition ?
                        proximity.Position - StartPosition :
                        StartPosition - proximity.Position) * _movementValue) + proximity.Value);
        }

        /// <summary>
        /// Applies the values to rewards that are not in the custom rewards set
        /// </summary>
        /// <param name="prioritizedRewards">Custom rewards (to be ignored)</param>
        protected virtual void ApplyGeneralRewardValues(IOrderedEnumerable<AdditionalReward> prioritizedRewards)
        {
            for (int i = 0; i < NumberOfStates; ++i)
            {
                Rewards[i] = new double[ObservationSpace[i].Length];

                int position = GetPosition(i);

                for (int j = 0; j < ObservationSpace[i].Length; ++j)
                {

                    var customReward = prioritizedRewards.Where(r => r.Position == GetPosition(i)).FirstOrDefault();

                    if (ObservationSpace[i][j] > 0 || customReward != null)
                    {
                        if (position == GoalPosition && i + TotalSpaces > NumberOfStates)
                        {
                            Rewards[i][j] = GoalReward;
                        }
                        else
                        {
                            if (j != (int)Actions.GetCustomReward)
                            {
                                Rewards[i][j] = _movementValue;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Applies the values to custom rewards
        /// </summary>
        /// <param name="prioritizedRewards">The prioritized customer rewards</param>
        protected virtual void ApplyCustomRewardValues(IOrderedEnumerable<AdditionalReward> prioritizedRewards)
        {
            int rewardPriority = 0;

            foreach (var prioritizedReward in prioritizedRewards)
            {
                if (prioritizedReward.Value >= 0)
                {
                    ObservationSpace[prioritizedReward.Position + rewardPriority][(int)Actions.GetCustomReward] = 1;
                    Rewards[prioritizedReward.Position + rewardPriority][(int)Actions.GetCustomReward] = prioritizedReward.Value;
                    rewardPriority += TotalSpaces;
                }
                else
                {
                    var phase = prioritizedReward.Position;

                    while (phase < NumberOfStates)
                    {
                        double[] rs = Rewards[phase];

                        for (int i = 0; i < rs.Length; ++i)
                        {
                            Rewards[phase][i] = prioritizedReward.Value;
                        }

                        phase += TotalSpaces;
                    }
                }
            }
        }

        /// <summary>
        /// Adds a custom reward to the maze
        /// </summary>
        /// <param name="position">The position (state w/o phase) to which the reward will be added</param>
        /// <param name="reward">The value of the reward</param>
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
                InitializeObservationSpace();
            }

        }

        /// <summary>
        /// Removes a custom reward from the maze
        /// </summary>
        /// <param name="position">The position (state w/o phase) from which the reward will be removed</param>
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

        /// <summary>
        /// Adds an obsrecution between 2 spaces
        /// </summary>
        public virtual void AddObstruction(int betweenSpace, int andSpace)
        {
            if (!_observationSpaceInitialized)
            {
                InitializeObservationSpace();
            }

            var actions = GetActionBetweenSpaces(betweenSpace, andSpace);
            var action = actions.action;
            var reverseAction = actions.reverseAction;

            int phase = 0;

            while (phase < NumberOfStates)
            {
                ObservationSpace[betweenSpace + phase][action] = 0;
                ObservationSpace[andSpace + phase][reverseAction] = 0;

                phase += TotalSpaces;
            }            

            if (GetObstructionFromList(betweenSpace, andSpace) == null)
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

        /// <summary>
        /// Removes an obstruction between 2 spaces
        /// </summary>
        public virtual void RemoveObstruction(int betweenSpace, int andSpace)
        {
            if (!_observationSpaceInitialized)
            {
                InitializeObservationSpace();
            }

            var actions = GetActionBetweenSpaces(betweenSpace, andSpace);
            var action = actions.action;
            var reverseAction = actions.reverseAction;

            int phase = 0;

            while (phase < NumberOfStates)
            {
                ObservationSpace[betweenSpace + phase][action] = 1;
                ObservationSpace[andSpace + phase][reverseAction] = 1;

                phase += TotalSpaces;
            }

            var wall = GetObstructionFromList(betweenSpace, andSpace);

            if (wall != null)
            {
                Obstructions.Remove(wall);
                OnObstructionRemoved(wall);
            }
        }

        /// <summary>
        /// Gets the actions that transport an agent between 2 spaces
        /// </summary>
        /// <returns> 
        /// action: The action to transport from betweenSpace to andSpace; 
        /// reverseAction: The action to transport from andSpace to betweenSpace
        /// </returns>
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

        /// <summary>
        /// Gets an obstruction between two spaces from the list of obstructions
        /// </summary>
        protected virtual MazeObstruction GetObstructionFromList(int betweenSpace, int andSpace)
        {
            return Obstructions.Where(x => (x.BetweenSpace == betweenSpace && x.AndSpace == andSpace) || (x.AndSpace == betweenSpace && x.BetweenSpace == andSpace)).FirstOrDefault();
        }

        /// <summary>
        /// Trains the agent to maneuver through the maze
        /// </summary>
        public virtual void Train()
        {
            InitializeObservationSpace();
            InitializeRewards();
            InitializeQualityTable();

            InitializeSaveFolder();

            Console.WriteLine("Please wait while I learn the maze");
            
            OnTrainingStatusChanged(true);

            DoTraining();

            OnTrainingStatusChanged(false);
            
            InitializeObservationSpace();
            InitializeRewards();
            Console.WriteLine();
            Console.WriteLine("I'm done learning");
        }

        /// <summary>
        /// Sets up the folder to which Quality tables will be saved during training
        /// </summary>
        protected virtual void InitializeSaveFolder()
        {
            if (!Directory.Exists(MazeUtilities.TRAINING_SESSIONS_DIRECTORY))
            {
                Directory.CreateDirectory(MazeUtilities.TRAINING_SESSIONS_DIRECTORY);
            }

            foreach (var file in Directory.GetFiles(MazeUtilities.TRAINING_SESSIONS_DIRECTORY))
            {
                File.Delete(file);
            }
        }

        /// <summary>
        /// Performs the training for the Q-Table
        /// </summary>
        protected virtual void DoTraining()
        {
            double epsilon = 1;
            _end_decay = (int)MaxEpisodes / 2;
            _epsilon_decay_value = epsilon / (_end_decay - _start_decay);

            for (int episode = 0; episode < MaxEpisodes; ++episode)
            {
                Console.Write($"Runnging through episode {(episode + 1).ToString("#,##0")} of {MaxEpisodes.ToString("#,##0")}\r");

                var episodeResults = RunTrainingEpisode(epsilon);
                var currentState = episodeResults.currentState;
                var moves = episodeResults.moves;

                epsilon = DecayEpsilon(episode, epsilon);

                if (GetPosition(currentState) == GoalPosition &&
                    (1 + episode) % SaveQualityFrequency == 0 &&
                    episode > 0)
                {
                    SaveQualityForEpisode(episode + 1, moves);
                }

                OnTrainingEpisodeCompleted(new TrainingEpisodeCompletedEventArgs(episode + 1, MaxEpisodes, moves, TotalRewards, GetPosition(currentState) == GoalPosition));
            }
        }

        /// <summary>
        /// Maneuvers the agent through a training episode
        /// </summary>
        /// <param name="epsilon">The "greedy strategy" epsilon value that will help determine whether to perform a random or a known action</param>
        protected virtual (int currentState, int moves) RunTrainingEpisode(double epsilon)
        {
            int moves = 0;
            int currentState = _random.Next(0, NumberOfStates);
            bool done = false;

            TotalRewards = 0;

            while (!done)
            {
                moves++;

                int nextAction = GetGreedyNextAction(currentState, epsilon);

                CalculateQValue(currentState, nextAction);

                currentState = GetNextState(currentState, nextAction);

                if (GetPosition(currentState) == GoalPosition ||
                    moves > 1000)
                {
                    if (GetPosition(currentState) == GoalPosition)
                    {
                        TotalRewards += GoalReward;
                    }

                    done = true;
                }
            }

            return (currentState, moves);
        }

        /// <summary>
        /// Saves the Q-Table for a run episode
        /// </summary>
        protected virtual void SaveQualityForEpisode(int episode, int moves)
        {
            MazeUtilities.SaveObject($@"{MazeUtilities.TRAINING_SESSIONS_DIRECTORY}\Episode_{episode}_{DateTime.Now.ToString("HH_mm_ss")}.json", new TrainingSession
            {
                Episode = episode,
                Moves = moves,
                Score = TotalRewards,
                Quality = Quality
            });
        }

        /// <summary>
        /// Decays the epsilon value so that as training progesses, known values will be more likely to be used
        /// </summary>
        protected virtual double DecayEpsilon(int episode, double epsilon)
        {
            if (_end_decay >= episode &&
                   episode >= _start_decay)
            {
                epsilon -= _epsilon_decay_value;
            }

            return epsilon;
        }

        /// <summary>
        /// Get the next action to take from the current state
        /// </summary>
        /// <param name="state">The state in which the agent currently resides</param>
        /// <param name="epsilon"></param>
        protected virtual int GetGreedyNextAction(int state, double epsilon)
        {
            double randRand = _random.NextDouble();
            int nextAction = -1;

            if (randRand > epsilon)
            {
                int preferredNext = GetPreferredNextAction(state);

                if (preferredNext >= 0)
                {
                    nextAction = preferredNext;
                }
            }

            while (nextAction < 0)
                nextAction = GetRandomNextAction(state);

            return nextAction;
        }

        /// <summary>
        /// Gets all possible actions available to the agent in its current state
        /// </summary>
        protected virtual List<int> GetPossibleNextActions(int state)
        {
            List<int> result = new List<int>();
            int actionCount = GetActionsCount();

            for (int i = 0; i < actionCount; ++i)
            {
                if (ObservationSpace[state][i] > 0)
                {
                    result.Add(i);
                }
            }

            return result;
        }

        /// <summary>
        /// Selects the agent's next action randomly based on its current state
        /// </summary>
        protected virtual int GetRandomNextAction(int state)
        {
            var position = GetPosition(state);
            List<int> possibleNextStates = GetPossibleNextActions(state);

            int count = possibleNextStates.Count;
            int index = _random.Next(0, count);

            if (possibleNextStates.Count > 0)
                return possibleNextStates[index];
            else
                throw new NullReferenceException($"There are no possible actions that can be taken from the position {GetPosition(state)}");
        }

        /// <summary>
        /// Selects the agent's next action based on the highest Q-Table's value for its current state
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        protected virtual int GetPreferredNextAction(int state)
        {
            int preferredNext = -1;
            double max = double.MinValue;

            for (int i = 0; i < this.Quality[state].Length; ++i)
            {
                if (this.Quality[state][i] > max &&
                    Quality[state][i] != 0)
                {
                    max = this.Quality[state][i];
                    preferredNext = i;
                }
            }

            return preferredNext;
        }

        /// <summary>
        /// Calculates the Quality for the agent based on its current state and chosen action
        /// </summary>
        protected virtual void CalculateQValue(int state, int action)
        {
            bool isBackTrack = false;
            int nextState = GetNextState(state, action);
            double oldtempQuality = Quality[state][action];

            var forecaster = GetFuturePositionMaxQ(nextState);
            var maxQ = forecaster.maxQ;
            var selectedNextPosition = forecaster.selectedNextPosition;

            if (GetPosition(state) == selectedNextPosition)
            {
                isBackTrack = true;
            }

            var r = Rewards[state][action] + (isBackTrack ? _backtrackPunishment : 0);
            var p = GetPosition(state);

            // I have found a couple of Q-Value formulas.  Aside form some rounding issues, this and the formula below it are identical
            Quality[state][action] = Quality[state][action] + (LearningRate * (r + (DiscountRate * maxQ) - Quality[state][action]));

            //double similarQFormula  = ((1 - LearningRate) * Quality[currentState][action]) + (LearningRate * (r + (DiscountRate * maxQ)));

            TotalRewards += r;

            OnTrainingAgentStateChanging(action, state, GetPosition(state), oldtempQuality, Quality[state][action]);
        }

        /// <summary>
        /// Gets the maximum quality for future position based on the agent's state
        /// </summary>
        protected virtual (int selectedNextPosition, double maxQ) GetFuturePositionMaxQ(int state)
        {
            double maxQ = double.MinValue;
            List<int> possNextNextActions = GetPossibleNextActions(state);
            int selectedNextPosition = -1;

            for (int j = 0; j < possNextNextActions.Count; ++j)
            {
                int futureNextAction = possNextNextActions[j];  // short alias

                double futureQuality = Quality[state][futureNextAction];
                int nextPosition = GetNextState(state, futureNextAction);
                nextPosition = GetPosition(nextPosition);

                if (futureQuality > maxQ)
                {
                    maxQ = futureQuality;
                    selectedNextPosition = nextPosition;
                }
            }

            return (selectedNextPosition, maxQ);
        }

        /// <summary>
        /// Gets the agent's next state based on a selected action
        /// </summary>
        /// <param name="state"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public virtual int GetNextState(int state, int action)
        {
            return GetNextState(state, (Actions)action);
        }

        /// <summary>
        /// Gets the agent's next state based on a selected action
        /// </summary>
        /// <param name="state"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public virtual int GetNextState(int state, Actions action)
        {
            switch(action)
            {
                case Actions.MoveUp:
                    return state - Columns;
                case Actions.MoveRight:
                    return state + 1;
                case Actions.MoveDown:
                    return state + Columns;
                case Actions.MoveLeft:
                    return state - 1;
                case Actions.Stay:
                    return state;
                case Actions.GetCustomReward:
                    return GetRewardState(state);
                default:
                    return -1;
            }
        }

        /// <summary>
        /// Gets the agent's next state after performing the GetCustomReward action
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        private int GetRewardState(int state)
        {
            if (state + TotalSpaces > NumberOfStates)
            {
                ObservationSpace[state][(int)Actions.GetCustomReward] = 0;
                return state;
            }

            int leveled = state + TotalSpaces;

            while (leveled < NumberOfStates)
            {
                ObservationSpace[leveled][(int)Actions.GetCustomReward] = 0;
                leveled += TotalSpaces;
            }

            return (state + TotalSpaces > NumberOfStates ? state : state + TotalSpaces);
        }

        /// <summary>
        /// Run through the maze using the current Quality Table
        /// </summary>
        public virtual void RunMaze()
        {
            int currentState = StartPosition; // Set the state to the start position
            int action;
            int nextState;
            int moves = 0;
            int previousState = -1;
            int backtrackTimes = 0;
            TotalRewards = 0;
            
            // Loop until the goal is reached
            while (GetPosition(currentState) != GoalPosition)
            {
                if (Quality == null)    // Ensure that the Quality Table is initialized
                    Train();

                // Get the best action for the current state, noting the previous state to prevent backtracks
                action = GetBestAction(currentState, previousState, ref backtrackTimes);

                // If the value of the state/action combo is not a 1, an illegal move was attempted
                if (ObservationSpace[currentState][action] < 1)
                {
                    string message = "I guess I didn't learn very  well, I just tried an illegal move.  Check the learning rate and discount rate and try again";                    
                    Console.WriteLine(message);
                    throw new InvalidOperationException(message);
                }
                
                // Update the rewards and the number of moves
                TotalRewards += Rewards[currentState][action];
                moves++;
                Console.Write($"State: {currentState} | Action: {(Actions)action} -> ");

                nextState = GetNextState(currentState, action);

                if (IsTooGreedy(currentState, nextState, previousState, moves, backtrackTimes)) // This condition should never happen, but sometimes I make bugs
                {
                    string message = "I'm a greedy idiot, and am backtracking to get more rewards.  Try adjusting the Discount and/or Learning Rate to stop me from doing this.";
                    throw new InvalidOperationException(message);
                }


                previousState = currentState;
                currentState = nextState;
                OnAgentStateChanged(GetPosition(currentState), currentState, TotalRewards, moves);
            }

            OnAgentCompletedMaze(new AgentCompletedMazeEventArgs(moves, TotalRewards + GoalReward));

            Console.WriteLine($"done (current state: {currentState})");
        }

        /// <summary>
        /// Checks for repetitious actions if the agent is being too greedy
        /// </summary>
        /// <param name="state">The state the agent is currently  in</param>
        /// <param name="nextState">The next state the agent will be in based on its selected action</param>
        /// <param name="previousState">The state the agent was in before it moved into the current state</param>
        /// <param name="moves">The total number of moves taken in the episode to get to the current state</param>
        /// <param name="backtrackTimes">The number of times that the agent has backtracked</param>
        /// <returns></returns>
        private bool IsTooGreedy(int state, int nextState, int previousState, int moves, int backtrackTimes)
        {
            int nextPosition = GetPosition(nextState);
            int previousPosition = GetPosition(previousState);
            int currentPosition = GetPosition(state);

            return ((nextPosition == currentPosition ||
                    nextPosition == previousPosition) &&
                    backtrackTimes > 3) ||
                    moves > NumberOfStates;
        }

        /// <summary>
        /// Gets the best action for a given state
        /// </summary>
        /// <param name="state">The state from which the available actions should be examined</param>
        /// <param name="previousState">The previous state, used to identify backtracking</param>
        /// <param name="backtrackTimes">The number of times to allow backtracking before omitting an action as a possiblity</param>
        /// <returns></returns>
        protected virtual int GetBestAction(int state, int previousState, ref int backtrackTimes)
        {
            int action = GetArgMax(state, (ObservationSpace[state][(int)Actions.GetCustomReward] == 0 ? (int)Actions.GetCustomReward : -1));
            int nextPosition = GetPosition(GetNextState(state, action));
            int previousPosition = GetPosition(previousState);

            if (nextPosition == previousPosition)
            {
                // We'll allow a singular backtrack, but anything more will cause the agent to select
                // a different action to prevent infinitely going back and forth
                if (backtrackTimes > 1)
                {
                    action = GetArgMax(state, action);
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

        /// <summary>
        /// Get the maximum Quality for a State
        /// </summary>
        /// <param name="state">The state for which to find the highest quality action</param>
        /// <param name="excludeAction">Any actions which should not be part of the results</param>
        protected virtual int GetArgMax(int state, int excludeAction = -1)
        {
            double[] vector = Quality[state];
            double maxVal = double.MinValue; 
            int idx = 0;

            for (int i = 0; i < vector.Length; ++i)
            {
                if (i == excludeAction) 
                    continue;

                if (vector[i] != 0 && 
                    vector[i] > maxVal)
                {
                    maxVal = vector[i]; 
                    idx = i;
                }
            }

            return idx;
        }

    }
}
