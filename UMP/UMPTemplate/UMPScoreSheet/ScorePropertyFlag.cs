//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    d1019837-a0b1-4fb6-b1c7-561f2653ef05
//        CLR Version:              4.0.30319.18444
//        Name:                     ScorePropertyFlag
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets
//        File Name:                ScorePropertyFlag
//
//        created by Charley at 2014/7/13 17:01:48
//        http://www.voicecyber.com 
//
//======================================================================
using System;

namespace VoiceCyber.UMP.ScoreSheets
{
    /// <summary>
    /// 属性标志
    /// </summary>
    [Flags]
    public enum ScorePropertyFlag
    {
        /// <summary>
        /// 默认（无标志）
        /// </summary>
        Default = 0,
        /// <summary>
        /// 是否在属性窗格中显示
        /// </summary>
        Visible = 1,
        /// <summary>
        /// 属性窗格中是否可编辑
        /// </summary>
        Enable = 2,
        /// <summary>
        /// 是否在xml文件显示
        /// </summary>
        Xml = 4,
        /// <summary>
        /// 是否为对象Copy的属性
        /// </summary>
        Copy = 8,
        /// <summary>
        /// 所有标志
        /// </summary>
        All = Visible | Enable | Xml | Copy,
        /// <summary>
        /// 普通（Enable|Xml|Copy）
        /// </summary>
        Normal = Xml | Copy
    }
}
