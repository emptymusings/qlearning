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
            MaxEpisodes = maxEpisodes;
            MaximumAllowedMoves = maximumAllowedMoves;
            MaximumAllowedBacktracks = maximumAllowedBacktracks;
        }

        public virtual int NumberOfStates
        {
            get
            {
                if (ObservationSpace == null)
                    return 0;

                return ObservationSpace.Length;
            }
        }
        public virtual int[][] ObservationSpace { get; set; }
        public virtual double[][] Rewards { get; set; }
        public virtual double[][] Quality { get; set; }
        public virtual double EpsilonDecayStart { get; set; } = 1;
        public virtual double EpsilonDecayEnd { get; set; }
        public virtual double LearningRate { get; set; }
        public virtual double DiscountRate { get; set; }
        public virtual List<int> ObjectiveStates { get; set; } = new List<int>();
        public virtual int ObjectiveAction { get; set; }
        public virtual int MaximumAllowedMoves { get; set; } = 1000;
        public virtual double ObjectiveReward { get; set; }
        public virtual string QualitySaveDirectory { get; set; }
        public virtual int MaximumAllowedBacktracks { get; set; } = -1;
        public virtual int MaxEpisodes { get; set; }
        public virtual List<TrainingSession> TrainingEpisodes { get; set; }
        public virtual int SaveQualityFrequency { get; set; } = 100;
        public virtual int TotalSpaces { get; set; }

        public virtual void InitializeStatesTable(int numberOfStates, int numberOfActions)
        {
            _numberOfStates = numberOfStates;
            _numberOfActions = numberOfActions;

            InitializeStatesTable();
        }

        public virtual void InitializeStatesTable()
        {
            ObservationSpace = new int[_numberOfStates][];
            
            for (int i = 0; i < _numberOfStates; ++i)
            {
                ObservationSpace[i] = new int[_numberOfActions];
                
                for (int j = 0; j < _numberOfActions; ++j)
                {
                    ObservationSpace[i][j] = -1;
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
            if (Rewards == null ||
                Rewards.Length < _numberOfStates)
            {
                Rewards = new double[_numberOfStates][];

                for (int i = 0; i < _numberOfStates; ++i)
                {
                    Rewards[i] = new double[_numberOfActions];
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
            Quality = new double[_numberOfStates][];

            for (int i = 0; i < _numberOfStates; ++i)
            {
                Quality[i] = new double[_numberOfActions];
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

            EpsilonDecayEnd = MaxEpisodes / 2;
            _epsilonDecayValue = epsilon / (EpsilonDecayEnd - EpsilonDecayStart);

            TrainingEpisodes = new List<TrainingSession>();
            OnTrainingStateChanged(true);

            for (int episode = 0; episode < MaxEpisodes; ++episode)
            {
                var episodeResults = RunTrainingEpisode(epsilon);
                var state = episodeResults.finalState;
                var moves = episodeResults.moves;

                if (ObjectiveStates.Contains(state) &&
                    (1 + episode) % SaveQualityFrequency == 0)
                {
                    SaveQualityForEpisode(episode + 1, moves);
                }

                epsilon = DecayEpsilon(episode, epsilon);

                OnTrainingEpisodeCompleted(episode, MaxEpisodes, moves, _accumulatedEpisodeRewards, ObjectiveStates.Contains(state));
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
            int state = _random.Next(0, _numberOfStates);

            _accumulatedEpisodeRewards = 0;

            while (!done)
            {
                moves++;
                int nextAction = GetGreedyNextAction(state, epsilon);
                CalculateQValue(state, nextAction);

                previousState = state;
                state = ObservationSpace[state][nextAction];

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
        /// Selects the agent's next action randomly based on its current state
        /// </summary>
        protected virtual int GetRandomNextAction(int state)
        {
            List<int> possibleNextStates = GetPossibleNextActions(state);

            int count = possibleNextStates.Count;
            int index = _random.Next(0, count);

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
                if (ObservationSpace[state][i] >= 0)
                {
                    result.Add(i);
                }
            }

            return result;
        }

        protected virtual void CalculateQValue(int state, int action)
        {
            int nextState = ObservationSpace[state][action];

            if (!IsValidState(nextState)) ThrowInvalidActionException(state, action);

            var forecaster = GetFuturePositionMaxQ(nextState);
            var maxQ = forecaster.maxQ;
            var selectedNextState = forecaster.selectedNextState;

            var r = Rewards[state][action];
            var q = Quality[state][action] + (LearningRate * (r + (DiscountRate * maxQ) - Quality[state][action]));
            // I have found a couple of Q-Value formulas.  Aside form some rounding issues, this and the formula below it are identical
            //double similarQFormula  = ((1 - LearningRate) * Quality[state][action]) + (LearningRate * (r + (DiscountRate * maxQ)));

            Quality[state][action] = q;
            _accumulatedEpisodeRewards += r;
        }

        /// <summary>
        /// Gets the maximum quality for future position based on the agent's state
        /// </summary>
        protected virtual (int selectedNextState, double maxQ) GetFuturePositionMaxQ(int state)
        {
            double maxQ = double.MinValue;
            List<int> possNextNextActions = GetPossibleNextActions(state);
            int selectedNextState = -1;

            for (int i = 0; i < possNextNextActions.Count; ++i)
            {
                int futureNextAction = possNextNextActions[i];  // short alias

                double futureQuality = Quality[state][futureNextAction];
                int nextState = ObservationSpace[state][futureNextAction];

                if (!IsValidState(nextState)) ThrowInvalidActionException(state, futureNextAction);

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
                Quality = Quality
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

        public virtual void RunMaze(int fromState)
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
                if (Quality == null)
                    throw new InvalidOperationException($"The Q-table has not been initialized.  Train the agent first");

                action = GetPreferredNextAction(fromState);
                nextState = ObservationSpace[fromState][action];

                if (ObservationSpace[fromState][action] < 0)
                {
                    throw new InvalidOperationException($"I guess I didn't learn very well.  Please try training again (perhaps adjusing the learning rate, discount rate, and/or episode count)");
                }

                _accumulatedEpisodeRewards += Rewards[fromState][action];
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

                if (ObjectiveStates.Contains((fromState % TotalSpaces)) &&
                    action == ObjectiveAction)
                {
                    done = true;
                }
            }

            OnAgentCompleted(moves, _accumulatedEpisodeRewards, (ObjectiveStates.Contains(fromState)));
        }

        public virtual int GetNextState(int state, int action)
        {
            return ObservationSpace[state][action];
        }

    }
}
