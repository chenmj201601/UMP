//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    3b08239c-5bf1-4077-8a12-a43f49089433
//        CLR Version:              4.0.30319.18444
//        Name:                     DateTimeInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.CustomControls.DateTimeUpDown.Implementation
//        File Name:                DateTimeInfo
//
//        created by Charley at 2014/7/17 16:24:12
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoiceCyber.Wpf.CustomControls
{
    internal class DateTimeInfo
    {
        public string Content
        {
            get;
            set;
        }
        public string Format
        {
            get;
            set;
        }
        public bool IsReadOnly
        {
            get;
            set;
        }
        public int Length
        {
            get;
            set;
        }
        public int StartPosition
        {
            get;
            set;
        }
        public DateTimePart Type
        {
            get;
            set;
        }
    }
}
