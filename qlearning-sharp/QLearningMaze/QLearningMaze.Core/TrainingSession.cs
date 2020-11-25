namespace QLearningMaze.Core
{
    using System.Collections.Generic;
    using System.IO;

    public class TrainingSession
    {
        public static IEnumerable<TrainingSession> GetTrainingSessions()
        {
            if (!Directory.Exists(MazeUtilities.TRAINING_SESSIONS_DIRECTORY))
            {
                Directory.CreateDirectory(MazeUtilities.TRAINING_SESSIONS_DIRECTORY);
            }
            
            return GetTrainingSessions(MazeUtilities.TRAINING_SESSIONS_DIRECTORY);
        }

        public static IEnumerable<TrainingSession> GetTrainingSessions(string trainingSessionsDirectory)
        {
            List<TrainingSession> results = new List<TrainingSession>();
            
            foreach(var file in Directory.GetFiles(trainingSessionsDirectory))
            {
                results.Add(MazeUtilities.LoadObject<TrainingSession>(file));
            }

            return results;   
        }

        public int Episode { get; set; }
        public int Moves { get; set; }
        public double Score { get; set; }
        public double[][] Quality { get; set; }
    }
}
