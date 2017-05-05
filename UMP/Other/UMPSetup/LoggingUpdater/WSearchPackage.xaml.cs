//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    2734ef68-e769-4716-9838-c7f7be00f21e
//        CLR Version:              4.0.30319.18408
//        Name:                     WSearchPackage
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                LoggingUpdater
//        File Name:                WSearchPackage
//
//        created by Charley at 2016/6/7 17:40:26
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
using System.Windows.Shapes;
using Microsoft.Win32;

namespace LoggingUpdater
{
    /// <summary>
    /// WSearchPackage.xaml 的交互逻辑
    /// </summary>
    public partial class WSearchPackage : Window
    {

        public MainWindow PageParent;
        private bool mIsInited;

        public WSearchPackage()
        {
            InitializeComponent();

            Loaded += WSearchPackage_Loaded;
            RadioSearch.Click += RadioSearch_Click;
            RadioSelect.Click += RadioSelect_Click;
            BtnBrowser.Click += BtnBrowser_Click;
            BtnConfirm.Click += BtnConfirm_Click;
            BtnClose.Click += BtnClose_Click;
        }


        void WSearchPackage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!mIsInited)
            {
                Init();
                mIsInited = true;
            }
        }

        private void Init()
        {
            try
            {
                RadioSearch.IsChecked = true;
                RadioSelect.IsChecked = false;
                GridDataPath.IsEnabled = false;
                TxtDataPath.Text = string.Empty;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }


        #region EventHandlers

        void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {

        }

        void BtnBrowser_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "UpdateData(*.zip)|*.zip";
            var result = dialog.ShowDialog();
            if (result != true) { return; }
            TxtDataPath.Text = dialog.FileName;
        }

        void RadioSelect_Click(object sender, RoutedEventArgs e)
        {
            GridDataPath.IsEnabled = RadioSelect.IsChecked == true;
        }

        void RadioSearch_Click(object sender, RoutedEventArgs e)
        {
            GridDataPath.IsEnabled = RadioSelect.IsChecked == true;
        }

        #endregion


        private void ShowException(string msg)
        {
            MessageBox.Show(msg, App.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
