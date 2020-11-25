namespace QLearningMaze.Core
{
    using System.Collections.Generic;
    using System.IO;

    public class TrainingSession
    {
        public static IEnumerable<TrainingSession> GetTrainingSessions()
        {
            if (!Directory.Exists(Utilities.TRAINING_SESSIONS_DIRECTORY))
            {
                Directory.CreateDirectory(Utilities.TRAINING_SESSIONS_DIRECTORY);
            }
            
            return GetTrainingSessions(Utilities.TRAINING_SESSIONS_DIRECTORY);
        }

        public static IEnumerable<TrainingSession> GetTrainingSessions(string trainingSessionsDirectory)
        {
            List<TrainingSession> results = new List<TrainingSession>();
            
            foreach(var file in Directory.GetFiles(trainingSessionsDirectory))
            {
                results.Add(Utilities.LoadObject<TrainingSession>(file));
            }

            return results;   
        }

        public int Episode { get; set; }
        public int Moves { get; set; }
        public double Score { get; set; }
        public double[][] Quality { get; set; }
    }
}
