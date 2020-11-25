namespace QLearning.Core.Environment
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
