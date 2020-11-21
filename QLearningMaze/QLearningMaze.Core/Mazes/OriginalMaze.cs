namespace QLearningMaze.Core.Mazes
{
    class OriginalMaze : MazeBaseOld, IMazeOld
    {
        public OriginalMaze(
            int rows,
            int columns,
            int startPosition,
            int goalPosition,
            double discountRate,
            double learningRate,
            int maxEpisodes) 
        : base (
            rows,
            columns,
            startPosition,
            goalPosition,
            discountRate,
            learningRate,
            maxEpisodes)
        {

        }

        protected override void InitializeObservationSpace()
        {
            base.InitializeObservationSpace();
            AddObstruction(0, 1);
            AddObstruction(1, 2);
            AddObstruction(4, 5);
            AddObstruction(6, 10);
            AddObstruction(10, 11);
        }
    }
}
