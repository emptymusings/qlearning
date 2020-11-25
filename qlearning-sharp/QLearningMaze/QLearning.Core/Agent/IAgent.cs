namespace QLearning.Core.Agent
{
    using System;
    using Environment;
    public interface IAgent<TEnvironment>
        where TEnvironment : IRLEnvironment
    {
        /// <summary>
        /// Occurs as the agent's current state has changed
        /// </summary>
        event EventHandler<AgentStateChangedEventArgs> AgentStateChanged;
        /// <summary>
        /// Occurs when an agent, in training, has had a change in state
        /// </summary>
        event EventHandler<TrainingAgentStateChangedEventArgs> TrainingAgentStateChanged;
        /// <summary>
        /// Occurs when a Training Episode has completed
        /// </summary>
        event EventHandler<TrainingEpisodeCompletedEventArgs> TrainingEpisodeCompleted;
        /// <summary>
        /// Occurs when training is started or stopped
        /// </summary>
        event EventHandler<bool> TrainingStateChanged;
        /// <summary>
        /// Occurs when the agent has reached a terminal state
        /// </summary>
        event EventHandler<AgentCompletedEventArgs> AgentCompleted;
    }
}
