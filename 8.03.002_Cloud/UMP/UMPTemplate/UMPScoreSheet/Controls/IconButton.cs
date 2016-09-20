//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    a7e0cb7d-58a8-4f45-83bf-b3f95d9cd273
//        CLR Version:              4.0.30319.18444
//        Name:                     IconButton
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets.Controls
//        File Name:                IconButton
//
//        created by Charley at 2014/8/5 12:01:59
//        http://www.voicecyber.com 
//
//======================================================================

using System.Windows;
using System.Windows.Controls;

namespace VoiceCyber.UMP.ScoreSheets.Controls
{
    public class IconButton : Button
    {
       
        public static readonly DependencyProperty DisplayProperty =
            DependencyProperty.Register("Display", typeof(string), typeof(IconButton), new PropertyMetadata(default(string)));

        public string Display
        {
            get { return (string)GetValue(DisplayProperty); }
            set { SetValue(DisplayProperty, value); }
        }

        public static readonly DependencyProperty IconPathProperty =
            DependencyProperty.Register("IconPath", typeof (string), typeof (IconButton), new PropertyMetadata(default(string)));

        public string IconPath
        {
            get { return (string) GetValue(IconPathProperty); }
            set { SetValue(IconPathProperty, value); }
        }

        static IconButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (IconButton),
                new FrameworkPropertyMetadata(typeof (IconButton)));
            
        }
    }
}
