using QLearningMaze.Core.Mazes;
using System.Collections.Generic;

namespace QLearningMaze.Core
{
    using System;

    public interface IMaze
    {
        event EventHandler ObservationSpaceCreatingEventHandler;

        event EventHandler ObservationSpaceCreatedEventHandler;

        event EventHandler RewardsCreatedEventHandler;

        event EventHandler QualityCreatedEventHandler;

        event EventHandler<ObstructionEventArgs> ObstructionAddedEventHandler;

        event EventHandler<ObstructionEventArgs> ObstructionRemovedEventHandler;

        event EventHandler<AgentStateChangedEventArgs> AgentStateChangedEventHandler;

        event EventHandler<TrainingAgentStateChangedEventArgs> TrainingAgentStateChangingEventHandler;

        event EventHandler<bool> TrainingStatusChangedEventHandler;

        event EventHandler<TrainingEpisodeCompletedEventArgs> TrainingEpisodeCompletedEventHandler;
        event EventHandler<AgentCompletedMazeEventArgs> AgentCompletedMazeEventHandler;
        /// <summary>
        /// Gets the number of rows in the maze
        /// </summary>
        int NumberOfStates { get; }
        /// <summary>
        /// Gets the square spaces of the maze (Rows * Columns)
        /// </summary>
        int TotalSpaces { get; }
        /// <summary>
        /// Gets or Sets the number of columns in the maze 
        /// </summary>
        int Rows { get; set; }
        /// <summary>
        /// Gets or Sets the Exit/Goal of the Maze (win condition)
        /// </summary>
        int Columns { get; set; }
        /// <summary>
        /// Gets or Sets the Exit/Goal of the Maze (win condition)
        /// </summary>
        int GoalPosition { get; set; }
        /// <summary>
        /// Gets or Sets the Agent's start position
        /// </summary>
        int StartPosition { get; set; }
        /// <summary>
        /// Decimal value between 0 and 1 that determines how much long term reward is weighted vs immediate.  Higher values reflect more regard to long term rewards.
        /// Represents Gamma in the Q Learning formula
        /// </summary>
        double DiscountRate { get; set; }
        /// <summary>
        /// Determines to what extent newly acquired information overrides old information. A factor of 0 makes the agent learn nothing (exclusively exploiting prior knowledge),
        /// while a factor of 1 makes the agent consider only the most recent information (ignoring prior knowledge to explore possibilities).
        /// Represents Alpha in the Q-Learning formula
        /// </summary
        double LearningRate { get; set; }
        /// <summary>
        /// Gets or Sets the total number of episodes (or episodes) to train for.
        /// </summary>
        int MaxEpisodes { get; set; }
        /// <summary>
        /// Maze states, where the first dimension is state, the second is movement to the next state (action), 
        /// and the value is binary (0 or 1) to determine if the action is allowed
        /// </summary>
        int[][] ObservationSpace { get; set; }
        /// <summary>
        /// Rewards table, where the first dimension is state, the second is movement to the next state (action), and the value is the reward
        /// </summary>
        double[][] Rewards { get; set; }
        /// <summary>
        /// The Q-Table, where the first dimesion is state, the second is movement to the next state (action), and the value is the result of the quality algorithm
        /// </summary>
        double[][] Quality { get; set; }
        /// <summary>
        /// Gets or Sets a list of obstructions (walls) to avoid 
        /// </summary>SZ
        List<MazeObstruction> Obstructions { get; set; }
        /// <summary>
        /// Gets or Sets any custom rewards
        /// </summary>
        List<AdditionalReward> AdditionalRewards { get; set; }
        /// <summary>
        /// Gets the total rewards received during a training or maze run session
        /// </summary>
        double TotalRewards { get; set; }
        /// <summary>
        /// Gets or Sets the episode interval between saving quality and run information during training
        /// </summary>
        int SaveEpisodes { get; set; }


        int GetNextState(int currentState, int action);
        int GetNextState(int currentState, Actions action);
        void AddWall(int betweenSpace, int andSpace);
        void RemoveWall(int betweenSpace, int andSpace);
        void AddCustomReward(int position, double reward);
        void RemoveCustomReward(int position);
        IEnumerable<AdditionalReward> GetAdditionalRewards();
        void Train();
        void RunMaze();
    }
}
