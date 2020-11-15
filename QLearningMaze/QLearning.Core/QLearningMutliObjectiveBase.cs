namespace QLearning.Core
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Linq;

    public class QLearningMutliObjectiveBase : QLearningBase, IQLearningMultiObjective
    {
        public virtual int ObjectiveAction { get; set; } = -1;
        public virtual int PrioritizeFromState { get; set; }
        public QLearningMutliObjectiveBase() { }

        public QLearningMutliObjectiveBase(int numberOfStates, int numberOfActions)
            : base(numberOfStates, numberOfActions)
        {
        }

        public QLearningMutliObjectiveBase(
            int numberOfStates,
            int numberOfActions,
            double learningRate,
            double discountRate,
            string qualitySaveDirectory,
            double objectiveReward,
            int objectiveAction,
            List<int> objectiveStates,
            int maximumAllowedMoves = 1000,
            int maximumAllowedBacktracks = -1)
            : base(
                  numberOfActions,
                  numberOfStates,
                  learningRate,
                  discountRate,
                  qualitySaveDirectory,
                  objectiveReward,
                  objectiveAction,
                  objectiveStates,
                  maximumAllowedMoves,
                  maximumAllowedBacktracks)
        {
            
        }

        public List<CustomObjective> AdditoinalRewards { get; set; } = new List<CustomObjective>();

        private void SetPhaseSize()
        {
            if (TotalSpaces <= 0)
            {
                TotalSpaces = ObservationSpace.Length;
            }
        }

        public override void InitializeRewardsTable(int numberOfStates, int numberOfActions)
        {
            SetPhaseSize();
            base.InitializeRewardsTable(numberOfStates, numberOfActions);
            SetCustomObjectives();
        }

        public override void InitializeRewardsTable()
        {
            SetPhaseSize();
            base.InitializeRewardsTable();
            SetCustomObjectives();
        }

        public virtual void SetCustomObjectives()
        {
            var prioritizedObjectives = AdditoinalRewards.OrderBy((priority) =>
                {
                    int differential = (priority.State > PrioritizeFromState ? priority.State - PrioritizeFromState : PrioritizeFromState - priority.State);
                    var value = differential + priority.Value;

                    return value;
                });

            SetCustomObjectives(prioritizedObjectives);
        }

        public virtual void SetCustomObjectives(IOrderedEnumerable<CustomObjective> prioritizedObjectives)
        {
            int rewardPriority = 0;
            SetPhaseSize();

            foreach(var objective in prioritizedObjectives)
            {
                if (objective.IsMultiphase ||
                    objective.Value < 0)
                {
                    SetMultiPhaseObjective(objective);
                }
                else
                {
                    SetObjectiveActionNextState(objective);
                    Rewards[objective.State + ObjectiveAction][ObjectiveAction] = objective.Value;
                    rewardPriority += TotalSpaces;
                }
            }
        }

        protected virtual void SetMultiPhaseObjective(CustomObjective objective)
        {
            int phase = 0;

            while (phase < ObservationSpace.Length)
            {
                SetObjectiveActionNextState(objective);
                Rewards[objective.State + phase][ObjectiveAction] = objective.Value;

                phase += TotalSpaces;
            }
        }

        protected virtual void SetObjectiveActionNextState(CustomObjective objective)
        {
            if (objective.State + (TotalSpaces * 2) <= ObservationSpace.Length - 1)
            {
                ObservationSpace[objective.State + TotalSpaces][ObjectiveAction] = objective.State + (TotalSpaces * 2);
            }
        }
    }
}
