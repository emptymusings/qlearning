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
    public partial class TabledDetailsView : Form
    {
        public enum ValueTypes
        {
            Quality,
            Rewards,
            StateSpace
        }

        private IMazeNew _mazeNew;
        private ValueTypes _valueType;

        public TabledDetailsView(IMazeNew mazeNew, ValueTypes valueType)
        {
            InitializeComponent();
            _mazeNew = mazeNew;
            _valueType = valueType;
        }

        private void TabledDetailsView_Shown(object sender, EventArgs e)
        {
            double[][] values;

            switch (_valueType)
            {
                case ValueTypes.Quality:
                    values = _mazeNew.Quality;
                    break;
                case ValueTypes.Rewards:
                    values = _mazeNew.Rewards;
                    break;
                case ValueTypes.StateSpace:
                    values = ConvertObservationSpace();
                    break;
                default:
                    throw new InvalidOperationException("Invalid Value Type");
            }

            detailsView1.ShowValues(values, _mazeNew.NumberOfStates);
        }

        private double[][] ConvertObservationSpace()
        {
            double[][] result = new double[_mazeNew.ObservationSpace.Length][];

            for (int i = 0; i < _mazeNew.ObservationSpace.Length; ++i)
            {
                result[i] = new double[_mazeNew.ObservationSpace[i].Length];

                for (int j = 0; j < result[i].Length; ++j)
                {
                    result[i][j] = _mazeNew.ObservationSpace[i][j];
                }
            }

            return result;
        }
    }
}
