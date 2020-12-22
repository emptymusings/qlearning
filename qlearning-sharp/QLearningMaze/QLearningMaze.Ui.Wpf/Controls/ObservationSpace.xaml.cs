namespace QLearningMaze.Ui.Wpf.Controls
{
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Interaction logic for ObservationSpace.xaml
    /// </summary>
    public partial class ObservationSpace : UserControl
    {
        public ObservationSpace()
        {
            InitializeComponent();
        }

        private void WallMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var context = (ViewModels.ObservationSpaceViewModel)DataContext;
            var wall = (Image)sender;
            var wallName = wall.Name.Replace("Wall", "");

            context.SetWallOpacity(wallName);
        }
    }
}
