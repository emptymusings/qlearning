namespace QLearning.Core.Agent
{
    using System;
    using System.Collections.Generic;
    using Environment;
    public abstract partial class AgentBase<TEnvironment> : IAgent<TEnvironment>
        where TEnvironment : IRLEnvironment
    {
        protected Random _random = new Random();

        public TEnvironment Environment { get; set; }

        public LearningStyles LearningStyle { get; set; } = LearningStyles.QLearning;
        /// <summary>
        /// Gets or Sets the Discount Rate, or Discount Factor, used in the Q-Learning formula (aka gamma)
        /// </summary>
        public double DiscountRate { get; set; }
        /// <summary>
        /// Gets or Sets the Learning Rate used in the Q-Learning formula (aka alpha)
        /// </summary>
        public double LearningRate { get; set; }
        /// <summary>
        /// Gets or Sets the episode in which epsilon (greedy strategy) decay will start.  Also used when calculating epsilon's decay value
        /// </summary>
        public double EpsilonDecayStart { get; set; }
        /// <summary>
        /// Gets or Sets the the episode in which epsilon (greedy strategy) decay will end.  Also used when calculating epsilon's decay value
        /// </summary>
        public double EpsilonDecayEnd { get; set; }
        public double EpsilonDecayValue { get; set; } = -1;

        public int MaximumAllowedMoves { get; set; }
        public int NumberOfTrainingEpisodes { get; set; }
        public int State { get; set; }
        public int Moves { get; set; }
        public double Score { get; set; }


        public virtual void Train()
        {
            throw new NotImplementedException();
        }

        public virtual void Train(bool overrideBaseEvents)
        {
            throw new NotImplementedException();
        }

        public virtual void Run(int fromState)
        {
            throw new NotImplementedException();
        }

        public virtual void Run(int fromState, bool overrideBaseEvents)
        {
            throw new NotImplementedException();
        }

        protected virtual int GetPreferredNextAction(int state, int[] excludedActions = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the next action to take from the current state
        /// </summary>
        /// <param name="state">The state in which the agent currently resides</param>
        /// <param name="epsilon"></param>
        protected virtual (int nextAction, bool usedGreedy) GetNextAction(int state, double epsilon)
        {
            throw new NotImplementedException();
        }

        protected virtual int GetRandomNextAction(int state)
        {
            throw new NotImplementedException();
        }

        protected virtual List<int> GetPossibleNextActions(int state)
        {
            throw new NotImplementedException();
        }
    }
}
