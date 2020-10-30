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
            Rewards
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

            if (_valueType == ValueTypes.Quality)
                values = _maze.Quality;
            else
                values = _maze.Rewards;

            detailsView1.ShowValues(values, _maze.NumberOfStates);
        }
    }
}
