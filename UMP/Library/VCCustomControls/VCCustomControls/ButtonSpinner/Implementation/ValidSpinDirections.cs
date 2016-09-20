//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    1c9fb1fb-4b59-43c8-9357-70c8ddcf14cd
//        CLR Version:              4.0.30319.18444
//        Name:                     ValidSpinDirections
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.CustomControls.ButtonSpinner.Implementation
//        File Name:                ValidSpinDirections
//
//        created by Charley at 2014/7/17 16:17:47
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoiceCyber.Wpf.CustomControls
{
    /// <summary>
    /// Represents spin directions that are valid.
    /// </summary>
    [Flags]
    public enum ValidSpinDirections
    {
        /// <summary>
        /// Can not increase nor decrease.
        /// </summary>
        None = 0,

        /// <summary>
        /// Can increase.
        /// </summary>
        Increase = 1,

        /// <summary>
        /// Can decrease.
        /// </summary>
        Decrease = 2
    }
}
