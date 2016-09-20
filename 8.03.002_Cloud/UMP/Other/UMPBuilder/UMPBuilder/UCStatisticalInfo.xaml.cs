//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    f45cdb70-5adc-4d8f-94ce-60934fcf4827
//        CLR Version:              4.0.30319.18063
//        Name:                     UCStatisticalInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPBuilder
//        File Name:                UCStatisticalInfo
//
//        created by Charley at 2015/12/24 18:19:32
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Windows;
using UMPBuilder.Models;

namespace UMPBuilder
{
    /// <summary>
    /// UCStatisticalInfo.xaml 的交互逻辑
    /// </summary>
    public partial class UCStatisticalInfo
    {

        public MainWindow PageParent;
        public StatisticalItem StatistialItem;


        public UCStatisticalInfo()
        {
            InitializeComponent();

            Loaded += UCStatisticalInfo_Loaded;
        }

        void UCStatisticalInfo_Loaded(object sender, RoutedEventArgs e)
        {
            TableStatistical.DataContext = StatistialItem;  
            Init();
        }

        private void Init()
        {
            try
            {

            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void ShowErrorMessage(string msg)
        {
            if (PageParent != null)
            {
                PageParent.ShowErrorMessage(msg);
            }
        }
    }
}
