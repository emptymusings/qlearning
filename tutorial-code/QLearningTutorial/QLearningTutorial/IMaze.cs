using System;
using System.Collections.Generic;
using System.Text;

namespace QLearningTutorial
{
    interface IMaze
    {
        int[][] CreateMaze(int ns);
        double[][] CreateRewards(int ns);
        double[][] CreateQuality(int ns);
        List<int> GetPossibleNextStates(int currentState, int[][] fromTo);
        int GetRandomNextState(int currentState, int[][] fromTo);
        void Train(int[][] fromTo, double[][] rewards, double[][] quality, int goal, double gamma, double learnRate, int maxEpochs);
        void Move(int start, int goal, double[][] quality);
        int ArgMax(double[] vector);
    }
}
