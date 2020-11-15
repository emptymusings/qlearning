using System;
using System.Collections.Generic;
using System.Text;

namespace QLearning.Core
{
    public interface IQLearning
    {
        int[][] StatesTable { get; set; }
        double[][] RewardsTable { get; set; }
        double[][] QualityTable { get; set; }
        double EpsilonDecayStart { get; set; }
        double EpsilonDecayEnd { get; set; }
        double LearningRate { get; set; }
        double DiscountRate { get; set; }
        List<int> ObjectiveStates { get; set; }
        public int MaximumAllowedMoves { get; set; }
        double ObjectiveReward { get; set; }
        string QualitySaveDirectory { get; set; }
        int MaximumAllowedBacktracks { get; set; }
        List<TrainingSession> TrainingEpisodes { get; set; }
        int SaveQualityFrequency { get; set; }
        int PhaseSize { get; set; }

        void InitializeStatesTable(int numberOfStates, int numberOfActions);
        void InitializeStatesTable();
        void InitializeRewardsTable(int numberOfStates, int numberOfActions);
        void InitializeRewardsTable();
        void InitializeQualityTable(int numberOfStates, int numberOfActions);
        void InitializeQualityTable();
        void Train(int numberOfEpisodes);
        void StartAgent(int fromState);
    }
}
