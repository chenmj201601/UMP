//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    dffceea1-104e-42e4-a714-deff0e787460
//        CLR Version:              4.0.30319.18408
//        Name:                     Shell
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS4413
//        File Name:                Shell
//
//        created by Charley at 2016/6/6 16:08:03
//        http://www.voicecyber.com 
//
//======================================================================

using System.Windows;

namespace UMPS4413
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
            SeatManageMainView view = new SeatManageMainView();
            view.CurrentApp = App.CurrentApp;
            BorderContent.Child = view;
        }
    }
}
