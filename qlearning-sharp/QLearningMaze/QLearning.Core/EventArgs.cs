namespace QLearning.Core
{
    using System;

    public class TrainingEpisodeCompletedEventArgs : EventArgs
    {
        public TrainingEpisodeCompletedEventArgs(int currentEpisode, int totalEpisodes, int totalMoves, double totalScore, bool succeeded)
        {
            CurrentEpisode = currentEpisode;
            TotalEpisodes = totalEpisodes;
            TotalMoves = totalMoves;
            TotalScore = totalScore;
            Success = succeeded;

        }

        public int CurrentEpisode { get; set; }
        public int TotalEpisodes { get; set; }
        public int TotalMoves { get; set; }
        public double TotalScore { get; set; }
        public bool Success { get; set; }
    }
    public class AgentStateChangedEventArgs : EventArgs
    {
        public AgentStateChangedEventArgs(int newState, int movesMade, double rewardsEarned)
        {
            this.NewState = newState;
            this.RewardsEarned = rewardsEarned;
            this.MovesMade = movesMade;
        }

        public int NewState { get; set; }
        public double RewardsEarned { get; set; }
        public int MovesMade { get; set; }
    }

    public class TrainingAgentStateChangedEventArgs : AgentStateChangedEventArgs
    {
        public TrainingAgentStateChangedEventArgs(
            int action,
            int state,
            int movesMade,
            double rewardsEarned,
            double newQuality,
            double oldQuality)
            : base(state, movesMade, rewardsEarned)
        {
            Action = action;
            NewQuality = newQuality;
            OldQuality = oldQuality;
        }

        public int Action { get; set; }
        public double NewQuality { get; set; }
        public double OldQuality { get; set; }
    }

    public class AgentCompletedEventArgs : EventArgs
    {
        public AgentCompletedEventArgs(int moves, double rewards, bool succeeded)
        {
            Moves = moves;
            Rewards = rewards;
            Succeeded = succeeded;
        }

        public int Moves { get; set; }
        public double Rewards { get; set; }
        public bool Succeeded { get; set; }
    }
}
