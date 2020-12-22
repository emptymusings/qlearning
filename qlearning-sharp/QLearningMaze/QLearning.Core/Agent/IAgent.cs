namespace QLearning.Core.Agent
{
    using Environment;

    using System;
    public interface IAgent<TEnvironment>
        where TEnvironment : IRLEnvironment
    {
        #region Events

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

        #endregion

        #region Properties

        /// <summary>
        /// Gets or Sets the environment in which the Agent will be performing its work
        /// </summary>
        TEnvironment Environment { get; set; }
        /// <summary>
        /// Gets or Sets the learning algorithm to use when training
        /// </summary>
        LearningStyles LearningStyle { get; set; }
        /// <summary>
        /// Gets or Sets the Discount Rate, or Discount Factor, used in the Q-Learning formula (aka gamma)
        /// </summary>
        double DiscountRate { get; set; }
        /// <summary>
        /// Gets or Sets the Learning Rate used in the Q-Learning formula (aka alpha)
        /// </summary>
        double LearningRate { get; set; }
        /// <summary>
        /// Gets or Sets the value which determines if Epsilon will decay while training
        /// </summary>
        bool UseDecayingEpsilon { get; set; }
        /// <summary>
        /// Gets or Sets the default value for Epsilon.  This number will change over the course of training if UseDecayingEpsilon is set to True
        /// </summary>
        double DefaultEpsilon { get; set; }
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
        /// Gets or Sets the number of training episodes utilized to build the Quality table
        /// </summary>
        int NumberOfTrainingEpisodes { get; set; }
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

        #endregion

        #region Methods
        /// <summary>
        /// Trains the Agent for its environment
        /// </summary>
        /// <param name="overrideBaseEvents">True if base events will be used; False if they should be ignored</param>
        void Train(bool overrideBaseEvents = false);
        /// <summary>
        /// Runs the agent through the environment utilizing the Q-Table policies for the best action(s) to take from a given state
        /// </summary>
        /// <param name="fromState">The state from which the agent will start</param>
        /// <param name="overrideBaseEvents">Boolean value that determines whether to override events in this base class</param>
        void Run(int fromState, bool overrideBaseEvents = false);
        
        #endregion
    }
}
