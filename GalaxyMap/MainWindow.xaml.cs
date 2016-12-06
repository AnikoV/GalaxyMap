using System;
using System.Collections.Generic;
using GalaxyMap.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GalaxyMap.Models;

namespace GalaxyMap
{
    public partial class MainWindow : Window
    {
        private Point FirstPoint;
        private MainWindowViewModel _viewModel;

        public List<Galaxy> Galaxies { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            _viewModel = new MainWindowViewModel();
            DataContext = _viewModel;

            EnableMouseMovements();
        }
        
        

        private void EnableMouseMovements()
        {
            BackgroundCanvas.MouseLeftButtonDown += (sender, args) =>
            {
                FirstPoint = args.GetPosition(this);
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
                    var res = new Point(FirstPoint.X - temp.X, FirstPoint.Y - temp.Y);

                    Canvas.SetLeft(GalaxyImage, Canvas.GetLeft(GalaxyImage) - res.X);
                    Canvas.SetTop(GalaxyImage, Canvas.GetTop(GalaxyImage) - res.Y);

                    Canvas.SetLeft(StarsCanvas, Canvas.GetLeft(StarsCanvas) - res.X);
                    Canvas.SetTop(StarsCanvas, Canvas.GetTop(StarsCanvas) - res.Y);

                    FirstPoint = temp;
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

        #region TEST UI

        private void TestUiCase()
        {
            CreateData();
            Draw();
        }
        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        private void Draw()
        {            
            dispatcherTimer.Tick += DispatcherTimerOnTick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }

        BitmapImage starBitmapImage = new BitmapImage(new Uri("pack://application:,,,/Images/stars.png", UriKind.Absolute));
        private void DispatcherTimerOnTick(object sender, EventArgs eventArgs)
        {
            if (counter == (Galaxies.Count - 1))
            {
                dispatcherTimer.Stop();
            }
            else
            {
                var galaxy = Galaxies[counter];
                foreach (var star in galaxy.Stars)
                {
                    var image = new Image();
                    var st = new ScaleTransform
                    {
                        ScaleX = 0.3,
                        ScaleY = 0.3
                    };
                    var rt = new TransformGroup();
                    rt.Children.Add(st);
                    image.RenderTransform = rt;
                    image.Source = starBitmapImage;
                    image.Width = starBitmapImage.Width;
                    image.Height = starBitmapImage.Height;
                    Canvas.SetLeft(image, star.X);
                    Canvas.SetTop(image, star.Y);
                    StarsCanvas.Children.Add(image);
                }
                counter++;
            }
        }

        int counter = 0;

        private void CreateData()
        {
            var random = new Random();
            Galaxies = new List<Galaxy>();
            for (int i = 0; i < 10; i++)
            {
                var stars = new List<Star>();
                for (int j = 0; j < 10; j++)
                {
                    var star = new Star();
                    star.Id = j;
                    star.Name = "Galaxy" + i + "Star" + j;
                    star.Source = "Images/stars.png";
                    star.X = random.Next(0, (int) StarsCanvas.ActualWidth);
                    star.Y = random.Next(0, (int)StarsCanvas.ActualHeight);
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
        
        #endregion

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            TestUiCase();
        }
    }
}
