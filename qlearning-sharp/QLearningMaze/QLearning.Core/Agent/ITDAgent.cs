namespace QLearning.Core.Agent
{
    using Environment;

    using System.Collections.Generic;

    public interface ITDAgent<TEnvironment> : IAgent<TEnvironment>
        where TEnvironment : ITDEnvironment
    {
        
        /// <summary>
        /// Gets or Sets the maximum amount of times the Agent may move back and forth between two spaces before failing
        /// </summary>
        int MaximumAllowedBacktracks { get; set; }
        /// <summary>
        /// Gets or sets a list of stored successful training episodes
        /// </summary>
        IList<TrainingSession> TrainingSessions { get; set; }
        /// <summary>
        /// Gets the best session from a training set, based on total score
        /// </summary>
        TrainingSession BestTrainingSession { get; }
        /// <summary>
        /// Updates the Q-Value for a state/action pair in the Q-Table
        /// </summary>
        /// <param name="state">The state that the agent is in</param>
        /// <param name="nextAction">The action the agent will be taking from its given state</param>
        /// <param name="learningRate">The learning rate (alpha) - how important new information is vs how much to rely on previous knowledge</param>
        /// <param name="discountFactor">The discount rate (gamma) - how important immediate rewards/punishments are vs efficiency of reaching terminal state</param>
        void CalculateQLearning(int state, int action, double learningRate, double discountFactor);
        /// <summary>
        /// Updates the SARSA value for a state/action pair in the Q-Table
        /// </summary>
        /// <param name="state">The state that the agent is in</param>
        /// <param name="action">The action the agent will be taking from its given state</param>
        /// <param name="learningRate">The learning rate (alpha) - how important new information is vs how much to rely on previous knowledge</param>
        /// <param name="discountFactor">The discount rate (gamma) - how important immediate rewards/punishments are vs efficiency of reaching terminal state</param>
        /// <param name="epsilon">The epsilon to use when determining random or greedy next actions</param>
        void CalculateSarsa(int state, int action, double learningRate, double discountFactor, double epsilon);
    }
}
