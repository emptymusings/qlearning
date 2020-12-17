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
    using Microsoft.Win32;
    using System.Linq;
    using QLearningMaze.Core.Mazes;

    public class AgentViewModel : ViewModelBase
    {
        TrainingSessionsViewModel _sessionsVm;

        public AgentViewModel()
        {
            //TempLoadAgent();     
            MazeVm.Maze = new MazeBase();
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
                    OnPropertyChanged(nameof(PrimaryAgentStartPosition));
                }
            }
        }

        public int PrimaryAgentStartPosition
        {
            get { return PrimaryAgent.StartPosition; }
            set 
            {
                ChangeStartPosition(PrimaryAgent, value);
                OnPropertyChanged(nameof(PrimaryAgentStartPosition));
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
                    OnPropertyChanged(nameof(SecondaryAgentStartPosition));
                }
            }
        }

        public int SecondaryAgentStartPosition
        {
            get { return SecondaryAgent.StartPosition; }
            set 
            {
                ChangeStartPosition(SecondaryAgent, value);
                OnPropertyChanged(nameof(SecondaryAgentStartPosition));
            }
        }



        private MazeViewModel _mazeVm = new MazeViewModel();

        public MazeViewModel MazeVm
        {
            get { return _mazeVm; }
            set { SetProperty(ref _mazeVm, value); }
        }

        public int GoalPosition
        {
            get { return MazeVm.Maze.GoalPosition; }
            set
            {
                ChangeGoalPosition(value);
                OnPropertyChanged(nameof(GoalPosition));
            }
        }

        private bool _useSecondAgent;

        public bool UseSecondAgent
        {
            get { return _useSecondAgent; }
            set 
            { 
                SetProperty(ref _useSecondAgent, value);

                if (_useSecondAgent)
                {
                    ChangeStartPosition(SecondaryAgent, SecondaryAgent.StartPosition);                    
                }
                else
                {
                    var space = MazeVm.GetSpaceByPosition(SecondaryAgent.StartPosition);
                    space.SetInactive();
                    space.IsStart = false;
                }

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

        public List<string> LearningStyles
        {
            get
            {
                return Enum.GetNames(typeof(QLearning.Core.LearningStyles)).ToList();
            }
        }

        public string SelectedLearningStyle
        {
            get { return PrimaryAgent.LearningStyle.ToString(); }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    PrimaryAgent.LearningStyle = (QLearning.Core.LearningStyles)Enum.Parse(typeof(QLearning.Core.LearningStyles), value, true);

                    if (PrimaryAgent.LearningStyle == QLearning.Core.LearningStyles.QLearning)
                    {
                        SecondaryAgent.LearningStyle = QLearning.Core.LearningStyles.SARSA;
                    }
                    else;
                    {
                        SecondaryAgent.LearningStyle = QLearning.Core.LearningStyles.QLearning;
                    }
                }

                OnPropertyChanged(nameof(SelectedLearningStyle));
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


        private RelayCommand _editObjectivesCommand;

        public RelayCommand EditObjectivesCommand
        {
            get 
            { 
                if (_editObjectivesCommand == null)
                {
                    _editObjectivesCommand = new RelayCommand(() => EditObjectives());
                }

                return _editObjectivesCommand; 
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

        private RelayCommand _viewQualityCommand;

        public RelayCommand ViewQualityCommand
        {
            get 
            { 
                if (_viewQualityCommand == null)
                {
                    _viewQualityCommand = new RelayCommand(() => ShowDetailedView(MazeTableViewModel.TableTypes.Quality));
                }
                return _viewQualityCommand; 
            }
        }

        private RelayCommand _viewRewardsCommand;

        public RelayCommand ViewRewardsCommand
        {
            get 
            { 
                if (_viewRewardsCommand == null)
                {
                    _viewRewardsCommand = new RelayCommand(() => ShowDetailedView(MazeTableViewModel.TableTypes.Rewards));
                }

                return _viewRewardsCommand; 
            }
        }

        private RelayCommand _viewStatesCommand;

        public RelayCommand ViewStatesCommand
        {
            get 
            { 
                if (_viewStatesCommand == null)
                {
                    _viewStatesCommand = new RelayCommand(() => ShowDetailedView(MazeTableViewModel.TableTypes.States));
                }

                return _viewStatesCommand; 
            }
        }

        private RelayCommand _openAgentMazeCommand;

        public RelayCommand OpenAgentMazeCommand
        {
            get 
            { 
                if (_openAgentMazeCommand == null)
                {
                    _openAgentMazeCommand = new RelayCommand(() => OpenAgentMaze());
                }

                return _openAgentMazeCommand; 
            }
        }

        private RelayCommand _saveAgentMazeCommand;

        public RelayCommand SaveAgentMazeCommand
        {
            get 
            { 
                if (_saveAgentMazeCommand == null)
                {
                    _saveAgentMazeCommand = new RelayCommand(() => SaveAgentMaze());
                }

                return _saveAgentMazeCommand; 
            }
        }

        private RelayCommand _selectQualityCommand;

        public RelayCommand SelectQualityCommand
        {
            get 
            { 
                if (_selectQualityCommand == null)
                {
                    _selectQualityCommand = new RelayCommand(() => GetQualityFromTraining());
                }

                return _selectQualityCommand; 
            }
        }


        private void ChangeStartPosition(MazeAgent agent, int newStart)
        {
            var space = MazeVm.GetSpaceByPosition(agent.StartPosition);
            space.IsStart = false;

            agent.StartPosition = newStart;
            space = MazeVm.GetSpaceByPosition(agent.StartPosition);
            space.IsStart = true;
            MazeVm.SetActiveState(newStart, agent == PrimaryAgent);
        }


        private void ChangeGoalPosition(int newGoal)
        {
            var space = MazeVm.GetSpaceByPosition(MazeVm.Maze.GoalPosition);
            space.IsGoal = false;

            space = MazeVm.GetSpaceByPosition(newGoal);
            space.IsGoal = true;
            MazeVm.Maze.GoalPosition = newGoal;
            PrimaryAgent.Environment.GoalPosition = newGoal;
            SecondaryAgent.Environment.GoalPosition = newGoal;
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

            OnPropertyChanged(nameof(SelectedLearningStyle));
            OnPropertyChanged(nameof(PrimaryAgent));
            OnPropertyChanged(nameof(SecondaryAgent));
        }

        private void EditObjectives()
        {
            var vm = new CustomObjectivesViewModel(PrimaryAgent.Environment);
            vm.InitializeObjectives();
            
            Views.CustomObjectivesView dialog = new Views.CustomObjectivesView();
            dialog.DataContext = vm;

            var result = dialog.ShowDialog();

            if (result.HasValue &&
                result.Value)
            {
                PrimaryAgent.Environment.AdditionalRewards.Clear();

                foreach(var objective in vm.Objectives)
                {
                    PrimaryAgent.Environment.AddReward(objective);
                }

                if (SecondaryAgent != null)
                {
                    SecondaryAgent.Environment = MazeUtilities.CopyEnvironment(PrimaryAgent.Environment);
                }

                MazeVm.InitializeMaze();
            }
            
        }

        public Task Train()
        {
            List<Task> tasks = new List<Task>();
            List<MazeAgent> agents = new List<MazeAgent>();

            agents.Add(PrimaryAgent);

            if (UseSecondAgent &&
                SecondaryAgent != null)
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

            _sessionsVm = null;

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

            if (_sessionsVm == null)
            {
                _sessionsVm = new TrainingSessionsViewModel(PrimaryAgent);
                
                Task.Run(() => _sessionsVm.InitializeSessions());
            }

            IsTraining = false;
            TrainingProgressVisibility = Visibility.Hidden;

            Application.Current.Dispatcher.Invoke(() => GetQualityFromTraining());
            

            return Task.CompletedTask;            
        }

        private Task TrainAgent(MazeAgent agent)
        {
            agent.Train();
            return Task.CompletedTask;
        }


        [STAThread]
        private void GetQualityFromTraining()
        {            
            var dialog = new Views.TrainingSessionSelectorView();
            dialog.DataContext = _sessionsVm;
            var results = dialog.ShowDialog();

            if (results != null && results == true)
            {
                PrimaryAgent.Environment.QualityTable = _sessionsVm.SelectedSession.Quality;
            }
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

        private void ShowDetailedView(MazeTableViewModel.TableTypes tableType)
        {
            MazeTableViewModel mtvm;

            switch(tableType)
            {
                case MazeTableViewModel.TableTypes.Quality:
                    mtvm = new MazeTableViewModel(PrimaryAgent.Environment.QualityTable, PrimaryAgent.Environment.NumberOfStates, tableType);
                    break;
                case MazeTableViewModel.TableTypes.Rewards:
                    mtvm = new MazeTableViewModel(PrimaryAgent.Environment.RewardsTable, PrimaryAgent.Environment.NumberOfStates, tableType);
                    break;
                default:
                    mtvm = new MazeTableViewModel(PrimaryAgent.Environment.StatesTable, PrimaryAgent.Environment.NumberOfStates, tableType);
                    break;
            }

            mtvm.InitializeTable();

            var view = new Views.TabledDetailsView();
            view.DataContext = mtvm;
            view.Show();
        }

        private void OpenAgentMaze()
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Ageng Maze File|*.maze";
            bool? result = dialog.ShowDialog();

            if (result.HasValue && result.Value == true)
            {
                LoadAgent(dialog.FileName);
            }
        }

        private void SaveAgentMaze()
        {
            var dialog = new SaveFileDialog();
            dialog.Filter = "Agent Maze File|*.maze";
            bool? result = dialog.ShowDialog();

            if (result.HasValue && result.Value == true)
            {
                MazeUtilities.SaveObject(dialog.FileName, PrimaryAgent);
            }
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
