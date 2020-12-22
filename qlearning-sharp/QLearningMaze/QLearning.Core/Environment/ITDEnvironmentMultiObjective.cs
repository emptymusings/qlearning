namespace QLearning.Core.Environment
{
    using System.Collections.Generic;

    public interface ITDEnvironmentMultiObjective : ITDEnvironment
    {
        /// <summary>
        /// Gets or Sets the action which an agent will use to collect a reward and pass to the next "phase" of states
        /// </summary>
        int GetRewardAction { get; set; }
        /// <summary>
        /// Gets or Sets a list of custom rewards
        /// </summary>
        List<CustomObjective> AdditionalRewards { get; set; }
    }
}
