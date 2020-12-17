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

            if (Maze.Rows == 0)
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
                    observationSpace.WallClicked += ObservationSpace_WallClicked;     
                    
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
            int visible = 100;
            int invisible = 0;

            foreach(var obstruction in Maze.Obstructions)
            {
                var betweenSpace = GetSpaceByPosition(obstruction.BetweenSpace);
                var andSpace = GetSpaceByPosition(obstruction.AndSpace);
                int diff = betweenSpace.Position - andSpace.Position;

                if (Maze.Columns * -1 == diff)
                {
                    betweenSpace.BottomWallVisibility = visible;
                    andSpace.TopWallVisibility = visible;
                }
                else if (Maze.Columns == diff)
                {
                    betweenSpace.TopWallVisibility = visible;
                    andSpace.BottomWallVisibility = visible;
                }
                else if (diff == -1)
                {
                    betweenSpace.RightWallVisibility = visible;
                    andSpace.LeftWallVisibility = visible;
                }
                else
                {
                    betweenSpace.LeftWallVisibility = visible;
                    andSpace.RightWallVisibility = visible;
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

        public ObservationRowViewModel GetRowByPosition(int position)
        {
            return ObservationRows.Where(r => r.ObservationSpaces.Any(s => s.Position == position)).FirstOrDefault();
        }

        public ObservationSpaceViewModel GetSpaceByPosition(int position)
        {
            var row = GetRowByPosition(position);
            return GetSpaceByPosition(row, position);
        }

        public ObservationSpaceViewModel GetSpaceByPosition(ObservationRowViewModel row, int position)
        {
            if (row == null)
                return null;

            var space = row.ObservationSpaces.Where(p => p.Position == position).FirstOrDefault();
            return space;
        }

        private void ObservationSpace_WallClicked(object sender, WallClickedEventArgs e)
        {
            var row = GetRowByPosition(e.Position);
            var space = GetSpaceByPosition(row, e.Position);
            int adjacentPosition = GetAdjacentSpace(row, space, e);
            var adjacentSpace = GetSpaceByPosition(adjacentPosition);

            SetAndSpaceOpacity(adjacentSpace, e.IsVisible, e.WallName);

            if (e.IsVisible)
                AddWall(e.Position, adjacentPosition);
            else
                RemoveWall(e.Position, adjacentPosition);
        }

        private int GetAdjacentSpace(
            ObservationRowViewModel row,
            ObservationSpaceViewModel space,
            WallClickedEventArgs e)
        {
            switch (e.WallName.ToLowerInvariant())
            {
                case "left":
                    if (row.ObservationSpaces[0].Position == e.Position)
                        return - 1;
                    else
                        return e.Position - 1;
                case "right":
                    if (row.ObservationSpaces[row.ObservationSpaces.Count - 1].Position == e.Position)
                        return - 1;
                    else
                        return e.Position + 1;
                case "top":
                    if (row.RowIndex == 0)
                        return - 1;
                    else
                        return e.Position - Maze.Columns;
                case "bottom":
                    if (row.RowIndex == Maze.Rows - 1)
                        return - 1;
                    else
                        return e.Position + Maze.Columns;
                default:
                    return - 1;
            }
        }

        private void SetAndSpaceOpacity(ObservationSpaceViewModel space, bool makeVisible, string betweenWallName)
        {
            switch(betweenWallName.ToLowerInvariant())
            {
                case "right":
                    space.LeftWallVisibility = (makeVisible ? 100 : 0);
                    break;
                case "left":
                    space.RightWallVisibility = (makeVisible ? 100 : 0);
                    break;
                case "bottom":
                    space.TopWallVisibility = (makeVisible ? 100 : 0);
                    break;
                case "top":
                    space.BottomWallVisibility = (makeVisible ? 100 : 0);
                    break;
                default:
                    break;
            }
        }

        private void AddWall(int betweenSpace, int andSpace)
        {
            Maze.AddObstruction(betweenSpace, andSpace);
        }

        private void RemoveWall(int betweenSpace, int andSpace)
        {
            Maze.RemoveObstruction(betweenSpace, andSpace);
        }
    }
}
