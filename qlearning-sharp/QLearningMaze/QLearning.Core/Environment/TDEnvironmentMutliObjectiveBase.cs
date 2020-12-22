namespace QLearning.Core.Environment
{
    using System.Collections.Generic;
    using System.Linq;

    public abstract class TDEnvironmentMutliObjectiveBase : TDEnvironmentBase, ITDEnvironmentMultiObjective
    {
        public virtual int GetRewardAction { get; set; } = -1;
        public virtual int PrioritizeFromState { get; set; }
        
        public TDEnvironmentMutliObjectiveBase() { }

        public TDEnvironmentMutliObjectiveBase(int numberOfStates, int numberOfActions)
            : base(numberOfStates, numberOfActions)
        {
        }

        public TDEnvironmentMutliObjectiveBase(
            int numberOfStates,
            int numberOfActions,
            string qualitySaveDirectory,
            double objectiveReward,
            int objectiveAction,
            List<int> objectiveStates)
            : base(
                  numberOfActions,
                  numberOfStates,
                  qualitySaveDirectory,
                  objectiveReward,
                  objectiveAction,
                  objectiveStates)
        {
            
        }

        public List<CustomObjective> AdditionalRewards { get; set; } = new List<CustomObjective>();

        protected virtual void SetPhaseSize()
        {
            if (StatesPerPhase <= 0)
                StatesPerPhase = StatesTable.Length;
        }

        protected override void InitializeRewardsTable(bool overrideBaseEvents)
        {
            if (!overrideBaseEvents)
                OnRewardTableCreating();

            SetPhaseSize();
            base.InitializeRewardsTable(true);
            SetCustomObjectives();

            if (!overrideBaseEvents)
                OnRewardTableCreated();
        }

        public virtual void SetCustomObjectives()
        {
            var prioritizedObjectives = GetPrioritizedObjectives();

            SetCustomObjectives(prioritizedObjectives);
        }

        protected virtual IOrderedEnumerable<CustomObjective> GetPrioritizedObjectives()
        {
            var prioritizedObjectives = AdditionalRewards.OrderByDescending((priority) =>
            {
                int differential = (priority.State > PrioritizeFromState ? priority.State - PrioritizeFromState : PrioritizeFromState - priority.State);
                var value = (differential * -1) + priority.Value; 

                return value;
            });

            return prioritizedObjectives;
        }

        public virtual void SetCustomObjectives(IOrderedEnumerable<CustomObjective> prioritizedObjectives)
        {
            int rewardPriority = 0;
            SetPhaseSize();

            foreach(var objective in prioritizedObjectives)
            {
                if (!objective.IsRequired ||
                    objective.Value < 0)
                {
                    SetMultiPhaseObjective(objective);
                }
                else
                {
                    if (objective.State + rewardPriority > RewardsTable.Length)
                        continue;

                    SetObjectiveActionNextState(objective.State + rewardPriority);
                    RewardsTable[objective.State + rewardPriority][GetRewardAction] = objective.Value;
                    rewardPriority += StatesPerPhase;
                }
            }
        }

        protected virtual void SetMultiPhaseObjective(CustomObjective objective)
        {

            int phase = objective.State;

            while (phase < StatesTable.Length)
            {
                for (int i = 0; i < NumberOfActions; ++i)
                {
                    RewardsTable[phase][i] = objective.Value;
                }

                phase += StatesPerPhase;
            }
        }

        protected virtual void SetObjectiveActionNextState(int state)
        {
            for (int i = 0; i < NumberOfActions; ++i)
            {
                StatesTable[state][i] = -1;
            }

            if (state + (StatesPerPhase) <= StatesTable.Length - 1)
            {
                StatesTable[state][GetRewardAction] = state + StatesPerPhase;
            }
        }
    }
}
