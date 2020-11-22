using System;
using System.Collections.Generic;
using System.Text;

namespace QLearning.Core
{
    public interface IQLearning
    {
        /// <summary>
        /// Occurs as the State Table's initializion is about to start
        /// </summary>
        event EventHandler StateTableCreating;
        /// <summary>
        /// Occurs as the State Table's initializion has completed
        /// </summary>
        event EventHandler StateTableCreated;
        /// <summary>
        /// Occurs as the Reward Table's initializion is about to start
        /// </summary>
        event EventHandler RewardTableCreating;
        /// <summary>
        /// Occurs as the Reward Table's initializion has completed
        /// </summary>
        event EventHandler RewardTableCreated;
        /// <summary>
        /// Occurs as the Quality Table's initializion is about to start
        /// </summary>
        event EventHandler QualityTableCreating;
        /// <summary>
        /// Occurs as the Quality Table's initializion has completed
        /// </summary>
        event EventHandler QualityTableCreated;
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
        
        /// <summary>
        /// Gets the number of States which an agent can occupy
        /// </summary>
        int NumberOfStates { get; }
        /// <summary>
        /// Gets or Sets the number of states per phase (phases are added per additional objective)
        /// </summary>
        int StatesPerPhase { get; set; }
        /// <summary>
        /// Gets or Sets the Discount Rate, or Discount Factor, used in the Q-Learning formula (aka gamma)
        /// </summary>
        double DiscountRate { get; set; }
        /// <summary>
        /// Gets or Sets the Learning Rate used in the Q-Learning formula (aka alpha)
        /// </summary>
        double LearningRate { get; set; }
        /// <summary>
        /// Gets or Sets the 2-dimensional array in which all states and actions stored.  The scalar value returns the 
        /// state to which the agent will "move" when performing a specific action from the current state
        /// </summary>
        int[][] StatesTable { get; set; }
        /// <summary>
        /// Gets or Sets the 2-dimensional array in which all rewards for state/actions are stored.  The scalar value returns the 
        /// reward which the agent will receive when performing a specific action from the current state
        /// </summary>
        double[][] RewardsTable { get; set; }
        /// <summary>
        /// Gets or Sets the 2-dimensional array in which the quality/policy of state/action pairs are stored.  The scalar value returns the 
        /// quality of a given state/action pair.
        /// </summary>
        double[][] QualityTable { get; set; }
        /// <summary>
        /// Gets or Sets the episode in which epsilon (greedy strategy) decay will start.  Also used when calculating epsilon's decay value
        /// </summary>
        double EpsilonDecayStart { get; set; }
        /// <summary>
        /// Gets or Sets the the episode in which epsilon (greedy strategy) decay will end.  Also used when calculating epsilon's decay value
        /// </summary>
        double EpsilonDecayEnd { get; set; }
        /// <summary>
        /// Gets or Sets the states in which terminal states exist
        /// </summary>
        List<int> ObjectiveStates { get; set; }
        /// <summary>
        /// Gets or Sets the specific action to be taken when reaching a terminal state to reach completion.
        /// </summary>
        int ObjectiveAction { get; set; }
        /// <summary>
        /// Gets or Sets the total number of moves allowed before the agent is considered in a terminal state due to failure.  Used
        /// to ensure that an error state cannot continue indefinitely
        /// </summary>
        public int MaximumAllowedMoves { get; set; }
        /// <summary>
        /// Gets or Sets the reward for reacing the final objective
        /// </summary>
        double ObjectiveReward { get; set; }
        /// <summary>
        /// Gets or Sets the directory to which quality tables should periodically be saved during the training process for later evaluation
        /// </summary>
        string QualitySaveDirectory { get; set; }
        /// <summary>
        /// Gets or Sets the number of times the agent should be allowed to move between two states
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
        /// Gets or Sets the frequency (in episodes) in which Quality values will be saved
        /// </summary>
        int QualitySaveFrequency { get; set; }

        /// <summary>
        /// Initializes the States Table
        /// </summary>
        /// <param name="overrideBaseEvents">Boolean value that determines whether to override events in this base class</param>
        void InitializeStatesTable(bool overrideBaseEvents);
        /// <summary>
        /// Initializes the States Table
        /// </summary>
        void InitializeStatesTable();
        /// <summary>
        /// Initializes the Rewards Table
        /// </summary>
        /// <param name="overrideBaseEvents">Boolean value that determines whether to override events in this base class</param>
        void InitializeRewardsTable(bool overrideBaseEvents);
        /// <summary>
        /// Initializes the Rewards Table
        /// </summary>
        void InitializeRewardsTable();
        /// <summary>
        /// Initializes the Quality Table
        /// </summary>
        /// <param name="overrideBaseEvents">Boolean value that determines whether to override events in this base class</param>
        void InitializeQualityTable(bool overrideBaseEvents);
        /// <summary>
        /// Initializes the Quality Table
        /// </summary>
        void InitializeQualityTable();
        /// <summary>
        /// Builds the Quality of state/action pairs by training an agent using the Q-Formula and exploration/exploitation (epsilon greedy strategy)
        /// </summary>
        /// <param name="overrideBaseEvents">Boolean value that determines whether to override events in this base class</param>
        void Train(bool overrideBaseEvents);
        /// <summary>
        /// Builds the Quality of state/action pairs by training an agent using the Q-Formula and exploration/exploitation (epsilon greedy strategy)
        /// </summary>
        void Train();
        /// <summary>
        /// Runs the agent through the environment utilizing the Q-Table policies for the best action(s) to take from a given state
        /// </summary>
        /// <param name="fromState">The state from which the agent will start</param>
        /// <param name="overrideBaseEvents">Boolean value that determines whether to override events in this base class</param>
        void RunAgent(int fromState, bool overrideBaseEvents);
        /// <summary>
        /// Runs the agent through the environment utilizing the Q-Table policies for the best action(s) to take from a given state
        /// </summary>
        /// <param name="fromState">The state from which the agent will start</param>
        void RunAgent(int fromState);
    }
}
