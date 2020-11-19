namespace QLearning.Core
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.IO;

    public partial class QLearningBase : IQLearning
    {
        private Random _random = new Random();
        private double _accumulatedEpisodeRewards;
        private double _epsilonDecayValue;

        protected int _numberOfStates;
        protected int _numberOfActions;
        
        public QLearningBase() { }

        public QLearningBase(int numberOfStates, int numberOfActions)
        {
            _numberOfStates = numberOfStates;
            _numberOfActions = numberOfActions;
        }

        public QLearningBase(
            int numberOfStates,
            int numberOfActions,
            double learningRate,
            double discountRate,
            string qualitySaveDirectory,
            double objectiveReward,
            int objectiveAction,
            List<int> objectiveStates,
            int maxEpisodes,
            int maximumAllowedMoves = 1000,
            int maximumAllowedBacktracks = -1)
        {
            _numberOfActions = numberOfActions;
            _numberOfStates = numberOfStates;
            LearningRate = learningRate;
            DiscountRate = discountRate;
            QualitySaveDirectory = qualitySaveDirectory;
            ObjectiveReward = objectiveReward;
            ObjectiveAction = objectiveAction;
            ObjectiveStates = objectiveStates;
            NumberOfTrainingEpisodes = maxEpisodes;
            MaximumAllowedMoves = maximumAllowedMoves;
            MaximumAllowedBacktracks = maximumAllowedBacktracks;
        }

        public virtual int NumberOfStates
        {
            get
            {
                if (StatesTable == null)
                    return 0;

                return StatesTable.Length;
            }
        }
        public virtual int[][] StatesTable { get; set; }
        public virtual double[][] RewardsTable { get; set; }
        public virtual double[][] QualityTable { get; set; }
        public virtual double EpsilonDecayStart { get; set; } = 1;
        public virtual double EpsilonDecayEnd { get; set; }
        public virtual double LearningRate { get; set; }
        public virtual double DiscountRate { get; set; }
        public virtual List<int> ObjectiveStates { get; set; } = new List<int>();
        public virtual int ObjectiveAction { get; set; }
        public virtual int MaximumAllowedMoves { get; set; } = 100000;
        public virtual double ObjectiveReward { get; set; }
        public virtual string QualitySaveDirectory { get; set; }
        public virtual int MaximumAllowedBacktracks { get; set; } = -1;
        public virtual int NumberOfTrainingEpisodes { get; set; }
        public virtual List<TrainingSession> TrainingEpisodes { get; set; }
        public virtual int QualitySaveFrequency { get; set; } = 100;
        public virtual int StatesPerPhase { get; set; }

        public virtual void InitializeStatesTable(int numberOfStates, int numberOfActions)
        {
            _numberOfStates = numberOfStates;
            _numberOfActions = numberOfActions;

            InitializeStatesTable();
        }

        public virtual void InitializeStatesTable()
        {
            StatesTable = new int[_numberOfStates][];
            
            for (int i = 0; i < _numberOfStates; ++i)
            {
                StatesTable[i] = new int[_numberOfActions];
                
                for (int j = 0; j < _numberOfActions; ++j)
                {
                    StatesTable[i][j] = -1;
                }
            }
        }

        public virtual void InitializeRewardsTable(int numberOfStates, int numberOfActions)
        {
            _numberOfActions = numberOfActions;
            _numberOfStates = numberOfStates;

            InitializeRewardsTable();
        }

        public virtual void InitializeRewardsTable()
        {
            if (RewardsTable == null ||
                RewardsTable.Length < _numberOfStates)
            {
                RewardsTable = new double[_numberOfStates][];

                for (int i = 0; i < _numberOfStates; ++i)
                {
                    RewardsTable[i] = new double[_numberOfActions];
                }
            }
        }

        public virtual void InitializeQualityTable(int numberOfStates, int numberOfActions)
        {
            _numberOfStates = numberOfStates;
            _numberOfActions = numberOfActions;

            InitializeQualityTable();
        }

        public virtual void InitializeQualityTable()
        {
            QualityTable = new double[_numberOfStates][];

            for (int i = 0; i < _numberOfStates; ++i)
            {
                QualityTable[i] = new double[_numberOfActions];
            }
        }

        protected virtual bool IsValidState(int state)
        {
            return state >= 0;
        }

        protected virtual void ThrowInvalidActionException(int state, int action)
        {
            throw new InvalidOperationException($"Attempting action {action} from state {state} returned an invalid value");
        }

        /// <summary>
        /// Performs the training necessary to populate the Q-Table
        /// </summary>
        public virtual void Train()
        {
            InitializeStatesTable();
            InitializeQualityTable();
            InitializeRewardsTable();

            InitializeSaveFolder();

            double epsilon = 1;

            EpsilonDecayEnd = NumberOfTrainingEpisodes / 2;
            _epsilonDecayValue = epsilon / (EpsilonDecayEnd - EpsilonDecayStart);

            TrainingEpisodes = new List<TrainingSession>();
            OnTrainingStateChanged(true);

            for (int episode = 0; episode < NumberOfTrainingEpisodes; ++episode)
            {
                var episodeResults = RunTrainingEpisode(epsilon);
                var state = episodeResults.finalState;
                var moves = episodeResults.moves;

                if (ObjectiveStates.Contains(state) &&
                    (1 + episode) % QualitySaveFrequency == 0)
                {
                    SaveQualityForEpisode(episode + 1, moves);
                }

                epsilon = DecayEpsilon(episode, epsilon);

                OnTrainingEpisodeCompleted(episode, NumberOfTrainingEpisodes, moves, _accumulatedEpisodeRewards, ObjectiveStates.Contains(state));
            }

            OnTrainingStateChanged(false);
        }

        /// <summary>
        /// Maneuvers the agent through a training episode
        /// </summary>
        /// <param name="epsilon">The "greedy strategy" epsilon value that will help determine whether to perform a random or a known action</param>
        protected virtual (int finalState, int moves) RunTrainingEpisode(double epsilon)
        {
            int moves = 0;
            int previousState = -1;
            bool done = false;
            int state = _random.Next(0, NumberOfStates);

            _accumulatedEpisodeRewards = 0;

            while (!done)
            {
                moves++;
                var nextActionSet = GetGreedyNextAction(state, epsilon);
                int nextAction = nextActionSet.nextAction;
                bool usedGreed = nextActionSet.usedGreedy;

                CalculateQValue(state, nextAction);

                previousState = state;
                state = StatesTable[state][nextAction];

                if (state == previousState)
                    state = _random.Next(0, _numberOfStates);

                if (ObjectiveStates.Contains(state) ||
                    moves > MaximumAllowedMoves)
                {
                    done = true;
                }
            }

            return (state, moves);
        }

        /// <summary>
        /// Get the next action to take from the current state
        /// </summary>
        /// <param name="state">The state in which the agent currently resides</param>
        /// <param name="epsilon"></param>
        protected virtual (int nextAction, bool usedGreedy) GetGreedyNextAction(int state, double epsilon)
        {
            double randRand = _random.NextDouble();
            int nextAction = -1;
            bool usedGreedy = false;

            if (randRand > epsilon)
            {
                int preferredNext = GetPreferredNextAction(state);

                if (preferredNext >= 0)
                {
                    nextAction = preferredNext;
                    usedGreedy = true;
                }
            }

            while (nextAction < 0)
                nextAction = GetRandomNextAction(state);

            return (nextAction, usedGreedy);
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
            
            for (int i = 0; i < this.QualityTable[state].Length; ++i)
            {
                if (this.QualityTable[state][i] > max &&
                    QualityTable[state][i] != 0)
                {
                    max = this.QualityTable[state][i];
                    preferredNext = i;
                }
            }

            return preferredNext;
        }

        /// <summary>
        /// Selects the agent's next action randomly based on its current state
        /// </summary>
        protected virtual int GetRandomNextAction(int state)
        {
            List<int> possibleNextStates = GetPossibleNextActions(state);

            int count = possibleNextStates.Count;
            int index = _random.Next(0, count);

            //List<int> possibleNextStates = StatesTable[state].ToList();
            //int index = _random.Next(0, possibleNextStates.Count);

            //while (StatesTable[state][index] < 0)
            //{
            //    index = _random.Next(0, possibleNextStates.Count);
            //}

            //return index;

            if (possibleNextStates.Count > 0)
                return possibleNextStates[index];
            else
                throw new NullReferenceException($"There are no possible actions that can be taken from the state {state}");
        }

        /// <summary>
        /// Gets all possible actions available to the agent in its current state
        /// </summary>
        protected virtual List<int> GetPossibleNextActions(int state)
        {
            List<int> result = new List<int>();
            int actionCount = _numberOfActions;

            for (int i = 0; i < actionCount; ++i)
            {
                if (StatesTable[state][i] >= 0)
                {
                    result.Add(i);
                }
            }

            return result;
        }

        protected virtual void CalculateQValue(int state, int action)
        {
            int nextState = StatesTable[state][action];

            if (!IsValidState(nextState)) ThrowInvalidActionException(state, action);

            var forecaster = GetFuturePositionMaxQ(nextState);
            var maxQ = forecaster.maxQ;
            var selectedNextState = forecaster.selectedNextState;

            var r = RewardsTable[state][action];
            var q = QualityTable[state][action] + (LearningRate * (r + (DiscountRate * maxQ) - QualityTable[state][action]));

            // I have found a couple of Q-Value formulas.  Aside form some rounding issues, this and the formula below it are identical
            double similarQFormula  = ((1 - LearningRate) * QualityTable[state][action]) + (LearningRate * (r + (DiscountRate * maxQ)));

            QualityTable[state][action] = similarQFormula;
            _accumulatedEpisodeRewards += r;
        }

        /// <summary>
        /// Gets the maximum quality for future position based on the agent's state
        /// </summary>
        protected virtual (int selectedNextState, double maxQ) GetFuturePositionMaxQ(int nextState)
        {
            double maxQ = double.MinValue;
            List<int> possNextNextActions = GetPossibleNextActions(nextState);
            int selectedNextState = -1;

            double max = QualityTable[nextState].Cast<double>().Max();

            for (int i = 0; i < possNextNextActions.Count; ++i)
            {
                int futureNextAction = possNextNextActions[i];  // short alias

                double futureQuality = QualityTable[nextState][futureNextAction];

                if (!IsValidState(nextState)) ThrowInvalidActionException(nextState, futureNextAction);

                if (futureQuality > maxQ)
                {
                    maxQ = futureQuality;
                    selectedNextState = nextState;
                }
            }

            
            return (selectedNextState, maxQ);
        }

        protected virtual void InitializeSaveFolder()
        {
            if (string.IsNullOrEmpty(QualitySaveDirectory))
            {
                var executingDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                QualitySaveDirectory = executingDirectory + @"\TrainingSessions";
            }

            if (!Directory.Exists(QualitySaveDirectory))
            {
                Directory.CreateDirectory(QualitySaveDirectory);
            }

            foreach(string file in Directory.GetFiles(QualitySaveDirectory))
            {
                File.Delete(file);
            }
        }

        protected virtual void SaveQualityForEpisode(int episode, int moves)
        {
            var trainingEpisode = new TrainingSession
            {
                Episode = episode,
                Moves = moves,
                Score = _accumulatedEpisodeRewards,
                Quality = QualityTable
            };

            TrainingEpisodes.Add(trainingEpisode);
            Utilities.SaveObject($@"{QualitySaveDirectory}\Episode_{episode}_{DateTime.Now.ToString("HH_mm_ss")}.json", trainingEpisode);
        }

        /// <summary>
        /// Decays the epsilon value so that as training progesses, known values will be more likely to be used
        /// </summary>
        protected virtual double DecayEpsilon(int episode, double epsilon)
        {
            if (EpsilonDecayEnd >= episode &&
                   episode >= EpsilonDecayStart)
            {
                epsilon -= _epsilonDecayValue;
            }

            return epsilon;
        }

        public virtual void RunAgent(int fromState)
        {
            int action = -1;
            int nextState;
            int moves = 0;
            int previousState = -1;
            int numberOfBacktracks = 0;
            bool done = false;

            _accumulatedEpisodeRewards = 0;

            while (!done)
            {
                if (QualityTable == null)
                    throw new InvalidOperationException($"The Q-table has not been initialized.  Train the agent first");

                action = GetPreferredNextAction(fromState);
                nextState = StatesTable[fromState][action];

                if (StatesTable[fromState][action] < 0)
                {
                    throw new InvalidOperationException($"I guess I didn't learn very well.  Please try training again (perhaps adjusing the learning rate, discount rate, and/or episode count)");
                }

                _accumulatedEpisodeRewards += RewardsTable[fromState][action];
                moves++;

                if (moves > MaximumAllowedMoves)
                {
                    throw new InvalidOperationException($"Something's gone wrong, I've wandered around far too many times");
                }

                if (nextState == previousState)
                {
                    if (MaximumAllowedBacktracks >= 0 &&
                        numberOfBacktracks > MaximumAllowedBacktracks)
                    {
                        throw new InvalidOperationException($"The agent has exceeded the maximum number of backtracks: {MaximumAllowedBacktracks}");
                    }
                    else
                    {
                        numberOfBacktracks++;
                    }
                }

                previousState = fromState;
                fromState = nextState;

                OnAgentStateChanged(fromState, moves, _accumulatedEpisodeRewards);

                if (ObjectiveStates.Contains((fromState % StatesPerPhase)) &&
                    action == ObjectiveAction)
                {
                    done = true;
                }
            }

            OnAgentCompleted(moves, _accumulatedEpisodeRewards, (ObjectiveStates.Contains(fromState)));
        }

        public virtual int GetNextState(int state, int action)
        {
            return StatesTable[state][action];
        }

    }
}
