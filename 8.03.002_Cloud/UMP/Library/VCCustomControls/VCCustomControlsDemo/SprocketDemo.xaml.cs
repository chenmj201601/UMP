//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    0dcea954-b8c0-415f-9889-93f26e00cc95
//        CLR Version:              4.0.30319.18444
//        Name:                     SprocketDemo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VCCustomControlsDemo
//        File Name:                SprocketDemo
//
//        created by Charley at 2014/6/20 13:13:56
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace VCCustomControlsDemo
{
    /// <summary>
    /// SprocketDemo.xaml 的交互逻辑
    /// </summary>
    public partial class SprocketDemo
    {
        private Timer mTime1;
        private Timer mTime2;

        public SprocketDemo()
        {
            InitializeComponent();

            mTime1 = new Timer(60);
            mTime2 = new Timer(60);
        }

        private void SprocketDemo_OnLoaded(object sender, RoutedEventArgs e)
        {
            mTime1.Elapsed += mTime1_Elapsed;
            mTime2.Elapsed += mTime2_Elapsed;
        }

        void mTime2_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                Sprocket4.Progress++;

                if (Sprocket4.Progress >= 100)
                {
                    mTime2.Enabled = false;
                    Btn4.IsEnabled = true;
                }
            }));
        }

        void mTime1_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                Sprocket3.Progress++;

                if (Sprocket3.Progress >= 100)
                {
                    mTime1.Enabled = false;
                    Btn3.IsEnabled = true;
                }
            }));
        }

        private void Btn3_OnClick(object sender, RoutedEventArgs e)
        {
            Btn3.IsEnabled = false;
            Sprocket3.Progress = 0;
            mTime1.Enabled = true;
        }

        private void Btn4_OnClick(object sender, RoutedEventArgs e)
        {
            Btn4.IsEnabled = false;
            Sprocket4.Progress = 0;
            mTime2.Enabled = true;
        }
    }
}
