//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    3098fb44-28e7-4c3a-95bd-b1564cee78d6
//        CLR Version:              4.0.30319.18444
//        Name:                     MessageState
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Communications
//        File Name:                MessageState
//
//        created by Charley at 2015/3/5 13:23:40
//        http://www.voicecyber.com 
//
//======================================================================
using System;

namespace VoiceCyber.UMP.Communications
{
    /// <summary>
    /// 数据包状态
    /// 由多个不同意义的位标识
    /// 可以按位组合
    /// </summary>
    [Flags]
    public enum MessageState
    {
        /// <summary>
        /// 未知
        /// </summary>
        Unkown = 0,
        /// <summary>
        /// 1bit：是否最后一个数据包
        /// </summary>
        LastPacket = 1
    }
}
