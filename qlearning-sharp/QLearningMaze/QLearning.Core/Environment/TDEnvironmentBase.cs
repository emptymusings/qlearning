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
                
        public TDEnvironmentBase() { }

        public TDEnvironmentBase(int numberOfStates, int numberOfActions)
        {
            _numberOfStates = numberOfStates;
            NumberOfActions = numberOfActions;
        }

        public TDEnvironmentBase(
            int numberOfStates,
            int numberOfActions,
            string qualitySaveDirectory,
            double objectiveReward,
            int objectiveAction,
            List<int> objectiveStates)
        {
            NumberOfActions = numberOfActions;
            _numberOfStates = numberOfStates;
            QualitySaveDirectory = qualitySaveDirectory;
            ObjectiveReward = objectiveReward;
            ObjectiveAction = objectiveAction;
            TerminalStates = objectiveStates;
        }

        protected int _numberOfStates;

        public virtual int NumberOfStates
        {
            get
            {
                return _numberOfStates;
            }
            set
            {
                _numberOfStates = value;
            }
        }

        public virtual int NumberOfActions { get; set; }
        public virtual int[][] StatesTable { get; set; }
        public virtual double[][] RewardsTable { get; set; }
        public virtual double[][] QualityTable { get; set; }
        public virtual List<int> TerminalStates { get; set; } = new List<int>();
        public virtual int ObjectiveAction { get; set; }
        public virtual double ObjectiveReward { get; set; } = 1;
        public virtual bool SaveQualityToDisk { get; set; } = true;
        public virtual string QualitySaveDirectory { get; set; }
        public virtual int QualitySaveFrequency { get; set; } = 100;
        public virtual int StatesPerPhase { get; set; }


        public virtual void Initialize(bool overrideBaseEvents = false)
        {
            InitializeStatesTable(overrideBaseEvents);
            InitializeQualityTable(overrideBaseEvents);
            InitializeRewardsTable(overrideBaseEvents);
            InitializeSaveFolder();
        }

        protected virtual void InitializeStatesTable(bool overrideBaseEvents = false)
        {
            if (!overrideBaseEvents)
                OnStateTableCreating();

            StatesTable = new int[_numberOfStates][];
            
            for (int i = 0; i < _numberOfStates; ++i)
            {
                StatesTable[i] = new int[NumberOfActions];
                
                for (int j = 0; j < NumberOfActions; ++j)
                {
                    StatesTable[i][j] = -1;
                }
            }

            if (!overrideBaseEvents)
                OnStateTableCreated();
        }

        protected virtual void InitializeRewardsTable(bool overrideBaseEvents = false)
        {
            if (!overrideBaseEvents)
                OnRewardTableCreating();

            if (RewardsTable == null ||
                RewardsTable.Length < _numberOfStates)
            {
                RewardsTable = new double[_numberOfStates][];

                for (int i = 0; i < _numberOfStates; ++i)
                {
                    RewardsTable[i] = new double[NumberOfActions];
                }
            }

            if (!overrideBaseEvents)
                OnRewardTableCreated();
        }

        protected virtual void InitializeQualityTable(bool overrideBaseEvents = false)
        {
            if (!overrideBaseEvents)
                OnQualityTableCreating();

            QualityTable = new double[_numberOfStates][];

            for (int i = 0; i < _numberOfStates; ++i)
            {
                QualityTable[i] = new double[NumberOfActions];
            }

            if (!overrideBaseEvents)
                OnQualityTableCreated();
        }

        public virtual void AddTerminalState(int state)
        {
            if (TerminalStates == null) TerminalStates = new List<int>();

            if (!TerminalStates.Contains(state))
            {
                TerminalStates.Add(state);
            }
        }

        public virtual bool IsValidState(int state)
        {
            return state >= 0;
        }

        protected virtual void ThrowInvalidActionException(int state, int action)
        {
            throw new InvalidOperationException($"Attempting action {action} from state {state} returned an invalid value");
        }

        public virtual bool IsTerminalState(int state, int moves, int maximumAllowedMoves)
        {
            bool result = TerminalStates.Contains(state % StatesPerPhase) ||
                    (maximumAllowedMoves > 0 && moves > maximumAllowedMoves);
            return result;
        }

        public virtual bool IsTerminalState(int state, int action, int moves, int maximumAllowedMoves)
        {
            if (moves > maximumAllowedMoves)
                return true;

            return IsTerminalState(state, moves, maximumAllowedMoves) && action == ObjectiveAction;
        }

        public virtual (int newState, double reward, double quality) Step(int state, int action)
        {
            return (
                StatesTable[state][action],
                RewardsTable[state][action],
                QualityTable[state][action]);
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
                try
                {
                    File.Delete(file);
                }
                catch { }
            }
        }

        public virtual TrainingSession SaveQualityForEpisode(int episode, int moves, double score)
        {
            var trainingEpisode = new TrainingSession
            {
                Episode = episode,
                Moves = moves,
                Score = score,
                Quality = CopyQuality()
            };

            if (SaveQualityToDisk)
            {
                Utilities.SaveObject($@"{QualitySaveDirectory}\Episode_{episode}_{DateTime.Now.ToString("HH_mm_ss")}.json", trainingEpisode);
            }

            return trainingEpisode;
        }


        public virtual double[][] CopyQuality()
        {
            double[][] result;

            if (QualityTable == null)
                return null;

            result = new double[QualityTable.Length][];

            for (int i = 0; i < QualityTable.Length; ++i)
            {
                result[i] = new double[QualityTable[i].Length];

                for (int j = 0; j < QualityTable[i].Length; ++j)
                {
                    result[i][j] = QualityTable[i][j];
                }
            }

            return result;
        }
    }
}
