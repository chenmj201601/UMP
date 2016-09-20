//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    b866dc59-65e6-43a7-8371-e5f99caed8ee
//        CLR Version:              4.0.30319.18444
//        Name:                     UCStandardItemViewer
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPTemplateDesigner
//        File Name:                UCStandardItemViewer
//
//        created by Charley at 2014/6/17 10:46:03
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
using VoiceCyber.UMP.ScoreSheets;

namespace UMPTemplateDesigner
{
    /// <summary>
    /// UCStandardItemViewer.xaml 的交互逻辑
    /// </summary>
    public partial class UCStandardItemViewer
    {
        public StandardItem StandardItem;

        public UCStandardItemViewer()
        {
            InitializeComponent();
        }

        private void UCStandardItemViewer_OnLoaded(object sender, RoutedEventArgs e)
        {
            DataContext = StandardItem;

            Init();
        }

        public void Init()
        {
            
        }
    }
}
