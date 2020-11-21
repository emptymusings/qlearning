namespace QLearningMaze.Core.Mazes
{
    public class UserDefinedMaze : MazeBaseOld, IMazeOld
    {
        public UserDefinedMaze(
            int rows = 4,
            int columns = 4,
            int startPosition = 0,
            int goalPosition = 0,
            double discountRate = 0.5,
            double learningRate = 0.5,
            int maxEpisodes = 1000)
        : base(
            rows,
            columns,
            startPosition,
            goalPosition,
            discountRate,
            learningRate,
            maxEpisodes)
        {

        }
    }
}
