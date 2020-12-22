namespace QLearningMaze.Ui.Wpf
{
    using System.Windows;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();            
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Close();            
        }
    }
}
