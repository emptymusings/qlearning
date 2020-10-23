namespace QLearningMaze
{
    using System;
    using System.IO;
    using Core;

    class QLearningMazeProgram
    {
        static bool _promptSave = true;

        static void Main(string[] args)
        {
            string response;
            IMaze maze = InitializeMaze();
            
            if (maze == null)
                Main(args);

            Console.WriteLine($"Running maze from cell {maze.StartPosition} to {maze.GoalPosition}");
            maze.RunMaze();
            Console.WriteLine();

            if (_promptSave)
            {
                Console.Write("Maze completed.  Do you want to save this maze (Y/N)? ");
                response = Console.ReadLine();

                if (response.ToLower() == "y")
                {
                    Console.Write("Enter a name for your maze: ");
                    var name = Console.ReadLine();

                    if (!name.EndsWith(".maze"))
                        name += ".maze";

                    MazeUtilities.SaveMaze(name, maze);
                }
            }

            Console.Write("All done.  Do you want to restart (Y/N)? ");
            response = Console.ReadLine();

            if (string.IsNullOrEmpty(response))
                return;
            else if (response.ToLower() == "y")
                Main(args);
        }

        static IMaze InitializeMaze()
        {
            IMaze maze = null;
            _promptSave = true;

            Console.Write("Do you want to (C)reate a new maze or (L)oad a Maze (C/L)? ");
            var response = Console.ReadLine();

            if (response.ToLower() == "c")
            {
                maze = MazeFactory.CreateMaze(MazeTypes.UserDefined);
                SetupMaze(maze);
                TrainMaze(maze);
            }
            else if (response.ToLower() == "l")
            {
                maze = LoadMaze();

                if (maze == null)
                {
                    return null;
                }

                Console.Write("Do you want to make modifications to the maze (Y/N)? ");
                response = Console.ReadLine();

                if (response.ToLower() == "y")
                {
                    SetupMaze(maze);
                    TrainMaze(maze);
                }
                else
                {
                    _promptSave = false;
                }
            }

            return maze;
        }

        static void SetupMaze(IMaze maze)
        {
            maze.Rows = PromptDimension("rows", maze.Rows);
            maze.Columns = PromptDimension("columns", maze.Columns);
            maze.StartPosition = PromptPosition(maze.Rows * maze.Columns, "starting", maze.StartPosition);
            maze.GoalPosition = PromptPosition(maze.Rows * maze.Columns, "goal", maze.GoalPosition);
            maze.DiscountRate = PromptRate(maze.DiscountRate, "discount");
            maze.LearningRate = PromptRate(maze.LearningRate, "learning");
            maze.MaxEpochs = PromptEpochs(maze.MaxEpochs);

            PromptWalls(maze);
        }

        static IMaze LoadMaze()
        {
            Console.Write("Type the name of the maze you want to load: ");
            string mazeName = Console.ReadLine();

            if (!mazeName.EndsWith(".maze")) mazeName += ".maze";

            if (!File.Exists(mazeName))
            {
                Console.WriteLine($"No maze named '{mazeName}' was found");
                return null;
            }
            else
            {
                return MazeUtilities.LoadMaze(mazeName);
            }
        }

        static void TrainMaze(IMaze maze)
        {
            maze.Train();

            Console.WriteLine();
            Console.WriteLine("Quality matrix: ");
            maze.PrintQuality();
            Console.WriteLine();
            Console.WriteLine();
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

        static int PromptPosition(int numberOfStates, string pointName, int currentPosition)
        {
            bool invalidEntry = false;

            Console.Write($"Enter a {pointName} point between 0 and {numberOfStates - 1} ({currentPosition}): ");
            var entry = Console.ReadLine();
            int result = numberOfStates + 1;

            if (string.IsNullOrWhiteSpace(entry))
            {
                entry = currentPosition.ToString();
            }

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
                return PromptPosition(numberOfStates, pointName, currentPosition);
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
            Console.Write("Would you like to add a wall in the maze (Y/N)? ");
            var entry = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(entry) ||
                ((entry.ToLower() != "n") &&
                (entry.ToLower() != "y")))
            {
                Console.WriteLine("Invalid entry.  Please type 'Y' or 'N'");
                PromptWalls(maze);
            }

            if (entry.ToLower() == "y")
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