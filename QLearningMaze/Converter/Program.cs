namespace Converter
{
    using System;
    using System.IO;
    using QLearningMaze.Core;
    using QLearningMaze.Core.Mazes;
    using QLearning.Core;

    class Program
    {
        static string _savedDirectory = @"C:\Dev\.Net\q-learning\qlearning-sharp\QLearningMaze\QLearningMaze.Core\assets\TestMazes\";
        static string _history = _savedDirectory + "Converted\\";
        static void Main(string[] args)
        {
            foreach(string f in Directory.GetFiles(_savedDirectory))
            {
                string fileName = Path.GetFileName(f);
                Console.WriteLine($"Converting {fileName}");

                var oldMaze = MazeUtilities.LoadObject<MazeBase>(f);
                
                var newMaze = new MazeBaseNew
                {
                    AdditionalRewards = oldMaze.AdditionalRewards,
                    Columns = oldMaze.Columns,
                    GetRewardAction = oldMaze.GetRewardAction,
                    GoalPosition = oldMaze.GoalPosition,
                    MovementPunishement = oldMaze.MovementPunishement,
                    ObjectiveAction = oldMaze.ObjectiveAction,
                    ObjectiveReward = oldMaze.ObjectiveReward,
                    ObjectiveStates = oldMaze.ObjectiveStates,
                    Obstructions = oldMaze.Obstructions,
                    PrioritizeFromState = oldMaze.PrioritizeFromState,
                    QualitySaveDirectory = oldMaze.QualitySaveDirectory,
                    QualitySaveFrequency = oldMaze.QualitySaveFrequency,
                    QualityTable = oldMaze.QualityTable,
                    RewardsTable = oldMaze.RewardsTable,
                    Rows = oldMaze.Rows,
                    StartPosition = oldMaze.StartPosition,
                    StatesPerPhase = oldMaze.StatesPerPhase,
                    StatesTable = oldMaze.StatesTable
                };

                var agent = new AgentBase<IMazeNew>
                {
                    DiscountRate = oldMaze.DiscountRate,
                    Environment = newMaze,
                    EpsilonDecayEnd = oldMaze.EpsilonDecayEnd,
                    EpsilonDecayStart = oldMaze.EpsilonDecayStart,
                    LearningRate = oldMaze.LearningRate,
                    MaximumAllowedBacktracks = oldMaze.MaximumAllowedBacktracks,
                    MaximumAllowedMoves = oldMaze.MaximumAllowedMoves,
                    NumberOfTrainingEpisodes = oldMaze.NumberOfTrainingEpisodes,
                    TrainingEpisodes = oldMaze.TrainingEpisodes
                };

                Console.WriteLine($"Backing up {fileName}");
                File.Move(f, _history + fileName);

                Console.WriteLine("Saving converted file");
                MazeUtilities.SaveObject(_savedDirectory + fileName, agent);
            }
        }
    }
}
