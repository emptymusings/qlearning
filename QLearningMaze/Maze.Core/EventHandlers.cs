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
}
