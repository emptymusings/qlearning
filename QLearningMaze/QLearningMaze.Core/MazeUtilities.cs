namespace QLearningMaze.Core
{
    using Newtonsoft.Json;
    using System.IO;
    using Mazes;
    using System.Linq;

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
    }
}
