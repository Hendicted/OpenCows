using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace PictureSlideshowScreensaver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Screensaver : Window
    {
        private string _path = @"C:\windows\cows";
        private double _updateInterval = 5; // seconds
        private int _fadeSpeed = 2000;      // milliseconds

        private List<string> _images;
        private IEnumerator<string> _imageEnum;
        private DispatcherTimer _switchImage;
        private Point _mouseLocation = new Point(0, 0);

        private System.Drawing.Rectangle _bounds;

        public Screensaver(System.Drawing.Rectangle bounds)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\PictureSlideshowScreensaver");
            if (key != null)
            {
                
                _updateInterval = double.Parse((string)key.GetValue("Interval"));
            }

            InitializeComponent();

            _bounds = bounds;

            _images = new List<string>();
            _switchImage = new DispatcherTimer();
            _switchImage.Interval = TimeSpan.FromSeconds(_updateInterval);
            _switchImage.Tick += new EventHandler(_fade_Tick);

        }

        void _fade_Tick(object sender, EventArgs e)
        {
            NextImage();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Maximize window
            this.WindowState = System.Windows.WindowState.Maximized;
//#if DEBUG
//            this.Topmost = false;
//#endif

            // Load images
            if (_path != null)
            {
                if (Directory.Exists(_path))
                {
                    foreach (string s in Directory.GetFiles(_path))
                    {
                        if (s.EndsWith(".jpg") | s.EndsWith(".png"))
                        {
                            _images.Add(s);
                        }
                    }
                    _images = RandomizeGenericList(_images);

                    if (_images.Count > 0)
                    {
                        _imageEnum = _images.GetEnumerator();
                        NextImage();
                        _switchImage.Start();
                    }
                }
                else
                {
                    lblScreen.Content = "Image folder does not exist! Please run configuration.";
                }
            }
            else
            {
                lblScreen.Content = "Image folder not set! Please run configuration.";
            }
        }

        private void bNext_Click(object sender, RoutedEventArgs e)
        {
            NextImage();
        }

        private void NextImage()
        {
            if (_imageEnum.MoveNext())
            {
                try
                {
                    FadeToImage(new BitmapImage(new Uri(_imageEnum.Current)));
                    return;
                }
                catch (Exception ex)
                {
                    //MessageBox.Show("ERROR: " + ex.Message);
                    _imageEnum.MoveNext();
                    return;
                }
            }

            _images = RandomizeGenericList(_images);
            _imageEnum = _images.GetEnumerator();
        }

        private void FadeToImage(BitmapImage img)
        {
            DoubleAnimation da1;
            DoubleAnimation da2;
            if (img1.Opacity == 0)
            {
                img1.Source = img;

                da1 = new DoubleAnimation(1, TimeSpan.FromMilliseconds(_fadeSpeed));
                da2 = new DoubleAnimation(0, TimeSpan.FromMilliseconds(_fadeSpeed));

                img1.BeginAnimation(Image.OpacityProperty, da1);
                img2.BeginAnimation(Image.OpacityProperty, da2);
            }
            else if (img2.Opacity == 0)
            {
                img2.Source = img;

                da1 = new DoubleAnimation(0, TimeSpan.FromMilliseconds(_fadeSpeed));
                da2 = new DoubleAnimation(1, TimeSpan.FromMilliseconds(_fadeSpeed));

                img1.BeginAnimation(Image.OpacityProperty, da1);
                img2.BeginAnimation(Image.OpacityProperty, da2);
            }
        }

        public static List<T> RandomizeGenericList<T>(IList<T> originalList)
        {
            List<T> randomList = new List<T>();
            Random random = new Random();
            T value = default(T);

            //now loop through all the values in the list
            while (originalList.Count() > 0)
            {
                //pick a random item from th original list
                var nextIndex = random.Next(0, originalList.Count());
                //get the value for that random index
                value = originalList[nextIndex];
                //add item to the new randomized list
                randomList.Add(value);
                //remove value from original list (prevents
                //getting duplicates
                originalList.RemoveAt(nextIndex);
            }

            //return the randomized list
            return randomList;
        }

        private void bExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void lblScreen_MouseMove(object sender, MouseEventArgs e)
        {
            Point newPos = e.GetPosition(this);
            System.Drawing.Point p = new System.Drawing.Point((int)newPos.X, (int)newPos.Y);
            if ((_mouseLocation.X != 0 & _mouseLocation.Y != 0) & ((p.X >= 0 & p.X <= _bounds.Width) & (p.Y >= 0 & p.Y <= _bounds.Height)))
            {
                if (Math.Abs(_mouseLocation.X - newPos.X) > 10 || Math.Abs(_mouseLocation.Y - newPos.Y) > 10)
                {
                    Application.Current.Shutdown();
                }
            }

            _mouseLocation = newPos;
        }

        private void lblScreen_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
