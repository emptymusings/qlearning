using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace QLearningTutorial
{
    abstract class MazeBase : IMaze
    {
        private Random random = new Random();

        public virtual int[][] CreateMaze(int ns)
        {
            int[][] fromTo = new int[ns][];
            for (int i = 0; i < ns; ++i) fromTo[i] = new int[ns];
            fromTo[0][1] = fromTo[0][4] = fromTo[1][0] = fromTo[1][5] = fromTo[2][3] = 1;
            fromTo[2][6] = fromTo[3][2] = fromTo[3][7] = fromTo[4][0] = fromTo[4][8] = 1;
            fromTo[5][1] = fromTo[5][6] = fromTo[5][9] = fromTo[6][2] = fromTo[6][5] = 1;
            fromTo[6][7] = fromTo[7][3] = fromTo[7][6] = fromTo[7][11] = fromTo[8][4] = 1;
            fromTo[8][9] = fromTo[9][5] = fromTo[9][8] = fromTo[9][10] = fromTo[10][9] = 1;
            fromTo[11][11] = 1;  // Goal
            return fromTo;
        }

        public virtual double[][] CreateRewards(int ns)
        {
            double[][] reward = new double[ns][];
            for (int i = 0; i < ns; ++i) reward[i] = new double[ns];
            reward[0][1] = reward[0][4] = reward[1][0] = reward[1][5] = reward[2][3] = -0.1;
            reward[2][6] = reward[3][2] = reward[3][7] = reward[4][0] = reward[4][8] = -0.1;
            reward[5][1] = reward[5][6] = reward[5][9] = reward[6][2] = reward[6][5] = -0.1;
            reward[6][7] = reward[7][3] = reward[7][6] = reward[7][11] = reward[8][4] = -0.1;
            reward[8][9] = reward[9][5] = reward[9][8] = reward[9][10] = reward[10][9] = -0.1;
            reward[7][11] = 10.0;  // Goal
            return reward;
        }

        public virtual double[][] CreateQuality(int ns)
        {
            double[][] quality = new double[ns][];
            for (int i = 0; i < ns; ++i)
                quality[i] = new double[ns];
            return quality;
        }

        public virtual List<int> GetPossibleNextStates(int currentState, int[][] fromTo)
        {
            List<int> result = new List<int>();

            for (int j = 0; j < fromTo.Length; ++j)
            {
                if (fromTo[currentState][j] == 1)
                {
                    result.Add(j);
                }
            }

            return result;
        }

        public virtual int GetRandomNextState(int currentState, int[][] fromTo)
        {
            List<int> possibleNextStates = GetPossibleNextStates(currentState, fromTo);

            int count = possibleNextStates.Count;
            int index = random.Next(0, count);

            return possibleNextStates[index];
        }

        public virtual void Train(int[][] fromTo, double[][] rewards, double[][] quality, int goal, double gamma, double learnRate, int maxEpochs)
        {
            for (int epoch = 0; epoch < maxEpochs; ++epoch)
            {
                int currState = random.Next(0, rewards.Length);

                while (true)
                {
                    int nextState = GetRandomNextState(currState, fromTo);
                    List<int> possNextNextStates = GetPossibleNextStates(nextState, fromTo);
                    double maxQ = double.MinValue;

                    for (int j = 0; j < possNextNextStates.Count; ++j)
                    {
                        int nns = possNextNextStates[j];  // short alias
                        double q = quality[nextState][nns];
                        if (q > maxQ) maxQ = q;
                    }

                    quality[currState][nextState] = ((1 - learnRate) * quality[currState][nextState]) + (learnRate * (rewards[currState][nextState] + (gamma * maxQ)));
                    currState = nextState;
                    if (currState == goal) break;
                }
            }
        }

        public virtual void Move(int start, int goal, double[][] quality)
        {
            int curr = start; int next;
            Console.Write(curr + "->");
            while (curr != goal)
            {
                next = ArgMax(quality[curr]);
                Console.Write(next + "->");
                curr = next;
            }
            Console.WriteLine("done");
        }

        public virtual int ArgMax(double[] vector)
        {
            double maxVal = vector[0]; int idx = 0;
            for (int i = 0; i < vector.Length; ++i)
            {
                if (vector[i] > maxVal)
                {
                    maxVal = vector[i]; idx = i;
                }
            }
            return idx;
        }
    }
}
