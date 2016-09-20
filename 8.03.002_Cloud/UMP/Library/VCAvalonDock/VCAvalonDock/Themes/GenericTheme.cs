//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    3736d5d5-3866-4e12-9494-e217feabffb1
//        CLR Version:              4.0.30319.18444
//        Name:                     GenericTheme
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.AvalonDock.Themes
//        File Name:                GenericTheme
//
//        created by Charley at 2014/7/22 10:58:28
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceCyber.Wpf.AvalonDock.Themes
{
    public class GenericTheme : Theme
    {
        public override Uri GetResourceUri()
        {
            return new Uri(
                "/VCAvalonDock;component/Themes/generic.xaml",
                UriKind.Relative);
        }
    }
}
