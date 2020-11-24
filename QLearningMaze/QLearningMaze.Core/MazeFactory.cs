
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
        public static IMazeNew CreateMaze(
            int rows,
            int columns,
            int startPosition,
            int goalPosition,
            double goalValue)
        {
            var maze = new MazeBaseNew(
                columns,
                rows,
                startPosition,
                goalPosition,
                goalValue);

            return maze;
        }
    }
}
