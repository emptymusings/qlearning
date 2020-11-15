namespace QLearning.Core
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Newtonsoft.Json;
    using System.IO;

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

        public static IEnumerable<TrainingSession> GetTrainingSessions()
        {
            List<TrainingSession> results = new List<TrainingSession>();

            foreach (var file in Directory.GetFiles(QTableSaveDirectory))
            {
                results.Add(LoadObject<TrainingSession>(file));
            }

            return results;
        }
    }
}
