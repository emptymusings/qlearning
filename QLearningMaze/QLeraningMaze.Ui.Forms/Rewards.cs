using QLearningMaze.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace QLearningMaze.Ui.Forms
{
    public partial class Rewards : Form
    {
        private IMazeNew _mazeNew;

        public Rewards(IMazeNew mazeNew)
        {
            InitializeComponent();
            _mazeNew = mazeNew;
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
            _mazeNew.AddReward(Convert.ToInt32(positionText.Text), Convert.ToDouble(valueText.Text));
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

            _mazeNew.RemoveReward(Convert.ToInt32(rewardsList.SelectedItems[0].Text));
            rewardsList.Items.Remove(rewardsList.SelectedItems[0]);
            rewardsList.SelectedItems.Clear();

            RewardsChanged = true;
        }

        private void Rewards_Shown(object sender, EventArgs e)
        {
            foreach(var reward in _mazeNew.AdditionalRewards)
            {
                var lvi = new ListViewItem();
                lvi.SubItems[0].Text = reward.State.ToString();
                lvi.SubItems.Add(reward.Value.ToString());
                rewardsList.Items.Add(lvi);
            }
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            _mazeNew.AdditionalRewards.Clear();
            rewardsList.Items.Clear();

            RewardsChanged = true;
        }
    }
}
