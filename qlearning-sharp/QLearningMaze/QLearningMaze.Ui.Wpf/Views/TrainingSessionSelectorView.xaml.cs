using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace QLearningMaze.Ui.Wpf.Views
{
    /// <summary>
    /// Interaction logic for TrainingSessionSelectorView.xaml
    /// </summary>
    public partial class TrainingSessionSelectorView : Window
    {
        public TrainingSessionSelectorView()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
