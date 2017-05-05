//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    0988fc41-1587-4344-a86e-e0b3b6bf16f8
//        CLR Version:              4.0.30319.42000
//        Name:                     Shell
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1203
//        File Name:                Shell
//
//        created by Charley at 2016/1/22 12:05:54
//        http://www.voicecyber.com 
//
//======================================================================

using System.Windows;
using System.Windows.Controls;

namespace UMPS1203
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
            PageHeadNewView view = new PageHeadNewView();
            view.CurrentApp = App.CurrentApp;
            BorderContent.Child = view;
        }
    }
}
