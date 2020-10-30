using QLearningMaze.Core.Mazes;
using System.Collections.Generic;

namespace QLearningMaze.Core
{
    using System;

    public interface IMaze
    {
        event EventHandler MazeCreatingEventHandler;

        event EventHandler MazeCreatedEventHandler;

        event EventHandler RewardsCreatedEventHandler;

        event EventHandler QualityCreatedEventHandler;

        event EventHandler<ObstructionEventArgs> ObstructionAddedEventHandler;

        event EventHandler<ObstructionEventArgs> ObstructionRemovedEventHandler;

        event EventHandler<AgentStateChangedEventArgs> AgentStateChangedEventHandler;

        event EventHandler<(int newState, int previousState, double newQuality, double oldQuality)> TrainingAgentStateChangingEventHandler;

        event EventHandler<bool> TrainingStatusChangedEventHandler;

        event EventHandler<TrainingEpochCompletedEventArgs> TrainingEpochCompletedEventHandler;
        /// <summary>
        /// Gets or Sets the number of rows in the maze
        /// </summary>
        int NumberOfStates { get; }
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
        /// Gets or Sets the total number of epochs (or episodes) to train for.
        /// </summary>
        int MaxEpochs { get; set; }
        /// <summary>
        /// Maze states, where the first dimension is state, the second is movement to the next state (action), 
        /// and the value is binary (0 or 1) to determine if the action is allowed
        /// </summary>
        int[][] MazeStates { get; set; }
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
        //List<AdditionalReward> AdditionalRewards { get; set; }
        double TotalRewards { get; set; }

        int GetNextState(int currentState, int action);
        int GetNextState(int currentState, Actions action);
        void AddWall(int betweenSpace, int andSpace);
        void RemoveWall(int betweenSpace, int andSpace);
        void AddCustomReward(int position, double reward);
        void RemoveCustomReward(int position);
        IEnumerable<AdditionalReward> GetAdditionalRewards();
        void Train();
        void RunMaze();
        void PrintRewards();
        void PrintQuality();
    }
}
