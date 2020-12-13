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
                    _primaryAgent.TrainingEpisodeCompleted += AgentTrainingEpisodeCompleted;
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

        private int _trainingEpisodesCompleted = 0;

        public int TrainingEpisodesCompleted
        {
            get { return _trainingEpisodesCompleted; }
            set { SetProperty(ref _trainingEpisodesCompleted, value); }
        }

        private Visibility _trainingProgressVisibility = Visibility.Hidden;

        public Visibility TrainingProgressVisibility
        {
            get { return _trainingProgressVisibility; }
            set { SetProperty(ref _trainingProgressVisibility, value); }
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
            LoadAgent(path);
        }

        private void LoadAgent(string path)
        {
            var loaded = MazeUtilities.LoadObject<MazeAgent>(path);
            PrimaryAgent = MazeUtilities.ConvertLoadedAgent(loaded);
            SecondaryAgent = MazeUtilities.ConvertLoadedAgent(loaded);
            SecondaryAgent.Environment = MazeUtilities.CopyEnvironment(loaded.Environment);
            SecondaryAgent.StartPosition = PrimaryAgent.StartPosition;

            if (PrimaryAgent.LearningStyle == QLearning.Core.LearningStyles.QLearning)
            {
                SecondaryAgent.LearningStyle = QLearning.Core.LearningStyles.SARSA;
            }
            else
            {
                SecondaryAgent.LearningStyle = QLearning.Core.LearningStyles.QLearning;
            }
        }

        public Task Train()
        {
            List<Task> tasks = new List<Task>();
            List<MazeAgent> agents = new List<MazeAgent>();

            agents.Add(PrimaryAgent);

            if (SecondaryAgent != null)
            {
                SecondaryAgent.LearningRate = PrimaryAgent.LearningRate;
                SecondaryAgent.DiscountRate = PrimaryAgent.DiscountRate;
                SecondaryAgent.NumberOfTrainingEpisodes = PrimaryAgent.NumberOfTrainingEpisodes;
                SecondaryAgent.Environment.QualitySaveDirectory = null;

                if (PrimaryAgent.LearningStyle == QLearning.Core.LearningStyles.QLearning)
                {
                    SecondaryAgent.LearningStyle = QLearning.Core.LearningStyles.SARSA;
                }
                else
                {
                    SecondaryAgent.LearningStyle = QLearning.Core.LearningStyles.QLearning;
                }

                agents.Add(SecondaryAgent);
            }

            TrainingEpisodesCompleted = 0;
            TrainingProgressVisibility = Visibility.Visible;
                        
            IsTraining = true;

            var result = Parallel.ForEach(agents, (agent) =>
            {
                TrainAgent(agent);
            });

            while (!result.IsCompleted)
            {

            }

            IsTraining = false;
            TrainingProgressVisibility = Visibility.Hidden;
            return Task.CompletedTask;
            
        }

        private Task TrainAgent(MazeAgent agent)
        {
            agent.Train();
            return Task.CompletedTask;
        }

        public Task Run()
        {
            var agents = new List<MazeAgent>();
            agents.Add(PrimaryAgent);

            if (UseSecondAgent)
            {
                agents.Add(SecondaryAgent);
            }


            IsRunning = true;

            var pResults = Parallel.ForEach(agents, (agent) =>
            {
                RunAgent(agent);
            });

            while (!pResults.IsCompleted)
            {

            }
            
            IsRunning = false;

            return Task.CompletedTask;
        }

        private Task RunAgent(MazeAgent agent)
        {
            try
            {
                agent.Run(agent.StartPosition);
            }
            catch (Exception ex)
            {
                string agentVal = agent == PrimaryAgent ? "Primary" : "Secondary";

                MessageBox.Show($"Error with {agentVal} Agent: {ex.Message}");
            }

            return Task.CompletedTask;
        }

        private void AgentStateChanged(object sender, QLearning.Core.AgentStateChangedEventArgs e)
        {
            bool isPrimary = sender == _primaryAgent;
            
            MazeVm.SetActiveState(e.NewState, isPrimary);
            System.Threading.Thread.Sleep(250);
        }

        private void AgentTrainingEpisodeCompleted(object sender, QLearning.Core.TrainingEpisodeCompletedEventArgs e)
        {
            TrainingEpisodesCompleted++;
        }
    }
}
