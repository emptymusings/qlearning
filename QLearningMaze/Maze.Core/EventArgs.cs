namespace QLearningMaze.Core
{
    using System;
    using Mazes;

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

    public class ObstructionEventArgs : EventArgs
    {
        public MazeObstruction Obstruction { get; set; }
    }

    public class AgentStateChangedEventArgs : EventArgs
    {
        public AgentStateChangedEventArgs(int newPosition, int movesMade, double rewardsEarned)
        {
            this.NewPosition = newPosition;
            this. RewardsEarned = rewardsEarned;
            this.MovesMade = movesMade;
        }

        public int NewPosition { get; set; }
        public double RewardsEarned { get; set; }
        public int MovesMade { get; set; }
    }
}
