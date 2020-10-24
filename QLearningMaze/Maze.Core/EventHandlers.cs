namespace QLearningMaze.Core
{
    using System;
    using Mazes;

    public class TrainingEpochCompletedEventArgs : EventArgs
    {
        public int CurrentEpoch { get; set; }
        public int TotalEpochs { get; set; }
    }

    public class ObstructionEventArgs : EventArgs
    {
        public MazeObstruction Obstruction { get; set; }
    }
}
