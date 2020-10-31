
namespace QLearningMaze.Core
{
    using Mazes;
    using System;

    public enum MazeTypes
    {
        Undefined,
        OriginalMaze,
        UserDefined
    }
    public class MazeFactory
    {
        public static IMaze CreateMaze(
            MazeTypes mazeType = MazeTypes.Undefined,
            int rows = 4,
            int columns = 4,
            int startPosition = 0,
            int goalPosition = 0,
            double discountRate = 0.5,
            double learningRate = 0.5,
            int maxEpisodes = 1000)
        {
            switch(mazeType)
            {
                case MazeTypes.OriginalMaze:
                    return new OriginalMaze(
                        rows,
                        columns,
                        startPosition,
                        goalPosition,
                        discountRate,
                        learningRate,
                        maxEpisodes);
                case MazeTypes.UserDefined:
                    return new UserDefinedMaze(
                        rows,
                        columns,
                        startPosition,
                        goalPosition,
                        discountRate,
                        learningRate,
                        maxEpisodes);
                default:
                    throw new ArgumentOutOfRangeException("MazeType");
            }
        }
    }
}
