namespace QLearningMaze.Ui.Forms
{
    using QLearning.Core.Agent;
    using QLearningMaze.Core.Agent;
    using QLearningMaze.Core.Mazes;
    using System;
    using System.Drawing;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using System.Linq;

    public partial class TrainingProgress : Form
    {
        private MazeAgent _agentPrimary;
        private MazeAgent _agentSecondary;
        int _successfulRuns = 0;
        double _averageMoves = 0;
        double _averageScore = 0;
        double _totalMoves = 0;
        double _totalScore = 0;
        double _percentComplete = 0;
        private int _episodeStartSpace;
        private int _showEvery = 2500;
        private MazeSpace _mazeSpace;
        private int _movementPause = 100;
        private int _runEpochs = 0;

        private delegate void EnableControlsHandler(bool value);
        private delegate void UpdateTextHandler(string withValue);
        private delegate void UpdateLabelHandler(string newText);

        public TrainingProgress(MazeAgent agentPrimary, MazeAgent agentSecondary)
        {
            InitializeComponent();
            _agentPrimary = agentPrimary;
            _agentSecondary = agentSecondary;

            if (_agentPrimary.MaximumAllowedBacktracks < 0)
            {
                _agentPrimary.MaximumAllowedBacktracks = 3;
            }

            _showEvery = _agentPrimary.NumberOfTrainingEpisodes/5;
            
            //_maze = agent.Environment;
            _agentPrimary.TrainingEpisodeCompleted += _maze_TrainingEpisodeCompleted;
            _totalMoves = 0;
            _totalScore = 0;
            _averageMoves = 0;
            _averageScore = 0;
        }

        private void _maze_TrainingEpisodeCompleted(object sender, QLearning.Core.TrainingEpisodeCompletedEventArgs e)
        {
            if (e.Success)
            {
                _successfulRuns++;

                _totalMoves += e.TotalMoves;
                _totalScore += e.TotalScore;

                _averageMoves = _totalMoves / _successfulRuns;
                _averageScore = _totalScore / _successfulRuns;
            }

            _runEpochs++;
            _percentComplete = (double)e.CurrentEpisode / (double)e.TotalEpisodes;

            if (e.CurrentEpisode % trainingProgressBar.Step == 0)
            {
                string message = $"Episode: {e.CurrentEpisode.ToString("#,##0")} | " +
                    $"{_percentComplete.ToString("0%")} Complete";

                UpdateLabel(message);
                UpdateProgressBar();
            }

            if ((e.CurrentEpisode + 1) % _showEvery == 0 &&
                e.CurrentEpisode > 1)
            {
                _episodeStartSpace = e.StartPoint;
                RenderTraining();
            }
        }

        private void RenderTraining()
        {
            _mazeSpace = new MazeSpace();
            _mazeSpace.CreateMazeControls(_agentPrimary.Environment);
            _mazeSpace.GetSpaceByPosition(_agentPrimary.Environment.StartPosition).SetStart(false);
            _mazeSpace.GetSpaceByPosition(_episodeStartSpace % _agentPrimary.Environment.StatesPerPhase).SetStart(true);
            Form frm = new Form();
            frm.Size = _mazeSpace.Size = new Size(_mazeSpace.Width + 10, _mazeSpace.Height + 10);
            frm.Controls.Add(_mazeSpace);
            _mazeSpace.Dock = DockStyle.Fill;
            frm.Controls.Add(_mazeSpace);
            frm.Show();
            frm.WindowState = FormWindowState.Maximized;
            frm.FormBorderStyle = FormBorderStyle.None;

            try
            {
                _agentPrimary.AgentStateChanged += _maze_AgentStateChanged;
                _agentPrimary.Run(_episodeStartSpace);
            }
            catch 
            {
                //MessageBox.Show(ex.ToString());
            }
            finally
            {
                _mazeSpace.Dispose();
                _mazeSpace = null;

                frm.Dispose();
                frm = null;

                _agentPrimary.AgentStateChanged -= _maze_AgentStateChanged;
            }
        }

        private void _maze_AgentStateChanged(object sender, QLearning.Core.AgentStateChangedEventArgs e)
        {
            int position = e.NewState % _agentPrimary.Environment.StatesPerPhase;
            var newSpace = _mazeSpace.GetSpaceByPosition(position);

            if (newSpace != null)
            {
                if (MazeSpace.ActiveSpace != null)
                {
                    MazeSpace.ActiveSpace.SetInactive();
                    MazeSpace.ActiveSpace.Invalidate();
                }

                MazeSpace.ActiveSpace = newSpace;

                var start = _mazeSpace.GetSpaceByPosition(_episodeStartSpace % _agentPrimary.Environment.StatesPerPhase);
                start.SetInactive();
                start.Invalidate();

                if (_agentPrimary.Environment.RewardsTable[e.NewState][_agentPrimary.Environment.GetRewardAction] > 0)
                {
                    newSpace.SetReward(false);
                }

                newSpace.SetActive();
                newSpace.Invalidate();
                _mazeSpace.Invalidate();
                newSpace.Refresh();
                _mazeSpace.Refresh();
                System.Threading.Thread.Sleep(_movementPause);
                //Application.DoEvents();
            }
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            _agentPrimary.TrainingEpisodeCompleted -= _maze_TrainingEpisodeCompleted;
            this.Close();
        }

        private Task RunAgentTrainingSessions()
        {
            if (_agentPrimary.LearningStyle == QLearning.Core.LearningStyles.QLearning)
                _agentSecondary.LearningStyle = QLearning.Core.LearningStyles.SARSA;
            else
                _agentSecondary.LearningStyle = QLearning.Core.LearningStyles.QLearning;

            Task[] tasks = new Task[]
            {
                TrainingTask(),
                Task.Run(() => _agentSecondary.Train())
            };

            Task.WaitAll(tasks);
            return Task.CompletedTask;
        }

        private Task TrainingTask()
        {
            EnableControls(false);
            this.Refresh();
            _agentPrimary.Train();
            EnableControls(true);
            this.DialogResult = DialogResult.OK;

            _agentPrimary.TrainingEpisodeCompleted -= _maze_TrainingEpisodeCompleted;

            return Task.CompletedTask;
        }

        private void EnableControls(bool value)
        {
            if (controlsPanel.InvokeRequired)
            {
                controlsPanel.BeginInvoke(new EnableControlsHandler(EnableControls), value);
            }
            else
            {
                controlsPanel.Enabled = value;
            }
        }

        private void UpdateText(string withValue)
        {
            if (trainingProgressTextbox.InvokeRequired)
            {
                trainingProgressTextbox.BeginInvoke(new UpdateTextHandler(UpdateText), withValue);
            }
            else
            {
                trainingProgressTextbox.AppendText(withValue + "\r\n");
                //trainingProgressTextbox.SelectionStart = trainingProgressTextbox.Text.Length;
            }
        }

        private void UpdateLabel(string newText)
        {
            if (progressLabel.InvokeRequired)
            {
                progressLabel.BeginInvoke(new UpdateLabelHandler(UpdateLabel), newText);
            }
            else
            {
                progressLabel.Text = newText;
                progressLabel.Refresh();
            }
        }

        private void UpdateProgressBar()
        {
            trainingProgressBar.PerformStep();
        }

        private void TrainingProgress_Shown(object sender, EventArgs e)
        {

            trainingProgressBar.Maximum = _agentPrimary.NumberOfTrainingEpisodes;
            trainingProgressBar.Value = 0;
            trainingProgressBar.Step = 1;
            RunAgentTrainingSessions();
        }

    }
}
