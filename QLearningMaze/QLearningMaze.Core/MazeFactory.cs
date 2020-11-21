
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
            int rows,
            int columns,
            int startPosition,
            int goalPosition,
            double discountRate,
            double learningRate,
            int trainingEpisodes)
        {
            var maze = new MazeBase(
                columns,
                rows,
                startPosition,
                goalPosition,
                discountRate,
                learningRate);

            maze.NumberOfTrainingEpisodes = trainingEpisodes;

            return maze;
        }
    }
}
