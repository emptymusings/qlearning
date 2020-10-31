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
        private int _showEvery = 2500;
        private MazeSpace _mazeSpace;
        private int _movementPause = 250;
        private int _runEpochs = 0;

        private delegate void EnableControlsHandler(bool value);
        private delegate void UpdateTextHandler(string withValue);

        public TrainingProgress(IMaze maze)
        {
            InitializeComponent();
            _maze = maze;
            _maze.TrainingEpisodeCompletedEventHandler += _maze_TrainingEpisodeCompletedEventHandler;
        }

        private void _maze_TrainingEpisodeCompletedEventHandler(object sender, TrainingEpisodeCompletedEventArgs e)
        {
            if (!e.Success) return;
            string message = $"Completed {e.CurrentEpisode.ToString("#,##0")} of {e.TotalEpisodes.ToString("#,##0")} episodes in {e.TotalMoves.ToString("#,##0")} moves. Agent {(e.Success ? "Succeeded" : "Failed")}";

            _runEpochs++;

            if (_runEpochs % _showEvery == 0)
            {
                RenderTraining();
            }

            UpdateText(message);
        }

        private void RenderTraining()
        {
            _mazeSpace = new MazeSpace();
            _mazeSpace.CreateMazeControls(_maze);
            _maze.AgentStateChangedEventHandler += _maze_AgentStateChangedEventHandler;
            Form frm = new Form();
            frm.Size = _mazeSpace.Size = new Size(_mazeSpace.Width + 10, _mazeSpace.Height + 10);
            frm.Controls.Add(_mazeSpace);
            _mazeSpace.Dock = DockStyle.Fill;
            frm.Controls.Add(_mazeSpace);
            frm.Show();
            frm.WindowState = FormWindowState.Maximized;
            frm.FormBorderStyle = FormBorderStyle.None;

            _maze.RunMaze();

            frm.Close();
            _maze.AgentStateChangedEventHandler -= _maze_AgentStateChangedEventHandler;
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
            }
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private Task TrainingTask()
        {
            EnableControls(false);
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

        private void TrainingProgress_Shown(object sender, EventArgs e)
        {
            TrainingTask();
        }
    }
}
