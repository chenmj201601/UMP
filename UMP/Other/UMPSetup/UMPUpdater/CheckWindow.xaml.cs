//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    920c5b2e-e7f7-48c7-aa2c-06fe192b78c9
//        CLR Version:              4.0.30319.18408
//        Name:                     CheckWindow
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPUpdater
//        File Name:                CheckWindow
//
//        created by Charley at 2016/8/1 16:45:23
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Windows;

namespace UMPUpdater
{
    /// <summary>
    /// CheckWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CheckWindow
    {
        private bool mIsInited;

        public CheckWindow()
        {
            InitializeComponent();

            Loaded += CheckWindow_Loaded;
            MouseLeftButtonDown += (s, e) => DragMove();

            BtnAppClose.Click += BtnAppClose_Click;
        }

        void CheckWindow_Loaded(object sender, RoutedEventArgs e)
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

            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void BtnAppClose_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(string.Format("Confirm close UMPUpdater?"), App.AppName, MessageBoxButton.YesNo,
                MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes) { return; }
            Close();
        }

        private void ShowException(string msg)
        {
            MessageBox.Show(msg, App.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
