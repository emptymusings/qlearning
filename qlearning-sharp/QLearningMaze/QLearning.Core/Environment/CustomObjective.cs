<<<<<<< HEAD:qlearning-sharp/QLearningMaze/QLearning.Core/CustomObjective.cs
﻿namespace QLearning.Core
=======
﻿namespace QLearning.Core.Environment
>>>>>>> main:qlearning-sharp/QLearningMaze/QLearning.Core/Environment/CustomObjective.cs
{
    public class CustomObjective
    {
        public int State { get; set; }
        public double Value { get; set; }
        public bool IsRequired { get; set; } = true;
        public int Priority { get; set; }
        public int DistanceFromStart { get; set; }
        public int DistanceFromEnd { get; set; }
    }
}
