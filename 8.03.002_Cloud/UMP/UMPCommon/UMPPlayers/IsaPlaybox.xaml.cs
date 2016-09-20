//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    233f98e2-a708-4725-a9fa-afaacee240cf
//        CLR Version:              4.0.30319.18408
//        Name:                     IsaPlaybox
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Controls.Players
//        File Name:                IsaPlaybox
//
//        created by Charley at 2016/7/28 11:06:37
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VoiceCyber.Common;

namespace VoiceCyber.UMP.Controls.Players
{
    /// <summary>
    /// IsaPlaybox.xaml 的交互逻辑
    /// </summary>
    public partial class IsaPlaybox
    {
        public event EventHandler<UMPEventArgs> PlayboxEvent;
        public int ScreenScale;

        public IsaPlaybox()
        {
            InitializeComponent();

            Loaded += IsaPlaybox_Loaded;
            Closing += IsaPlaybox_Closing;
        }

        void IsaPlaybox_Loaded(object sender, RoutedEventArgs e)
        {
            UMPEventArgs args = new UMPEventArgs();
            args.Code = Defines.EVT_PAGE_LOADED;
            args.Data = ImgVedio;
            OnPlayboxEvent(args);
            //SetScreenScale();
        }

        void IsaPlaybox_Closing(object sender, CancelEventArgs e)
        {
            UMPEventArgs args = new UMPEventArgs();
            args.Code = Defines.EVT_PAGE_UNLOADED;
            args.Data = ImgVedio;
            OnPlayboxEvent(args);
        }

        private void SetScreenScale()
        {
            try
            {
                double scale = 1.0;
                if (ScreenScale > 0 && ScreenScale < 100)
                {
                    scale = ScreenScale / 100.0;
                }
                ScaleTransform tran = new ScaleTransform();
                tran.ScaleX = scale;
                tran.ScaleY = scale;
                GridVedio.LayoutTransform = tran;
            }
            catch { }
        }

        public void SetImgSource(string path)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                try
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);
                    bitmap.EndInit();
                    ImgVedio.Source = bitmap;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "IsaPlaybox", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }));
        }

        private void OnPlayboxEvent(UMPEventArgs e)
        {
            if (PlayboxEvent != null)
            {
                PlayboxEvent(this, e);
            }
        }
    }
}
