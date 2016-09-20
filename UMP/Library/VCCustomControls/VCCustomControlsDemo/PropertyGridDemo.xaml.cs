//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    5b0e126a-05ca-4eee-9329-18ef1e3359fc
//        CLR Version:              4.0.30319.18444
//        Name:                     PropertyGridDemo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VCCustomControlsDemo
//        File Name:                PropertyGridDemo
//
//        created by Charley at 2014/7/21 11:35:03
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

namespace VCCustomControlsDemo
{
    /// <summary>
    /// PropertyGridDemo.xaml 的交互逻辑
    /// </summary>
    public partial class PropertyGridDemo : Window
    {
        public PropertyGridDemo()
        {
            InitializeComponent();
        }

        private void PropertyGridDemo_OnLoaded(object sender, RoutedEventArgs e)
        {
            TestObject test = new TestObject();
            test.Name = "jjj";
            test.Display = "kkk";
            test.BirthDay = DateTime.Now;
            PgDemo.SelectedObject = test;
            //PgDemo.SelectedObject = BtnDemo;
        }
    }

    public class TestObject
    {
        public string Name { get; set; }
        public string Display { get; set; }
        public DateTime BirthDay { get; set; }
    }
}
