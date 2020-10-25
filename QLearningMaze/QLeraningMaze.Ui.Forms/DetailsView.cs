using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Globalization;

namespace QLearningMaze.Ui.Forms
{
    public partial class DetailsView : UserControl
    {
        public DetailsView()
        {
            InitializeComponent();
        }

        public void ShowValues(double[][] values, int numberOfStates)
        {
            var dt = new DataTable();
            PopulateTable(dt, numberOfStates, values);

            dataGridView1.DataSource = dt;
            
            dataGridView1.Show();
        }


        private void PopulateTable(DataTable dt, int numberOfStates, double[][] values)
        {

            for (int i = 0; i < numberOfStates; ++i)
            {
                dt.Columns.Add(i.ToString());
            }

            // Add row number column
            var dc = new DataColumn("Row Number");
            dt.Columns.Add(dc);
            dc.SetOrdinal(0);

            for (int i = 0; i < numberOfStates; ++i)
            {
                var dr = dt.NewRow();
                dr[0] = i;

                for (int j = 0; j < numberOfStates; ++j)
                {
                    dr[j + 1] = values[i][j];
                }

                dt.Rows.Add(dr);
            }
        }
    }
}
