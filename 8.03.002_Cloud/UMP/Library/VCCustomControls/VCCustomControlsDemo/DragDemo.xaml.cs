//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    2db3dd4c-f613-4a2b-90d6-efd38f01e74c
//        CLR Version:              4.0.30319.18444
//        Name:                     DragDemo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VCCustomControlsDemo
//        File Name:                DragDemo
//
//        created by Charley at 2014/5/15 14:28:45
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
using System.Windows.Shapes;
using VoiceCyber.Wpf.CustomControls;
using Window = System.Windows.Window;

namespace VCCustomControlsDemo
{
    /// <summary>
    /// DragDemo.xaml 的交互逻辑
    /// </summary>
    public partial class DragDemo : Window
    {
        private DragHelper mDragHelper;

        public DragDemo()
        {
            InitializeComponent();

            mDragHelper = new DragHelper();
        }

        private void DragDemo_OnLoaded(object sender, RoutedEventArgs e)
        {
            mDragHelper.Init(GridMain,MyDragElement);
        }
    }
}
