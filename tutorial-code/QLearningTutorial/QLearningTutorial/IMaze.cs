using System;
using System.Collections.Generic;
using System.Text;

namespace QLearningTutorial
{
    interface IMaze
    {
        int NumberOfStates { get; set; }
        int GoalPosition { get; set; }
        int StartPosition { get; set; }
        double Gamma { get; set; }
        double LearnRate { get; set; }
        int MaxEpochs { get; set; }
        int[][] MazeStates { get; set; }
        double[][] Rewards { get; set; }
        double[][] Quality { get; set; }
        void CreateMaze();
        void CreateRewards();
        void CreateQuality();
        List<int> GetPossibleNextStates(int currentState, int[][] mazeNextStates);
        int GetRandomNextState(int currentState, int[][] mazeNextStates);
        void Train();
        void RunMaze();
        int ArgMax(double[] vector);
    }
}
