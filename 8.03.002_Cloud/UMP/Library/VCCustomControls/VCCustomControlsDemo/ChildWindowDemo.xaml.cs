//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    1ee9dcd4-675f-419c-ad78-ec745d047be3
//        CLR Version:              4.0.30319.18444
//        Name:                     ChildWindowDemo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VCCustomControlsDemo
//        File Name:                ChildWindowDemo
//
//        created by Charley at 2014/10/8 11:41:53
//        http://www.voicecyber.com 
//
//======================================================================

using System.Windows;
using VoiceCyber.Wpf.CustomControls;
using Window = System.Windows.Window;

namespace VCCustomControlsDemo
{
    /// <summary>
    /// ChildWindowDemo.xaml 的交互逻辑
    /// </summary>
    public partial class ChildWindowDemo : Window
    {
        public ChildWindowDemo()
        {
            InitializeComponent();

            Loaded += ChildWindowDemo_Loaded;
        }

        void ChildWindowDemo_Loaded(object sender, RoutedEventArgs e)
        {
            BtnTest.Click += BtnTest_Click;
        }

        void BtnTest_Click(object sender, RoutedEventArgs e)
        {
            ChildWindow childWindow = new ChildWindow();
            childWindow.Caption = "Child Window";
            childWindow.Width = 350;
            childWindow.Height = 250;
            childWindow.IsModal = true;
            childWindow.WindowStyle =WindowStyle.SingleBorderWindow;
            childWindow.WindowStartupLocation = VoiceCyber.Wpf.CustomControls.Windows.WindowStartupLocation.Center;
            childWindow.WindowState = VoiceCyber.Wpf.CustomControls.Windows.WindowState.Open;
            WindowContainer.Children.Clear();
            WindowContainer.Children.Add(childWindow);
        }
    }
}
