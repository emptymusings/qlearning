namespace QLearningMaze.Ui.Forms
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    public partial class ObservationSpace : UserControl
    {
        public event EventHandler<WallMouseUpEventArgs> WallMouseUpEvent;

        public ObservationSpace()
        {
            InitializeComponent();
        }

        public int Position { get; private set; }

        public void SetActive()
        {
            activeImage.Visible = true;
        }

        public void SetInactive()
        {
            activeImage.Visible = false;
        }

        public void SetGoal()
        {
            goalLabel.Visible = true;
        }

        public void SetStart()
        {
            startLabel.Visible = true;
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

            var height = this.Height;
            var width = this.Width;
            var position = this.Location;


            var rowClientPosition = this.Parent.PointToClient(new Point(e.X, e.Y));
            var spaceClientPosition = this.PointToClient(new Point(e.X, e.Y));
            var spaceClientPositionEmpty = this.PointToClient(Point.Empty);
            var rowPointToScreen = this.Parent.PointToScreen(new Point(e.X, e.Y));
            var spacePointToScreen = this.PointToScreen(new Point(e.X, e.Y));
            var spacePointToScreenEmpty = this.PointToScreen(Point.Empty);

            var topWallClientPosition = topWall.PointToClient(new Point(e.X, e.Y));
            var topWallClientEmpty = topWall.PointToClient(Point.Empty);
            var topWallScreenPosition = topWall.PointToScreen(new Point(e.X, e.Y));
            var topWallScreenEmpty = topWall.PointToScreen(Point.Empty);

            var fromWeb = this.PointToClient(this.Parent.PointToScreen(Point.Empty));

            if (e.Button != MouseButtons.Left) return;

            if (e.Y <= topWall.Height + fudgeRoom)
                OnWallMouseUp(new WallMouseUpEventArgs(topWall, Position));
            else if (e.Y >= bottomWall.Top - fudgeRoom)
                OnWallMouseUp(new WallMouseUpEventArgs(bottomWall, Position));
            else if (e.X <= leftWall.Width + fudgeRoom)
                OnWallMouseUp(new WallMouseUpEventArgs(leftWall, Position));
            else if (e.X >= rightWall.Left - fudgeRoom)
                OnWallMouseUp(new WallMouseUpEventArgs(rightWall, Position));

        }
    }

    public class WallMouseUpEventArgs : EventArgs
    {
        public WallMouseUpEventArgs(PictureBox wallPictureBox, int spacePosition)
        {
            this.WallPictureBox = wallPictureBox;
            this.WallName = wallPictureBox.Name;
            this.SpacePosition = spacePosition;
        }

        public PictureBox WallPictureBox { get; set; }
        public string WallName { get; set; }
        public int SpacePosition { get; set; }
        public int RowNumber { get; set; }
    }
}
