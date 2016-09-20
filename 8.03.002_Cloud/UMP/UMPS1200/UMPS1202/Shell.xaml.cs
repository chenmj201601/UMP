//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    dbb46e8c-d037-4a87-9526-27453be4deac
//        CLR Version:              4.0.30319.42000
//        Name:                     Shell
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1202
//        File Name:                Shell
//
//        created by Charley at 2016/1/22 10:59:09
//        http://www.voicecyber.com 
//
//======================================================================

using System.Windows;

namespace UMPS1202
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
            LoginView view=new LoginView();
            view.CurrentApp = App.CurrentApp;
            BorderContent.Child = view;
        }
    }
}
