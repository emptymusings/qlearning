namespace QLearningMaze.Ui.Wpf.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Text;
    using Core.Mazes;
    using QLearning.Core.Environment;
    using System.Linq;
    using Commands;
    using System.Windows;

    public class CustomObjectivesViewModel : ViewModelBase
    {
        private readonly IMaze _maze;

        public CustomObjectivesViewModel(IMaze maze)
        {
            _maze = maze;
        }

        private ObservableCollection<CustomObjective> _objectives;

        public ObservableCollection<CustomObjective> Objectives
        {
            get { return _objectives; }
            set { SetProperty(ref _objectives, value); }
        }

        private int? _objectiveState;

        public int? ObjectiveState
        {
            get { return _objectiveState; }
            set { SetProperty(ref _objectiveState, value); }
        }

        private double? _objectiveValue;

        public double? ObjectiveValue
        {
            get { return _objectiveValue; }
            set { SetProperty(ref _objectiveValue, value); }
        }

        private RelayCommand _addObjectiveCommand;

        public RelayCommand AddObjectiveCommand
        {
            get 
            { 
                if (_addObjectiveCommand == null)
                {
                    _addObjectiveCommand = new RelayCommand(() => AddObjective());
                }

                return _addObjectiveCommand; 
            }
        }

        private CustomObjective _selectedObjective;

        public CustomObjective SelectedObjective
        {
            get { return _selectedObjective; }
            set { SetProperty(ref _selectedObjective, value); }
        }


        private RelayCommand _removeObjectiveCommand;

        public RelayCommand RemoveObjectiveCommand
        {
            get 
            { 
                if (_removeObjectiveCommand == null)
                {
                    _removeObjectiveCommand = new RelayCommand(() => RemoveObjective());
                }

                return _removeObjectiveCommand; 
            }
        }


        public void InitializeObjectives()
        {
            if (_maze == null)
                return;

            _objectives = new ObservableCollection<CustomObjective>();

            foreach(var objective in _maze.AdditionalRewards.OrderBy(r => r.Value > 0 ? 0 : 1).ThenBy(p => p.State % _maze.StatesPerPhase))
            {
                _objectives.Add(objective);
            }
        }

        private void AddObjective()
        {
            if (!ObjectiveState.HasValue ||
                ObjectiveState.Value > _maze.NumberOfStates ||
                ObjectiveState.Value < 0)
            {
                MessageBox.Show("Error: Invalid Reward Position");
                return;
            }

            if (!ObjectiveValue.HasValue)
            {
                MessageBox.Show("Error: Invalid Reward Value");
                return;
            }

            var objective = new CustomObjective
            {
                State = ObjectiveState.Value,
                Value = ObjectiveValue.Value
            };

            Objectives.Add(objective);
        }

        private void RemoveObjective()
        {
            if (_selectedObjective == null)
            {
                MessageBox.Show("Error: No Reward Selected");
                return;
            }

            var objective = Objectives.Where(s => s.State == _selectedObjective.State && s.Value == _selectedObjective.Value).FirstOrDefault();

            if (objective == null)
            {
                return;
            }

            Objectives.Remove(objective);
        }
    }
}
