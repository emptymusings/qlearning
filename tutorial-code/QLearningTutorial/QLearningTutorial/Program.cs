using System;
using System.Collections.Generic;

namespace QLearningTutorial
{
    class QLearningProgram
    {
        static Random rnd = new Random(1);
        static void Main(string[] args)
        {
            int numberOfStates = 12;
            int startPosition = 8;
            int goalPosition = 11;
            double gamma = 0.5;
            double learnRate = 0.5;
            int maxEpochs = 1000;

            IMaze maze = new OriginalMaze(
                numberOfStates,
                startPosition,
                goalPosition,
                gamma,
                learnRate,
                maxEpochs);

            maze.Train();
            
            Console.WriteLine();
            Console.WriteLine("Done. Quality matrix: ");
            Print(maze.Quality);
            Console.WriteLine();
            Console.WriteLine();
            
            Console.WriteLine($"Using Quality Matrix to walk from cell {startPosition} to {goalPosition}");
            maze.RunMaze();
            Console.WriteLine();
            
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