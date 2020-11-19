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
        //private IMaze _maze = MazeFactory.CreateMaze(MazeTypes.UserDefined);
        private IMazeNew _mazeNew = new MazeBaseNew(8, 8, 11, 31, 0.5, 0.5);
        private int _movementPause = 100;
        private bool _overrideRespawn = false;
        private bool _needsRetrain = false;
        private List<QLearning.Core.CustomObjective> _additionalRewards = new List<QLearning.Core.CustomObjective>();
        private TrainingSessionSelector _trainingSessionSelector = null;

        public Form1()
        {
            InitializeComponent();
            saveMenuItem.Click += SaveMenuItem_Click;
            openMenuItem.Click += OpenMenuItem_Click;
            exitMenuItem.Click += ExitMenuItem_Click;
            qualityMenuItem.Click += QualityMenuItem_Click;
            rewardsMenuItem.Click += RewardsMenuItem_Click;
            statesMenuItem.Click += StatesMenuItem_Click;
            trainStripMenuItem.Click += TrainStripMenuItem_Click;
            runMazeStripMenuItem.Click += RunMazeStripMenuItem_Click;
            qualityStripMenuItem.Click += QualityStripMenuItem_Click;
            //_maze.AgentStateChangedEventHandler += Maze_AgentStateChangedEventHandler;
            _mazeNew.AgentStateChanged += MazeNew_AgentStateChanged;
            //_maze.AgentCompletedMazeEventHandler += Maze_AgentCompletedMazeEventHandler;
            _mazeNew.AgentCompleted += MazeNew_AgentCompleted;
            //_maze.TrainingAgentStateChangingEventHandler += Maze_TrainingAgentStateChangingEventHandler;
            _mazeNew.TrainingAgentStateChanged += MazeNew_TrainingAgentStateChanged;
        }


        private void QualityStripMenuItem_Click(object sender, EventArgs e)
        {
            GetEpisodeSelection();
        }

        private void RunMazeStripMenuItem_Click(object sender, EventArgs e)
        {
            RunMaze();
        }

        private void TrainStripMenuItem_Click(object sender, EventArgs e)
        {
            Train();
        }

        private void Maze_TrainingAgentStateChangingEventHandler(object sender, TrainingAgentStateChangedEventArgs e)
        {
            int newState = e.State;
            double newQuality = e.NewQuality;
            double oldQuality = e.OldQuality;

        }
        private void MazeNew_TrainingAgentStateChanged(object sender, QLearning.Core.TrainingAgentStateChangedEventArgs e)
        {
            int newState = e.NewState;
            double newQuality = e.NewQuality;
            double oldQuality = e.OldQuality;
        }

        private void RewardsMenuItem_Click(object sender, EventArgs e)
        {
            var view = new TabledDetailsView(_mazeNew, TabledDetailsView.ValueTypes.Rewards);
            view.Show();
        }

        private void QualityMenuItem_Click(object sender, EventArgs e)
        {
            if (_mazeNew.QualityTable == null) trainMazeButton_Click(sender, e);

            var view = new TabledDetailsView(_mazeNew, TabledDetailsView.ValueTypes.Quality);
            view.Show();
        }

        private void StatesMenuItem_Click(object sender, EventArgs e)
        {
            if (_mazeNew.StatesTable == null) trainMazeButton_Click(sender, e);

            var view = new TabledDetailsView(_mazeNew, TabledDetailsView.ValueTypes.StateSpace);
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
                obstructionsList.Items.Clear();
                mazeSpace.Enabled = false;
                _mazeNew = MazeUtilities.LoadObject<MazeBaseNew>(dlg.FileName);

                _mazeNew.ObjectiveReward = 200;
                _mazeNew.AgentStateChanged += MazeNew_AgentStateChanged;
                _mazeNew.AgentCompleted += MazeNew_AgentCompleted;
                _mazeNew.TrainingAgentStateChanged += MazeNew_TrainingAgentStateChanged;
                //_maze = MazeUtilities.LoadObject<UserDefinedMaze>(dlg.FileName);
                //_maze.AgentStateChangedEventHandler += Maze_AgentStateChangedEventHandler;
                //_maze.AgentCompletedMazeEventHandler += Maze_AgentCompletedMazeEventHandler;
                //_maze.TrainingAgentStateChangingEventHandler += Maze_TrainingAgentStateChangingEventHandler;
                SetFormValuesFromMaze();
                
                //_mazeNew = new MazeBaseNew(_maze.Columns, _maze.Rows, _maze.StartPosition, _maze.GoalPosition, _maze.DiscountRate, _maze.LearningRate);
                //_mazeNew.QualitySaveDirectory = MazeUtilities.TRAINING_SESSIONS_DIRECTORY;

                //foreach (var obstruction in _maze.Obstructions)
                //{
                //    _mazeNew.AddObstruction(obstruction.BetweenSpace, obstruction.AndSpace);
                //}

                //foreach(var reward in _maze.AdditionalRewards)
                //{
                //    _mazeNew.AddReward(reward.Position, reward.Value);
                //}

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
                MazeUtilities.SaveObject(dlg.FileName, _mazeNew);
            }
        }

        

        private void Form1_Shown(object sender, EventArgs e)
        {
            
        }

        private void RespawnMaze()
        {
            var newSpace = new MazeSpace();

            SetMazeValuesFromForm();
            mazeSpace.CreateMazeControls(_mazeNew);

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

        private void MazeNew_AgentStateChanged(object sender, QLearning.Core.AgentStateChangedEventArgs e)
        {
            var newSpace = GetSpaceByPosition(e.NewState % _mazeNew.StatesPerPhase);

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

                if (newSpace.rewardLabel.Visible == true)
                {
                    if (_mazeNew.RewardsTable[e.NewState][(int)Actions.GetCustomReward] > -1)
                    {
                        newSpace.rewardLabel.Visible = false;
                    }
                }

                Application.DoEvents();
            }
        }

        private void Maze_AgentCompletedMazeEventHandler(object sender, AgentCompletedMazeEventArgs e)
        {
            
        }

        private void MazeNew_AgentCompleted(object sender, QLearning.Core.AgentCompletedEventArgs e)
        {
            rewardsLabel.Text = $"Moves: {e.Moves} Reward: {e.Rewards}";
        }

        private void runMaze_Click(object sender, EventArgs e)
        {
            RunMaze();
        }

        private void RunMaze()
        {
            if (MazeSpace.ActiveSpace != null)
            {
                MazeSpace.ActiveSpace.SetInactive();
            }

            Cursor = Cursors.WaitCursor;
            entryPanel.Enabled = false;
            if (_needsRetrain)
            {
                //_maze.Train();
                _mazeNew.Train();
                _needsRetrain = false;
            }

            foreach (var reward in _additionalRewards)
            {
                var space = GetSpaceByPosition(reward.State);
                
                if (reward.Value < 0)
                    space.rewardLabel.Text = $"Punishment: {reward.Value}";
                else
                    space.rewardLabel.Text = $"Reward: {reward.Value}";

                space.rewardLabel.Visible = true;
            }

            var startSpace = GetSpaceByPosition(_mazeNew.StartPosition);
            startSpace.SetActive();
            MazeSpace.ActiveSpace = startSpace;
            startSpace.Refresh();

            System.Threading.Thread.Sleep(_movementPause);

            //try
            //{
            //    _maze.RunMaze();
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}

            try
            {
                _mazeNew.RunAgent(_mazeNew.StartPosition);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
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

            rowsText.Text = _mazeNew.Rows.ToString();
            columnsText.Text = _mazeNew.Columns.ToString();
            startPositionText.Text = _mazeNew.StartPosition.ToString();
            goalPositionText.Text = _mazeNew.GoalPosition.ToString();
            discountRateText.Text = _mazeNew.DiscountRate.ToString("0.##");
            learningRateText.Text = _mazeNew.LearningRate.ToString("0.##");
            trainingEpisodesText.Text = _mazeNew.NumberOfTrainingEpisodes.ToString();

            //_mazeNew.Obstructions = _maze.Obstructions;
            foreach (var obstruction in _mazeNew.Obstructions)
            {
                var lvi = new ListViewItem(obstruction.BetweenSpace.ToString());
                lvi.SubItems.Add(obstruction.AndSpace.ToString());
                obstructionsList.Items.Add(lvi);
            }

            //_mazeNew.AdditionalRewards = new List<QLearning.Core.CustomObjective>();

            //foreach(var reward in _maze.AdditionalRewards)
            //{
            //    _mazeNew.AddReward(reward.Position, reward.Value);
            //}

            _additionalRewards = _mazeNew.AdditionalRewards;

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
                    _additionalRewards = new List<QLearning.Core.CustomObjective>();
                }

                foreach(var reward in _additionalRewards)
                {
                    //_maze.AddCustomReward(reward.Position, reward.Value);
                    _mazeNew.AddReward(reward.State, reward.Value);
                }


                //_maze.AddCustomReward(9, 30);
                _additionalRewards = _mazeNew.GetAdditionalRewards().ToList();
            }
            catch
            {
                return;
            }

            for (int i = _mazeNew.Obstructions.Count - 1; i >= 0; i--)
            {
                var wall = _mazeNew.Obstructions[i];
                //_maze.RemoveObstruction(wall.BetweenSpace, wall.AndSpace);
                _mazeNew.RemoveObstruction(wall.BetweenSpace, wall.AndSpace);
            }

            foreach (ListViewItem lvi in obstructionsList.Items)
            {
                //_maze.AddObstruction(Convert.ToInt32(lvi.Text), Convert.ToInt32(lvi.SubItems[1].Text));
                _mazeNew.AddObstruction(Convert.ToInt32(lvi.Text), Convert.ToInt32(lvi.SubItems[1].Text));
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
            _mazeNew.AddObstruction(between, and);
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
            _mazeNew.RemoveObstruction(between, and);
            
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
            
            for (int i = _mazeNew.Obstructions.Count - 1; i >= 0; --i)
            {
                var wall = _mazeNew.Obstructions[i];
                _mazeNew.RemoveObstruction(wall.BetweenSpace, wall.AndSpace);
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
            Train();
        }

        private void Train()
        {
            if (Convert.ToInt32(trainingEpisodesText.Text) != _mazeNew.NumberOfTrainingEpisodes)
            {
                _mazeNew.NumberOfTrainingEpisodes = Convert.ToInt32(trainingEpisodesText.Text);
            }

            //_mazeNew.MaxEpisodes = Convert.ToInt32(trainingEpisodesText.Text);

            try
            { 
                if (_trainingSessionSelector != null)
                {
                    _trainingSessionSelector.Dispose();
                    _trainingSessionSelector = null;
                }
            }
            catch { }

            var dlg = new TrainingProgress(_mazeNew);
            this.Cursor = Cursors.WaitCursor;
            this.Enabled = false;

            try
            {
                //_maze.AgentStateChangedEventHandler -= Maze_AgentStateChangedEventHandler;
                _mazeNew.AgentStateChanged -= MazeNew_AgentStateChanged;
                //_maze.AgentCompletedMazeEventHandler -= Maze_AgentCompletedMazeEventHandler;
                _mazeNew.AgentCompleted -= MazeNew_AgentCompleted;
                dlg.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex}");
            }
            finally
            {
                dlg.Dispose();
                _needsRetrain = false;
                //_maze.AgentStateChangedEventHandler += Maze_AgentStateChangedEventHandler;
                _mazeNew.AgentStateChanged += MazeNew_AgentStateChanged;
                //_maze.AgentCompletedMazeEventHandler += Maze_AgentCompletedMazeEventHandler;
                _mazeNew.AgentCompleted += MazeNew_AgentCompleted;

                this.Enabled = true;
                this.Cursor = Cursors.Default;
            }

            GetEpisodeSelection();
        }

        private void GetEpisodeSelection()
        {
            if (_trainingSessionSelector == null ||
                _trainingSessionSelector.IsDisposed)
            {
                _trainingSessionSelector = new TrainingSessionSelector(_mazeNew);
            }

            if (_trainingSessionSelector.ShowDialog() == DialogResult.OK)
            {
                _mazeNew.QualityTable = _trainingSessionSelector.SelectedSession.Quality;
            }
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
            if (_mazeNew.Rows.ToString() != rowsText.Text &&
                MazeTextChanged(rowsText))
            {
                clearObstructionsButton_Click(sender, e);
                _mazeNew.Rows = Convert.ToInt32(rowsText.Text);
                _needsRetrain = true;
                RespawnMaze();
            }
            
        }

        private void columnsText_Leave(object sender, EventArgs e)
        {
            if (_mazeNew.Columns.ToString() != columnsText.Text &&
                MazeTextChanged(columnsText))
            {
                clearObstructionsButton_Click(sender, e);
                _mazeNew.Columns = Convert.ToInt32(columnsText.Text);
                _needsRetrain = true;
                RespawnMaze();
            }
        }

        private void startPositionText_Leave(object sender, EventArgs e)
        {
            int newStartPosition;
            int oldStartPosition;

            if (_mazeNew.StartPosition.ToString() != startPositionText.Text &&
                MazeTextChanged(startPositionText, false))
            {
                oldStartPosition = _mazeNew.StartPosition;
                newStartPosition = Convert.ToInt32(startPositionText.Text);
                _mazeNew.StartPosition = newStartPosition;
                GetSpaceByPosition(oldStartPosition).SetStart(false);
                GetSpaceByPosition(newStartPosition).SetStart(true);
                //_needsRetrain = true;
            }
        }

        private void goalPositionText_Leave(object sender, EventArgs e)
        {
            int newGoalPosition;
            int oldGoalPosition;

            if (_mazeNew.GoalPosition.ToString() != goalPositionText.Text &&
                MazeTextChanged(goalPositionText))
            {
                oldGoalPosition = _mazeNew.GoalPosition;
                newGoalPosition = Convert.ToInt32(goalPositionText.Text);
                _mazeNew.GoalPosition = newGoalPosition;
                GetSpaceByPosition(oldGoalPosition).SetGoal(false);
                GetSpaceByPosition(newGoalPosition).SetGoal(true);
            }            
        }

        private void discountRateText_Leave(object sender, EventArgs e)
        {
            _overrideRespawn = true;

            if (_mazeNew.DiscountRate.ToString() != discountRateText.Text &&
                MazeTextChanged(discountRateText))
            {
                _mazeNew.DiscountRate = Convert.ToDouble(discountRateText.Text);
                _needsRetrain = true;
            }

            _overrideRespawn = false;
        }

        private void learningRateText_Leave(object sender, EventArgs e)
        {
            _overrideRespawn = true;

            if (_mazeNew.LearningRate.ToString() != learningRateText.Text &&
                MazeTextChanged(learningRateText))
            {
                _mazeNew.LearningRate = Convert.ToDouble(learningRateText.Text);
                _needsRetrain = true;
            }

            _overrideRespawn = false;
        }

        private void trainingEpisodesText_Leave(object sender, EventArgs e)
        {
            _overrideRespawn = true;

            if (_mazeNew.NumberOfTrainingEpisodes.ToString() != trainingEpisodesText.Text &&
                MazeTextChanged(trainingEpisodesText))
            {
                _mazeNew.NumberOfTrainingEpisodes = Convert.ToInt32(trainingEpisodesText.Text);
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
                    andSpacePosition = space.Position - _mazeNew.Columns;
                    andSpace = GetSpaceByPosition(andSpacePosition);
                    space.topWall.Visible = true;
                    andSpace.bottomWall.Visible = true;
                    break;
                case "bottomWall":
                    if (rowNumber == _mazeNew.Rows - 1) return;
                    andSpacePosition = space.Position + _mazeNew.Columns;
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

            between = space.Position;

            switch (wallBox.Name)
            {
                case "topWall":
                    and = between - _mazeNew.Columns;
                    andSpace = GetSpaceByPosition(and);
                    space.topWall.Visible = false;
                    andSpace.bottomWall.Visible = false;
                    break;
                case "bottomWall":
                    and = between + _mazeNew.Columns;
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
            _needsRetrain = true;
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
            var dlg = new Objectives(_mazeNew);
            dlg.ShowDialog();
            
            if (dlg.RewardsChanged)
            {
                _additionalRewards = _mazeNew.AdditionalRewards;
                RespawnMaze();
                _needsRetrain = true;
            }
        }

    }
}
