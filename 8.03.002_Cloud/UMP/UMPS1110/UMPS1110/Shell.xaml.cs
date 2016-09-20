//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    7f737cc9-0d53-445a-8b78-ba74c21cb05c
//        CLR Version:              4.0.30319.42000
//        Name:                     Shell
//        Computer:                 DESKTOP-VUMCK8M
//        Organization:             VoiceCyber
//        Namespace:                UMPS1110
//        File Name:                Shell
//
//        created by Charley at 2016/2/22 10:32:43
//        http://www.voicecyber.com 
//
//======================================================================

using System.Windows;
using System.Windows.Controls;

namespace UMPS1110
{
    /// <summary>
    /// Shell.xaml 的交互逻辑
    /// </summary>
    public partial class Shell : Page
    {
        public Shell()
        {
            InitializeComponent();

            Loaded += Shell_Loaded;
        }

        void Shell_Loaded(object sender, RoutedEventArgs e)
        {
            ResourceMainView view=new ResourceMainView();
            view.CurrentApp = App.CurrentApp;
            BorderContent.Child = view;
        }
    }
}
