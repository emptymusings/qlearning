using QLearningMaze.Core.Mazes;
using System.Collections.Generic;

namespace QLearningMaze.Core
{
    public interface IMaze
    {
        int NumberOfStates { get; }
        int Rows { get; set; }
        int Columns { get; set; }
        int GoalPosition { get; set; }
        int StartPosition { get; set; }
        double DiscountRate { get; set; }
        double LearningRate { get; set; }
        int MaxEpochs { get; set; }
        int[][] MazeStates { get; set; }
        double[][] Rewards { get; set; }
        double[][] Quality { get; set; }
        List<MazeObstruction> Obstructions { get; set; }

        void AddWall(int betweenSpace, int andSpace);
        void RemoveWall(int betweenSpace, int andSpace);
        void Train();
        void RunMaze();
        void PrintRewards();
        void PrintQuality();
    }
}
