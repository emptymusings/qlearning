using System.Windows;
using System.Windows.Input;

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
