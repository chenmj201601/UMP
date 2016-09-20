//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    af7b8b67-c118-4ede-ad6c-40cb17429523
//        CLR Version:              4.0.30319.42000
//        Name:                     Shell
//        Computer:                 DESKTOP-VUMCK8M
//        Organization:             VoiceCyber
//        Namespace:                UMPS3101
//        File Name:                Shell
//
//        created by Charley at 2016/2/23 16:20:14
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Linq;
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
using VoiceCyber.UMP.Controls;

namespace UMPS3101
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
            SSMMainView view = new SSMMainView();
            view.CurrentApp = App.CurrentApp;
            BorderContent.Child = view;
        }

        public void SetView(UMPMainView view)
        {
            BorderContent.Child = view;
        }
     
    }
}
