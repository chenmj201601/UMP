//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    be1d33db-8509-422b-9492-6bb05099d213
//        CLR Version:              4.0.30319.42000
//        Name:                     Shell
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS2102
//        File Name:                Shell
//
//        created by Charley at 2016/2/1 16:25:19
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UMPS2102
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
            ChanMonitorMainView view = new ChanMonitorMainView();
            view.CurrentApp = App.CurrentApp;
            BorderContent.Child = view;
        }
    }
}
