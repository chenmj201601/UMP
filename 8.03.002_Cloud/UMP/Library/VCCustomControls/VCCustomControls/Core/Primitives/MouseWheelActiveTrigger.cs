//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    6dba2ee9-56ab-40dd-bbaa-bdc7338e2445
//        CLR Version:              4.0.30319.18444
//        Name:                     MouseWheelActiveTrigger
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.CustomControls.Core.Primitives
//        File Name:                MouseWheelActiveTrigger
//
//        created by Charley at 2014/7/17 16:14:26
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoiceCyber.Wpf.CustomControls.Primitives
{
    /// <summary>
    /// Specify when the mouse wheel is active.
    /// </summary>
    public enum MouseWheelActiveTrigger
    {
        /// <summary>
        /// FocusedMouseOver
        /// </summary>
        FocusedMouseOver,
        /// <summary>
        /// MouseOver
        /// </summary>
        MouseOver,
        /// <summary>
        /// Disabled
        /// </summary>
        Disabled
    }
}
