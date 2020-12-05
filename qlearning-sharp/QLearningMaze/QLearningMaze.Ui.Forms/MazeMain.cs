namespace QLearningMaze.Ui.Forms
{
    using Core;
    using QLearning.Core;
    using QLearning.Core.Environment;
    using QLearningMaze.Core.Agent;
    using QLearningMaze.Core.Mazes;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Windows.Forms;
    using System.Threading.Tasks;

    public partial class MazeMain : Form
    {
        private MazeAgent _agentPrimary;
        private MazeAgent _agentSecondary;
        private bool _useSecondaryAgent;
        private int _movementPause = 100;
        private bool _overrideRespawn = false;
        private bool _needsRetrain = false;
        private List<CustomObjective> _additionalRewards = new List<CustomObjective>();
        private TrainingSessionSelector _trainingSessionSelector = null;
        private List<MazeObstruction> _walls = new List<MazeObstruction>();

        private delegate void AgentStateChangedDelegate(object sender, QLearning.Core.AgentStateChangedEventArgs e);
        private delegate void AgentCompletedDelegate(object sender, QLearning.Core.AgentCompletedEventArgs e);

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

            InitializeAgents();         
        }

        private void InitializeAgents()
        {
            _agentPrimary = InitializeAgent();
            _agentSecondary = InitializeAgent();
        }

        private MazeAgent InitializeAgent()
        {
            var agent = new MazeAgent(
                0.5,
                0.5,
                1000,
                1000,
                3);

            AgentSubscribeEvents(agent);
            return agent;
        }

        private void AgentSubscribeEvents(MazeAgent agent)
        {
            agent.AgentStateChanged += Maze_AgentStateChanged;
            agent.AgentCompleted += Maze_AgentCompleted;
        }

        private void AgentUnsubscribeEvents(MazeAgent agent)
        {
            agent.AgentStateChanged -= Maze_AgentStateChanged;
            agent.AgentCompleted -= Maze_AgentCompleted;
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

        private void RewardsMenuItem_Click(object sender, EventArgs e)
        {
            var view = new TabledDetailsView(_agentPrimary.Environment, TabledDetailsView.ValueTypes.Rewards);
            view.Show();
        }

        private void QualityMenuItem_Click(object sender, EventArgs e)
        {
            if (_agentPrimary.Environment.QualityTable == null) trainMazeButton_Click(sender, e);

            var view = new TabledDetailsView(_agentPrimary.Environment, TabledDetailsView.ValueTypes.Quality);
            view.Show();
        }

        private void StatesMenuItem_Click(object sender, EventArgs e)
        {
            if (_agentPrimary.Environment.StatesTable == null) trainMazeButton_Click(sender, e);

            var view = new TabledDetailsView(_agentPrimary.Environment, TabledDetailsView.ValueTypes.StateSpace);
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

                var loaded = MazeUtilities.LoadObject<MazeAgent>(dlg.FileName);
                _agentPrimary = MazeUtilities.ConvertLoadedAgent(loaded);
                _agentSecondary = MazeUtilities.ConvertLoadedAgent(loaded);
                _agentSecondary.Environment = MazeUtilities.CopyEnvironment(loaded.Environment);
                _agentSecondary.StartPosition = _agentPrimary.StartPosition;
                
                AgentSubscribeEvents(_agentPrimary);
                AgentSubscribeEvents(_agentSecondary);

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
                MazeUtilities.SaveObject(dlg.FileName, _agentPrimary);
            }
        }
                
        private void RespawnMaze()
        {
            if (_overrideRespawn) return;

            var newSpace = new MazeSpace();

            SetMazeValuesFromForm();
            mazeSpace.CreateMazeControls(_agentPrimary.Environment);

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

        private void Maze_AgentStateChanged(object sender, QLearning.Core.AgentStateChangedEventArgs e)
        {
            if (mazeSpace.InvokeRequired)
            {
                mazeSpace.BeginInvoke(new AgentStateChangedDelegate(Maze_AgentStateChanged), sender, e);
            }
            else
            {
                var agent = (MazeAgent)sender;

                var newSpace = GetSpaceByPosition(e.NewState % agent.Environment.StatesPerPhase);

                Label rewardsLabel;
                string prefix;
                ObservationSpace activeSpace;

                if (newSpace != null)
                {
                    if (agent == _agentPrimary)
                    {
                        if (MazeSpace.ActiveSpacePrimary != null)
                        {
                            MazeSpace.ActiveSpacePrimary.SetInactive();
                            MazeSpace.ActiveSpacePrimary.Invalidate();
                        }

                        MazeSpace.ActiveSpacePrimary = newSpace;
                        prefix = "Primary Agent -";
                        rewardsLabel = rewardsLabelPrimary;

                        newSpace.SetActive();
                    }
                    else
                    {
                        if (MazeSpace.ActiveSpaceSecondary != null)
                        {
                            MazeSpace.ActiveSpaceSecondary.SetInactive(false);
                            MazeSpace.ActiveSpaceSecondary.Invalidate();
                        }

                        MazeSpace.ActiveSpaceSecondary = newSpace;
                        prefix = "Secondary Agent -";
                        rewardsLabel = rewardsLabelSecondary;

                        newSpace.SetActive(false);
                    }

                    newSpace.Invalidate();
                    mazeSpace.Invalidate();
                    newSpace.Refresh();
                    System.Threading.Thread.Sleep(_movementPause);


                    rewardsLabel.Text = $"{prefix} Moves: {e.MovesMade} Reward: {e.RewardsEarned}";

                    if (newSpace.rewardLabel.Visible == true)
                    {
                        if (agent.Environment.RewardsTable[e.NewState][(int)Actions.GetCustomReward] > -1)
                        {
                            newSpace.rewardLabel.Visible = false;
                        }
                    }
                    rewardsLabel.Invalidate();
                    rewardsLabel.Refresh();
                }
            }
        }

        private void Maze_AgentCompleted(object sender, QLearning.Core.AgentCompletedEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new AgentCompletedDelegate(Maze_AgentCompleted), sender, e);
            }
            else
            {
                var agent = (MazeAgent)sender;
                Label rewardsLabel;
                string prefix;

                if (agent == _agentPrimary)
                {
                    prefix = "Primary Agent -";
                    rewardsLabel = rewardsLabelPrimary;
                }
                else
                {
                    prefix = "Secondary Agent -";
                    rewardsLabel = rewardsLabelSecondary;
                }


                rewardsLabel.Text = $"{prefix} Moves: {e.Moves} Reward: {e.Rewards}";
            }
        }

        private void runMaze_Click(object sender, EventArgs e)
        {
            RunMaze();
        }

        private void RunMaze()
        {
            if (MazeSpace.ActiveSpacePrimary != null)
            {
                MazeSpace.ActiveSpacePrimary.SetInactive();
            }

            if (MazeSpace.ActiveSpaceSecondary != null)
            {
                MazeSpace.ActiveSpaceSecondary.SetInactive(false);
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

            var startSpacePrimary = GetSpaceByPosition(_agentPrimary.StartPosition);
            startSpacePrimary.SetActive();
            MazeSpace.ActiveSpacePrimary = startSpacePrimary;
            startSpacePrimary.Refresh();

            if (_useSecondaryAgent)
            {
                var startSpaceSecondary = GetSpaceByPosition(_agentSecondary.StartPosition);
                startSpaceSecondary.SetActive(false);
                MazeSpace.ActiveSpaceSecondary = startSpaceSecondary;
                startSpaceSecondary.Refresh();
            }

            System.Threading.Thread.Sleep(_movementPause);
            
            try
            {
                Task[] tasks;

                if (_useSecondaryAgent)
                {
                    tasks = new Task[]
                    {
                        Task.Run(() => RunAgentAsync(_agentPrimary)),
                        Task.Run(() => RunAgentAsync(_agentSecondary))
                    };
                }
                else
                {
                    tasks = new Task[]
                    {
                        Task.Run(() => RunAgentAsync(_agentPrimary))
                    };
                }

                Task.WaitAll(tasks);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            entryPanel.Enabled = true;
            Cursor = Cursors.Default;
        }

        private Task RunAgentAsync(MazeAgent agent)
        {
            try
            {
                agent.Run(agent.StartPosition);
            }
            catch(Exception ex)
            {
                string prefix;

                if (agent == _agentPrimary)
                {
                    prefix = "Primary";
                }
                else
                {
                    prefix = "Secondary";
                }

                MessageBox.Show($"{prefix} Agent Error: {ex.Message}");
            }
            return Task.CompletedTask;
        }

        private ObservationSpace GetSpaceByPosition(int position)
        {
            return mazeSpace.GetSpaceByPosition(position);
        }

        private void SetFormValuesFromMaze()
        {
            _overrideRespawn = true;

            rowsText.Text = _agentPrimary.Environment.Rows.ToString();
            columnsText.Text = _agentPrimary.Environment.Columns.ToString();
            startPositionText.Text = _agentPrimary.StartPosition.ToString();
            secondaryStartTextBox.Text = startPositionText.Text;
            goalPositionText.Text = _agentPrimary.Environment.GoalPosition.ToString();
            discountRateText.Text = _agentPrimary.DiscountRate.ToString("0.##");
            learningRateText.Text = _agentPrimary.LearningRate.ToString("0.##");
            trainingEpisodesText.Text = _agentPrimary.NumberOfTrainingEpisodes.ToString();


            _agentPrimary.Environment.AddObjective(_agentPrimary.Environment.GoalPosition);

            if (_agentPrimary.LearningStyle == LearningStyles.QLearning)
            {
                qLearningRadio.Checked = true;
            }
            else
            {
                sarsaRadio.Checked = true;
            }

            foreach (var obstruction in _agentPrimary.Environment.Obstructions)
            {
                _walls.Add(new MazeObstruction { BetweenSpace = obstruction.BetweenSpace, AndSpace = obstruction.AndSpace });
            }

            _additionalRewards = _agentPrimary.Environment.AdditionalRewards;

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
                    _agentPrimary.Environment.AddReward(reward.State, reward.Value, (reward.IsRequired ? true : reward.Value >= 0));
                }

                _additionalRewards = _agentPrimary.Environment.GetAdditionalRewards().ToList();
            }
            catch
            {
                return;
            }

            for (int i = _agentPrimary.Environment.Obstructions.Count - 1; i >= 0; i--)
            {
                var wall = _agentPrimary.Environment.Obstructions[i];
                _agentPrimary.Environment.RemoveObstruction(wall.BetweenSpace, wall.AndSpace);
            }

            foreach (var wall in _walls)
            {
                _agentPrimary.Environment.AddObstruction(wall.BetweenSpace, wall.AndSpace);
            }
        }

        private void AddObstruction(int between, int and)
        {
            _walls.Add(new MazeObstruction { BetweenSpace = between, AndSpace = and });
            _agentPrimary.Environment.AddObstruction(between, and);
            
            _needsRetrain = true;
        }

        private void RemoveObstructionFromMaze(int between, int and)
        {
            _agentPrimary.Environment.RemoveObstruction(between, and);
            
            _needsRetrain = true;
        }

        private MazeObstruction GetWallObstruction(int between, int and)
        {
            return _walls.Where(b => (b.BetweenSpace == between && b.AndSpace == and) || (b.AndSpace == between && b.BetweenSpace == and)).FirstOrDefault();            
        }

        private void clearObstructionsButton_Click(object sender, EventArgs e)
        {
            _walls.Clear();
                        
            for (int i = _agentPrimary.Environment.Obstructions.Count - 1; i >= 0; --i)
            {
                var wall = _agentPrimary.Environment.Obstructions[i];
                _agentPrimary.Environment.RemoveObstruction(wall.BetweenSpace, wall.AndSpace);
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
            if (Convert.ToInt32(trainingEpisodesText.Text) != _agentPrimary.NumberOfTrainingEpisodes)
            {
                _agentPrimary.NumberOfTrainingEpisodes = Convert.ToInt32(trainingEpisodesText.Text);
            }

            if (_useSecondaryAgent)
            {
                _agentSecondary.NumberOfTrainingEpisodes = _agentPrimary.NumberOfTrainingEpisodes;
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

            _agentPrimary.State = _agentPrimary.StartPosition;
            var dlg = new TrainingProgress(_agentPrimary, (_useSecondaryAgent ? _agentSecondary : null));
            this.Cursor = Cursors.WaitCursor;
            this.Enabled = false;

            try
            {
                AgentUnsubscribeEvents(_agentPrimary);
                AgentUnsubscribeEvents(_agentSecondary);
                
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
           // _agentSecondary.Environment.QualityTable = _agentPrimary.Environment.QualityTable;
            AgentSubscribeEvents(_agentPrimary);
            AgentSubscribeEvents(_agentSecondary);
        }

        private void GetEpisodeSelection()
        {
            if (_trainingSessionSelector == null ||
                _trainingSessionSelector.IsDisposed)
            {
                _trainingSessionSelector = new TrainingSessionSelector(_agentPrimary);
            }

            if (_trainingSessionSelector.ShowDialog() == DialogResult.OK)
            {
                _agentPrimary.Environment.QualityTable = _trainingSessionSelector.SelectedSession.Quality;
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
            if (_agentPrimary.Environment.Rows.ToString() != rowsText.Text &&
                MazeTextChanged(rowsText))
            {
                clearObstructionsButton_Click(sender, e);
                _agentPrimary.Environment.Rows = Convert.ToInt32(rowsText.Text);
                _needsRetrain = true;
                RespawnMaze();
            }
            
        }

        private void columnsText_Leave(object sender, EventArgs e)
        {
            if (_agentPrimary.Environment.Columns.ToString() != columnsText.Text &&
                MazeTextChanged(columnsText))
            {
                clearObstructionsButton_Click(sender, e);
                _agentPrimary.Environment.Columns = Convert.ToInt32(columnsText.Text);
                _needsRetrain = true;
                RespawnMaze();
            }
        }

        private void startPositionText_Leave(object sender, EventArgs e)
        {
            if (sender == startPositionText)
            {
                ChangeAgentStartPosition(_agentPrimary, startPositionText);
            }
            else
            {
                ChangeAgentStartPosition(_agentSecondary, secondaryStartTextBox);
            }
        }

        private void ChangeAgentStartPosition(MazeAgent agent, TextBox sourceTextBox)
        {
            int newStartPosition;
            int oldStartPosition;

            if (agent.StartPosition.ToString() != sourceTextBox.Text &&
                MazeTextChanged(sourceTextBox, false))
            {
                oldStartPosition = agent.StartPosition;
                newStartPosition = Convert.ToInt32(sourceTextBox.Text);
                agent.StartPosition = newStartPosition;
                bool isPrimary = (agent == _agentPrimary);
                var otherAgent = (isPrimary ? _agentSecondary : _agentPrimary);

                GetSpaceByPosition(oldStartPosition).SetStart(false, isPrimary);
                GetSpaceByPosition(newStartPosition).SetStart(true, isPrimary);

                if (otherAgent.StartPosition == oldStartPosition &&
                    _useSecondaryAgent)
                {
                    var space = GetSpaceByPosition(oldStartPosition);
                    space.SetStart(true, !isPrimary);
                    space.SetActive(!isPrimary);
                }
            }
        }

        private void goalPositionText_Leave(object sender, EventArgs e)
        {
            int newGoalPosition;
            int oldGoalPosition;

            if (_agentPrimary.Environment.GoalPosition.ToString() != goalPositionText.Text &&
                MazeTextChanged(goalPositionText))
            {
                oldGoalPosition = _agentPrimary.Environment.GoalPosition;
                newGoalPosition = Convert.ToInt32(goalPositionText.Text);
                _agentPrimary.Environment.AddObjective(newGoalPosition);
                GetSpaceByPosition(oldGoalPosition).SetGoal(false);
                GetSpaceByPosition(newGoalPosition).SetGoal(true);
            }            
        }

        private void discountRateText_Leave(object sender, EventArgs e)
        {
            _overrideRespawn = true;

            if (_agentPrimary.DiscountRate.ToString() != discountRateText.Text &&
                MazeTextChanged(discountRateText))
            {
                _agentPrimary.DiscountRate = Convert.ToDouble(discountRateText.Text);
                _needsRetrain = true;
            }

            _agentSecondary.DiscountRate = _agentPrimary.DiscountRate;

            _overrideRespawn = false;
        }

        private void learningRateText_Leave(object sender, EventArgs e)
        {
            _overrideRespawn = true;

            if (_agentPrimary.LearningRate.ToString() != learningRateText.Text &&
                MazeTextChanged(learningRateText))
            {
                _agentPrimary.LearningRate = Convert.ToDouble(learningRateText.Text);
                _needsRetrain = true;
            }

            _agentSecondary.LearningRate = _agentPrimary.LearningRate;

            _overrideRespawn = false;
        }

        private void trainingEpisodesText_Leave(object sender, EventArgs e)
        {
            _overrideRespawn = true;

            if (_agentPrimary.NumberOfTrainingEpisodes.ToString() != trainingEpisodesText.Text &&
                MazeTextChanged(trainingEpisodesText))
            {
                _agentPrimary.NumberOfTrainingEpisodes = Convert.ToInt32(trainingEpisodesText.Text);
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
                    andSpacePosition = space.Position - _agentPrimary.Environment.Columns;
                    andSpace = GetSpaceByPosition(andSpacePosition);
                    space.topWall.Visible = true;
                    andSpace.bottomWall.Visible = true;
                    break;
                case "bottomWall":
                    if (rowNumber == _agentPrimary.Environment.Rows - 1) return;
                    andSpacePosition = space.Position + _agentPrimary.Environment.Columns;
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
                    and = between - _agentPrimary.Environment.Columns;
                    andSpace = GetSpaceByPosition(and);
                    space.topWall.Visible = false;
                    andSpace.bottomWall.Visible = false;
                    break;
                case "bottomWall":
                    and = between + _agentPrimary.Environment.Columns;
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
            var dlg = new Objectives(_agentPrimary.Environment);
            dlg.ShowDialog();
            
            if (dlg.RewardsChanged)
            {
                _additionalRewards = _agentPrimary.Environment.AdditionalRewards;
                RespawnMaze();
                _needsRetrain = true;
            }
        }

        private void qLearningRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (qLearningRadio.Checked)
            {
                _agentPrimary.LearningStyle = LearningStyles.QLearning;
            }
            else
            {
                _agentPrimary.LearningStyle = LearningStyles.SARSA;
            }
        }

        private void secondaryAgentCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            _useSecondaryAgent = secondaryAgentCheckbox.Checked;

            secondaryAgentStartLabel.Visible = _useSecondaryAgent;
            secondaryStartTextBox.Visible = _useSecondaryAgent;
        }
    }
}
