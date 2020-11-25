namespace QLearningMaze.Ui.Forms
{
    using System.Collections.Generic;
    using System.Windows.Forms;

    public partial class ObservationSpaceRow : UserControl
    {
        public List<ObservationSpace> Spaces { get; set; } = new List<ObservationSpace>();

        public ObservationSpaceRow()
        {
            InitializeComponent();
        }

        public int RowNumber { get; private set; }

        public void SetRowNumber(int rowNumber)
        {
            rowLabel.Text = $"Row: {rowNumber}";
            this.RowNumber = rowNumber;
        }

        public void AddSpace(ObservationSpace observationSpace)
        {
            observationSpace.Dock = DockStyle.Left;
            this.Controls.Add(observationSpace);
            Spaces.Add(observationSpace);
        }
    }
}
