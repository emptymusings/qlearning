namespace QLearning.Core.Agent
{
    using System;
    using Environment;
    public partial class AgentBase<TEnvironment> : IAgent<TEnvironment>
        where TEnvironment : IRLEnvironment
    {
    }
}
