using QLearningMaze.Core;
using QLearningMaze.Core.Mazes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using QLearning.Core;
using QLearningMaze.Core.Mazes;

namespace QLearningMaze.Ui.Forms
{
    public partial class TrainingSessionSelector : Form
    {
        private IAgent<MazeBase> _agent;
        private bool _isChecking;
        private int _moves;
        private double _score;

        private List<TrainingSessionEx> trainingSessions = new List<TrainingSessionEx>();
        public TrainingSessionEx SelectedSession { get; set; }

        public TrainingSessionSelector(IAgent<MazeBase> agent)
        {
            InitializeComponent();
            _agent = agent;
            _agent.AgentCompleted += Maze_AgentCompleted;
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
            _agent.AgentCompleted -= Maze_AgentCompleted;
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
            var sessions = QLearningMaze.Core.TrainingSession.GetTrainingSessions(_agent.Environment.QualitySaveDirectory).OrderBy(e => e.Episode).ToList();
            var maze = GetTestMaze();

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

                maze.QualityTable = session.Quality;

                try
                {
                    _agent.RunAgent(maze.StartPosition);
                    session.Succeeded = true;
                }
                catch (Exception ex)
                {
                    //sessions.RemoveAt(i);
                    session.Succeeded = false;                    
                    //continue;
                }

                session.Moves = _moves;
                session.Score = _score; 
                
                trainingSessions.Add(session);
            }

            //trainingSessions = sessions.OrderByDescending(s => s.Score).ThenBy(m => m.Moves).ThenBy(e => e.Episode);
            var selection = trainingSessions
                //.GroupBy(x => new 
                //{
                //    x.Moves,
                //    x.Score,
                //    x.Succeeded
                //})
                .Select(s => new TrainingSessionEx()
                {
                    MinEpisode = s.Episode,
                    MaxEpisode = s.Episode,
                    Episode = s.Episode,
                    Moves = s.Moves,
                    Score = s.Score,
                    Succeeded = s.Succeeded,
                    Quality = s.Quality
                });

            //trainingSessions = selection.OrderBy(e => e.MinEpisode).ToList();
            trainingSessions = selection.OrderByDescending(s => s.Succeeded).ThenByDescending(s => s.Score).ThenByDescending(m => m.Moves).ThenByDescending(e => e.MinEpisode).ToList();
            SelectedSession = trainingSessions.FirstOrDefault();

            _isChecking = false;
            return Task.CompletedTask;
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
