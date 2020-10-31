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

            UpdateText(message);
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
