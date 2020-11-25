namespace QLearningMaze.Core
{
    using QLearning.Core;
    using System.Collections.Generic;

    public interface IMaze : IQEnvironmentMultiObjective, IQEnvironment
    {
        /// <summary>
        /// Gets or Sets the number of columns in the maze
        /// </summary>
        int Columns { get; set; }
        /// <summary>
        /// Gets or Sets the number of rows in the maze
        /// </summary>
        int Rows { get; set; }
        /// <summary>
        /// Gets or Sets the position (state) in which the agent will start
        /// </summary>
        int StartPosition { get; set; }
        /// <summary>
        /// Gets or Sets the position (state) in which the maze ends
        /// </summary>
        int GoalPosition { get; set; }
        /// <summary>
        /// Gets or Sets any obstructions (walls) preventing movement of the agent between states
        /// </summary>
        List<MazeObstruction> Obstructions { get; set; }
        /// <summary>
        /// Adds an obstruction between 2 states
        /// </summary>
        void AddObstruction(int betweenState, int andState);
        /// <summary>
        /// Removes an obstruction between 2 states
        /// </summary>
        void RemoveObstruction(int betweenState, int andState);
        /// <summary>
        /// Adds a custom reward/objective to the maze
        /// </summary>
        void AddReward(CustomObjective reward);
        /// <summary>
        /// Adds a custom reward/objective to the maze
        /// </summary>
        /// <param name="state">The state in which the reward resides</param>
        /// <param name="value">The value of the reward</param>
        /// <param name="isRequired">(CURRENTLY UNUSED - always resolves to True)</param>
        void AddReward(int state, double value, bool isRequired = true);
        /// <summary>
        /// Removes a custom reward/objective from the specified state
        /// </summary>
        void RemoveReward(int state);
        /// <summary>
        /// Gets a list of all custom rewards/objectives
        /// </summary>
        List<CustomObjective> GetAdditionalRewards();
    }
}
