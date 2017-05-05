//======================================================================
//
//        Copyright © 2016 VoiceCyber Technologies Ltd.
//        All rights reserved
//        guid1:                    f86b466f-7677-4046-83fd-d5ee5ba55d7a
//        CLR Version:              4.0.30319.42000
//        Name:                     Shell
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                UMPS2106
//        File Name:                Shell
//
//        Created by Charley at 2016/10/19 14:04:26
//        http://www.voicecyber.com 
//
//======================================================================

using System.Windows;

namespace UMPS2106
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
            PackageRecoverMainView view = new PackageRecoverMainView();
            view.CurrentApp = App.CurrentApp;
            BorderContent.Child = view;
        }
    }
}
