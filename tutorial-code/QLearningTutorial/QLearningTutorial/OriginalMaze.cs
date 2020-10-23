using System;
using System.Collections.Generic;
using System.Text;

namespace QLearningTutorial
{
    class OriginalMaze : MazeBase, IMaze
    {
        public OriginalMaze(
            int rows,
            int columns,
            int startPosition,
            int goalPosition,
            double gamma,
            double learnRate,
            int maxEpochs) 
        : base (
            rows,
            columns,
            startPosition,
            goalPosition,
            gamma,
            learnRate,
            maxEpochs)
        {

        }

        protected override void CreateMazeStates()
        {
            base.CreateMazeStates();
            AddWall(0, 1);
            AddWall(1, 2);
            AddWall(4, 5);
            AddWall(6, 10);
            AddWall(10, 11);
        }
    }
}
