namespace QLearningMaze.Core
{
    using Mazes;
    using Newtonsoft.Json;
    using QLearning.Core.Agent;
    using System.IO;

    public class MazeUtilities
    {
        public static readonly string TRAINING_SESSIONS_DIRECTORY = Directory.GetCurrentDirectory() + $@"\TrainingSessions\";

        public static void SaveObject(string saveToPath, object objectToSave)
        {
            string json = JsonConvert.SerializeObject(objectToSave, Formatting.Indented);
            File.WriteAllText(saveToPath, json);
        }

        public static T LoadObject<T>(string loadFromPath)
        {
            T loaded = JsonConvert.DeserializeObject<T>(File.ReadAllText(loadFromPath));
            return loaded;
        }

        public static ITDAgent<IMaze> ConvertLoadedAgent(TDAgent<MazeBase> loaded)
        {
            var converted = new TDAgent<IMaze>
            {
                DiscountRate = loaded.DiscountRate,
                Environment = loaded.Environment,
                EpsilonDecayEnd = loaded.EpsilonDecayEnd,
                EpsilonDecayStart = loaded.EpsilonDecayStart,
                LearningRate = loaded.LearningRate,
                MaximumAllowedBacktracks = loaded.MaximumAllowedBacktracks,
                MaximumAllowedMoves = loaded.MaximumAllowedMoves,
                NumberOfTrainingEpisodes = loaded.NumberOfTrainingEpisodes,
                TrainingSessions = loaded.TrainingSessions
            };

            if (converted.Environment.ObjectiveReward == 0)
            {
                converted.Environment.ObjectiveReward = 20;
            }

            return converted;
        }

        public static IMaze CopyEnvironment(MazeBase environment)
        {
            var converted = new MazeBase
            {
                AdditionalRewards = environment.AdditionalRewards,
                Columns = environment.Columns,
                GetRewardAction = environment.GetRewardAction,
                GoalPosition = environment.GoalPosition,
                MovementPunishement = environment.MovementPunishement,
                ObjectiveAction = environment.ObjectiveAction,
                ObjectiveReward = environment.ObjectiveReward,
                ObjectiveStates = environment.ObjectiveStates,
                Obstructions = environment.Obstructions,
                PrioritizeFromState = environment.PrioritizeFromState,
                QualitySaveDirectory = environment.QualitySaveDirectory,
                QualitySaveFrequency = environment.QualitySaveFrequency,
                QualityTable = environment.CopyQuality(),
                RewardsTable = environment.RewardsTable,
                Rows = environment.Rows,
                StartPosition = environment.StartPosition,
                StatesPerPhase = environment.StatesPerPhase,
                StatesTable = environment.StatesTable
            };

            return converted;
        }
    }
}
