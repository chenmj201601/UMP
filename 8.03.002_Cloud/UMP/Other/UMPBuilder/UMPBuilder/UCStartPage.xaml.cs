//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    d8728e39-e732-487a-b7b8-75b180f4cd4f
//        CLR Version:              4.0.30319.18063
//        Name:                     UCStartPage
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPBuilder
//        File Name:                UCStartPage
//
//        created by Charley at 2015/12/22 9:47:22
//        http://www.voicecyber.com 
//
//======================================================================

using System.Windows;
using UMPBuilder.Models;

namespace UMPBuilder
{
    /// <summary>
    /// UCStartPage.xaml 的交互逻辑
    /// </summary>
    public partial class UCStartPage
    {
        public SystemConfig SystemConfig;
        public MainWindow PageParent;

        public UCStartPage()
        {
            InitializeComponent();

            Loaded += UCStartPage_Loaded;
            LinkSettings.Click += LinkSettings_Click;
        }

        void UCStartPage_Loaded(object sender, RoutedEventArgs e)
        {
            ListSettings.DataContext = SystemConfig;
        }

        void LinkSettings_Click(object sender, RoutedEventArgs e)
        {
            if (PageParent != null)
            {
                PageParent.ShowSettingsWindow();
            }
        }
    }
}
