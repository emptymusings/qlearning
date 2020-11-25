<<<<<<< HEAD:qlearning-sharp/QLearningMaze/QLearning.Core/IQEnvironmentMultiObjective.cs
﻿namespace QLearning.Core
=======
﻿namespace QLearning.Core.Environment
>>>>>>> main:qlearning-sharp/QLearningMaze/QLearning.Core/Environment/IQEnvironmentMultiObjective.cs
{
    using System.Collections.Generic;

    public interface IQEnvironmentMultiObjective : IQEnvironment
    {
        /// <summary>
        /// Gets or Sets a list of custom rewards
        /// </summary>
        List<CustomObjective> AdditionalRewards { get; set; }
    }
}
