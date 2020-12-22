using System.Windows;

namespace QLearningMaze.Ui.Wpf.Views
{
    /// <summary>
    /// Interaction logic for CustomObjectivesView.xaml
    /// </summary>
    public partial class CustomObjectivesView : Window
    {
        public CustomObjectivesView()
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
    }
}
