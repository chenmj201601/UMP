//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    1a548ba1-73fe-4448-861b-b99639d4a5f9
//        CLR Version:              4.0.30319.18408
//        Name:                     LoginState
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.CommonService10
//        File Name:                LoginState
//
//        created by Charley at 2016/6/27 17:17:00
//        http://www.voicecyber.com 
//
//======================================================================

using System;

namespace VoiceCyber.UMP.CommonService10
{
    /// <summary>
    /// 坐席登录状态
    /// 按位依次，CTI，Voice，Screen
    /// </summary>
    [Flags]
    public enum LoginState
    {
        /// <summary>
        /// 无
        /// </summary>
        None = 0,
        /// <summary>
        /// 仅登录CTI
        /// </summary>
        CTI = 1,
        /// <summary>
        /// 仅登录录音服务器
        /// </summary>
        Voice = 2,
        /// <summary>
        /// 仅登录录屏服务器
        /// </summary>
        Screen = 4,
        /// <summary>
        /// 在录音录屏服务器同时登录
        /// </summary>
        VoiceScreen = 6,
        /// <summary>
        /// 在CTI及录音录屏上都登录
        /// </summary>
        All = 7,
    }
}
