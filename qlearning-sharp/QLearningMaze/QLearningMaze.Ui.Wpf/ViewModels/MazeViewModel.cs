namespace QLearningMaze.Ui.Wpf.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Collections.ObjectModel;
    using Core.Mazes;
    using Core.Agent;
    using System.Runtime.CompilerServices;
    using Controls;
    using System.Linq;
    using Core;
    
    public class MazeViewModel : ViewModelBase
    {
        public int PrimaryActiveState { get; set; }
        public int SecondaryActiveState { get; set; }

        private IMaze _maze;

        public IMaze Maze
        {
            get { return _maze; }
            set 
            {
                if (_maze != value)
                {
                    SetProperty(ref _maze, value);
                    InitializeMaze();
                }
            }
        }

        private ObservableCollection<ObservationRowViewModel> _observationRows;

        public ObservableCollection<ObservationRowViewModel> ObservationRows
        {
            get { return _observationRows; }
            set { SetProperty(ref _observationRows, value); }
        }

        public virtual void InitializeMaze()
        {
            int position = 0;
            ObservableCollection<ObservationRowViewModel> rows = new ObservableCollection<ObservationRowViewModel>();

            if (Maze == null)
                return;

            for (int i = 0; i < Maze.Rows; i++)
            {
                var row = new ObservationRowViewModel();
                row.RowIndex = i;
                row.ObservationSpaces = new ObservableCollection<ObservationSpaceViewModel>();

                for (int j = 0; j < Maze.Columns; j++)
                {
                    var observationSpace = new ObservationSpaceViewModel();
                    observationSpace.Position = position;
                    
                    if (Maze.TerminalStates.Contains(position))
                    {
                        observationSpace.IsGoal = true;
                    }

                    if (position == Maze.GetInitialState())
                    {
                        observationSpace.IsStart = true;
                    }

                    var reward = Maze.AdditionalRewards.Where(p => p.State == position).FirstOrDefault();

                    if (reward != null)
                    {
                        observationSpace.Reward = reward.Value;
                    }

                    row.ObservationSpaces.Add(observationSpace);

                    position++;
                }

                rows.Add(row);
            }

            ObservationRows = rows;

            SetObstructions();

            var goalSpace = GetSpaceByPosition(Maze.GoalPosition);
            goalSpace.IsGoal = true;
        }
        
        protected void SetObstructions()
        {
            foreach(var obstruction in Maze.Obstructions)
            {
                var betweenSpace = GetSpaceByPosition(obstruction.BetweenSpace);
                var andSpace = GetSpaceByPosition(obstruction.AndSpace);
                int diff = betweenSpace.Position - andSpace.Position;

                if (Maze.Columns * -1 == diff)
                {
                    betweenSpace.BottomWallVisibility = System.Windows.Visibility.Visible;
                    andSpace.TopWallVisibility = System.Windows.Visibility.Visible;
                }
                else if (Maze.Columns == diff)
                {
                    betweenSpace.TopWallVisibility = System.Windows.Visibility.Visible;
                    andSpace.BottomWallVisibility = System.Windows.Visibility.Visible;
                }
                else if (diff == -1)
                {
                    betweenSpace.RightWallVisibility = System.Windows.Visibility.Visible;
                    andSpace.LeftWallVisibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    betweenSpace.LeftWallVisibility = System.Windows.Visibility.Visible;
                    andSpace.RightWallVisibility = System.Windows.Visibility.Visible;
                }
            }
        }

        public void SetActiveState(int state, bool isPrimary = true)
        {
            int previousState;

            if (isPrimary)
            {
                previousState = PrimaryActiveState;
            }
            else
            {
                previousState = SecondaryActiveState;
            }

            int position = previousState % Maze.StatesPerPhase;
            var oldSpace = GetSpaceByPosition(position);
            oldSpace.SetInactive();

            position = state % Maze.StatesPerPhase;
            var newSpace = GetSpaceByPosition(position);
            newSpace.SetActive(isPrimary);

            if (isPrimary)
            {
                PrimaryActiveState = state;
            }
            else
            {
                SecondaryActiveState = state;
            }
        }

        public ObservationSpaceViewModel GetSpaceByPosition(int position)
        {
            var row = ObservationRows.Where(r => r.ObservationSpaces.Any(s => s.Position == position)).FirstOrDefault();
            var space = row.ObservationSpaces.Where(p => p.Position == position).FirstOrDefault();
            return space;
        }
    }
}
