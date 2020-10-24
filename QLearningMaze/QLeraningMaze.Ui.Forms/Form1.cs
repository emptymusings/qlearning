using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;

namespace QLearningMaze.Ui.Forms
{
    using Core;
    using Microsoft.VisualBasic;
    using QLearningMaze.Core.Mazes;

    public partial class Form1 : Form
    {
        private IMaze _maze = MazeFactory.CreateMaze(MazeTypes.UserDefined);
        private int _movementPause = 150;
        private bool _overrideRespawn = false;
        private bool _needsRetrain = false;

        public Form1()
        {
            InitializeComponent();
            saveMenuItem.Click += SaveMenuItem_Click;
            openMenuItem.Click += OpenMenuItem_Click;
            exitMenuItem.Click += ExitMenuItem_Click;
            _maze.AgentStateChangedEventHandler += Maze_AgentStateChangedEventHandler;

        }

        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void OpenMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Maze Files (*.maze)|*.maze";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                _maze = MazeUtilities.LoadMaze(dlg.FileName);
                _maze.AgentStateChangedEventHandler += Maze_AgentStateChangedEventHandler;
                SetFormValuesFromMaze();
            }
        }

        private void SaveMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();

            dlg.Filter = "Maze Files (*.maze)|*.maze";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                MazeUtilities.SaveMaze(dlg.FileName, _maze);
            }
        }

        

        private void Form1_Shown(object sender, EventArgs e)
        {
            
        }

        private void RespawnMaze()
        {
            var newSpace = new MazeSpace();
            SetMazeValuesFromForm();
            mazeSpace.CreateMazeControls(_maze);

            foreach(var row in mazeSpace.Rows)
            {
                foreach(var space in row.Spaces)
                {
                    space.topWall.MouseUp += Wall_MouseUp;
                    space.bottomWall.MouseUp += Wall_MouseUp;
                    space.leftWall.MouseUp += Wall_MouseUp;
                    space.rightWall.MouseUp += Wall_MouseUp;

                    space.WallMouseUpEvent += Space_WallMouseUpEvent;
                    
                }
            }
        }

        private void Space_WallMouseUpEvent(object sender, WallMouseUpEventArgs e)
        {
            if (!e.WallPictureBox.Visible)
                AddWallFromClick((ObservationSpace)sender, e.WallPictureBox);
        }

        private void Maze_AgentStateChangedEventHandler(object sender, int e)
        {
            var newSpace = GetSpaceByPosition(e);

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
                mazeSpace.Invalidate();
                newSpace.Refresh();
                System.Threading.Thread.Sleep(_movementPause);
            }
        }

        private void runMaze_Click(object sender, EventArgs e)
        {
            if (MazeSpace.ActiveSpace != null)
            {
                MazeSpace.ActiveSpace.SetInactive();                
            }

            Cursor = Cursors.WaitCursor;
            this.Enabled = false;
            if (_needsRetrain)
            {
                _maze.Train();
                _needsRetrain = false;
            }

            var startSpace = GetSpaceByPosition(_maze.StartPosition);
            startSpace.SetActive();
            MazeSpace.ActiveSpace = startSpace;
            startSpace.Refresh();

            System.Threading.Thread.Sleep(_movementPause);

            _maze.RunMaze();
            this.Enabled = true;
            Cursor = Cursors.Default;
        }

        private ObservationSpace GetSpaceByPosition(int position)
        {
            var row = mazeSpace.Rows.Where(row => row.Spaces.Any(s => s.Position == position)).FirstOrDefault();
            var newSpace = row.Spaces.Where(s => s.Position == position).FirstOrDefault();

            return newSpace;
        }

        private void SetFormValuesFromMaze()
        {
            _overrideRespawn = true;

            rowsText.Text = _maze.Rows.ToString();
            columnsText.Text = _maze.Columns.ToString();
            startPositionText.Text = _maze.StartPosition.ToString();
            goalPositionText.Text = _maze.GoalPosition.ToString();
            discountRateText.Text = _maze.DiscountRate.ToString("0.##");
            learningRateText.Text = _maze.LearningRate.ToString("0.##");
            trainingEpochsText.Text = _maze.MaxEpochs.ToString();

            foreach (var obstruction in _maze.Obstructions)
            {
                var lvi = new ListViewItem(obstruction.BetweenSpace.ToString());
                lvi.SubItems.Add(obstruction.AndSpace.ToString());
                obstructionsList.Items.Add(lvi);
            }

            _overrideRespawn = false;

            RespawnMaze();
        }

        private void SetMazeValuesFromForm()
        {
            try
            {
                _maze = MazeFactory.CreateMaze(MazeTypes.UserDefined);
                _maze.AgentStateChangedEventHandler += Maze_AgentStateChangedEventHandler;
                _maze.Rows = Convert.ToInt32(rowsText.Text);
                _maze.Columns = Convert.ToInt32(columnsText.Text);
                _maze.StartPosition = Convert.ToInt32(startPositionText.Text);
                _maze.GoalPosition = Convert.ToInt32(goalPositionText.Text);
                _maze.DiscountRate = Convert.ToDouble(discountRateText.Text);
                _maze.LearningRate = Convert.ToDouble(learningRateText.Text);
                _maze.MaxEpochs = Convert.ToInt32(trainingEpochsText.Text);
            }
            catch
            {
                return;
            }

            for (int i = _maze.Obstructions.Count - 1; i >= 0; i--)
            {
                var wall = _maze.Obstructions[i];
                _maze.RemoveWall(wall.BetweenSpace, wall.AndSpace);
            }

            foreach(ListViewItem lvi in obstructionsList.Items)
            {
                _maze.AddWall(Convert.ToInt32(lvi.Text), Convert.ToInt32(lvi.SubItems[1].Text));
            }
            
        }

        private void addObstructionButton_Click(object sender, EventArgs e)
        {
            int between;
            int and;

            if (string.IsNullOrWhiteSpace(betweenText.Text) ||
                !int.TryParse(betweenText.Text, out between))
            {
                MessageBox.Show("Invalid 'Between' value");
                return;
            }

            if (string.IsNullOrWhiteSpace(andText.Text) ||
                !int.TryParse(andText.Text, out and))
            {
                MessageBox.Show("Invalid 'and' value");
                return;
            }

            AddObstruction(between, and);
        }

        private void AddObstruction(int between, int and)
        {
            var lvi = new ListViewItem(between.ToString());
            lvi.SubItems.Add(and.ToString());

            obstructionsList.Items.Add(lvi);
            _maze.AddWall(between, and);
            betweenText.Text = null;
            andText.Text = null;

            RespawnMaze();
            _needsRetrain = true;
        }

        private void removeObstructionButton_Click(object sender, EventArgs e)
        {
            if (obstructionsList.SelectedItems.Count == 0)
                return;

            int between = Convert.ToInt32(obstructionsList.SelectedItems[0].Text);
            int and = Convert.ToInt32(obstructionsList.SelectedItems[0].SubItems[0].Text);

            obstructionsList.Items.Remove(obstructionsList.SelectedItems[0]);
            RemoveObstruction(between, and);
        }

        private void RemoveObstruction(int between, int and)
        {
            _maze.RemoveWall(between, and);

            RespawnMaze();
            _needsRetrain = true;
        }

        private void clearObstructionsButton_Click(object sender, EventArgs e)
        {
            obstructionsList.Items.Clear();
            _maze.Obstructions.Clear();
            RespawnMaze();
            _needsRetrain = true;
        }

        private void respawnButton_Click(object sender, EventArgs e)
        {
            RespawnMaze();
        }

        private void trainMazeButton_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(trainingEpochsText.Text) != _maze.MaxEpochs)
            {
                _maze.MaxEpochs = Convert.ToInt32(trainingEpochsText.Text);
            }

            this.Cursor = Cursors.WaitCursor;
            this.Enabled = false;
            _maze.Train();
            _needsRetrain = false;
            this.Enabled = true;
            this.Cursor = Cursors.Default;
        }

        private void MazeTextChanged(TextBox textbox)
        {
            double value;

            if (_overrideRespawn) return;

            if (string.IsNullOrWhiteSpace(textbox.Text) ||
                !double.TryParse(textbox.Text, out value))
            {
                MessageBox.Show("Invalid entry");
                textbox.Focus();
                textbox.SelectAll();
            }

            RespawnMaze();
            _needsRetrain = true;
        }

        private void rowsText_Leave(object sender, EventArgs e)
        {
            MazeTextChanged(rowsText);
        }

        private void columnsText_Leave(object sender, EventArgs e)
        {
            MazeTextChanged(columnsText);
        }

        private void startPositionText_Leave(object sender, EventArgs e)
        {
            MazeTextChanged(startPositionText);
        }

        private void goalPositionText_Leave(object sender, EventArgs e)
        {
            MazeTextChanged(goalPositionText);
        }

        private void discountRateText_Leave(object sender, EventArgs e)
        {
            //MazeTextChanged(discountRateText);
            _needsRetrain = true;
        }

        private void learningRateText_Leave(object sender, EventArgs e)
        {
            //MazeTextChanged(learningRateText);
            _needsRetrain = true;
        }

        private void trainingEpochsText_Leave(object sender, EventArgs e)
        {
            //MazeTextChanged(trainingEpochsText);
            _needsRetrain = true;
        }

        private void Wall_MouseUp(object sender, MouseEventArgs e)
        {
            var wallBox = sender as PictureBox;

            if (wallBox == null)
            {
                return;
            }

            if (wallBox.Visible)
            {
                RemoveWallFromClick(wallBox);
            }
        }

        private void AddWallFromClick(ObservationSpace space, PictureBox wallBox)
        {            
            ObservationSpaceRow row = space.Parent as ObservationSpaceRow;

            switch (wallBox.Name)
            {
                case "topWall":
                    if (row.RowNumber == 0) return;
                    AddObstruction(space.Position, space.Position - _maze.Columns);                    
                    break;
                case "bottomWall":
                    if (row.RowNumber == _maze.Rows - 1) return;
                    AddObstruction(space.Position, space.Position + _maze.Columns);
                    break;
                case "leftWall":
                    var prev = row.Spaces.Where(x => x.Position == space.Position - 1);
                    if (prev == null) return;
                    AddObstruction(space.Position, space.Position - 1);
                    break;
                case "rightWall":
                    var next = row.Spaces.Where(x => x.Position == space.Position + 1);
                    if (next == null) return;
                    AddObstruction(space.Position, space.Position + 1);
                    break;
                default:
                    return;
            }

            _needsRetrain = true;
        }

        private void RemoveWallFromClick(PictureBox wallBox)
        {
            int between, and;
            ObservationSpace space = wallBox.Parent as ObservationSpace;

            if (space == null) return;

            ObservationSpaceRow row = space.Parent as ObservationSpaceRow;

            if (row == null || row.RowNumber == _maze.Rows - 1) return;

            between = space.Position;

            switch (wallBox.Name)
            {
                case "topWall":
                    and = between - _maze.Columns;
                    break;
                case "bottomWall":
                    and = between + _maze.Columns;
                    break;
                case "leftWall":
                    and = between - 1;
                    break;
                case "rightWall":
                    and = between + 1;
                    break;
                default:
                    return;
            }

            RemoveObstructionsFromList(between, and);
            RemoveObstruction(between, and);
        }

        private void RemoveObstructionsFromList(int between, int and)
        {
            foreach(ListViewItem item in obstructionsList.Items)
            {
                if ((item.Text == between.ToString() && item.SubItems[1].Text == and.ToString()) ||
                    (item.Text == and.ToString() && item.SubItems[1].Text == between.ToString()))
                {
                    obstructionsList.Items.Remove(item);
                }
            }
        }
    }
}
