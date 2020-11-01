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

        private delegate void EnableControlsHandler(bool value);
        private delegate void UpdateTextHandler(string withValue);
        private delegate void UpdateLabelHandler(string newText);

        public TrainingProgress(IMaze maze)
        {
            InitializeComponent();
            _maze = maze;
            _maze.TrainingEpisodeCompletedEventHandler += _maze_TrainingEpisodeCompletedEventHandler;
        }

        private void _maze_TrainingEpisodeCompletedEventHandler(object sender, TrainingEpisodeCompletedEventArgs e)
        {
            //UpdateText(message);
            if (e.Success)
                _successfulRuns++;

            _totalMoves += e.TotalMoves;
            _totalScore += e.TotalScore;

            if (e.CurrentEpisode % trainingProgressBar.Step == 0)
            {
                _percentComplete = (double)e.CurrentEpisode / (double)e.TotalEpisodes;
                _averageMoves = _totalMoves / e.CurrentEpisode;
                _averageScore = _totalScore / e.CurrentEpisode;

                string message = $"Successful Runs: {_successfulRuns}/{e.TotalEpisodes} | " +
                    $"{_percentComplete.ToString("0%")} Complete | " +
                    $"Avg Moves: {_averageMoves.ToString("#,##0")} | " +
                    $"Avg Score: {_averageScore.ToString("#,##0")}";

                UpdateLabel(message);
                UpdateProgressBar();
            }
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private Task TrainingTask()
        {
            EnableControls(false);
            this.Refresh();
            _maze.Train();
            EnableControls(true);
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
