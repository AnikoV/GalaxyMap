using System;
using System.Diagnostics;
using System.Linq;
using GalaxyMap.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using GalaxyMap.Models;

namespace GalaxyMap
{
    public partial class MainWindow : Window
    {
        private MainWindowViewModel _viewModel;
        private const double ImageScale = 0.3;
        private Point _mousePos;
        private readonly BitmapImage _starBitmapImage = new BitmapImage(new Uri("pack://application:,,,/Images/stars.png", UriKind.Absolute));
        private Vector _starCenterOffset;
        private Vector _labelOffset;

        public MainWindow()
        {
            InitializeComponent();

            _starCenterOffset = new Vector(_starBitmapImage.Width / 2.0 * ImageScale, _starBitmapImage.Height / 2.0 * ImageScale);
            _labelOffset = new Vector(_starBitmapImage.Width / 2.0 * ImageScale, -5);
            _viewModel = new MainWindowViewModel();
            DataContext = _viewModel;
            EnableMouseManipulations();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            DrawMap();
            Debug.WriteLine("===BackgroundCanvas===");
            Debug.WriteLine(BackgroundCanvas.ActualWidth + " " + BackgroundCanvas.ActualHeight);
        }
        
        private void SearchTextBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                _viewModel.SearchStarsInDataBase(SearchTextBox.Text.ToLower());
            }
        }

        #region DRAW MAP

        private void DrawMap()
        {
            foreach (var constellation in _viewModel.Constellations)
            {
                DrawLinesBetweenStars(constellation);
                DrawStars(constellation);
            }
        }

        private void DrawStars(Constellation constellation)
        {
            double sumX = 0, sumY = 0;
            foreach (var star in constellation.Star)
            {
                var scaletransform = new ScaleTransform
                {
                    ScaleX = ImageScale,
                    ScaleY = ImageScale
                };
                var transformGroup = new TransformGroup();
                transformGroup.Children.Add(scaletransform);
                var image = new Image
                {
                    RenderTransform = transformGroup,
                    Source = _starBitmapImage,
                    Width = _starBitmapImage.Width,
                    Height = _starBitmapImage.Height
                };
                Canvas.SetLeft(image, star.x);
                Canvas.SetTop(image, star.y);

                var textBlock = new TextBlock
                {
                    Text = star.nameOfStar,
                    Foreground = Brushes.Chartreuse,
                    TextAlignment = TextAlignment.Center
                };

                sumX += star.x;
                sumY += star.y;

                Canvas.SetLeft(textBlock, star.x + _labelOffset.X -30);
                Canvas.SetTop(textBlock, star.y + _labelOffset.Y);

                StarsCanvas.Children.Add(textBlock);
                StarsCanvas.Children.Add(image);
            }
            var textBlockCName = new TextBlock
            {
                Text = constellation.ConstellationName,
                Foreground = new SolidColorBrush(Color.FromArgb(100, 61, 202, 205)),
                TextAlignment = TextAlignment.Center,
                FontSize = 40
            };

            sumX = sumX/constellation.Star.Count;
            sumY = sumY/constellation.Star.Count;
            Canvas.SetLeft(textBlockCName, sumX);
            Canvas.SetTop(textBlockCName, sumY);
            StarsCanvas.Children.Add(textBlockCName);
        }

        private void DrawLinesBetweenStars(Constellation constellation)
        {
            var list = constellation.Star.ToList();
            for (int i = 0; i < list.Count - 1; i++)
            {
                var star0 = list[i];
                var star1 = list[i + 1];
                
                var line = new Line
                {
                    X1 = star0.x + _starCenterOffset.X,
                    Y1 = star0.y + _starCenterOffset.Y,
                    X2 = star1.x + _starCenterOffset.X,
                    Y2 = star1.y + _starCenterOffset.Y,
                    Stroke = Brushes.LightSteelBlue,
                    StrokeThickness = 2
                };
                StarsCanvas.Children.Add(line);
            }
        }

        #endregion

        #region MouseManipulations

        private void EnableMouseManipulations()
        {
            Canvas.SetLeft(GalaxyImage, 0);
            Canvas.SetTop(GalaxyImage, 0);

            Canvas.SetLeft(StarsCanvas, 0);
            Canvas.SetTop(StarsCanvas, 0);

            BackgroundCanvas.MouseLeftButtonDown += (sender, args) =>
            {
                _mousePos = args.GetPosition(this);
                GalaxyImage.CaptureMouse();
            };

            BackgroundCanvas.MouseWheel += (sender, args) =>
            {
                Scale(args, GalaxyImage);
                Scale(args, StarsCanvas);
            };

            BackgroundCanvas.MouseMove += (sender, args) =>
            {
                if (args.LeftButton == MouseButtonState.Pressed)
                {
                    Debug.WriteLine("IMAGE: " + Canvas.GetLeft(GalaxyImage) + " " + Canvas.GetTop(GalaxyImage)
                        + " STARS: " + Canvas.GetLeft(StarsCanvas) + " " + Canvas.GetTop(StarsCanvas));
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

        private void OnListItemClicked(object sender, MouseButtonEventArgs e)
        {
            Star clickedStar = ((SearchResult)((ListViewItem) sender).DataContext).Star;

            Canvas.SetLeft(GalaxyImage, BackgroundCanvas.ActualWidth / 2 - (clickedStar.x + _starCenterOffset.X)); //clickedStar.x - BackgroundCanvas.ActualWidth * 2.05);
            Canvas.SetTop(GalaxyImage, BackgroundCanvas.ActualHeight / 2 - (clickedStar.y + _starCenterOffset.Y)); //clickedStar.y - BackgroundCanvas.ActualHeight * 2.12);

            Canvas.SetLeft(StarsCanvas, BackgroundCanvas.ActualWidth / 2 - (clickedStar.x + _starCenterOffset.X)); //clickedStar.x - BackgroundCanvas.ActualWidth * 2.05);
            Canvas.SetTop(StarsCanvas, BackgroundCanvas.ActualHeight / 2 - (clickedStar.y + _starCenterOffset.Y)); //clickedStar.y - BackgroundCanvas.ActualHeight * 2.12);

            Debug.WriteLine("STAR: " + clickedStar.x + " " + clickedStar.y);
            Debug.WriteLine("IMAGE: " + Canvas.GetLeft(GalaxyImage) + " " + Canvas.GetTop(GalaxyImage) + " STARS_CANVAS: " + Canvas.GetLeft(StarsCanvas) + " " + Canvas.GetTop(StarsCanvas));

            //MoveTo(GalaxyImage, BackgroundCanvas.ActualWidth / 2 - (clickedStar.x + _starCenterOffset.X),
            //                    BackgroundCanvas.ActualHeight / 2 - (clickedStar.y + _starCenterOffset.Y));

            //MoveTo(StarsCanvas, BackgroundCanvas.ActualWidth / 2 - (clickedStar.x + _starCenterOffset.X),
            //                    BackgroundCanvas.ActualHeight / 2 - (clickedStar.y + _starCenterOffset.Y));

        }

        public static void MoveTo(FrameworkElement target, double newX, double newY)
        {
            var left = Canvas.GetLeft(target);
            var top = Canvas.GetTop(target);
            TranslateTransform trans = new TranslateTransform();
            var savedTransform = target.RenderTransform.Clone();
            target.RenderTransform = trans;
            DoubleAnimation anim1 = new DoubleAnimation(left, newX, TimeSpan.FromSeconds(0.5));
            DoubleAnimation anim2 = new DoubleAnimation(top, newY, TimeSpan.FromSeconds(0.5));
            anim1.EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut};
            anim2.EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut};
            Debug.WriteLine("new vals: " + newX + " " + newY);

            anim1.Completed += (sender, args) =>
            {
                target.RenderTransform = savedTransform;
                Canvas.SetLeft(target, newX);
            };

            anim2.Completed += (sender, args) =>
            {
                Canvas.SetTop(target, newY);
                Debug.WriteLine("IMAGE: " + Canvas.GetLeft(target) + " " + Canvas.GetTop(target));
            };

            trans.BeginAnimation(TranslateTransform.XProperty, anim1);
            trans.BeginAnimation(TranslateTransform.YProperty, anim2);
            
        }
    }
}
