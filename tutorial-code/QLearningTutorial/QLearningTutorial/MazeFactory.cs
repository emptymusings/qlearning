using System;
using System.Collections.Generic;
using System.Text;

namespace QLearningTutorial
{
    public enum MazeTypes
    {
        Undefined,
        OriginalMaze,
        CustomMaze1
    }
    class MazeFactory
    {
        public static IMaze CreateMaze(
            MazeTypes mazeType,
            int rows,
            int columns,
            int startPosition,
            int goalPosition,
            double gamma,
            double learnRate,
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
                        gamma,
                        learnRate,
                        maxEpochs);
                case MazeTypes.CustomMaze1:
                    return new CustomMaze1(
                        rows,
                        columns,
                        startPosition,
                        goalPosition,
                        gamma,
                        learnRate,
                        maxEpochs);
                default:
                    throw new ArgumentOutOfRangeException("MazeType");
            }
        }
    }
}
