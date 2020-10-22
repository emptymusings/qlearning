using System;
using System.Collections.Generic;

namespace QLearningTutorial
{
    class QLearningProgram
    {
        static Random rnd = new Random(1);
        static void Main(string[] args)
        {
            IMaze maze = new OriginalMaze();

            Console.WriteLine("Begin Q-learning maze demo");
            Console.WriteLine("Setting up maze and rewards");
            int ns = 12;            
            int[][] FT = maze.CreateMaze(ns);
            double[][] R = maze.CreateRewards(ns);
            double[][] Q = maze.CreateQuality(ns);
            Console.WriteLine("Analyzing maze using Q-learning");
            int goal = 11;
            double gamma = 0.5;
            double learnRate = 0.5;
            int maxEpochs = 1000;
            maze.Train(FT, R, Q, goal, gamma, learnRate, maxEpochs);
            Console.WriteLine("Done. Q matrix: ");
            Print(Q);
            Console.WriteLine("Using Q to walk from cell 8 to 11");
            maze.Move(8, 11, Q);
            Console.WriteLine("End demo");
            Console.ReadLine();
        }
        
        static void Print(double[][] quality)
        {
            int ns = quality.Length;
            Console.WriteLine("[0] [1] . . [11]");
            for (int i = 0; i < ns; ++i)
            {
                for (int j = 0; j < ns; ++j)
                {
                    Console.Write(quality[i][j].ToString("F2") + " ");
                }
                Console.WriteLine();
            }
        }

    } // Program
} // ns