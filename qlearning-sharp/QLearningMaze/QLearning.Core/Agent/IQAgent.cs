namespace QLearning.Core.Agent
{
    using System;
    using System.Collections.Generic;
    using Environment;

    public interface IQAgent<TEnvironment> : IAgent<TEnvironment>
        where TEnvironment : IQEnvironment
    {
        TEnvironment Environment { get; set; }
        /// <summary>
        /// Gets or Sets the Discount Rate, or Discount Factor, used in the Q-Learning formula (aka gamma)
        /// </summary>
        double DiscountRate { get; set; }
        /// <summary>
        /// Gets or Sets the Learning Rate used in the Q-Learning formula (aka alpha)
        /// </summary>
        double LearningRate { get; set; }
        /// <summary>
        /// Gets or Sets the episode in which epsilon (greedy strategy) decay will start.  Also used when calculating epsilon's decay value
        /// </summary>
        double EpsilonDecayStart { get; set; }
        /// <summary>
        /// Gets or Sets the the episode in which epsilon (greedy strategy) decay will end.  Also used when calculating epsilon's decay value
        /// </summary>
        double EpsilonDecayEnd { get; set; }
        /// <summary>
        /// Gets or Sets the decay rate of epsilon
        /// </summary>
        double EpsilonDecayValue { get; set; }
        /// <summary>
        /// Gets or Sets the maximum amount of moves that the Agent can take before failing
        /// </summary>
        int MaximumAllowedMoves { get; set; }
        /// <summary>
        /// Gets or Sets the maximum amount of times the Agent may move back and forth between two spaces before failing
        /// </summary>
        int MaximumAllowedBacktracks { get; set; }
        /// <summary>
        /// Gets or Sets the number of training episodes utilized to build the Quality table
        /// </summary>
        int NumberOfTrainingEpisodes { get; set; }
        /// <summary>
        /// Gets or sets a list of stored successful training episodes
        /// </summary>
        List<TrainingSession> TrainingEpisodes { get; set; }
        /// <summary>
        /// Gets the best session from a training set, based on total score
        /// </summary>
        TrainingSession BestTrainingSession { get; }
        /// <summary>
        /// The current state of the Agent
        /// </summary>
        int State { get; set; }
        /// <summary>
        /// The number of moves that the agent has made while training (per episode) or running through the environment
        /// </summary>
        int Moves { get; set; }
        /// <summary>
        /// The sum of rewards/punishments that the agent has acquired while navigating the environment
        /// </summary>
        double Score { get; set; }
        /// <summary>
        /// Trains the Agent for its environment, uses base events
        /// </summary>
        void Train();
        /// <summary>
        /// Trains the Agent for its environment
        /// </summary>
        /// <param name="overrideBaseEvents">True if base events will be used; False if they should be ignored</param>
        void Train(bool overrideBaseEvents);
        /// <summary>
        /// Runs the agent through the environment utilizing the Q-Table policies for the best action(s) to take from a given state
        /// </summary>
        void Run(int fromState);
        /// <summary>
        /// Runs the agent through the environment utilizing the Q-Table policies for the best action(s) to take from a given state
        /// </summary>
        /// <param name="fromState">The state from which the agent will start</param>
        /// <param name="overrideBaseEvents">Boolean value that determines whether to override events in this base class</param>
        void Run(int fromState, bool overrideBaseEvents);
    }
}
