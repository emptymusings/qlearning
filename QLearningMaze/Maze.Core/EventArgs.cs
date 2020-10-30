namespace QLearningMaze.Core
{
    using System;
    using Mazes;

    public class TrainingEpochCompletedEventArgs : EventArgs
    {
        public TrainingEpochCompletedEventArgs(int currentEpoch, int totalEpochs, int totalMoves, double totalScore, bool succeeded)
        {
            CurrentEpoch = currentEpoch;
            TotalEpochs = totalEpochs;
            TotalMoves = totalMoves;
            TotalScore = totalScore;
            Success = succeeded;

        }

        public int CurrentEpoch { get; set; }
        public int TotalEpochs { get; set; }
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
