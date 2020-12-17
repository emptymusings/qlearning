namespace QLearningMaze.Ui.Wpf.Controls
{
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
    using System.Windows.Navigation;
    using System.Windows.Shapes;
    using Controls;

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
