using System;
using System.Collections.Generic;
using System.Text;

namespace QLearning.Core
{
    public class CustomObjective
    {
        public int State { get; set; }
        public double Value { get; set; }
        public bool IsRequired { get; set; } = true;
        public int Priority { get; set; }
        public int MovesFromStart { get; set; }
        public int MovesFromGoal { get; set; }
    }
}
