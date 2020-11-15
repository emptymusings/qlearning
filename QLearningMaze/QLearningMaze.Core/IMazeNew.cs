namespace QLearningMaze.Core
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using QLearning.Core;

    public interface IMazeNew : IQLearningMultiObjective
    {
        int Columns { get; set; }
        int Rows { get; set; }
        List<MazeObstruction> Obstructions { get; set; }
        void AddObstruction(int betweenState, int andState);
        void RemoveObstruction(int betweenState, int andState);
    }
}
