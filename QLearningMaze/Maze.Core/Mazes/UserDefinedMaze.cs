using System;
using System.Collections.Generic;
using System.Text;

namespace QLearningMaze.Core.Mazes
{
    class UserDefinedMaze : MazeBase, IMaze
    {
        public UserDefinedMaze(
            int rows,
            int columns,
            int startPosition,
            int goalPosition,
            double discountRate,
            double learningRate,
            int maxEpochs)
        : base(
            rows,
            columns,
            startPosition,
            goalPosition,
            discountRate,
            learningRate,
            maxEpochs)
        {

        }
    }
}
