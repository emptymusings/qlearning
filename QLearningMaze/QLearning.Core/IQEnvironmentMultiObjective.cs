using System;
using System.Collections.Generic;
using System.Text;

namespace QLearning.Core
{
    public interface IQEnvironmentMultiObjective : IQEnvironment
    {
        /// <summary>
        /// Gets or Sets a list of custom rewards
        /// </summary>
        List<CustomObjective> AdditionalRewards { get; set; }
    }
}
