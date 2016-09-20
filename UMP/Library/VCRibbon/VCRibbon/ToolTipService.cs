//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    9ef79f5b-9b7d-431d-a879-f15dfaf54973
//        CLR Version:              4.0.30319.18444
//        Name:                     ToolTipService
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Fluent
//        File Name:                ToolTipService
//
//        created by Charley at 2014/5/27 17:27:00
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Windows;

namespace VoiceCyber.Ribbon
{
    /// <summary>
    /// Represents additional toltip functionality
    /// </summary>
    public static class ToolTipService
    {
        /// <summary>
        /// Attach ooltip properties to control
        /// </summary>
        /// <param name="type">Control type</param>
        public static void Attach(Type type)
        {
            System.Windows.Controls.ToolTipService.ShowOnDisabledProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(true));
            System.Windows.Controls.ToolTipService.InitialShowDelayProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(900));
            System.Windows.Controls.ToolTipService.BetweenShowDelayProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(0));
            System.Windows.Controls.ToolTipService.ShowDurationProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(20000));
        }
    }
}
