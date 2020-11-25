namespace QLearning.Core
{
    using System;
    using System.Collections.Generic;

    public interface IQEnvironment
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
        /// Gets the number of States which an agent can occupy
        /// </summary>
        int NumberOfStates { get; }
        /// <summary>
        /// Gets or Sets the number of states per phase (phases are added per additional objective)
        /// </summary>
        int StatesPerPhase { get; set; }
        
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
        /// Gets or Sets the states in which terminal states exist
        /// </summary>
        List<int> ObjectiveStates { get; set; }
        /// <summary>
        /// Gets or Sets the specific action to be taken when reaching a terminal state to reach completion.
        /// </summary>
        int ObjectiveAction { get; set; }       
        /// <summary>
        /// Gets or Sets the reward for reacing the final objective
        /// </summary>
        double ObjectiveReward { get; set; }
        /// <summary>
        /// Gets or Sets the directory to which quality tables should periodically be saved during the training process for later evaluation
        /// </summary>
        string QualitySaveDirectory { get; set; }        
        
        /// <summary>
        /// Gets or Sets the frequency (in episodes) in which Quality values will be saved
        /// </summary>
        int QualitySaveFrequency { get; set; }
        /// <summary>
        /// Initializes the Environment (i.e. Q-Table, Rewards, and State spaces
        /// </summary>
        void Initialize();
        /// <summary>
        /// Initializes the Environment (i.e. Q-Table, Rewards, and State spaces
        /// </summary>
        /// <param name="overrideBaseEvents">Boolean value that determines whether to override events in this base class</param>
        void Initialize(bool overrideBaseEvents);
        
        int GetPreferredNextAction(int state, int[] excludedActions = null);
        int GetRandomNextAction(int state);
        bool IsTerminalState(int state, int moves, int maximumAllowedMoves);
        bool IsTerminalState(int state, int action, int moves, int maximumAllowedMoves);
        TrainingSession SaveQualityForEpisode(int episode, int moves, double score);
        (int nextAction, bool usedGreedy) GetNextAction(int state, double epsilon);
        void CalculateQValue(int state, int nextAction, double learningRate, double discountRate);
    }
}
