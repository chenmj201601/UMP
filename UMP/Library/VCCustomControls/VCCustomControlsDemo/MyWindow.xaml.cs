//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    40f53b2e-666d-4e4c-b694-96f7cbc6d331
//        CLR Version:              4.0.30319.18063
//        Name:                     MyWindow
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VCCustomControlsDemo
//        File Name:                MyWindow
//
//        created by Charley at 2014/4/1 16:36:24
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using VoiceCyber.Wpf.CustomControls;

namespace VCCustomControlsDemo
{
    /// <summary>
    /// MyWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MyWindow
    {
        private DragHelper mDragHelper;

        public MyWindow()
        {
            InitializeComponent();
            mDragHelper = new DragHelper();
        }

        private void MyWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            mDragHelper.Init(GridContainer, UCDragPanel);
        }
    }
}
