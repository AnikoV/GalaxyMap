using GalaxyMap.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using GalaxyMap.Models;

namespace GalaxyMap
{
    public partial class MainWindow : Window
    {
        private Point FirstPoint;
        private MainWindowViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();

            _viewModel = new MainWindowViewModel();
            DataContext = _viewModel;

            EnableMouseMovements();
        }

        private void DrawGalaxy(Galaxy galaxy)
        {
        }

        private void DrawStar(Star star)
        {
            
        }

        private void EnableMouseMovements()
        {
            RootCanvas.MouseLeftButtonDown += (sender, args) =>
            {
                FirstPoint = args.GetPosition(this);
                GalaxyImage.CaptureMouse();
            };

            RootCanvas.MouseWheel += (sender, args) =>
            {
                var matrix = GalaxyImage.RenderTransform.Value;
                var mousePoint = args.GetPosition(GalaxyImage);

                if (args.Delta > 0)
                    matrix.ScaleAtPrepend(1.15, 1.15, mousePoint.X, mousePoint.Y);
                else
                    matrix.ScaleAtPrepend(1 / 1.15, 1 / 1.15, mousePoint.X, mousePoint.Y);

                var newMatrix = new MatrixTransform(matrix);
                GalaxyImage.RenderTransform = newMatrix;
            };

            RootCanvas.MouseMove += (sender, args) =>
            {
                if (args.LeftButton == MouseButtonState.Pressed)
                {
                    var temp = args.GetPosition(this);
                    var res = new Point(FirstPoint.X - temp.X, FirstPoint.Y - temp.Y);

                    Canvas.SetLeft(GalaxyImage, Canvas.GetLeft(GalaxyImage) - res.X);
                    Canvas.SetTop(GalaxyImage, Canvas.GetTop(GalaxyImage) - res.Y);

                    FirstPoint = temp;
                }
            };

            RootCanvas.MouseUp += (sender, args) => { GalaxyImage.ReleaseMouseCapture(); };
        }
    }
}
