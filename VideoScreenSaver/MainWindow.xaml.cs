using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace VideoScreenSaver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Point lastMouseLocation = default;
        DispatcherTimer timer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();

            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (o, e) => { txtTime.Text = DateTime.Now.ToString("dddd, dd MMMM yyyy hh:mm:ss tt"); };
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            Point point = e.GetPosition(this);
            if (lastMouseLocation != default)
            {
                if (Math.Abs(lastMouseLocation.X - point.X) > 5 || Math.Abs(lastMouseLocation.Y - point.Y) > 5)
                    Application.Current.Shutdown();
            }
            lastMouseLocation = point;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void player_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            timer.Start();
            (sender as FrameworkElement).Visibility = Visibility.Collapsed;
        }

        private void player_MediaEnded(object sender, RoutedEventArgs e)
        {
            (sender as MediaElement).Position = new TimeSpan(0, 0, 1);
            (sender as MediaElement).Play();
        }
    }
}
