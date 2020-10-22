using System;
using System.Collections.Generic;
using System.Text;

namespace QLearningTutorial
{
    class OriginalMaze : MazeBase, IMaze
    {
        public OriginalMaze(
            int numberOfStates,
            int startPosition,
            int goalPosition,
            double gamma,
            double learnRate,
            int maxEpochs) 
        : base (
            numberOfStates,
            startPosition,
            goalPosition,
            gamma,
            learnRate,
            maxEpochs)
        {

        }
    }
}
