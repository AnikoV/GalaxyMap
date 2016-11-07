using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace GalaxyMap
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            EnableMouseMovements();
        }

        private Point FirstPoint = new Point();
        private void EnableMouseMovements()
        {
            galaxyImage.MouseLeftButtonDown += (sender, args) =>
            {
                FirstPoint = args.GetPosition(this);
                galaxyImage.CaptureMouse();
            };

            galaxyImage.MouseWheel += (sender, args) =>
            {
                var matrix = galaxyImage.RenderTransform.Value;
                var mousePoint = args.GetPosition(galaxyImage);

                if (args.Delta > 0)
                    matrix.ScaleAtPrepend(1.15,1.15,mousePoint.X,mousePoint.Y);
                else
                    matrix.ScaleAtPrepend(1/1.15,1/1.15, mousePoint.X, mousePoint.Y);

                var newMatrix = new MatrixTransform(matrix);
                galaxyImage.RenderTransform = newMatrix;
            };

            galaxyImage.MouseMove += (sender, args) =>
            {
                if (args.LeftButton == MouseButtonState.Pressed)
                {
                    var temp = args.GetPosition(this);
                    var res = new Point(FirstPoint.X - temp.X,FirstPoint.Y - temp.Y);

                    Canvas.SetLeft(galaxyImage, Canvas.GetLeft(galaxyImage) - res.X);
                    Canvas.SetTop(galaxyImage, Canvas.GetTop(galaxyImage) - res.Y);

                    FirstPoint = temp;
                }
            };

            galaxyImage.MouseUp += (sender, args) => { galaxyImage.ReleaseMouseCapture(); };
        }
    }
}
