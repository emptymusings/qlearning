namespace QLearning.Core.Environment
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Agent;

    public abstract partial class TDEnvironmentBase : ITDEnvironment
    {
        protected Random _random = new Random();
        protected int _numberOfStates;
        protected int _numberOfActions;
        
        public TDEnvironmentBase() { }

        public TDEnvironmentBase(int numberOfStates, int numberOfActions)
        {
            _numberOfStates = numberOfStates;
            _numberOfActions = numberOfActions;
        }

        public TDEnvironmentBase(
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
        public virtual List<int> ObjectiveStates { get; set; } = new List<int>();
        public virtual int ObjectiveAction { get; set; }
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

        public virtual void AddObjective(int state)
        {
            if (ObjectiveStates == null) ObjectiveStates = new List<int>();

            if (!ObjectiveStates.Contains(state))
            {
                ObjectiveStates.Add(state);
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

        public virtual bool IsTerminalState(int state, int moves, int maximumAllowedMoves)
        {
            bool result = ObjectiveStates.Contains(state % StatesPerPhase) ||
                    (maximumAllowedMoves > 0 && moves > maximumAllowedMoves);
            return result;
        }

        public virtual bool IsTerminalState(int state, int action, int moves, int maximumAllowedMoves)
        {
            if (moves > maximumAllowedMoves)
                return true;

            return IsTerminalState(state, moves, maximumAllowedMoves) && action == ObjectiveAction;
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

        public virtual (int newState, double reward, double quality) Step(int state, int action)
        {
            return (
                StatesTable[state][action],
                RewardsTable[state][action],
                QualityTable[state][action]);
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

        public virtual void CalculateQLearning(int state, int action, double learningRate, double discountRate)
        {
            var step = Step(state, action);
            var forecaster = GetFuturePositionMaxQ(step.newState);
            var maxQ = forecaster.maxQ;
            QualityTable[state][action] += (learningRate * (step.reward + (discountRate * maxQ) - step.quality));

        }

        public virtual void CalculateSarsa(int state, int action, double learningRate, double discountRate, double epsilon)
        {
            var step = Step(state, action);
            var nextActionSet = GetNextAction(step.newState, epsilon);
            var newQ = QualityTable[step.newState][nextActionSet.nextAction];
            QualityTable[state][action] += (learningRate * (step.reward + (discountRate * newQ) - step.quality));
        }

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
