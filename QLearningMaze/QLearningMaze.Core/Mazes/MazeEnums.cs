using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace QLearningMaze.Core.Mazes
{
    public enum Actions
    {
        CompleteRun = 0,
        MoveUp = 1,
        MoveRight = 2,
        MoveDown = 3,
        MoveLeft = 4,
        GetCustomReward = 5
    }
}
