using System;
using System.Collections.Generic;
using System.Text;

namespace QLearning.Core
{
    public interface IQLearningMultiObjective : IQLearning
    {
        List<CustomObjective> AdditionalRewards { get; set; }
    }
}
