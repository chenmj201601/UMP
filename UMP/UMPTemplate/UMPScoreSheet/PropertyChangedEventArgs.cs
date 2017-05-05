//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    c81a1580-dd7d-46a1-89a9-023aae3846f6
//        CLR Version:              4.0.30319.18444
//        Name:                     PropertyChangedEventArgs
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets
//        File Name:                PropertyChangedEventArgs
//
//        created by Charley at 2014/6/13 16:35:59
//        http://www.voicecyber.com 
//
//======================================================================
using System;

namespace VoiceCyber.UMP.ScoreSheets
{
    /// <summary>
    /// 属性该表事件的参数
    /// </summary>
    public class PropertyChangedEventArgs : EventArgs
    {
        /// <summary>
        /// 评分对象
        /// </summary>
        public ScoreObject ScoreObject { get; set; }
        /// <summary>
        /// 属性名称
        /// </summary>
        public string PropertyName { get; set; }
        /// <summary>
        /// 原值
        /// </summary>
        public object OldValue { get; set; }
        /// <summary>
        /// 新值
        /// </summary>
        public object NewValue { get; set; }
    }
}
