//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    ce41d675-8a19-42a5-b6fb-66f11319471d
//        CLR Version:              4.0.30319.18444
//        Name:                     Theme
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.AvalonDock.Themes
//        File Name:                Theme
//
//        created by Charley at 2014/7/22 10:58:02
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace VoiceCyber.Wpf.AvalonDock.Themes
{
    public abstract class Theme : DependencyObject
    {
        public Theme()
        {

        }

        public abstract Uri GetResourceUri();


    }
}
