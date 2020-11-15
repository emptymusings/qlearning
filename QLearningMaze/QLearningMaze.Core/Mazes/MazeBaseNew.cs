namespace QLearningMaze.Core.Mazes
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using QLearning.Core;
    using Mazes;
    using System.Linq;

    public partial class MazeBaseNew : QLearningMutliObjectiveBase, IMazeNew
    {
        public MazeBaseNew(
            int columns, 
            int rows, 
            int startPosition,
            int goalPosition, 
            double discountRate,
            double learningRate)
            : base((columns * rows), Enum.GetNames(typeof(Actions)).Length)
        {
            Columns = columns;
            Rows = rows;
            TotalSpaces = columns * rows;
            StartPosition = startPosition;
            GoalPosition = goalPosition;
            ObjectiveAction = (int)Actions.CompleteRun;
            LearningRate = learningRate;
            DiscountRate = discountRate;
            
        }

        public int Columns { get; set; }
        public int Rows { get; set; }
        public int StartPosition { get; set; }

        private int _goalPosition;

        public int GoalPosition
        {
            get { return _goalPosition; }
            set 
            {
                if (value != _goalPosition)
                {
                    _goalPosition = value;

                    ObjectiveStates.Clear();
                    ObjectiveStates.Add(_goalPosition);
                }
            }
        }

        public double MovementPunishement { get; set; } = -1;
        public List<MazeObstruction> Obstructions { get; set; } = new List<MazeObstruction>();

        public override void InitializeStatesTable()
        {
            base.InitializeStatesTable();
            ObjectiveAction = (int)Actions.CompleteRun;

            for (int state = 0; state < NumberOfStates; ++state)
            {
                int position = GetPosition(state);

                if (position >= Columns) ObservationSpace[state][(int)Actions.MoveUp] = state - Columns;
                if (position % Columns != 0 && position > 0) ObservationSpace[state][(int)Actions.MoveLeft] = state - 1;
                if (position < TotalSpaces - 1 && (position + 1) % Columns != 0) ObservationSpace[state][(int)Actions.MoveRight] = state + 1;
                if (position < TotalSpaces - Columns) ObservationSpace[state][(int)Actions.MoveDown] = state + Columns;

                if (position == GoalPosition)
                {
                    ObservationSpace[state][(int)Actions.CompleteRun] = state;
                    ObservationSpace[state][(int)Actions.MoveDown] = -1;
                    ObservationSpace[state][(int)Actions.MoveLeft] = -1;
                    ObservationSpace[state][(int)Actions.MoveRight] = -1;
                    ObservationSpace[state][(int)Actions.MoveUp] = -1;
                }
            }

            SetObstructions();
        }

        public override void InitializeRewardsTable()
        {
            base.InitializeRewardsTable();

            for(int i = 0; i < Rewards.Length; ++i)
            {
                for (int j = 0; j < Rewards[i].Length; ++j)
                {
                    Rewards[i][j] = -1;
                }
            }

            int phase = 0;

            while (GoalPosition + phase <= NumberOfStates)
            {
                Rewards[GoalPosition + phase][(int)Actions.CompleteRun] = ObjectiveReward;
                phase += TotalSpaces;
            }
        }

        protected virtual int GetPosition(int state)
        {
            return state % TotalSpaces;
        }

        public virtual void AddObstruction(int betweenState, int andState)
        {
            if (ObservationSpace == null)
                InitializeStatesTable();

            var exists = Obstructions.Where(o => (o.BetweenSpace == betweenState && o.AndSpace == andState) ||
                (o.AndSpace == betweenState && o.BetweenSpace == andState));

            if (exists == null)
                return;

            var obstruction = new MazeObstruction
            {
                BetweenSpace = betweenState,
                AndSpace = andState
            };

            Obstructions.Add(obstruction);
            SetObstruction(obstruction);
        }

        protected virtual void SetObstructions()
        {
            foreach (var obstruction in Obstructions)
            {
                SetObstruction(obstruction);
            }
        }

        protected virtual void SetObstruction(MazeObstruction obstruction)
        {
            var betweenAction = GetActionBetweenStates(obstruction.BetweenSpace, obstruction.AndSpace);
            var andAction = GetActionBetweenStates(obstruction.AndSpace, obstruction.BetweenSpace);
            var phase = 0;

            while (obstruction.BetweenSpace + phase <= TotalSpaces)
            {
                ObservationSpace[obstruction.BetweenSpace + phase][(int)betweenAction] = -1;
                ObservationSpace[obstruction.AndSpace + phase][(int)andAction] = -1;
                phase += TotalSpaces;
            }
        }

        public virtual void RemoveObstruction(int betweenState, int andState)
        {
            var exists = Obstructions.Where(o => (o.BetweenSpace == betweenState && o.AndSpace == andState) ||
                (o.AndSpace == betweenState && o.BetweenSpace == andState));

            foreach(var obstruction in exists)
            {
                Obstructions.Remove(obstruction);
            }

            int betweenAction = (int)GetActionBetweenStates(betweenState, andState);
            int andAction = (int)GetActionBetweenStates(andState, betweenState);

            ObservationSpace[betweenState][betweenAction] = andState;
            ObservationSpace[andState][andAction] = betweenState;
        }

        protected virtual Actions GetActionBetweenStates(int betweenState, int andState)
        {
            int differential = betweenState - andState;

            if (differential == Columns) return Actions.MoveUp;
            if (differential == -Columns) return Actions.MoveDown;
            if (differential == 1) return Actions.MoveLeft;
            if (differential == -1) return Actions.MoveRight;
            if (differential == TotalSpaces) return Actions.GetCustomReward;

            return Actions.CompleteRun;

        }

        public virtual void AddReward(CustomObjective reward)
        {
            var r = AdditionalRewards.Where(s => s.State == reward.State).FirstOrDefault();

            if (r != null)
                return;

            AdditionalRewards.Add(reward);
        }

        public virtual void AddReward(int state, double reward)
        {
            AddReward(new CustomObjective { State = state, Value = reward });
        }

        public virtual void RemoveReward(int state)
        {
            var reward = AdditionalRewards.Where(s => s.State == state).FirstOrDefault();

            if (reward != null)
                AdditionalRewards.Remove(reward);
        }

        public List<CustomObjective> GetAdditionalRewards()
        {
            return AdditionalRewards;
        }
    }
}
