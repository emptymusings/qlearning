using System;
using System.Collections.Generic;
using System.Text;

namespace QLearningMaze
{
    public enum MazeTypes
    {
        Undefined,
        OriginalMaze,
        CustomMaze1,
        UserDefined
    }
    class MazeFactory
    {
        public static IMaze CreateMaze(
            MazeTypes mazeType,
            int rows,
            int columns,
            int startPosition,
            int goalPosition,
            double discountRate,
            double learningRate,
            int maxEpochs)
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
                        maxEpochs);
                case MazeTypes.CustomMaze1:
                    return new CustomMaze1(
                        rows,
                        columns,
                        startPosition,
                        goalPosition,
                        discountRate,
                        learningRate,
                        maxEpochs);
                case MazeTypes.UserDefined:
                    return new UserDefinedMaze(
                        rows,
                        columns,
                        startPosition,
                        goalPosition,
                        discountRate,
                        learningRate,
                        maxEpochs);
                default:
                    throw new ArgumentOutOfRangeException("MazeType");
            }
        }
    }
}
