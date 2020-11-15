using System;
using System.Collections.Generic;
using System.Text;

namespace QLearning.Core
{
    public class CustomObjective
    {
        public int State { get; set; }
        public double Value { get; set; }
        public bool IsMultiphase { get; set; } = false;
    }
}
