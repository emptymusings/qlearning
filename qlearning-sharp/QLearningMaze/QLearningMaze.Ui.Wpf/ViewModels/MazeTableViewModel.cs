namespace QLearningMaze.Ui.Wpf.ViewModels
{
    using System;
    using System.Data;
    using System.Collections.Generic;
    using System.Text;
    using Core.Mazes;

    public class MazeTableViewModel : ViewModelBase
    {
        private CustomObjectivesViewModel _test;
        public enum TableTypes
        {
            Quality,
            Rewards,
            States
        }

        private readonly TableTypes _tableType;
        private readonly double[][] _arrayValues;
        private readonly int _numberOfStates;

        public MazeTableViewModel() { }

        public MazeTableViewModel(double[][] arrayValues, int numberOfStates, TableTypes tableType)
        {
            _tableType = tableType;
            _arrayValues = arrayValues;
            _numberOfStates = numberOfStates;
            DetailsViewTitle = tableType.ToString() + " Details";
        }

        public MazeTableViewModel(int[][] arrayValues, int numberOfStates, TableTypes tableType)
        {
            _tableType = tableType;
            _numberOfStates = numberOfStates;
            _arrayValues = new double[arrayValues.Length][];

            for (int i = 0; i < arrayValues.Length; ++i)
            {
                _arrayValues[i] = new double[arrayValues[i].Length];

                for(int j = 0; j < arrayValues[i].Length; ++j)
                {
                    _arrayValues[i][j] = arrayValues[i][j];
                }
            }

            DetailsViewTitle = tableType.ToString() + " Details";
        }

        private DataTable _tableValues;

        public DataTable TableValues
        {
            get { return _tableValues; }
            set { SetProperty(ref _tableValues, value); }
        }

        private string _detailsViewTitle;

        public string DetailsViewTitle
        {
            get { return _detailsViewTitle; }
            set { SetProperty(ref _detailsViewTitle, value); }
        }


        public void InitializeTable()
        {
            DataTable result = new DataTable();

            for (int i = 0; i < _arrayValues[i].Length; ++i)
            {
                result.Columns.Add(((Actions)i).ToString());
            }

            var dc = new DataColumn("Row Number");
            result.Columns.Add(dc);
            dc.SetOrdinal(0);

            for (int i = 0; i < _numberOfStates; ++i)
            {
                var dr = result.NewRow();
                dr[0] = i;

                for (int j = 0; j < _arrayValues[i].Length; ++j)
                {
                    dr[j + 1] = _arrayValues[i][j];
                }

                result.Rows.Add(dr);
            }

            TableValues = result;
        }
    }
}
