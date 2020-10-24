namespace QLearningMaze.Ui.Forms
{
    using System.Drawing;
    using System.Windows.Forms;

    public partial class ObservationSpace : UserControl
    {
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

    }
}
