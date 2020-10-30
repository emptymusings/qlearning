using System;
using System.Collections.Generic;
using System.Linq;

namespace QLearningMaze.Core.Mazes
{
    public enum Actions
    {
        Stay = 0,
        MoveNorth = 1,
        MoveEast = 2,
        MoveSouth = 3,
        MoveWest = 4
    }

    public abstract partial class MazeBase : IMaze
    {
        protected Random _random = new Random();
        protected int _goalValue = 50;
        protected double _movementValue = -1;
        protected double _start_decay = 1;
        protected double _end_decay;
        protected double _epsilon_decay_value;
        protected bool _mazeInitialized = false;
        protected int _numberOfActions = 5;
        protected int _backtrackPunishment = 400;
        protected List<AdditionalReward> _additionalRewards = new List<AdditionalReward>();

        public MazeBase(
            int rows,
            int columns,
            int startPosition,
            int goalPosition,
            double discountRate,
            double learningRate,
            int maxEpochs)
        {
            this.Rows = rows;
            this.Columns = columns;
            this.StartPosition = startPosition;
            this.GoalPosition = goalPosition;
            this.DiscountRate = discountRate;
            this.LearningRate = learningRate;
            this.MaxEpochs = maxEpochs;
        }
        /// <summary>
        /// Gets the total number of states for the Q-Table (for the maze, this is the area)
        /// </summary>
        public int NumberOfStates 
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
        /// Decimal value between 0 and 1 that determines how much long term reward is weighted vs immediate.  Higher values reflect more regard to long term rewards
        /// </summary>
        public double DiscountRate { get; set; } = 0.5;
        /// <summary>
        /// Determines to what extent newly acquired information overrides old information. A factor of 0 makes the agent learn nothing (exclusively exploiting prior knowledge),
        /// while a factor of 1 makes the agent consider only the most recent information (ignoring prior knowledge to explore possibilities).
        /// </summary>
        public double LearningRate { get; set; } = 0.5;
        /// <summary>
        /// Gets or Sets the total number of epochs (or episodes) to train for
        /// </summary>
        public int MaxEpochs { get; set; } = 1000;
        /// <summary>
        /// Maze states, where the first dimension is state, the second is movement to the next state (action), 
        /// and the value is binary (0 or 1) to determine if the action is allowed
        /// </summary>
        public int[][] MazeStates { get; set; }
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

        public double TotalRewards { get; set; }


        /// <summary>
        /// Creates the maze matrix.  Anything with a value of 1 indicates the ability of free movement between 2 spaces (needs to be assigned bi-directionally).  A value of 0 (zero)
        /// indicates a blocked path (also bi-directional)
        /// </summary>
        protected virtual void CreateMazeStates()
        {
            OnMazeCreatingEventHandler();

            Console.WriteLine("Creating Maze States (Observation Space)");

            int[][] mazeStatesActions = new int[NumberOfStates][];

            for (int i = 0; i < NumberOfStates; ++i)
            {
                mazeStatesActions[i] = new int[Enum.GetNames(typeof(Actions)).Length];

                if (i >= Columns)
                {
                    mazeStatesActions[i][(int)Actions.MoveNorth] = 1;

                    if (i < NumberOfStates - Columns)
                    {
                        mazeStatesActions[i][(int)Actions.MoveSouth] = 1;
                    }
                }
                else
                {
                    mazeStatesActions[i][(int)Actions.MoveSouth] = 1;
                }

                if (i < NumberOfStates - 1 &&
                    (i + 1) % Columns != 0)
                {
                    mazeStatesActions[i][(int)Actions.MoveEast] = 1;
                }

                if (i > 0 &&
                    i % Columns != 0)
                {
                    mazeStatesActions[i][(int)Actions.MoveWest] = 1;
                }

                if (i == GoalPosition)
                {
                    mazeStatesActions[i][0] = 1;
                    mazeStatesActions[i][1] = 0;
                    mazeStatesActions[i][2] = 0;
                    mazeStatesActions[i][3] = 0;
                    mazeStatesActions[i][4] = 0;
                }
            }

            OnMazeCreatedEventhHandler();
            
            MazeStates = mazeStatesActions;
            _mazeInitialized = true;

            foreach (var wall in Obstructions)
            {
                AddWall(wall.BetweenSpace, wall.AndSpace);
            }
        }

        protected virtual void CreateRewards()
        {
            Console.WriteLine("Creating Reward States");
            double[][] reward = new double[NumberOfStates][];

            if (NumberOfStates != MazeStates.Length)
            {
                CreateMazeStates();
            }

            for (int i = 0; i < NumberOfStates; ++i)
            {
                reward[i] = new double[MazeStates[i].Length];

                for (int j = 0; j < MazeStates[i].Length; ++j)
                {
                    if (MazeStates[i][j] > 0)
                    {
                        if (j == GoalPosition)
                        {
                            reward[i][j] = _goalValue;
                        }                        
                        else
                        {
                            var customReward = _additionalRewards.Where(r => r.Position == j).FirstOrDefault();

                            if (customReward != null)
                            {
                                reward[i][j] = customReward.Value;
                            }
                            else
                            {
                                reward[i][j] = _movementValue;
                            }
                        }
                    }
                }
            }

            Rewards = reward;
            PrintRewards();
            OnRewardsCreated();
        }

        public virtual void AddCustomReward(int position, double reward)
        {
            var exists = _additionalRewards.Where(x => x.Position == position).FirstOrDefault();
            
            if (exists != null)
            {
                if (exists.Value == reward)
                    return;

                exists.Value = reward;
            }
            else
            {
                _additionalRewards.Add(new AdditionalReward {  Position = position, Value = reward });
            }

            for (int i = 0; i < MazeStates.Length; ++i)
            {
                if (MazeStates[i][position] > 0)
                {
                    Rewards[i][position] = reward;
                }
            }
        }

        public virtual void RemoveCustomReward(int position)
        {
            var exists = _additionalRewards.Where(x => x.Position == position).FirstOrDefault();

            if (exists == null) return;

            _additionalRewards.Remove(exists);

            for (int i = 0; i < MazeStates.Length; ++i)
            {
                if (MazeStates[i][position] > 0)
                {
                    Rewards[i][position] = _movementValue;
                }

            }
        }

        public IEnumerable<AdditionalReward> GetAdditionalRewards()
        {
            return this._additionalRewards;
        }

        public void RemoveReward(int position)
        {

        }

        /// <summary>
        /// Creates the Q-Table matrix
        /// </summary>
        protected virtual void CreateQuality()
        {
            Console.WriteLine("Creating Quality of States");

            double[][] quality = new double[NumberOfStates][];

            for (int i = 0; i < NumberOfStates; ++i)
            {
                quality[i] = new double[MazeStates[i].Length];
            }
            
            Quality = quality;
            OnQualityCreated();
        }

        public virtual void AddWall(int betweenSpace, int andSpace)
        {
            if (!_mazeInitialized)
            {
                CreateMazeStates();
                CreateRewards();
            }


            int differential = betweenSpace - andSpace;
            var actions = GetActionBetweenSpaces(betweenSpace, andSpace);
            var action = actions.action;
            var reverseAction = actions.reverseAction;

            MazeStates[betweenSpace][action] = 0;
            MazeStates[andSpace][reverseAction] = 0;


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

            CreateRewards();
        }

        public virtual void RemoveWall(int betweenSpace, int andSpace)
        {
            if (!_mazeInitialized)
            {
                CreateMazeStates();
                CreateRewards();
            }

            var actions = GetActionBetweenSpaces(betweenSpace, andSpace);
            var action = actions.action;
            var reverseAction = actions.reverseAction;

            MazeStates[betweenSpace][action] = 1;
            MazeStates[andSpace][reverseAction] = 1;
            Rewards[betweenSpace][action] = _movementValue;
            Rewards[andSpace][reverseAction] = _movementValue;

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
                action = (int)Actions.MoveWest;
                reverseAction = (int)Actions.MoveEast;
            }

            if (differential == -1)
            {
                action = (int)Actions.MoveEast;
                reverseAction = (int)Actions.MoveWest;
            }

            if (differential == Columns * -1)
            {
                action = (int)Actions.MoveSouth;
                reverseAction = (int)Actions.MoveNorth;
            }

            if (differential == Columns)
            {
                action = (int)Actions.MoveNorth;
                reverseAction = (int)Actions.MoveSouth;
            }

            return (action, reverseAction);
        }

        protected virtual MazeObstruction GetObstructionFromList(int betweenSpace, int andSpace)
        {
            return Obstructions.Where(x => (x.BetweenSpace == betweenSpace && x.AndSpace == andSpace) || (x.AndSpace == betweenSpace && x.BetweenSpace == andSpace)).FirstOrDefault();
        }

        protected virtual List<int> GetPossibleNextActions(int currentState, int[][] mazeNextStates)
        {
            List<int> result = new List<int>();

            for (int i = 0; i < mazeNextStates[0].Length; ++i)
            {
                if (mazeNextStates[currentState][i] > 0)
                {
                    result.Add(i);
                }
            }

            return result;
        }

        protected virtual int GetRandomNextAction(int currentState, int[][] mazeNextStates)
        {
            List<int> possibleNextStates = GetPossibleNextActions(currentState, mazeNextStates);

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
            CreateMazeStates();
            CreateRewards();
            CreateQuality();
            
            double epsilon = 1;
            _end_decay = (int)MaxEpochs / 2;
            _epsilon_decay_value = epsilon / (_end_decay - _start_decay);

            Console.WriteLine("Please wait while I learn the maze");
            
            OnTrainingStatusChanged(true);
            
            for (int epoch = 0; epoch < MaxEpochs; ++epoch)
            {
                moves = 0;
                TotalRewards = 0;
                Console.Write($"Runnging through epoch {(epoch + 1).ToString("#,##0")} of {MaxEpochs.ToString("#,##0")}\r");

                int currentState = _random.Next(0, Rewards.Length);
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

                    if (currentState == GoalPosition ||
                        moves > 10000)
                    {
                        done = true;
                    }
                }

                if (_end_decay >= epoch &&
                    epoch >= _start_decay)
                {
                    epsilon -= _epsilon_decay_value;
                }

                OnTrainingEpochCompleted(new TrainingEpochCompletedEventArgs(epoch + 1, MaxEpochs, moves, TotalRewards, currentState == GoalPosition));
            }

            OnTrainingStatusChanged(false);
            Console.WriteLine();
            Console.WriteLine("I'm done learning");
        }

        protected virtual int TrainingDetermineNextAction(int currentState, double epsilon)
        {
            double randRand = _random.NextDouble();
            int nextAction = GetRandomNextAction(currentState, MazeStates);

            if (randRand > epsilon)
            {
                int preferredNext = -1;
                double max = 0;

                for (int i = 0; i < this.Quality[currentState].Length; ++i)
                {
                    if (this.Quality[currentState][i] > max)
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

            return nextAction;
        }

        protected virtual void CalculateQValue(int currentState, int action)
        {
            double maxQ = double.MinValue;
            bool isBackTrack = false;
            int nextState = GetNextState(currentState, action);
            List<int> possNextNextActions = GetPossibleNextActions(nextState, MazeStates);
            double oldtempQuality = Quality[currentState][action];

            for (int j = 0; j < possNextNextActions.Count; ++j)
            {
                int futureNextAction = possNextNextActions[j];  // short alias

                double futureQuality = Quality[nextState][futureNextAction];

                if (currentState == GetNextState(nextState, futureNextAction))
                {
                    isBackTrack = true;
                }

                if (futureQuality > maxQ)
                {
                    maxQ = futureQuality;
                    //Quality[action][currentState] -= 1;
                }                
            }            

            TotalRewards = Rewards[currentState][action] + (isBackTrack ? (GetAdditionalRewards().Max(x => x.Value)) - _backtrackPunishment : 0);

            Quality[currentState][action] = ((1 - LearningRate) * Quality[currentState][action]) + (LearningRate * (TotalRewards + (DiscountRate * maxQ)));

            OnTrainingAgentStateChanging(action, currentState, oldtempQuality, Quality[currentState][action]);
        }

        public virtual int GetNextState(int currentState, int action)
        {
            return GetNextState(currentState, (Actions)action);
        }

        public virtual int GetNextState(int currentState, Actions action)
        {
            switch(action)
            {
                case Actions.MoveNorth:
                    return currentState - Columns;
                case Actions.MoveEast:
                    return currentState + 1;
                case Actions.MoveSouth:
                    return currentState + Columns;
                case Actions.MoveWest:
                    return currentState - 1;
                case Actions.Stay:
                    return currentState;
                default:
                    return -1;
            }
        }

        public virtual void RunMaze()
        {
            int curr = StartPosition; 
            int action;
            int nextState;
            int moves = 0;
            int previousPosition = -1;
            int _backtrackTimes = 0;
            TotalRewards = 0;
            Console.Write(curr + "->");

            while (curr != GoalPosition)
            {
                if (Quality == null)
                    Train();                
                
                action = GetMaxQ(Quality[curr]);

                if (GetNextState(curr, action) == previousPosition)
                {
                    if (_backtrackTimes > 1)
                    {
                        action = GetMaxQ(Quality[curr], action);
                        _backtrackTimes = 0;
                    }
                    else
                    {
                        _backtrackTimes++;
                    }
                }
                else
                {
                    _backtrackTimes = 0;
                }

                if (MazeStates[curr][action] < 1)
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

                if (nextState == curr ||
                    nextState == previousPosition)
                {
                    _backtrackTimes++;

                    string message = "I'm a greedy idiot, and am backtracking to get more rewards.  Try adjusting the Discount and/or Learning Rate to stop me from doing this.";
                    throw new InvalidOperationException(message);
                }

                previousPosition = curr;
                curr = nextState;
                OnAgentStateChanged(curr, TotalRewards, moves);
            }

            Console.WriteLine("done");
        }

        protected virtual int GetMaxQ(double[] vector, int excludeAction = -1)
        {
            double maxVal = double.MinValue; 
            int idx = 0;

            for (int i = 0; i < vector.Length; ++i)
            {
                if (i == excludeAction) continue;

                if (vector[i] != 0 && vector[i] > maxVal)
                {
                    maxVal = vector[i]; 
                    idx = i;
                }
            }

            return idx;
        }

        public virtual void PrintRewards()
        {
            int ns = Rewards.Length;
            Console.WriteLine($"Rewards [0] [1] . . [{NumberOfStates - 1}]");

            for (int i = 0; i < ns; ++i)
            {
                for (int j = 0; j < MazeStates[i].Length; ++j)
                {
                    Console.Write(Rewards[i][j].ToString("F2") + " ");
                }
                Console.WriteLine();
            }

        }
        public virtual void PrintQuality()
        {
            int ns = Quality.Length;
            Console.WriteLine($"Quality [0] [1] . . [{NumberOfStates - 1}]");
            for (int i = 0; i < ns; ++i)
            {
                for (int j = 0; j < MazeStates[i].Length; ++j)
                {
                    Console.Write(Quality[i][j].ToString("F2") + " ");
                }
                Console.WriteLine();
            }
        }
    }
}
