namespace QLearningMaze.Ui.Wpf.ViewModels
{
    using System;
    using System.Windows;    
    using System.Windows.Input;
    using System.Collections.Generic;
    using System.Text;
    using Core;
    using Core.Agent;
    using Commands;
    using System.Threading;
    using System.Threading.Tasks;

    public class AgentViewModel : ViewModelBase
    {
        public AgentViewModel()
        {
            TempLoadAgent();            
        }

        private MazeAgent _primaryAgent = new MazeAgent();

        public MazeAgent PrimaryAgent
        {
            get { return _primaryAgent; }
            set 
            {
                MazeVm.Maze = value.Environment;
                SetProperty(ref _primaryAgent, value);

                if (_primaryAgent != null)
                {
                    _primaryAgent.AgentStateChanged += AgentStateChanged;
                    MazeVm.PrimaryActiveState = _primaryAgent.StartPosition;                    
                    MazeVm.SetActiveState(MazeVm.PrimaryActiveState);
                }
            }
        }

        private MazeAgent _secondaryAgent = new MazeAgent();

        public MazeAgent SecondaryAgent
        {
            get { return _secondaryAgent; }
            set 
            { 
                SetProperty(ref _secondaryAgent, value);

                if (_secondaryAgent != null)
                {
                    _secondaryAgent.AgentStateChanged += AgentStateChanged;
                    MazeVm.SecondaryActiveState = _secondaryAgent.StartPosition;
                    MazeVm.SetActiveState(MazeVm.SecondaryActiveState);
                }
            }
        }

        private MazeViewModel _mazeVm = new MazeViewModel();

        public MazeViewModel MazeVm
        {
            get { return _mazeVm; }
            set { SetProperty(ref _mazeVm, value); }
        }

        private bool _useSecondAgent;

        public bool UseSecondAgent
        {
            get { return _useSecondAgent; }
            set 
            { 
                SetProperty(ref _useSecondAgent, value);
                OnPropertyChanged(nameof(SecondAgentVisibility));
            }
        }

        private bool _isTraining;

        public bool IsTraining
        {
            get { return _isTraining; }
            set 
            {
                SetProperty(ref _isTraining, value);
                OnPropertyChanged(nameof(EnableEntry));
            }
        }

        private bool _isRunning;

        public bool IsRunning
        {
            get { return _isRunning; }
            set 
            {
                SetProperty(ref _isRunning, value);
                OnPropertyChanged(nameof(EnableEntry));
            }
        }


        public bool EnableEntry 
        { 
            get
            {
                return !IsTraining && !IsRunning;
            }
        }

        public Visibility SecondAgentVisibility
        {
            get
            {
                return (_useSecondAgent ? Visibility.Visible : Visibility.Hidden);
            }
        }

        private RelayCommand _trainCommand;

        public RelayCommand TrainCommand
        {
            get 
            { 
                if (_trainCommand == null)
                {
                    _trainCommand = new RelayCommand(() => Task.Run(Train));
                }

                return _trainCommand; 
            }
        }

        private RelayCommand _runMazeCommand;

        public RelayCommand RunMazeCommand
        {
            get 
            { 
                if (_runMazeCommand == null)
                {
                    _runMazeCommand = new RelayCommand(() => Task.Run(Run));
                }

                return _runMazeCommand; 
            }
        }

        private void TempLoadAgent()
        {
            string path = @"C:\Dev\.Net\q-learning\qlearning-sharp\QLearningMaze\QLearningMaze.Core\assets\TestMazes\Demo 8x8 c.maze";
            PrimaryAgent = MazeUtilities.LoadObject<MazeAgent>(path);
        }

        public Task Train()
        {
            List<Task> tasks;

            IsTraining = true;

            if (UseSecondAgent)
            {
                tasks = new List<Task>()
                {
                    TrainAgent(PrimaryAgent),
                    TrainAgent(SecondaryAgent)
                };

            }
            else
            {
                tasks = new List<Task>()
                {
                    TrainAgent(PrimaryAgent)
                };
            }

            Task.WaitAll(tasks.ToArray());

            IsTraining = false;
            return Task.CompletedTask;
            
        }

        private Task TrainAgent(MazeAgent agent)
        {
            agent.Train();
            return Task.CompletedTask;
        }

        public Task Run()
        {
            List<Task> tasks;

            IsRunning = true;

            if (UseSecondAgent)
            {
                tasks = new List<Task>()
                {
                    RunAgent(PrimaryAgent),
                    RunAgent(SecondaryAgent)
                };
            }
            else
            {
                tasks = new List<Task>()
                {
                    RunAgent(PrimaryAgent)
                };
            }

            Task.WaitAll(tasks.ToArray());

            IsRunning = false;

            return Task.CompletedTask;
        }

        private Task RunAgent(MazeAgent agent)
        {
            agent.Run(agent.StartPosition);
            return Task.CompletedTask;
        }

        private void AgentStateChanged(object sender, QLearning.Core.AgentStateChangedEventArgs e)
        {
            bool isPrimary = sender == _primaryAgent;
            
            MazeVm.SetActiveState(e.NewState, isPrimary);
            System.Threading.Thread.Sleep(250);
        }
    }
}
