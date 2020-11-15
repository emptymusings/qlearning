using System;
using System.Collections.Generic;
using System.Text;

namespace QLearning.Core
{
    public interface IQLearning
    {
        event EventHandler<AgentStateChangedEventArgs> AgentStateChanged;
        event EventHandler<TrainingAgentStateChangedEventArgs> TrainingAgentStateChanged;
        event EventHandler<TrainingEpisodeCompletedEventArgs> TrainingEpisodeCompleted;
        event EventHandler<bool> TrainingStateChanged;
        event EventHandler<AgentCompletedEventArgs> AgentCompleted;
        
        int NumberOfStates { get; }
        int TotalSpaces { get; set; }
        double DiscountRate { get; set; }
        double LearningRate { get; set; }
        int[][] ObservationSpace { get; set; }
        double[][] Rewards { get; set; }
        double[][] Quality { get; set; }
        double EpsilonDecayStart { get; set; }
        double EpsilonDecayEnd { get; set; }
        List<int> ObjectiveStates { get; set; }
        int ObjectiveAction { get; set; }
        public int MaximumAllowedMoves { get; set; }
        double ObjectiveReward { get; set; }
        string QualitySaveDirectory { get; set; }
        int MaximumAllowedBacktracks { get; set; }
        int MaxEpisodes { get; set; }
        List<TrainingSession> TrainingEpisodes { get; set; }
        int SaveQualityFrequency { get; set; }

        void InitializeStatesTable(int numberOfStates, int numberOfActions);
        void InitializeStatesTable();
        void InitializeRewardsTable(int numberOfStates, int numberOfActions);
        void InitializeRewardsTable();
        void InitializeQualityTable(int numberOfStates, int numberOfActions);
        void InitializeQualityTable();
        void Train();
        void RunMaze(int fromState);
        int GetNextState(int state, int action);
    }
}
