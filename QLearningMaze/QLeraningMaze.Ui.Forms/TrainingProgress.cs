using QLearningMaze.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace QLearningMaze.Ui.Forms
{
    public partial class TrainingProgress : Form
    {
        private IMaze _maze;
        private bool _trainingInProgress = false;
        int _successfulRuns = 0;
        double _averageMoves = 0;
        double _averageScore = 0;
        double _totalMoves = 0;
        double _totalScore = 0;
        double _percentComplete = 0;

        private int _showEvery = 2500;
        private MazeSpace _mazeSpace;
        private int _movementPause = 100;
        private int _runEpochs = 0;

        private delegate void EnableControlsHandler(bool value);
        private delegate void UpdateTextHandler(string withValue);
        private delegate void UpdateLabelHandler(string newText);

        public TrainingProgress(IMaze maze)
        {
            InitializeComponent();
            _maze = maze;
            _maze.TrainingEpisodeCompletedEventHandler += _maze_TrainingEpisodeCompletedEventHandler;
            _totalMoves = 0;
            _totalScore = 0;
            _averageMoves = 0;
            _averageScore = 0;
        }

        private void _maze_TrainingEpisodeCompletedEventHandler(object sender, TrainingEpisodeCompletedEventArgs e)
        {
            //UpdateText(message);
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
                    $"Avg Moves: {_averageMoves.ToString("#,##0")} | " +
                    $"Avg Score: {_averageScore.ToString("#,##0")} | " +
                    $"{_percentComplete.ToString("0%")} Complete";

                UpdateLabel(message);
                UpdateProgressBar();
            }

            if (_runEpochs % _showEvery == 0 && 
                _runEpochs != _maze.MaxEpisodes)
            {
                //RenderTraining();
            }
        }


        private void RenderTraining()
        {
            _mazeSpace = new MazeSpace();
            _mazeSpace.CreateMazeControls(_maze);
            Form frm = new Form();
            frm.Size = _mazeSpace.Size = new Size(_mazeSpace.Width + 10, _mazeSpace.Height + 10);
            frm.Controls.Add(_mazeSpace);
            _mazeSpace.Dock = DockStyle.Fill;
            frm.Controls.Add(_mazeSpace);
            frm.Show();
            frm.WindowState = FormWindowState.Maximized;
            frm.FormBorderStyle = FormBorderStyle.None;
            _maze.AgentStateChangedEventHandler += _maze_AgentStateChangedEventHandler;

            try
            {                
                _maze.RunMaze();
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
            }
            finally
            {
                frm.Dispose();
                _mazeSpace.Dispose();
                _maze.AgentStateChangedEventHandler -= _maze_AgentStateChangedEventHandler;
            }
        }

        private void _maze_AgentStateChangedEventHandler(object sender, AgentStateChangedEventArgs e)
        {
            var newSpace = _mazeSpace.GetSpaceByPosition(e.NewPosition);

            if (newSpace != null)
            {
                if (MazeSpace.ActiveSpace != null)
                {
                    MazeSpace.ActiveSpace.SetInactive();
                    MazeSpace.ActiveSpace.Invalidate();
                }

                MazeSpace.ActiveSpace = newSpace;
                newSpace.SetActive();
                newSpace.Invalidate();
                _mazeSpace.Invalidate();
                newSpace.Refresh();
                System.Threading.Thread.Sleep(_movementPause);
                Application.DoEvents();
            }
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            _maze.TrainingEpisodeCompletedEventHandler -= _maze_TrainingEpisodeCompletedEventHandler;
            this.Close();
        }

        private Task TrainingTask()
        {
            EnableControls(false);
            this.Refresh();
            _maze.Train();
            EnableControls(true);
            this.DialogResult = DialogResult.OK;
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

            trainingProgressBar.Maximum = _maze.MaxEpisodes;
            trainingProgressBar.Value = 0;
            trainingProgressBar.Step = 1;
            TrainingTask();
        }

    }
}
