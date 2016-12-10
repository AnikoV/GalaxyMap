using System;
using System.Collections.Generic;
using GalaxyMap.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using GalaxyMap.Models;

namespace GalaxyMap
{
    public partial class MainWindow : Window
    {
        public List<Galaxy> Galaxies { get; set; }

        private MainWindowViewModel _viewModel;
        private const double ImageScale = 0.3;
        private Point _mousePos;
        private readonly BitmapImage _starBitmapImage = new BitmapImage(new Uri("pack://application:,,,/Images/stars.png", UriKind.Absolute));

        public MainWindow()
        {
            InitializeComponent();

            _viewModel = new MainWindowViewModel();
            DataContext = _viewModel;
            EnableMouseManipulations();
        }

#region MouseManipulations
        private void EnableMouseManipulations()
        {
            BackgroundCanvas.MouseLeftButtonDown += (sender, args) =>
            {
                _mousePos = args.GetPosition(this);
                GalaxyImage.CaptureMouse();
            };

            BackgroundCanvas.MouseWheel += (sender, args) =>
            {
                Scale(args,GalaxyImage);
                Scale(args,StarsCanvas);
            };

            BackgroundCanvas.MouseMove += (sender, args) =>
            {
                if (args.LeftButton == MouseButtonState.Pressed)
                {
                    var temp = args.GetPosition(this);
                    var res = new Point(_mousePos.X - temp.X, _mousePos.Y - temp.Y);

                    Canvas.SetLeft(GalaxyImage, Canvas.GetLeft(GalaxyImage) - res.X);
                    Canvas.SetTop(GalaxyImage, Canvas.GetTop(GalaxyImage) - res.Y);

                    Canvas.SetLeft(StarsCanvas, Canvas.GetLeft(StarsCanvas) - res.X);
                    Canvas.SetTop(StarsCanvas, Canvas.GetTop(StarsCanvas) - res.Y);

                    _mousePos = temp;
                }
            };

            BackgroundCanvas.MouseUp += (sender, args) => { GalaxyImage.ReleaseMouseCapture(); };
        }

        private void Scale(MouseWheelEventArgs args, UIElement element)
        {
            var matrix = element.RenderTransform.Value;
            var mousePoint = args.GetPosition(element);

            if (args.Delta > 0)
                matrix.ScaleAtPrepend(1.15, 1.15, mousePoint.X, mousePoint.Y);
            else
                matrix.ScaleAtPrepend(1 / 1.15, 1 / 1.15, mousePoint.X, mousePoint.Y);

            var newMatrix = new MatrixTransform(matrix);
            element.RenderTransform = newMatrix;
        }
        #endregion

        #region TEST UI

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            TestUiCase();
        }

        private void TestUiCase()
        {
            CreateData();
            DrawData();
        }

        private void CreateData()
        {
            var random = new Random();
            Galaxies = new List<Galaxy>();
            for (int i = 0; i < 10; i++)
            {
                var stars = new List<Star>();
                for (int j = 0; j < 10; j++)
                {
                    var star = new Star
                    {
                        Id = j,
                        Name = "Galaxy" + i + "Star" + j,
                        Source = "Images/stars.png",
                        X = random.Next(0, (int)StarsCanvas.ActualWidth),
                        Y = random.Next(0, (int)StarsCanvas.ActualHeight)
                    };
                    stars.Add(star);
                }
                Galaxies.Add(new Galaxy
                {
                    Id = i,
                    Name = "Galaxy" + i,
                    Stars = stars
                });
            }
        }

        private void DrawData()
        {
            foreach (var galaxy in Galaxies)
            {
                DrawLinesBetweenStars(galaxy);
                DrawStars(galaxy);
            }
        }

        private void DrawStars(Galaxy galaxy)
        {
            foreach (var star in galaxy.Stars)
            {
                var image = new Image();
                var st = new ScaleTransform
                {
                    ScaleX = ImageScale,
                    ScaleY = ImageScale
                };
                var rt = new TransformGroup();
                rt.Children.Add(st);
                image.RenderTransform = rt;
                image.Source = _starBitmapImage;
                image.Width = _starBitmapImage.Width;
                image.Height = _starBitmapImage.Height;
                Canvas.SetLeft(image, star.X);
                Canvas.SetTop(image, star.Y);
                StarsCanvas.Children.Add(image);
            }
        }

        private void DrawLinesBetweenStars(Galaxy galaxy)
        {
            for (int i = 0; i < galaxy.Stars.Count - 1; i++)
            {
                var star0 = galaxy.Stars[i];
                var star1 = galaxy.Stars[i + 1];
                var offset = new Vector((_starBitmapImage.Width / 2.0) * ImageScale,
                                           (_starBitmapImage.Height / 2.0) * ImageScale);
                var line = new Line
                {
                    X1 = star0.X + offset.X,
                    Y1 = star0.Y + offset.Y,
                    X2 = star1.X + offset.X,
                    Y2 = star1.Y + offset.Y,
                    Stroke = Brushes.LightSteelBlue,
                    StrokeThickness = 2
                };
                StarsCanvas.Children.Add(line);
            }
        }
        
        #endregion
    }
}
