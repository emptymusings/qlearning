using System;
using System.Collections.Generic;
using System.Xml.Schema;

namespace QLearningTutorial
{
    class QLearningProgram
    {
        static Random rnd = new Random(1);
        static void Main(string[] args)
        {
            int rows = 3;
            int columns = 4;
            int startPosition = PromptPosition(rows * columns, "starting");
            int goalPosition = 11;
            double gamma = 0.6;
            double learnRate = 0.5;
            int maxEpochs = 1500;

            IMaze maze = MazeFactory.CreateMaze(
                MazeTypes.CustomMaze1,
                rows,
                columns,
                startPosition,
                goalPosition,
                gamma,
                learnRate,
                maxEpochs);

            maze.Train();
            
            Console.WriteLine();
            Console.WriteLine("Done. Quality matrix: ");
            maze.PrintQuality();
            Console.WriteLine();
            Console.WriteLine();
            
            Console.WriteLine($"Using Quality Matrix to walk from cell {startPosition} to {goalPosition}");
            maze.RunMaze();
            Console.WriteLine();
            
            Console.WriteLine("End demo");
            Console.ReadLine();
        }
        
        static int PromptPosition(int numberOfStates, string pointName)
        {
            bool invalidEntry = false;

            Console.Write($"Enter a {pointName} point between 0 and {numberOfStates - 1}: ");
            var entry = Console.ReadLine();
            int result = numberOfStates + 1;

            try
            {
                result = Convert.ToInt32(entry);

                if (result < 0 || result > numberOfStates - 1) invalidEntry = true;
            }
            catch
            {
                invalidEntry = true;
            }

            if (invalidEntry)
            {
                Console.WriteLine($"'{entry}' is not a valid entry");
                return PromptPosition(numberOfStates, pointName);
            }

            return result;
        }

    } // Program
} // ns