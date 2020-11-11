﻿using QLearningMaze.Core;
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

namespace QLearningMaze.Ui.Forms
{
    public partial class TrainingSessionSelector : Form
    {
        private IMaze _maze;
        private bool _isChecking;
        private int _moves;
        private double _score;

        private IEnumerable<TrainingSession> trainingSessions;
        public TrainingSession SelectedSession { get; set; }

        public TrainingSessionSelector(IMaze maze)
        {
            InitializeComponent();
            _maze = maze;
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
                foreach (var session in trainingSessions)
                {
                    ListViewItem lvi = new ListViewItem(session.Episode.ToString());
                    lvi.SubItems.Add(session.Moves.ToString("#,##0"));
                    lvi.SubItems.Add(session.Score.ToString("#,##0.##"));
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
            var sessions = TrainingSession.GetTrainingSessions().ToList();
            var maze = GetTestMaze();

            for (int i = sessions.Count - 1; i >= 0; --i)
            {
                var session = sessions[i];
                _moves = 0;
                _score = 0;

                maze.Quality = session.Quality;

                try
                {
                    maze.RunMaze();
                }
                catch
                {
                    //sessions.RemoveAt(i);
                    continue;
                }

                session.Moves = _moves;
                session.Score = _score;                
            }

            trainingSessions = sessions.OrderByDescending(s => s.Score).ThenBy(m => m.Moves).ThenBy(e => e.Episode);
            SelectedSession = trainingSessions.FirstOrDefault();

            _isChecking = false;
            return Task.CompletedTask;
        }

        private UserDefinedMaze GetTestMaze()
        {
            var maze = new UserDefinedMaze();
            maze.Rows = _maze.Rows;
            maze.Columns = _maze.Columns;
            maze.Rewards = _maze.Rewards;
            maze.ObservationSpace = _maze.ObservationSpace;
            maze.AdditionalRewards = _maze.AdditionalRewards;
            maze.StartPosition = _maze.StartPosition;
            maze.GoalPosition = _maze.GoalPosition;

            maze.AgentCompletedMazeEventHandler += Maze_AgentCompletedMazeEventHandler;

            return maze;
        }

        private void Maze_AgentCompletedMazeEventHandler(object sender, AgentCompletedMazeEventArgs e)
        {
            _moves += e.Moves;
            _score += e.Rewards;
        }

        private void TrainingSessionSelector_Shown(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            Task.Run(StartTestManager);
        }

        private void sesssionList_DoubleClick(object sender, EventArgs e)
        {
            if (sesssionList.SelectedItems.Count > 0)
            {
                useSessionButton_Click(sender, e);
            }
        }
    }
}