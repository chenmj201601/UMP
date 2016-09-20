//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    b09b1ca7-de7b-41e4-aa4a-8d3b5a0c03c0
//        CLR Version:              4.0.30319.42000
//        Name:                     Shell
//        Computer:                 DESKTOP-VUMCK8M
//        Organization:             VoiceCyber
//        Namespace:                UMPS1206
//        File Name:                Shell
//
//        created by Charley at 2016/3/2 15:42:23
//        http://www.voicecyber.com 
//
//======================================================================

using System.Windows;

namespace UMPS1206
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
            DashboardMainView view=new DashboardMainView();
            view.CurrentApp = App.CurrentApp;
            BorderContent.Child = view;
        }
    }
}
