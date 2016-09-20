//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    ad9acfb8-c3f4-47da-9916-a92d64085da2
//        CLR Version:              4.0.30319.18408
//        Name:                     Shell
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS4412
//        File Name:                Shell
//
//        created by Charley at 2016/5/10 11:14:05
//        http://www.voicecyber.com 
//
//======================================================================

using System.Windows;

namespace UMPS4412
{
    /// <summary>
    /// Shell.xaml 的交互逻辑
    /// </summary>
    public partial class Shell
    {
        public Shell()
        {
            InitializeComponent();

            Loaded += Shell_Loaded;
        }

        void Shell_Loaded(object sender, RoutedEventArgs e)
        {
            RegionManageMainView view = new RegionManageMainView();
            view.CurrentApp = App.CurrentApp;
            BorderContent.Child = view;
        }
    }
}
