namespace QLearningMaze.Ui.Forms
{
    using System;
    using System.Windows.Forms;

    public partial class ObservationSpace : UserControl
    {
        public event EventHandler<WallMouseUpEventArgs> WallMouseUpEvent;

        public ObservationSpace()
        {
            InitializeComponent();
        }

        public int Position { get; private set; }

        public void SetActive(bool isPrimary = true)
        {
            if (isPrimary)
            {
                activeImage.Visible = true;
            }
            else
            {
                activeImageSecondary.Visible = true;
            }
        }

        public void SetInactive(bool isPrimary = true)
        {
            if (isPrimary)
            {
                activeImage.Visible = false;
            }
            else
            {
                activeImageSecondary.Visible = false;
            }
        }

        public void SetGoal(bool isGoal)
        {
            goalLabel.Visible = isGoal;
        }

        public void SetStart(bool isStart, bool isPrimary = true)
        {
            startLabel.Visible = isStart;

            if (isPrimary)
            {
                activeImage.Visible = isStart;
            }
            else
            {
                activeImageSecondary.Visible = isStart;
            }
        }

        public void SetReward(bool isReward, double value = 0)
        {
            rewardLabel.Visible = isReward;

            if (value < 0)
                rewardLabel.Text = $"Punishment: {value}";
            else
                rewardLabel.Text = $"Reward: {value}";
        }

        public void SetPosition(int position)
        {
            Position = position;
            positionLabel.Text = position.ToString();
        }

        protected virtual void OnWallMouseUp(WallMouseUpEventArgs e)
        {
            WallMouseUpEvent?.Invoke(this, e);
        }

        private void ObservationSpace_MouseUp(object sender, MouseEventArgs e)
        {
            int fudgeRoom = 5;

            if (e.Button != MouseButtons.Left) return;

            if (e.Y <= topWall.Height + fudgeRoom)
                OnWallMouseUp(new WallMouseUpEventArgs(topWall, Position, ((ObservationSpaceRow)this.Parent).RowNumber));
            else if (e.Y >= this.Height - bottomWall.Height - fudgeRoom)
                OnWallMouseUp(new WallMouseUpEventArgs(bottomWall, Position, ((ObservationSpaceRow)this.Parent).RowNumber));
            else if (e.X <= leftWall.Width + fudgeRoom)
                OnWallMouseUp(new WallMouseUpEventArgs(leftWall, Position, ((ObservationSpaceRow)this.Parent).RowNumber));
            else if (e.X >= rightWall.Left - fudgeRoom)
                OnWallMouseUp(new WallMouseUpEventArgs(rightWall, Position, ((ObservationSpaceRow)this.Parent).RowNumber));
        }

        private void topWall_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            
            OnWallMouseUp(new WallMouseUpEventArgs(topWall, Position, ((ObservationSpaceRow)this.Parent).RowNumber));
        }

        private void leftWall_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            OnWallMouseUp(new WallMouseUpEventArgs(leftWall, Position, ((ObservationSpaceRow)this.Parent).RowNumber));
        }

        private void bottomWall_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            OnWallMouseUp(new WallMouseUpEventArgs(bottomWall, Position, ((ObservationSpaceRow)this.Parent).RowNumber));
        }

        private void rightWall_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            OnWallMouseUp(new WallMouseUpEventArgs(rightWall, Position, ((ObservationSpaceRow)this.Parent).RowNumber));
        }
    }

    public class WallMouseUpEventArgs : EventArgs
    {
        public WallMouseUpEventArgs(PictureBox wallPictureBox, int spacePosition, int rowNumber)
        {
            this.WallPictureBox = wallPictureBox;
            this.WallName = wallPictureBox.Name;
            this.SpacePosition = spacePosition;
            this.RowNumber = rowNumber;
        }

        public PictureBox WallPictureBox { get; set; }
        public string WallName { get; set; }
        public int SpacePosition { get; set; }
        public int RowNumber { get; set; }
    }
}
