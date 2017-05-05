//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    196f71bf-b388-4062-ad30-27a6154d70d0
//        CLR Version:              4.0.30319.18408
//        Name:                     Shell
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS4411
//        File Name:                Shell
//
//        created by Charley at 2016/6/17 09:27:47
//        http://www.voicecyber.com 
//
//======================================================================

using System.Windows;

namespace UMPS4411
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
            OnsiteMonitorMainView view = new OnsiteMonitorMainView();
            view.CurrentApp = App.CurrentApp;
            BorderContent.Child = view;
        }
    }
}
