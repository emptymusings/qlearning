<<<<<<< HEAD:qlearning-sharp/QLearningMaze/QLearning.Core/TrainingSession.cs
﻿namespace QLearning.Core
=======
﻿namespace QLearning.Core.Agent
>>>>>>> main:qlearning-sharp/QLearningMaze/QLearning.Core/Agent/TrainingSession.cs
{
    public class TrainingSession
    {
        public int Episode { get; set; }
        public int Moves { get; set; }
        public double Score { get; set; }
        public double[][] Quality { get; set; }
    }
}
