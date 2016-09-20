//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    31b3c1ed-bd4c-469e-b2ac-63ed596977fe
//        CLR Version:              4.0.30319.18444
//        Name:                     IconButtonItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets.Models
//        File Name:                IconButtonItem
//
//        created by Charley at 2014/6/27 13:59:00
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceCyber.UMP.ScoreSheets.Models
{
    /// <summary>
    /// 带图标的按钮
    /// </summary>
    public class IconButtonItem
    {
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Display
        /// </summary>
        public string Display { get; set; }
        /// <summary>
        /// ToolTip
        /// </summary>
        public string ToolTip { get; set; }
        /// <summary>
        /// Icon
        /// </summary>
        public string Icon { get; set; }
    }
}
