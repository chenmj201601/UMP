//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    566e4c3a-e67e-49e9-a836-06821ffcea1e
//        CLR Version:              4.0.30319.18063
//        Name:                     LogMode
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Common
//        File Name:                LogMode
//
//        created by Charley at 2014/3/22 17:56:09
//        http://www.voicecyber.com 
//
//======================================================================
using System;

namespace VoiceCyber.Common
{
    /// <summary>
    /// 日志级别,可任意组合
    /// </summary>
    [Flags]
    public enum LogMode
    {
        /// <summary>
        /// 调试
        /// </summary>
        Debug = 1,
        /// <summary>
        /// 信息
        /// </summary>
        Info = 2,
        /// <summary>
        /// 警告
        /// </summary>
        Warn = 4,
        /// <summary>
        /// 错误
        /// </summary>
        Error = 8,
        /// <summary>
        /// 致命
        /// </summary>
        Fatal = 16,
        /// <summary>
        /// 所有消息
        /// </summary>
        All = Debug | Info | Warn | Error | Fatal,
        /// <summary>
        /// 一般（默认）
        /// </summary>
        General = Info | Warn | Error | Fatal
    }
}
