using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace QLearningMaze.Ui.Forms
{
    using Core;

    public partial class Form1 : Form
    {
        private IMaze _maze = MazeFactory.CreateMaze(MazeTypes.UserDefined);
        private int _movementPause = 150;
        private bool _overrideRespawn = false;

        public Form1()
        {
            InitializeComponent();
            saveMenuItem.Click += SaveMenuItem_Click;
            openMenuItem.Click += OpenMenuItem_Click;
            exitMenuItem.Click += ExitMenuItem_Click;
            _maze.AgentStateChangedEventHandler += _maze_AgentStateChangedEventHandler;
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
                _maze.AgentStateChangedEventHandler += _maze_AgentStateChangedEventHandler;
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
            mazeSpace.Controls.Clear();
            mazeSpace.Rows.Clear();
            SetMazeValuesFromForm();
            mazeSpace.CreateMazeControls(_maze);
            mazeSpace.Refresh();
        }

        private void _maze_AgentStateChangedEventHandler(object sender, int e)
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

            var startSpace = GetSpaceByPosition(_maze.StartPosition);
            startSpace.SetActive();
            MazeSpace.ActiveSpace = startSpace;
            startSpace.Refresh();
            System.Threading.Thread.Sleep(_movementPause);

            _maze.RunMaze();
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
            _maze = MazeFactory.CreateMaze(MazeTypes.UserDefined);
            _maze.AgentStateChangedEventHandler += _maze_AgentStateChangedEventHandler;
            _maze.Rows = Convert.ToInt32(rowsText.Text);
            _maze.Columns = Convert.ToInt32(columnsText.Text);
            _maze.StartPosition = Convert.ToInt32(startPositionText.Text);
            _maze.GoalPosition = Convert.ToInt32(goalPositionText.Text);
            _maze.DiscountRate = Convert.ToDouble(discountRateText.Text);
            _maze.LearningRate = Convert.ToDouble(learningRateText.Text);
            _maze.MaxEpochs = Convert.ToInt32(trainingEpochsText.Text);

            _maze.Train();

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

            var lvi = new ListViewItem(betweenText.Text);
            lvi.SubItems.Add(andText.Text);

            obstructionsList.Items.Add(lvi);
            _maze.AddWall(between, and);
            betweenText.Text = null;
            andText.Text = null;

            RespawnMaze();
        }

        private void removeObstructionButton_Click(object sender, EventArgs e)
        {
            if (obstructionsList.SelectedItems.Count == 0)
                return;

            int between = Convert.ToInt32(obstructionsList.SelectedItems[0].Text);
            int and = Convert.ToInt32(obstructionsList.SelectedItems[0].SubItems[0].Text);

            obstructionsList.Items.Remove(obstructionsList.SelectedItems[0]);
            _maze.RemoveWall(between, and);

            RespawnMaze();
        }

        private void respawnButton_Click(object sender, EventArgs e)
        {
            RespawnMaze();
        }

        private void trainMazeButton_Click(object sender, EventArgs e)
        {
            _maze.Train();
            MessageBox.Show("Training Complete");
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
        }

        private void rowsText_TextChanged(object sender, EventArgs e)
        {
            MazeTextChanged(rowsText);
        }

        private void columnsText_TextChanged(object sender, EventArgs e)
        {
            MazeTextChanged(columnsText);
        }

        private void startPositionText_TextChanged(object sender, EventArgs e)
        {
            MazeTextChanged(startPositionText);
        }

        private void goalPositionText_TextChanged(object sender, EventArgs e)
        {
            MazeTextChanged(goalPositionText);
        }

        private void discountRateText_TextChanged(object sender, EventArgs e)
        {
            MazeTextChanged(discountRateText);
        }

        private void learningRateText_TextChanged(object sender, EventArgs e)
        {
            MazeTextChanged(learningRateText);
        }

        private void trainingEpochsText_TextChanged(object sender, EventArgs e)
        {
            MazeTextChanged(trainingEpochsText);
        }
    }
}
