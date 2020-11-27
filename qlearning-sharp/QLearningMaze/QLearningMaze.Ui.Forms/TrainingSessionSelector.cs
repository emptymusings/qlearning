namespace QLearningMaze.Ui.Forms
{
    using QLearning.Core.Agent;
    using QLearningMaze.Core;
    using QLearningMaze.Core.Mazes;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    public partial class TrainingSessionSelector : Form
    {
        private ITDAgent<IMaze> _agent;
        private bool _isChecking;
        private int _moves;
        private double _score;

        private List<TrainingSessionEx> trainingSessions = new List<TrainingSessionEx>();
        public TrainingSessionEx SelectedSession { get; set; }

        public TrainingSessionSelector(ITDAgent<IMaze> agent)
        {
            InitializeComponent();
            _agent = agent;
        }

        private delegate void ShowResultsHandler();

        private void TrainingSessionSelector_Load(object sender, EventArgs e)
        {
            
        }

        private void sesssionList_SelectedIndexChanged(object sender, EventArgs e)
        {
            useSessionButton.Enabled = true;
        }

        private void useSessionButton_Click(object sender, EventArgs e)
        {
            var episode = Convert.ToInt32(sesssionList.SelectedItems[0].Text);
            SelectedSession = trainingSessions.Where(s => s.Episode == episode).FirstOrDefault();

            this.DialogResult = DialogResult.OK;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void FillList()
        {
            if (sesssionList.InvokeRequired)
            {
                sesssionList.BeginInvoke(new ShowResultsHandler(FillList));
            }
            else
            {
                sesssionList.Items.Clear();

                foreach (var session in trainingSessions)
                {
                    ListViewItem lvi = new ListViewItem(session.MinEpisode.ToString());
                    lvi.SubItems.Add(session.MaxEpisode.ToString());
                    lvi.SubItems.Add(session.Moves.ToString("#,##0"));
                    lvi.SubItems.Add(session.Score.ToString("#,##0.##"));
                    lvi.SubItems.Add(session.Succeeded ? "Yes" : "No");
                    sesssionList.Items.Add(lvi);
                }

                if (sesssionList.Items.Count > 0)
                    sesssionList.Items[0].Selected = true;

                cancelButton.Enabled = true;
                this.ControlBox = true;
                Cursor = Cursors.Default;
            }
        }

        private Task StartTestManager()
        {
            _isChecking = true;

            Task.Run(RunTests);

            while (_isChecking)
            {
                System.Threading.Thread.Sleep(1);
            }
            
            FillList();

            return Task.CompletedTask;
        }

        private Task RunTests()
        {
            //var sessions = QLearningMaze.Core.TrainingSession
            //    .GetTrainingSessions(_agent.Environment.QualitySaveDirectory)
            //    .OrderBy(e => e.Episode).ToList();

            var sessions = _agent.TrainingSessions.OrderBy(e => e.Episode).ToList();

            var agent = GetTestAgent();
            agent.AgentCompleted += Maze_AgentCompleted;

            for (int i = sessions.Count - 1; i >= 0; --i)
            {
                var session = new TrainingSessionEx
                {
                    Episode = sessions[i].Episode,
                    Moves = sessions[i].Moves,
                    Quality = sessions[i].Quality,
                    Score = sessions[i].Score
                };

                _moves = 0;
                _score = 0;

                agent.Environment.QualityTable = session.Quality;

                try
                {
                    agent.Run(agent.Environment.StartPosition);
                    session.Succeeded = true;
                }
                catch 
                {
                    session.Succeeded = false;                                     
                }

                session.Moves = _moves;
                session.Score = _score; 
                
                trainingSessions.Add(session);
            }

            var selection = trainingSessions
                .GroupBy(x => new
                {
                    x.Moves,
                    x.Score,
                    x.Succeeded
                })
                .Select(s => new TrainingSessionEx()
                {
                    MinEpisode = s.Last().Episode,
                    MaxEpisode = s.First().Episode,
                    Episode = s.Last().Episode,
                    Moves = s.Key.Moves,
                    Score = s.Key.Score,
                    Succeeded = s.Key.Succeeded,
                    Quality = s.First().Quality
                });

            trainingSessions = selection.OrderByDescending(s => s.Succeeded).ThenByDescending(s => s.Score).ThenByDescending(m => m.Moves).ThenByDescending(e => e.MinEpisode).ToList();
            SelectedSession = trainingSessions.FirstOrDefault();

            _isChecking = false;

            agent.AgentCompleted -= Maze_AgentCompleted;
            return Task.CompletedTask;
        }

        private ITDAgent<IMaze> GetTestAgent()
        {
            ITDAgent<IMaze> agent = new TDAgent<IMaze>
            {
                DiscountRate = _agent.DiscountRate,
                Environment = GetTestMaze(),
                EpsilonDecayEnd = _agent.EpsilonDecayEnd,
                EpsilonDecayStart = _agent.EpsilonDecayStart,
                LearningRate = _agent.LearningRate,
                MaximumAllowedBacktracks = _agent.MaximumAllowedBacktracks,
                MaximumAllowedMoves = _agent.MaximumAllowedMoves,
                NumberOfTrainingEpisodes = _agent.NumberOfTrainingEpisodes
            };


            if (agent.MaximumAllowedBacktracks < 0)
            {
                agent.MaximumAllowedBacktracks = 3;
            }

            return agent;
        }

        private MazeBase GetTestMaze()
        {
            var maze = new MazeBase(
                _agent.Environment.Columns,
                _agent.Environment.Rows,
                _agent.Environment.StartPosition,
                _agent.Environment.GoalPosition,
                _agent.Environment.ObjectiveReward);
            maze.RewardsTable = _agent.Environment.RewardsTable;
            maze.StatesTable = _agent.Environment.StatesTable;
            maze.AdditionalRewards = _agent.Environment.AdditionalRewards;

            

            return maze;
        }

        private void Maze_AgentCompleted(object sender, QLearning.Core.AgentCompletedEventArgs e)
        {
            _moves += e.Moves;
            _score += e.Rewards;
        }

        private void TrainingSessionSelector_Shown(object sender, EventArgs e)
        {
            if (trainingSessions == null ||
                trainingSessions.Count == 0)
            {
                Cursor = Cursors.WaitCursor;
                Task.Run(StartTestManager);
            }
        }

        private void sesssionList_DoubleClick(object sender, EventArgs e)
        {
            if (sesssionList.SelectedItems.Count > 0)
            {
                useSessionButton_Click(sender, e);
            }
        }
    }

    public class TrainingSessionEx : QLearningMaze.Core.TrainingSession
    {
        public int MinEpisode { get; set; }
        public int MaxEpisode { get; set; }
        public bool Succeeded { get; set;}
    }
}
