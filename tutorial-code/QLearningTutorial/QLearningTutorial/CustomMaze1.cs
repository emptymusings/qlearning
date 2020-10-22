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
            int numberOfStates,
            int startPosition,
            int goalPosition,
            double gamma,
            double learnRate,
            int maxEpochs)
        : base(
            numberOfStates,
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
            MazeStates[5][6] = MazeStates[6][5] = 0;
            MazeStates[10][11] = MazeStates[11][10] = 1;
            
        }

        protected override void CreateRewards()
        {
            base.CreateRewards();
            Rewards[5][6] = MazeStates[6][5] = 0;
            Rewards[7][11] = Rewards[11][10] = -0.1;
            Rewards[10][11] = 10.0;
        }
    }
}
