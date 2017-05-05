using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using UMP.Tools.PublicClasses;

namespace UMP.Tools.BasicModule
{
    public partial class WaitForApplicationDoing : WindowsAClass
    {
        private Timer TimerChangeImageOpacity = new Timer(100);
        private delegate void ChangeImageOpactiyDelegage();

        public bool IBoolIsDoing = false;

        public WaitForApplicationDoing()
        {
            InitializeComponent();
            TimerChangeImageOpacity.Elapsed += TimerChangeImageOpacity_Elapsed;
            this.Closing += WaitForApplicationDoing_Closing;
        }

        public void StatusBarShowLeftStart(int AIntType, string AStrLeftText)
        {
            ChangeImageSource(AIntType);
            LabelStatus.Content = AStrLeftText;
            if (AIntType != int.MaxValue) { TimerChangeImageOpacity.Start(); }
        }

        public void StatusBarShowStop(int AIntType)
        {
            ChangeImageSource(AIntType);
            TimerChangeImageOpacity.Stop();
            ImageStatus.Opacity = 1.0;
            LabelStatus.Content = "";

        }

        public void CloseThisWindow()
        {
            this.Close();
        }

        private void WaitForApplicationDoing_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (IBoolIsDoing) { e.Cancel = true; return; }
        }

        private void TimerChangeImageOpacity_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal, new ChangeImageOpactiyDelegage(ChangeImageOpactiy));
        }

        private void ChangeImageOpactiy()
        {
            if (ImageStatus.Opacity > 0.4) { ImageStatus.Opacity = ImageStatus.Opacity - 0.1; } else { ImageStatus.Opacity = 1.0; }
        }

        private void ChangeImageSource(int AIntType)
        {
            try
            {
                if (AIntType == 0)
                {
                    ImageStatus.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000003.ico"), UriKind.RelativeOrAbsolute));
                }
                else if (AIntType == 1)
                {
                    ImageStatus.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000002.ico"), UriKind.RelativeOrAbsolute));
                }
                else if (AIntType == 2)
                {
                    ImageStatus.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000005.ico"), UriKind.RelativeOrAbsolute));
                }
                else
                {
                    ImageStatus.Height = 16; ImageStatus.Width = 16;
                    ImageStatus.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000001.ico"), UriKind.RelativeOrAbsolute));
                }
            }
            catch { }
        }
    }
}
