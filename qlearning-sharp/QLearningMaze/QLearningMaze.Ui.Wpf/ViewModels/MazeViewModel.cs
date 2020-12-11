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

    public class MazeViewModel : ViewModelBase
    {
        public MazeViewModel()
        {
            _maze = new MazeBase(8, 8, 0, 63, 200);
        }

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

                    row.ObservationSpaces.Add(observationSpace);

                    position++;
                }

                rows.Add(row);
            }

            ObservationRows = rows;
        }


    }
}
