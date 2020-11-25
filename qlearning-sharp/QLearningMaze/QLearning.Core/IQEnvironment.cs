namespace QLearning.Core
{
    using System;
    using System.Collections.Generic;

    public interface IQEnvironment : IRLEnvironment
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
        /// <summary>
        /// Determines the next state, reward, and quality of taking the specified action from the specified state.
        /// Returns the next state, the reward for taking the given action, and the quality of taking the given action from the given state
        /// </summary>
        /// <param name="state">The state from which the action will be taken to determine the next values</param>
        /// <param name="action">The action to take from the given state</param>
        /// <returns></returns>
        (int newState, double reward, double quality) Step(int state, int action);
        /// <summary>
        /// Find the preferred action, based on existing quality, to take from the given state
        /// </summary>
        /// <param name="state">The state to inspect for the best (highest quality) action</param>
        /// <param name="excludedActions">Optional: Any specific actions to exclude from the possible result</param>
        int GetPreferredNextAction(int state, int[] excludedActions = null);
        /// <summary>
        /// Returns a random action from a state's available actions
        /// </summary>
        /// <param name="state">The state to inspect</param>
        int GetRandomNextAction(int state);
        /// <summary>
        /// Determines if an agent is in a terminal state
        /// </summary>
        /// <param name="state"></param>
        /// <param name="moves"></param>
        /// <param name="maximumAllowedMoves"></param>
        /// <returns></returns>
        bool IsTerminalState(int state, int moves, int maximumAllowedMoves);
        /// <summary>
        /// Determines if an agent is in a terminal state
        /// </summary>
        /// <param name="state"></param>
        /// <param name="action"></param>
        /// <param name="moves"></param>
        /// <param name="maximumAllowedMoves"></param>
        /// <returns></returns>
        bool IsTerminalState(int state, int action, int moves, int maximumAllowedMoves);
        /// <summary>
        /// Saves the Q-Table for the current episode
        /// </summary>
        /// <param name="episode">The episode number (will be used in the file name)</param>
        /// <param name="moves">The number of moves made to complete the episode</param>
        /// <param name="score">The sum of rewards in the current episode</param>
        /// <returns></returns>
        TrainingSession SaveQualityForEpisode(int episode, int moves, double score);
        /// <summary>
        /// Determine the next action to take from a given state
        /// </summary>
        /// <param name="state">The state from which the action will be taken</param>
        /// <param name="epsilon">The current epsilon value from which the determination will be made whether to select an action
        /// at random, or to be greedy and use a known value</param>
        /// <returns></returns>
        (int nextAction, bool usedGreedy) GetNextAction(int state, double epsilon);
        /// <summary>
        /// Updates the Q-Value for a state/action pair in the Q-Table
        /// </summary>
        /// <param name="state">The state that the agent is in</param>
        /// <param name="nextAction">The action the agent will be taking from its given state</param>
        /// <param name="learningRate">The learning rate (alpha) - how important new information is vs how much to rely on previous knowledge</param>
        /// <param name="discountRate">The discount rate (gamma) - how important immediate rewards/punishments are vs efficiency of reaching terminal state</param>
        void CalculateQValue(int state, int nextAction, double learningRate, double discountRate);
    }
}
