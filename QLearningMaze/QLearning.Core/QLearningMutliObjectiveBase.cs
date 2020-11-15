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
                  objectiveStates,
                  maximumAllowedMoves,
                  maximumAllowedBacktracks)
        {
            
        }

        public List<CustomObjective> CustomObjectives { get; set; } = new List<CustomObjective>();

        private void SetPhaseSize()
        {
            if (PhaseSize <= 0)
            {
                PhaseSize = StatesTable.Length;
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
            var prioritizedObjectives = CustomObjectives.OrderBy((priority) =>
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
                    RewardsTable[objective.State + ObjectiveAction][ObjectiveAction] = objective.Value;
                    rewardPriority += PhaseSize;
                }
            }
        }

        protected virtual void SetMultiPhaseObjective(CustomObjective objective)
        {
            int phase = 0;

            while (phase < StatesTable.Length)
            {
                SetObjectiveActionNextState(objective);
                RewardsTable[objective.State + phase][ObjectiveAction] = objective.Value;

                phase += PhaseSize;
            }
        }

        protected virtual void SetObjectiveActionNextState(CustomObjective objective)
        {
            if (objective.State + (PhaseSize * 2) <= StatesTable.Length - 1)
            {
                StatesTable[objective.State + PhaseSize][ObjectiveAction] = objective.State + (PhaseSize * 2);
            }
        }
    }
}
