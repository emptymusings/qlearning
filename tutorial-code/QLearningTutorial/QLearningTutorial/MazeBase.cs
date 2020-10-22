using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace QLearningTutorial
{
    public abstract class MazeBase : IMaze
    {
        private Random random = new Random();
        
        public MazeBase(
            int numberOfStates,
            int startPosition,
            int goalPosition,
            double gamma,
            double learnRate,
            int maxEpochs)
        {
            this.NumberOfStates = numberOfStates;
            this.StartPosition = startPosition;
            this.GoalPosition = goalPosition;
            this.Gamma = gamma;
            this.LearnRate = learnRate;
            this.MaxEpochs = maxEpochs;
        }

        public int NumberOfStates { get; set; } = 12;
        public int GoalPosition { get; set; }
        public int StartPosition { get; set; }
        public double Gamma { get; set; }
        public double LearnRate { get; set; }
        public int MaxEpochs { get; set; }

        public int[][] MazeStates { get; set; }
        public double[][] Rewards { get; set; }
        public double[][] Quality { get; set; }

        public virtual void CreateMaze()
        {
            Console.WriteLine("Creating Maze States (Observation Space)");

            int[][] mazeNextStates = new int[NumberOfStates][];

            for (int i = 0; i < NumberOfStates; ++i) mazeNextStates[i] = new int[NumberOfStates];

            mazeNextStates[0][1] = mazeNextStates[0][4] = mazeNextStates[1][0] = mazeNextStates[1][5] = mazeNextStates[2][3] = 1;
            mazeNextStates[2][6] = mazeNextStates[3][2] = mazeNextStates[3][7] = mazeNextStates[4][0] = mazeNextStates[4][8] = 1;
            mazeNextStates[5][1] = mazeNextStates[5][6] = mazeNextStates[5][9] = mazeNextStates[6][2] = mazeNextStates[6][5] = 1;
            mazeNextStates[6][7] = mazeNextStates[7][3] = mazeNextStates[7][6] = mazeNextStates[7][11] = mazeNextStates[8][4] = 1;
            mazeNextStates[8][9] = mazeNextStates[9][5] = mazeNextStates[9][8] = mazeNextStates[9][10] = mazeNextStates[10][9] = 1;
            mazeNextStates[11][11] = 1;  // Goal

            MazeStates = mazeNextStates;
        }

        public virtual void CreateRewards()
        {
            Console.WriteLine("Creating Reward States");
            double[][] reward = new double[NumberOfStates][];

            for (int i = 0; i < NumberOfStates; ++i) reward[i] = new double[NumberOfStates];

            reward[0][1] = reward[0][4] = reward[1][0] = reward[1][5] = reward[2][3] = -0.1;
            reward[2][6] = reward[3][2] = reward[3][7] = reward[4][0] = reward[4][8] = -0.1;
            reward[5][1] = reward[5][6] = reward[5][9] = reward[6][2] = reward[6][5] = -0.1;
            reward[6][7] = reward[7][3] = reward[7][6] = reward[7][11] = reward[8][4] = -0.1;
            reward[8][9] = reward[9][5] = reward[9][8] = reward[9][10] = reward[10][9] = -0.1;
            reward[7][11] = 10.0;  // Goal

            Rewards = reward;
        }

        public virtual void CreateQuality()
        {
            Console.WriteLine("Creating Quality of States");

            double[][] quality = new double[NumberOfStates][];

            for (int i = 0; i < NumberOfStates; ++i)
            {
                quality[i] = new double[NumberOfStates];
            }
            
            Quality = quality;
        }

        public virtual List<int> GetPossibleNextStates(int currentState, int[][] mazeNextStates)
        {
            List<int> result = new List<int>();

            for (int j = 0; j < mazeNextStates.Length; ++j)
            {
                if (mazeNextStates[currentState][j] == 1)
                {
                    result.Add(j);
                }
            }

            return result;
        }

        public virtual int GetRandomNextState(int currentState, int[][] mazeNextStates)
        {
            List<int> possibleNextStates = GetPossibleNextStates(currentState, mazeNextStates);

            int count = possibleNextStates.Count;
            int index = random.Next(0, count);

            return possibleNextStates[index];
        }

        public virtual void Train()
        {
            CreateMaze();
            CreateRewards();
            CreateQuality();

            for (int epoch = 0; epoch < MaxEpochs; ++epoch)
            {
                int currState = random.Next(0, Rewards.Length);

                while (true)
                {
                    int nextState = GetRandomNextState(currState, MazeStates);
                    List<int> possNextNextStates = GetPossibleNextStates(nextState, MazeStates);
                    double maxQ = double.MinValue;

                    for (int j = 0; j < possNextNextStates.Count; ++j)
                    {
                        int nnumberOfStates = possNextNextStates[j];  // short alias
                        double q = Quality[nextState][nnumberOfStates];
                        if (q > maxQ) maxQ = q;
                    }

                    Quality[currState][nextState] = ((1 - LearnRate) * Quality[currState][nextState]) + (LearnRate * (Rewards[currState][nextState] + (Gamma * maxQ)));
                    currState = nextState;

                    if (currState == GoalPosition) break;
                }
            }
        }

        public virtual void RunMaze()
        {
            int curr = StartPosition; int next;

            Console.Write(curr + "->");

            while (curr != GoalPosition)
            {
                next = ArgMax(Quality[curr]);
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
