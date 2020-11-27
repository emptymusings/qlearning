namespace QLearningMaze.Ui.Forms
{
    using Core;
    using QLearning.Core;
    using QLearning.Core.Agent;
    using QLearning.Core.Environment;
    using QLearningMaze.Core.Mazes;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Windows.Forms;

    public partial class MazeMain : Form
    {
        private ITDAgent<IMaze> _agent;
        
        private int _movementPause = 100;
        private bool _overrideRespawn = false;
        private bool _needsRetrain = false;
        private List<CustomObjective> _additionalRewards = new List<CustomObjective>();
        private TrainingSessionSelector _trainingSessionSelector = null;
        private List<MazeObstruction> _walls = new List<MazeObstruction>();

        public MazeMain()
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

            _agent = new TDAgent<IMaze>(
                new MazeBase(1, 1, 0, 0, 200),
                0.5,
                0.5,
                1000,
                1000,
                3);
            _agent.AgentStateChanged += Maze_AgentStateChanged;
            _agent.AgentCompleted += Maze_AgentCompleted;
            _agent.TrainingAgentStateChanged += Maze_TrainingAgentStateChanged;
            
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

        private void Maze_TrainingAgentStateChangingEventHandler(object sender, Core.TrainingAgentStateChangedEventArgs e)
        {
            int newState = e.State;
            double newQuality = e.NewQuality;
            double oldQuality = e.OldQuality;

        }
        private void Maze_TrainingAgentStateChanged(object sender, QLearning.Core.TrainingAgentStateChangedEventArgs e)
        {
            int newState = e.NewState;
            double newQuality = e.NewQuality;
            double oldQuality = e.OldQuality;
        }

        private void RewardsMenuItem_Click(object sender, EventArgs e)
        {
            var view = new TabledDetailsView(_agent.Environment, TabledDetailsView.ValueTypes.Rewards);
            view.Show();
        }

        private void QualityMenuItem_Click(object sender, EventArgs e)
        {
            if (_agent.Environment.QualityTable == null) trainMazeButton_Click(sender, e);

            var view = new TabledDetailsView(_agent.Environment, TabledDetailsView.ValueTypes.Quality);
            view.Show();
        }

        private void StatesMenuItem_Click(object sender, EventArgs e)
        {
            if (_agent.Environment.StatesTable == null) trainMazeButton_Click(sender, e);

            var view = new TabledDetailsView(_agent.Environment, TabledDetailsView.ValueTypes.StateSpace);
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
                _walls.Clear();
                mazeSpace.Enabled = false;

                var loaded = Core.MazeUtilities.LoadObject<TDAgent<MazeBase>>(dlg.FileName);
                _agent = QLearningMaze.Core.MazeUtilities.ConvertLoadedAgent(loaded);

                _agent.AgentStateChanged += Maze_AgentStateChanged;
                _agent.AgentCompleted += Maze_AgentCompleted;
                _agent.TrainingAgentStateChanged += Maze_TrainingAgentStateChanged;
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
                Core.MazeUtilities.SaveObject(dlg.FileName, _agent);
            }
        }
                
        private void RespawnMaze()
        {
            if (_overrideRespawn) return;

            var newSpace = new MazeSpace();

            SetMazeValuesFromForm();
            mazeSpace.CreateMazeControls(_agent.Environment);

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

        private void Maze_AgentStateChangedEventHandler(object sender, QLearningMaze.Core.AgentStateChangedEventArgs e)
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

        private void Maze_AgentStateChanged(object sender, QLearning.Core.AgentStateChangedEventArgs e)
        {
            var newSpace = GetSpaceByPosition(e.NewState % _agent.Environment.StatesPerPhase);

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
                    if (_agent.Environment.RewardsTable[e.NewState][(int)Actions.GetCustomReward] > -1)
                    {
                        newSpace.rewardLabel.Visible = false;
                    }
                }

                Application.DoEvents();
            }
        }

        private void Maze_AgentCompleted(object sender, QLearning.Core.AgentCompletedEventArgs e)
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
                Train();
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

            var startSpace = GetSpaceByPosition(_agent.Environment.StartPosition);
            startSpace.SetActive();
            MazeSpace.ActiveSpace = startSpace;
            startSpace.Refresh();

            System.Threading.Thread.Sleep(_movementPause);

            try
            {
                _agent.Run(_agent.Environment.StartPosition);
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
            return mazeSpace.GetSpaceByPosition(position);
        }

        private void SetFormValuesFromMaze()
        {
            _overrideRespawn = true;

            rowsText.Text = _agent.Environment.Rows.ToString();
            columnsText.Text = _agent.Environment.Columns.ToString();
            startPositionText.Text = _agent.Environment.StartPosition.ToString();
            goalPositionText.Text = _agent.Environment.GoalPosition.ToString();
            discountRateText.Text = _agent.DiscountRate.ToString("0.##");
            learningRateText.Text = _agent.LearningRate.ToString("0.##");
            trainingEpisodesText.Text = _agent.NumberOfTrainingEpisodes.ToString();

            if (_agent.LearningStyle == LearningStyles.QLearning)
            {
                qLearningRadio.Checked = true;
            }
            else
            {
                sarsaRadio.Checked = true;
            }

            foreach (var obstruction in _agent.Environment.Obstructions)
            {
                _walls.Add(new MazeObstruction { BetweenSpace = obstruction.BetweenSpace, AndSpace = obstruction.AndSpace });
            }

            _additionalRewards = _agent.Environment.AdditionalRewards;

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
                    _additionalRewards = new List<CustomObjective>();
                }

                foreach(var reward in _additionalRewards)
                {
                    _agent.Environment.AddReward(reward.State, reward.Value, (reward.IsRequired ? true : reward.Value >= 0));
                }

                _additionalRewards = _agent.Environment.GetAdditionalRewards().ToList();
            }
            catch
            {
                return;
            }

            for (int i = _agent.Environment.Obstructions.Count - 1; i >= 0; i--)
            {
                var wall = _agent.Environment.Obstructions[i];
                _agent.Environment.RemoveObstruction(wall.BetweenSpace, wall.AndSpace);
            }

            foreach (var wall in _walls)
            {
                _agent.Environment.AddObstruction(wall.BetweenSpace, wall.AndSpace);
            }
        }

        private void AddObstruction(int between, int and)
        {
            _walls.Add(new MazeObstruction { BetweenSpace = between, AndSpace = and });
            _agent.Environment.AddObstruction(between, and);
            
            _needsRetrain = true;
        }

        private void RemoveObstructionFromMaze(int between, int and)
        {
            _agent.Environment.RemoveObstruction(between, and);
            
            _needsRetrain = true;
        }

        private MazeObstruction GetWallObstruction(int between, int and)
        {
            return _walls.Where(b => (b.BetweenSpace == between && b.AndSpace == and) || (b.AndSpace == between && b.BetweenSpace == and)).FirstOrDefault();            
        }

        private void clearObstructionsButton_Click(object sender, EventArgs e)
        {
            _walls.Clear();
                        
            for (int i = _agent.Environment.Obstructions.Count - 1; i >= 0; --i)
            {
                var wall = _agent.Environment.Obstructions[i];
                _agent.Environment.RemoveObstruction(wall.BetweenSpace, wall.AndSpace);
            }

            RespawnMaze();
            _needsRetrain = true;
        }

        private void trainMazeButton_Click(object sender, EventArgs e)
        {
            Train();
        }

        private void Train()
        {
            if (Convert.ToInt32(trainingEpisodesText.Text) != _agent.NumberOfTrainingEpisodes)
            {
                _agent.NumberOfTrainingEpisodes = Convert.ToInt32(trainingEpisodesText.Text);
            }

            try
            { 
                if (_trainingSessionSelector != null)
                {
                    _trainingSessionSelector.Dispose();
                    _trainingSessionSelector = null;
                }
            }
            catch { }

            _agent.State = _agent.Environment.StartPosition;
            var dlg = new TrainingProgress(_agent);
            this.Cursor = Cursors.WaitCursor;
            this.Enabled = false;

            try
            {
                _agent.AgentStateChanged -= Maze_AgentStateChanged;
                _agent.AgentCompleted -= Maze_AgentCompleted;
                
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

                this.Enabled = true;
                this.Cursor = Cursors.Default;
            }

            GetEpisodeSelection();
            _agent.AgentStateChanged += Maze_AgentStateChanged;
            _agent.AgentCompleted += Maze_AgentCompleted;
        }

        private void GetEpisodeSelection()
        {
            if (_trainingSessionSelector == null ||
                _trainingSessionSelector.IsDisposed)
            {
                _trainingSessionSelector = new TrainingSessionSelector(_agent);
            }

            if (_trainingSessionSelector.ShowDialog() == DialogResult.OK)
            {
                _agent.Environment.QualityTable = _trainingSessionSelector.SelectedSession.Quality;
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
            if (_agent.Environment.Rows.ToString() != rowsText.Text &&
                MazeTextChanged(rowsText))
            {
                clearObstructionsButton_Click(sender, e);
                _agent.Environment.Rows = Convert.ToInt32(rowsText.Text);
                _needsRetrain = true;
                RespawnMaze();
            }
            
        }

        private void columnsText_Leave(object sender, EventArgs e)
        {
            if (_agent.Environment.Columns.ToString() != columnsText.Text &&
                MazeTextChanged(columnsText))
            {
                clearObstructionsButton_Click(sender, e);
                _agent.Environment.Columns = Convert.ToInt32(columnsText.Text);
                _needsRetrain = true;
                RespawnMaze();
            }
        }

        private void startPositionText_Leave(object sender, EventArgs e)
        {
            int newStartPosition;
            int oldStartPosition;

            if (_agent.Environment.StartPosition.ToString() != startPositionText.Text &&
                MazeTextChanged(startPositionText, false))
            {
                oldStartPosition = _agent.Environment.StartPosition;
                newStartPosition = Convert.ToInt32(startPositionText.Text);
                _agent.Environment.StartPosition = newStartPosition;
                GetSpaceByPosition(oldStartPosition).SetStart(false);
                GetSpaceByPosition(newStartPosition).SetStart(true);
            }
        }

        private void goalPositionText_Leave(object sender, EventArgs e)
        {
            int newGoalPosition;
            int oldGoalPosition;

            if (_agent.Environment.GoalPosition.ToString() != goalPositionText.Text &&
                MazeTextChanged(goalPositionText))
            {
                oldGoalPosition = _agent.Environment.GoalPosition;
                newGoalPosition = Convert.ToInt32(goalPositionText.Text);
                _agent.Environment.GoalPosition = newGoalPosition;
                GetSpaceByPosition(oldGoalPosition).SetGoal(false);
                GetSpaceByPosition(newGoalPosition).SetGoal(true);
            }            
        }

        private void discountRateText_Leave(object sender, EventArgs e)
        {
            _overrideRespawn = true;

            if (_agent.DiscountRate.ToString() != discountRateText.Text &&
                MazeTextChanged(discountRateText))
            {
                _agent.DiscountRate = Convert.ToDouble(discountRateText.Text);
                _needsRetrain = true;
            }

            _overrideRespawn = false;
        }

        private void learningRateText_Leave(object sender, EventArgs e)
        {
            _overrideRespawn = true;

            if (_agent.LearningRate.ToString() != learningRateText.Text &&
                MazeTextChanged(learningRateText))
            {
                _agent.LearningRate = Convert.ToDouble(learningRateText.Text);
                _needsRetrain = true;
            }

            _overrideRespawn = false;
        }

        private void trainingEpisodesText_Leave(object sender, EventArgs e)
        {
            _overrideRespawn = true;

            if (_agent.NumberOfTrainingEpisodes.ToString() != trainingEpisodesText.Text &&
                MazeTextChanged(trainingEpisodesText))
            {
                _agent.NumberOfTrainingEpisodes = Convert.ToInt32(trainingEpisodesText.Text);
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
                    andSpacePosition = space.Position - _agent.Environment.Columns;
                    andSpace = GetSpaceByPosition(andSpacePosition);
                    space.topWall.Visible = true;
                    andSpace.bottomWall.Visible = true;
                    break;
                case "bottomWall":
                    if (rowNumber == _agent.Environment.Rows - 1) return;
                    andSpacePosition = space.Position + _agent.Environment.Columns;
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
                    and = between - _agent.Environment.Columns;
                    andSpace = GetSpaceByPosition(and);
                    space.topWall.Visible = false;
                    andSpace.bottomWall.Visible = false;
                    break;
                case "bottomWall":
                    and = between + _agent.Environment.Columns;
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
            RemoveObstructionFromMaze(between, and);
            _needsRetrain = true;
        }

        private void RemoveObstructionsFromList(int between, int and)
        {
            var wall = GetWallObstruction(between, and);
            if (wall != null) _walls.Remove(wall);
        }

        private void rewardsButton_Click(object sender, EventArgs e)
        {
            var dlg = new Objectives(_agent.Environment);
            dlg.ShowDialog();
            
            if (dlg.RewardsChanged)
            {
                _additionalRewards = _agent.Environment.AdditionalRewards;
                RespawnMaze();
                _needsRetrain = true;
            }
        }

        private void qLearningRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (qLearningRadio.Checked)
            {
                _agent.LearningStyle = LearningStyles.QLearning;
            }
            else
            {
                _agent.LearningStyle = LearningStyles.SARSA;
            }
        }

    }
}
