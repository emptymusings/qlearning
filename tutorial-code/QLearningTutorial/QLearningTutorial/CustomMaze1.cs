using System;
using System.Collections.Generic;
using System.Text;

namespace QLearningTutorial
{
    /// <summary>
    /// Based off of the original maze, but adds a wall between spaces 5 & 6, and removes
    /// the wall between 10 and 11
    /// </summary>
    public class CustomMaze1 : MazeBase, IMaze
    {
        public CustomMaze1(
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

        protected override void CreateMazeStates()
        {
            base.CreateMazeStates();
            AddWall(0, 1);            
            AddWall(4, 5);
            AddWall(9, 10);
            AddWall(5, 6);
            AddWall(2, 3);
            AddWall(6, 10);
        }

        protected override void CreateRewards()
        {
            base.CreateRewards();
        }
    }
}
