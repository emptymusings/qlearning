namespace QLearning.Core
{
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.IO;
    using Agent;

    public class Utilities
    {
        public static string QTableSaveDirectory { get; set; }

        public static void SaveObject(string saveToPath, object objectToSave)
        {
            string json = JsonConvert.SerializeObject(objectToSave, Formatting.Indented);
            File.WriteAllText(saveToPath, json);
        }

        public static T LoadObject<T>(string fromPath)
        {
            T loaded = JsonConvert.DeserializeObject<T>(fromPath);
            return loaded;
        }

        public static IEnumerable<TrainingSession> GetTrainingSessions(string sessionsPath)
        {
            List<TrainingSession> results = new List<TrainingSession>();

            foreach (var file in Directory.GetFiles(sessionsPath, "*.json"))
            {
                results.Add(LoadObject<TrainingSession>(file));
            }

            return results;
        }
    }
}
