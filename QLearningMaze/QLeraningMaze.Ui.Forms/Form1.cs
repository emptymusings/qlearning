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
        private int _movementPause = 250;
        private bool _overrideRespawn = false;
        private bool _needsRetrain = false;
        private List<AdditionalReward> _additionalRewards = new List<AdditionalReward>();

        public Form1()
        {
            InitializeComponent();
            saveMenuItem.Click += SaveMenuItem_Click;
            openMenuItem.Click += OpenMenuItem_Click;
            exitMenuItem.Click += ExitMenuItem_Click;
            qualityMenuItem.Click += QualityMenuItem_Click;
            rewardsMenuItem.Click += RewardsMenuItem_Click;
            _maze.AgentStateChangedEventHandler += Maze_AgentStateChangedEventHandler;
            _maze.TrainingAgentStateChangingEventHandler += Maze_TrainingAgentStateChangingEventHandler;

        }

        private void Maze_TrainingAgentStateChangingEventHandler(object sender, (int action, int state, int position, double newQuality, double oldQuality) e)
        {
            int newState = e.state;
            double newQuality = e.newQuality;
            double oldQuality = e.oldQuality;

        }

        private void RewardsMenuItem_Click(object sender, EventArgs e)
        {
            var view = new TabledDetailsView(_maze, TabledDetailsView.ValueTypes.Rewards);
            view.Show();
        }

        private void QualityMenuItem_Click(object sender, EventArgs e)
        {
            if (_maze.Quality == null) trainMazeButton_Click(sender, e);

            var view = new TabledDetailsView(_maze, TabledDetailsView.ValueTypes.Quality);
            view.Show();
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
                mazeSpace.Enabled = false;
                _maze = MazeUtilities.LoadMaze(dlg.FileName);
                _maze.AgentStateChangedEventHandler += Maze_AgentStateChangedEventHandler;
                _maze.TrainingAgentStateChangingEventHandler += Maze_TrainingAgentStateChangingEventHandler;
                SetFormValuesFromMaze();
                _needsRetrain = true;
                mazeSpace.Enabled = true;
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
                    space.WallMouseUpEvent += Space_WallMouseUpEvent;
                    
                }
            }
        }

        private void Space_WallMouseUpEvent(object sender, WallMouseUpEventArgs e)
        {
            if (!e.WallPictureBox.Visible)
                AddWallFromClick((ObservationSpace)sender, e.WallPictureBox, e.RowNumber);
            else
                RemoveWallFromClick((ObservationSpace)sender, e.WallPictureBox, e.RowNumber);
        }

        private void Maze_AgentStateChangedEventHandler(object sender, AgentStateChangedEventArgs e)
        {
            var newSpace = GetSpaceByPosition(e.NewPosition);

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
                rewardsLabel.Text = $"Moves: {e.MovesMade} Reward: {e.RewardsEarned}";
                newSpace.rewardLabel.Visible = false;
                Application.DoEvents();
            }
        }

        private void runMaze_Click(object sender, EventArgs e)
        {
            if (MazeSpace.ActiveSpace != null)
            {
                MazeSpace.ActiveSpace.SetInactive();                
            }

            Cursor = Cursors.WaitCursor;
            entryPanel.Enabled = false;
            if (_needsRetrain)
            {
                _maze.Train();
                _needsRetrain = false;
            }

            foreach(var reward in _additionalRewards)
            {
                var space = GetSpaceByPosition(reward.Position);
                space.rewardLabel.Text = $"Reward: {reward.Value}";
                space.rewardLabel.Visible = true;
            }

            var startSpace = GetSpaceByPosition(_maze.StartPosition);
            startSpace.SetActive();
            MazeSpace.ActiveSpace = startSpace;
            startSpace.Refresh();

            System.Threading.Thread.Sleep(_movementPause);

            try
            {
                _maze.RunMaze();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            entryPanel.Enabled = true;
            Cursor = Cursors.Default;

        }

        private ObservationSpace GetSpaceByPosition(int position)
        {
            //var row = mazeSpace.Rows.Where(row => row.Spaces.Any(s => s.Position == position)).FirstOrDefault();
            //var newSpace = row.Spaces.Where(s => s.Position == position).FirstOrDefault();

            //return newSpace;
            return mazeSpace.GetSpaceByPosition(position);
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
            trainingEpisodesText.Text = _maze.MaxEpisodes.ToString();

            foreach (var obstruction in _maze.Obstructions)
            {
                var lvi = new ListViewItem(obstruction.BetweenSpace.ToString());
                lvi.SubItems.Add(obstruction.AndSpace.ToString());
                obstructionsList.Items.Add(lvi);
            }

            _additionalRewards = _maze.AdditionalRewards;

            _overrideRespawn = false;

            RespawnMaze();
        }

        private void SetMazeValuesFromForm()
        {
            try
            {  
                if (_additionalRewards == null ||
                    _additionalRewards.Count == 0)
                {
                    _additionalRewards = new List<AdditionalReward>();
                }

                foreach(var reward in _additionalRewards)
                {
                    _maze.AddCustomReward(reward.Position, reward.Value);
                }


                //_maze.AddCustomReward(9, 30);
                _additionalRewards = _maze.GetAdditionalRewards().ToList();
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

            foreach (ListViewItem lvi in obstructionsList.Items)
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

            //RespawnMaze();
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
            
            for (int i = obstructionsList.Items.Count - 1; i >= 0; i--)
            {
                if (
                    (obstructionsList.Items[i].Text == between.ToString() && obstructionsList.Items[i].SubItems[1].Text == and.ToString()) ||
                    (obstructionsList.Items[i].Text == and.ToString() && obstructionsList.Items[i].SubItems[1].Text == between.ToString())
                    )
                {
                    obstructionsList.Items.Remove(obstructionsList.Items[i]);
                }
            }

            //RespawnMaze();
            _needsRetrain = true;
        }

        private void clearObstructionsButton_Click(object sender, EventArgs e)
        {
            obstructionsList.Items.Clear();
            
            for (int i = _maze.Obstructions.Count - 1; i >= 0; --i)
            {
                var wall = _maze.Obstructions[i];
                _maze.RemoveWall(wall.BetweenSpace, wall.AndSpace);
            }

            RespawnMaze();
            _needsRetrain = true;
        }

        private void respawnButton_Click(object sender, EventArgs e)
        {
            RespawnMaze();
        }

        private void trainMazeButton_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(trainingEpisodesText.Text) != _maze.MaxEpisodes)
            {
                _maze.MaxEpisodes = Convert.ToInt32(trainingEpisodesText.Text);
            }

            this.Cursor = Cursors.WaitCursor;
            this.Enabled = false;

            var dlg = new TrainingProgress(_maze);
            dlg.ShowDialog();
            dlg.Dispose();
            _needsRetrain = false;
            this.Enabled = true;
            this.Cursor = Cursors.Default;
        }

        private bool MazeTextChanged(TextBox textbox, bool setRetrain = true)
        {
            double value;

            if (string.IsNullOrWhiteSpace(textbox.Text) ||
                !double.TryParse(textbox.Text, out value))
            {
                MessageBox.Show("Invalid entry");
                textbox.Focus();
                textbox.SelectAll();
                return false;
            }

            if (setRetrain)
                _needsRetrain = true;

            return true;
        }

        private void rowsText_Leave(object sender, EventArgs e)
        {
            if (_maze.Rows.ToString() != rowsText.Text &&
                MazeTextChanged(rowsText))
            {
                clearObstructionsButton_Click(sender, e);
                _maze.Rows = Convert.ToInt32(rowsText.Text);
                _needsRetrain = true;
                RespawnMaze();
            }
            
        }

        private void columnsText_Leave(object sender, EventArgs e)
        {
            if (_maze.Columns.ToString() != columnsText.Text &&
                MazeTextChanged(columnsText))
            {
                clearObstructionsButton_Click(sender, e);
                _maze.Columns = Convert.ToInt32(columnsText.Text);
                _needsRetrain = true;
                RespawnMaze();
            }
        }

        private void startPositionText_Leave(object sender, EventArgs e)
        {
            int newStartPosition;
            int oldStartPosition;

            if (_maze.StartPosition.ToString() != startPositionText.Text &&
                MazeTextChanged(startPositionText, false))
            {
                oldStartPosition = _maze.StartPosition;
                newStartPosition = Convert.ToInt32(startPositionText.Text);
                _maze.StartPosition = newStartPosition;
                GetSpaceByPosition(oldStartPosition).SetStart(false);
                GetSpaceByPosition(newStartPosition).SetStart(true);
                //RespawnMaze();
            }
        }

        private void goalPositionText_Leave(object sender, EventArgs e)
        {
            int newGoalPosition;
            int oldGoalPosition;

            if (_maze.GoalPosition.ToString() != goalPositionText.Text &&
                MazeTextChanged(goalPositionText))
            {
                oldGoalPosition = _maze.GoalPosition;
                newGoalPosition = Convert.ToInt32(goalPositionText.Text);
                _maze.GoalPosition = newGoalPosition;
                GetSpaceByPosition(oldGoalPosition).SetGoal(false);
                GetSpaceByPosition(newGoalPosition).SetGoal(true);
                //RespawnMaze();
            }            
        }

        private void discountRateText_Leave(object sender, EventArgs e)
        {
            _overrideRespawn = true;

            if (_maze.DiscountRate.ToString() != discountRateText.Text &&
                MazeTextChanged(discountRateText))
            {
                _maze.DiscountRate = Convert.ToDouble(discountRateText.Text);
                _needsRetrain = true;
            }

            _overrideRespawn = false;
        }

        private void learningRateText_Leave(object sender, EventArgs e)
        {
            _overrideRespawn = true;

            if (_maze.LearningRate.ToString() != learningRateText.Text &&
                MazeTextChanged(learningRateText))
            {
                _maze.LearningRate = Convert.ToDouble(learningRateText.Text);
                _needsRetrain = true;
            }

            _overrideRespawn = false;
        }

        private void trainingEpisodesText_Leave(object sender, EventArgs e)
        {
            _overrideRespawn = true;

            if (_maze.MaxEpisodes.ToString() != trainingEpisodesText.Text &&
                MazeTextChanged(trainingEpisodesText))
            {
                _maze.MaxEpisodes = Convert.ToInt32(trainingEpisodesText.Text);
                _needsRetrain = true;
            }

            _overrideRespawn = false;
        }

        private void AddWallFromClick(ObservationSpace space, PictureBox wallBox, int rowNumber)
        {
            int andSpacePosition;
            ObservationSpace andSpace;

            switch (wallBox.Name)
            {
                case "topWall":
                    if (rowNumber == 0) return;
                    andSpacePosition = space.Position - _maze.Columns;
                    andSpace = GetSpaceByPosition(andSpacePosition);
                    space.topWall.Visible = true;
                    andSpace.bottomWall.Visible = true;
                    break;
                case "bottomWall":
                    if (rowNumber == _maze.Rows - 1) return;
                    andSpacePosition = space.Position + _maze.Columns;
                    andSpace = GetSpaceByPosition(andSpacePosition);
                    space.bottomWall.Visible = true;
                    andSpace.topWall.Visible = true;
                    break;
                case "leftWall":
                    var prev = ((ObservationSpaceRow)space.Parent).Spaces.Where(x => x.Position == space.Position - 1);
                    if (prev == null) return;
                    andSpacePosition = space.Position - 1;
                    andSpace = GetSpaceByPosition(andSpacePosition);
                    space.leftWall.Visible = true;
                    andSpace.rightWall.Visible = true;
                    break;
                case "rightWall":
                    var next = ((ObservationSpaceRow)space.Parent).Spaces.Where(x => x.Position == space.Position + 1);
                    if (next == null) return;
                    andSpacePosition = space.Position + 1;
                    andSpace = GetSpaceByPosition(andSpacePosition);
                    space.rightWall.Visible = true;
                    andSpace.leftWall.Visible = true;
                    break;
                default:
                    return;
            }


            AddObstruction(space.Position, andSpacePosition);
            _needsRetrain = true;
        }

        private void RemoveWallFromClick(ObservationSpace space, PictureBox wallBox, int rowNumber)
        {
            int between, and;
            ObservationSpace andSpace;
            //if (rowNumber == _maze.Rows - 1) return;

            between = space.Position;

            switch (wallBox.Name)
            {
                case "topWall":
                    and = between - _maze.Columns;
                    andSpace = GetSpaceByPosition(and);
                    space.topWall.Visible = false;
                    andSpace.bottomWall.Visible = false;
                    break;
                case "bottomWall":
                    and = between + _maze.Columns;
                    andSpace = GetSpaceByPosition(and);
                    space.bottomWall.Visible = false;
                    andSpace.topWall.Visible = false;
                    break;
                case "leftWall":
                    and = between - 1;
                    andSpace = GetSpaceByPosition(and);
                    space.leftWall.Visible = false;
                    andSpace.rightWall.Visible = false;
                    break;
                case "rightWall":
                    and = between + 1;
                    andSpace = GetSpaceByPosition(and);
                    space.rightWall.Visible = false;
                    andSpace.leftWall.Visible = false;
                    break;
                default:
                    return;
            }

            RemoveObstructionsFromList(between, and);
            RemoveObstruction(between, and);
        }

        private void RemoveObstructionsFromList(int between, int and)
        {
            foreach (ListViewItem item in obstructionsList.Items)
            {
                if ((item.Text == between.ToString() && item.SubItems[1].Text == and.ToString()) ||
                    (item.Text == and.ToString() && item.SubItems[1].Text == between.ToString()))
                {
                    obstructionsList.Items.Remove(item);
                }
            }
        }

        private void rewardsButton_Click(object sender, EventArgs e)
        {
            var dlg = new Rewards(_maze);
            dlg.ShowDialog();
            
            if (dlg.RewardsChanged)
            {
                _additionalRewards = _maze.AdditionalRewards;
                RespawnMaze();
                _needsRetrain = true;
            }
        }
    }
}
