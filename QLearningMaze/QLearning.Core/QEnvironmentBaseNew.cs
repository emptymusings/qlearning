namespace QLearning.Core
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.IO;

    public abstract partial class QEnvironmentBaseNew : IQEnvironmentNew
    {
        protected Random _random = new Random();
        protected int _numberOfStates;
        protected int _numberOfActions;
        
        public QEnvironmentBaseNew() { }

        public QEnvironmentBaseNew(int numberOfStates, int numberOfActions)
        {
            _numberOfStates = numberOfStates;
            _numberOfActions = numberOfActions;
        }

        public QEnvironmentBaseNew(
            int numberOfStates,
            int numberOfActions,
            string qualitySaveDirectory,
            double objectiveReward,
            int objectiveAction,
            List<int> objectiveStates)
        {
            _numberOfActions = numberOfActions;
            _numberOfStates = numberOfStates;
            QualitySaveDirectory = qualitySaveDirectory;
            ObjectiveReward = objectiveReward;
            ObjectiveAction = objectiveAction;
            ObjectiveStates = objectiveStates;
        }

        public virtual int NumberOfStates
        {
            get
            {
                if (StatesTable == null)
                    InitializeStatesTable();

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
        public virtual double ObjectiveReward { get; set; } = 1;
        public virtual string QualitySaveDirectory { get; set; }
        public virtual int QualitySaveFrequency { get; set; } = 100;
        public virtual int StatesPerPhase { get; set; }

        public virtual void Initialize()
        {
            InitializeStatesTable();
            InitializeQualityTable();
            InitializeRewardsTable();
            InitializeSaveFolder();
        }

        public virtual void Initialize(bool overrideBaseEvents)
        {
            InitializeStatesTable(overrideBaseEvents);
            InitializeQualityTable(overrideBaseEvents);
            InitializeRewardsTable(overrideBaseEvents);
            InitializeSaveFolder();
        }

        protected virtual void InitializeStatesTable()
        {
            InitializeStatesTable(false);
        }

        protected virtual void InitializeStatesTable(bool overrideBaseEvents)
        {
            if (!overrideBaseEvents)
                OnStateTableCreating();

            StatesTable = new int[_numberOfStates][];
            
            for (int i = 0; i < _numberOfStates; ++i)
            {
                StatesTable[i] = new int[_numberOfActions];
                
                for (int j = 0; j < _numberOfActions; ++j)
                {
                    StatesTable[i][j] = -1;
                }
            }

            if (!overrideBaseEvents)
                OnStateTableCreated();
        }

        protected virtual void InitializeRewardsTable()
        {
            InitializeRewardsTable(false);
        }

        protected virtual void InitializeRewardsTable(bool overrideBaseEvents)
        {
            if (!overrideBaseEvents)
                OnRewardTableCreating();

            if (RewardsTable == null ||
                RewardsTable.Length < _numberOfStates)
            {
                RewardsTable = new double[_numberOfStates][];

                for (int i = 0; i < _numberOfStates; ++i)
                {
                    RewardsTable[i] = new double[_numberOfActions];
                }
            }

            if (!overrideBaseEvents)
                OnRewardTableCreated();
        }

        protected virtual void InitializeQualityTable()
        {
            InitializeQualityTable(false);
        }

        protected virtual void InitializeQualityTable(bool overrideBaseEvents)
        {
            if (!overrideBaseEvents)
                OnQualityTableCreating();

            QualityTable = new double[_numberOfStates][];

            for (int i = 0; i < _numberOfStates; ++i)
            {
                QualityTable[i] = new double[_numberOfActions];
            }

            if (!overrideBaseEvents)
                OnQualityTableCreated();
        }

        protected virtual bool IsValidState(int state)
        {
            return state >= 0;
        }

        protected virtual void ThrowInvalidActionException(int state, int action)
        {
            throw new InvalidOperationException($"Attempting action {action} from state {state} returned an invalid value");
        }

        protected virtual double GetEpsilonDecayValue()
        {
            return 1 / (EpsilonDecayEnd - EpsilonDecayStart);
        }

        protected virtual double GetEpsilonDecayValue(double epsilon)
        {
            return epsilon / (EpsilonDecayEnd - EpsilonDecayStart);
        }

        public virtual bool IsTerminalState(int state, int moves)
        {
            return ObjectiveStates.Contains(state % StatesPerPhase) ||
                    (MaximumAllowedMoves > 0 && moves > MaximumAllowedMoves);
        }

        public virtual bool IsTerminalState(int state, int action, int moves)
        {
            return IsTerminalState(state, moves) && action == ObjectiveAction;
        }

        /// <summary>
        /// Get the next action to take from the current state
        /// </summary>
        /// <param name="state">The state in which the agent currently resides</param>
        /// <param name="epsilon"></param>
        public virtual (int nextAction, bool usedGreedy) GetNextAction(int state, double epsilon)
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
        public virtual int GetPreferredNextAction(int state, int[] excludedActions = null)
        {
            int preferredNext = -1;
            double max = double.MinValue;
            
            for (int i = 0; i < this.QualityTable[state].Length; ++i)
            {
                if (excludedActions != null &&
                    excludedActions.Contains(i))
                {
                    continue;
                }

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
        public virtual int GetRandomNextAction(int state)
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
                if (StatesTable[state][i] >= 0)
                {
                    result.Add(i);
                }
            }

            return result;
        }

        public virtual void CalculateQValue(int state, int action, double learningRate, double discountRate)
        {
            int nextState = StatesTable[state][action];

            if (!IsValidState(nextState)) ThrowInvalidActionException(state, action);

            var forecaster = GetFuturePositionMaxQ(nextState);
            var maxQ = forecaster.maxQ;
            var selectedNextState = forecaster.selectedNextState;
            
            var r = RewardsTable[state][action];

            var newQuality = QualityTable[state][action] + (learningRate * (r + (discountRate * maxQ) - QualityTable[state][action]));
            
            // Below is the original Q-Function as found in the example.  It produces results that are only slightly different than the 
            // newQuality Q-Function (presumably rounding errors), but I opted for newQuality because it is more commonly referred to 
            // in reference documents
            //double similarQFormula = ((1 - LearningRate) * QualityTable[state][action]) + (LearningRate * (r + (DiscountRate * maxQ))); 


            QualityTable[state][action] = newQuality;
        }

        /// <summary>
        /// Gets the maximum quality for future position based on the agent's state
        /// </summary>
        protected virtual (int selectedNextState, double maxQ) GetFuturePositionMaxQ(int nextState)
        {
            double maxQ = double.MinValue;


            List<int> possNextNextActions = GetPossibleNextActions(nextState);
            int selectedNextState = -1;

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

        public virtual TrainingSession SaveQualityForEpisode(int episode, int moves, double score)
        {
            var trainingEpisode = new TrainingSession
            {
                Episode = episode,
                Moves = moves,
                Score = score,
                Quality = QualityTable
            };

            Utilities.SaveObject($@"{QualitySaveDirectory}\Episode_{episode}_{DateTime.Now.ToString("HH_mm_ss")}.json", trainingEpisode);

            return trainingEpisode;
        }

    }
}
