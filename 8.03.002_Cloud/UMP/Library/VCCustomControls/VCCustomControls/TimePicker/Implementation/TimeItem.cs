//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    c29aee95-cec8-4963-b014-855130063af8
//        CLR Version:              4.0.30319.18444
//        Name:                     TimeItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.CustomControls.TimePicker.Implementation
//        File Name:                TimeItem
//
//        created by Charley at 2014/7/17 16:42:51
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoiceCyber.Wpf.CustomControls
{
    public class TimeItem
    {
        public string Display
        {
            get;
            set;
        }
        public TimeSpan Time
        {
            get;
            set;
        }

        public TimeItem(string display, TimeSpan time)
        {
            Display = display;
            Time = time;
        }

        #region Base Class Overrides

        public override bool Equals(object obj)
        {
            var item = obj as TimeItem;
            if (item != null)
                return Time == item.Time;
            else
                return false;
        }

        public override int GetHashCode()
        {
            return Time.GetHashCode();
        }

        #endregion //Base Class Overrides
    }
}
