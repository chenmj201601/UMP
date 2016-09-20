//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    b8ee3b2f-d578-4c75-88be-e0a71761ac5f
//        CLR Version:              4.0.30319.18444
//        Name:                     FilterInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.PropertyGrids
//        File Name:                FilterInfo
//
//        created by Charley at 2014/7/23 12:06:52
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceCyber.Wpf.PropertyGrids
{
    internal struct FilterInfo
    {
        public string InputString;
        public Predicate<object> Predicate;
    }
}
