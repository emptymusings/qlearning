namespace QLearningMaze.Core
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using QLearning.Core;

    public interface IMaze : IQLearningMultiObjective
    {
        int Columns { get; set; }
        int Rows { get; set; }
        int StartPosition { get; set; }
        int GoalPosition { get; set; }
        List<MazeObstruction> Obstructions { get; set; }
        
        void AddObstruction(int betweenState, int andState);
        void RemoveObstruction(int betweenState, int andState);

        void AddReward(CustomObjective reward);
        void AddReward(int state, double value, bool isRequired = true);
        void RemoveReward(int state);
        List<CustomObjective> GetAdditionalRewards();
    }
}
