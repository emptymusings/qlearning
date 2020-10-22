using System;
using System.Collections.Generic;
using System.Text;

namespace QLearningTutorial
{
    interface IMaze
    {
        int NumberOfStates { get; }
        int Rows { get; set; }
        int Columns { get; set; }
        int GoalPosition { get; set; }
        int StartPosition { get; set; }
        double Gamma { get; set; }
        double LearnRate { get; set; }
        int MaxEpochs { get; set; }
        int[][] MazeStates { get; set; }
        double[][] Rewards { get; set; }
        double[][] Quality { get; set; }

        void Train();
        void RunMaze();
    }
}
