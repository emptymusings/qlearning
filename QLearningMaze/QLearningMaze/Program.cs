namespace QLearningMaze
{
    using System;
    using Core;

    class QLearningMazeProgram
    {
        static Random rnd = new Random(1);
        static void Main(string[] args)
        {
            int rows = PromptDimension("rows", 3);
            int columns = PromptDimension("columns", 4);
            int startPosition = PromptPosition(rows * columns, "starting");
            int goalPosition = PromptPosition(rows * columns, "goal");
            double discountRate = PromptRate(0.6, "discount");
            double learningRate = PromptRate(0.5, "learning");
            int maxEpochs = PromptEpochs(1000);

            IMaze maze = MazeFactory.CreateMaze(
                MazeTypes.UserDefined,
                rows,
                columns,
                startPosition,
                goalPosition,
                discountRate,
                learningRate,
                maxEpochs);

            PromptWalls(maze);

            maze.Train();
            
            Console.WriteLine();
            Console.WriteLine("Done. Quality matrix: ");
            maze.PrintQuality();
            Console.WriteLine();
            Console.WriteLine();
            
            Console.WriteLine($"Using Quality Matrix to walk from cell {startPosition} to {goalPosition}");
            maze.RunMaze();
            Console.WriteLine();

            Console.Write("Maze completed.  Do you want to do it again (yes/no)? ");
            var response = Console.ReadLine();

            if (string.IsNullOrEmpty(response))
                return;
            else if (response.ToLower() == "yes")
                Main(args);
        }

        static int PromptDimension(string dimensionType, int defaultValue)
        {
            bool invalidEntry = false;
            int result = defaultValue;

            Console.Write($"How many {dimensionType} should there be in the maze ({result})? ");
            var entry = Console.ReadLine();

            if (!string.IsNullOrEmpty(entry))
            {
                try
                {
                    result = Convert.ToInt32(entry);
                }
                catch
                {
                    invalidEntry = true;
                }
            }

            if (invalidEntry)
            {
                Console.WriteLine($"'{entry}' is not a valid entry");
                return PromptDimension(dimensionType, defaultValue);
            }

            return result;
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

        static int PromptEpochs(int defaultValue)
        {
            bool invalidEntry = false;
            int result = defaultValue;

            Console.Write($"How many simulations should be run for training ({result})? ");
            var entry = Console.ReadLine();

            if (!string.IsNullOrEmpty(entry))
            {
                try
                {
                    result = Convert.ToInt32(entry);
                }
                catch
                {
                    invalidEntry = true;
                }
            }

            if (invalidEntry)
            {
                Console.WriteLine($"'{entry}' is not a valid entry");
                return PromptEpochs(defaultValue);
            }

            return result;
        }

        static double PromptRate(double defaultValue, string rateType)
        {
            bool invalidEntry = false;
            double result = defaultValue;

            Console.Write($"What should be the {rateType} rate ({result})? ");
            var entry = Console.ReadLine();

            if (!string.IsNullOrEmpty(entry))
            {
                try
                {
                    result = Convert.ToDouble(entry);
                }
                catch
                {
                    invalidEntry = true;
                }
            }

            if (invalidEntry)
            {
                Console.WriteLine($"'{entry}' is not a valid entry");
                return PromptRate(defaultValue, rateType);
            }

            return result;
        }

        static void PromptWalls(IMaze maze)
        {
            Console.Write("Would you like to add a wall in the maze (yes/no)? ");
            var entry = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(entry) ||
                ((entry.ToLower() != "no") &&
                (entry.ToLower() != "yes")))
            {
                Console.WriteLine("Invalid entry.  Please type 'yes' or 'no'");
                PromptWalls(maze);
            }

            if (entry.ToLower() == "yes")
            {
                var wallInfo = PromptWallInfo();

                maze.AddWall(wallInfo.betweenSpace, wallInfo.andSpace);

                PromptWalls(maze);
            }
        }

        static (int betweenSpace, int andSpace) PromptWallInfo()
        {
            (int betweenSpace, int andSpace) result;

            Console.Write("Enter the first adjacent space to the wall: ");
            var entryBetween = Console.ReadLine();
            result.betweenSpace = Convert.ToInt32(entryBetween);

            Console.Write("Enter the second adjacent space to the wall: ");
            var entryAnd = Console.ReadLine();
            result.andSpace = Convert.ToInt32(entryAnd);

            return result;
        }
    } // Program
} // ns