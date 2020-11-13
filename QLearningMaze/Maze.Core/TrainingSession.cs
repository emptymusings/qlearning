using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace QLearningMaze.Core
{
    public class TrainingSession
    {
        public static IEnumerable<TrainingSession> GetTrainingSessions()
        {
            List<TrainingSession> results = new List<TrainingSession>();

            foreach(var file in Directory.GetFiles(MazeUtilities.TRAINING_SESSIONS_DIRECTORY))
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
