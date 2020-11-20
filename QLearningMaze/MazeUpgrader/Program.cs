namespace MazeUpgrader
{
    using System;
    using System.IO;
    using QLearning.Core;
    using QLearningMaze.Core;
    using QLearningMaze.Core.Mazes;

    class Program
    {
        static string _folder = @"C:\Dev\.Net\q-learning\qlearning-sharp\QLearningMaze\QLearningMaze.Core\assets\TestMazes\";
        static string _converted = _folder + @"Converted\";
        static void Main(string[] args)
        {

            if (!Directory.Exists(_converted))
                Directory.CreateDirectory(_converted);

            foreach(string file in Directory.GetFiles(_folder))
            {
                var maze = MazeUtilities.LoadObject<MazeBaseNew>(file);
                maze.GetRewardAction = 5;
                MazeUtilities.SaveObject(file, maze);
            }
        }

        static void CopyAndUpgrade(string file)
        {
            var fn = Path.GetFileName(file);
            var oldMaze = MazeUtilities.LoadObject<UserDefinedMaze>(file);
            var newMaze = new MazeBaseNew
            {
                Columns = oldMaze.Columns,
                DiscountRate = oldMaze.DiscountRate,
                GoalPosition = oldMaze.GoalPosition,
                LearningRate = oldMaze.LearningRate,
                NumberOfTrainingEpisodes = oldMaze.MaxEpisodes,
                StatesTable = oldMaze.ObservationSpace,
                Obstructions = oldMaze.Obstructions,
                QualityTable = oldMaze.Quality,
                RewardsTable = oldMaze.Rewards,
                Rows = oldMaze.Rows,
                QualitySaveFrequency = oldMaze.SaveQualityFrequency,
                StartPosition = oldMaze.StartPosition,
                StatesPerPhase = oldMaze.TotalSpaces
            };

            foreach (var reward in oldMaze.AdditionalRewards)
            {
                newMaze.AddReward(reward.Position, reward.Value, (reward.Value >= 0));
            }

            File.Move(file, _converted + fn);
            MazeUtilities.SaveObject(_folder + fn, newMaze);

            Console.WriteLine($"Upgraded {fn}");
        }
    }
}
