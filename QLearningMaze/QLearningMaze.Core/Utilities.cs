namespace QLearningMaze.Core
{
    using Newtonsoft.Json;
    using System.IO;
    using Mazes;
    using System.Linq;
    using QLearning.Core;

    public class Utilities
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

        public static IAgent<IMaze> ConvertLoadedAgent(AgentBase<MazeBase> loaded)
        {
            var converted = new AgentBase<IMaze>
            {
                DiscountRate = loaded.DiscountRate,
                Environment = loaded.Environment,
                EpsilonDecayEnd = loaded.EpsilonDecayEnd,
                EpsilonDecayStart = loaded.EpsilonDecayStart,
                LearningRate = loaded.LearningRate,
                MaximumAllowedBacktracks = loaded.MaximumAllowedBacktracks,
                MaximumAllowedMoves = loaded.MaximumAllowedMoves,
                NumberOfTrainingEpisodes = loaded.NumberOfTrainingEpisodes,
                TrainingEpisodes = loaded.TrainingEpisodes
            };

            if (converted.Environment.ObjectiveReward == 0)
            {
                converted.Environment.ObjectiveReward = 200;
            }

            return converted;
        }
    }
}
