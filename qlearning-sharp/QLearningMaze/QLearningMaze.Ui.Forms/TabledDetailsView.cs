namespace QLearningMaze.Ui.Forms
{
    using QLearningMaze.Core.Mazes;
    using System;
    using System.Windows.Forms;

    public partial class TabledDetailsView : Form
    {
        public enum ValueTypes
        {
            Quality,
            Rewards,
            StateSpace
        }

        private IMaze _maze;
        private ValueTypes _valueType;

        public TabledDetailsView(IMaze maze, ValueTypes valueType)
        {
            InitializeComponent();
            _maze = maze;
            _valueType = valueType;
        }

        private void TabledDetailsView_Shown(object sender, EventArgs e)
        {
            double[][] values;

            switch (_valueType)
            {
                case ValueTypes.Quality:
                    values = _maze.QualityTable;
                    break;
                case ValueTypes.Rewards:
                    values = _maze.RewardsTable;
                    break;
                case ValueTypes.StateSpace:
                    values = ConvertObservationSpace();
                    break;
                default:
                    throw new InvalidOperationException("Invalid Value Type");
            }

            detailsView1.ShowValues(values, _maze.NumberOfStates);
        }

        private double[][] ConvertObservationSpace()
        {
            double[][] result = new double[_maze.StatesTable.Length][];

            for (int i = 0; i < _maze.StatesTable.Length; ++i)
            {
                result[i] = new double[_maze.StatesTable[i].Length];

                for (int j = 0; j < result[i].Length; ++j)
                {
                    result[i][j] = _maze.StatesTable[i][j];
                }
            }

            return result;
        }
    }
}
