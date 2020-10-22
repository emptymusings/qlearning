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

        protected override void CreateMaze()
        {
            base.CreateMaze();

            MazeStates[0][1] = MazeStates[0][4] = 1;
            MazeStates[1][0] = MazeStates[1][5] = 1;
            MazeStates[2][3] = MazeStates[2][6] = 1;
            MazeStates[3][2] = MazeStates[3][7] = 1;
            MazeStates[4][0] = MazeStates[4][8] = 1;
            MazeStates[5][1] = MazeStates[5][6] = MazeStates[5][9] = 1;
            MazeStates[6][2] = MazeStates[6][5] = MazeStates[6][7] = 1;
            MazeStates[7][3] = MazeStates[7][6] = MazeStates[7][11] = 1;
            MazeStates[8][4] = MazeStates[8][9] = 1;
            MazeStates[9][5] = MazeStates[9][8] = MazeStates[9][10] = 1;
            MazeStates[10][9] = 1;
            MazeStates[11][11] = 1;  // Goal - Note that there is no pathway FROM space 11 to any other space
        }
    }
}
