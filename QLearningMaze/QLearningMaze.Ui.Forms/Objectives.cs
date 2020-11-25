namespace QLearningMaze.Ui.Forms
{
    using QLearningMaze.Core;
    using System;
    using System.Windows.Forms;


    public partial class Objectives : Form
    {
        private IMaze _maze;

        public Objectives(IMaze maze)
        {
            InitializeComponent();
            _maze = maze;
        }

        public bool RewardsChanged { get; set; }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            int position = Convert.ToInt32(positionText.Text);
            double value = Convert.ToDouble(valueText.Text);
            bool isRequired = value >= 0;

            _maze.AddReward(position, value, isRequired);
            var lvi = new ListViewItem(positionText.Text);
            lvi.SubItems.Add(valueText.Text);
            rewardsList.Items.Add(lvi);
            positionText.Text = null;
            valueText.Text = null;

            RewardsChanged = true;
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            if (rewardsList.SelectedItems.Count == 0)
                return;

            _maze.RemoveReward(Convert.ToInt32(rewardsList.SelectedItems[0].Text));
            _maze.RemoveReward(Convert.ToInt32(rewardsList.SelectedItems[0].Text));
            rewardsList.Items.Remove(rewardsList.SelectedItems[0]);
            rewardsList.SelectedItems.Clear();

            RewardsChanged = true;
        }

        private void Rewards_Shown(object sender, EventArgs e)
        {
            foreach(var reward in _maze.AdditionalRewards)
            {
                var lvi = new ListViewItem();
                lvi.SubItems[0].Text = reward.State.ToString();
                lvi.SubItems.Add(reward.Value.ToString());
                rewardsList.Items.Add(lvi);
            }
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            _maze.AdditionalRewards.Clear();
            rewardsList.Items.Clear();

            RewardsChanged = true;
        }
    }
}
