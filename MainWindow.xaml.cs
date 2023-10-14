using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MediaPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(0.2);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            if (Player.Source != null)
            {
                if (Player.NaturalDuration.HasTimeSpan)
                {
                    lblStatus.Content = String.Format("{0} / {1}", Player.Position.ToString(@"mm\:ss"), Player.NaturalDuration.TimeSpan.ToString(@"mm\:ss"));
                    SeekSlider.ValueChanged -= SeekSlider_ValueChanged;
                    SeekSlider.Value = Player.Position.TotalSeconds;
                    SeekSlider.ValueChanged += SeekSlider_ValueChanged;
                }
            }
            else
                lblStatus.Content = "No file selected...";
        }

        private void Play()
        {
            if (Player.NaturalDuration.HasTimeSpan && Player.Position.TotalSeconds == Player.NaturalDuration.TimeSpan.TotalSeconds)
            {
                // Restart the video if it has ended
                Player.Position = TimeSpan.Zero;
            }
            Player.Play();
        }

        private void BtnPlay_Click(object sender, RoutedEventArgs e)
        {
            Play();
        }

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            Player.Pause();
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            Player.Stop();
        }

        private void Player_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            MessageBox.Show("Media failed: " + e.ErrorException.Message);
        }

        private void SeekSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Player.Position = TimeSpan.FromSeconds(SeekSlider.Value);
        }

        private void Player_MediaOpened(object sender, RoutedEventArgs e)
        {
            SeekSlider.Maximum = Math.Round(Player.NaturalDuration.TimeSpan.TotalSeconds, 0, MidpointRounding.ToNegativeInfinity);
        }

        private String? ChooseVideo()
        {
            OpenFileDialog openFileDialog = new()
            {
                Filter = "Video Files|*.mp4;*.avi;*.wmv;*.mkv|All Files|*.*",
                Title = "Select a Video File"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                return openFileDialog.FileName;
            }

            return null;
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            String? video = ChooseVideo();
            if (video != null) {
                Player.Source = new System.Uri(video);
                Play();
            }
        }

        private void prevButton_Click(object sender, RoutedEventArgs e)
        {
            Player.Position = Player.Position.Subtract(TimeSpan.FromMilliseconds(100));
        }

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            Player.Position = Player.Position.Add(TimeSpan.FromMilliseconds(100));
        }
    }
}
