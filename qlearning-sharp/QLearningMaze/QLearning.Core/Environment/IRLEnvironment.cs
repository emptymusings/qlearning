﻿namespace QLearning.Core.Environment
{
    public interface IRLEnvironment
    {
        /// <summary>
        /// Updates the Q-Value for a state/action pair in the Q-Table
        /// </summary>
        /// <param name="state">The state that the agent is in</param>
        /// <param name="nextAction">The action the agent will be taking from its given state</param>
        /// <param name="learningRate">The learning rate (alpha) - how important new information is vs how much to rely on previous knowledge</param>
        /// <param name="discountRate">The discount rate (gamma) - how important immediate rewards/punishments are vs efficiency of reaching terminal state</param>
        void CalculateQuality(int state, int nextAction, double learningRate, double discountRate);
    }
}
