//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    0f11e02a-8fbe-4b4d-b2d1-2ad378a750b0
//        CLR Version:              4.0.30319.18408
//        Name:                     LoginTerminal
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common
//        File Name:                LoginTerminal
//
//        created by Charley at 2016/8/9 10:13:45
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.Common
{
    /// <summary>
    /// 登录终端
    /// </summary>
    public enum LoginTerminal
    {
        /// <summary>
        /// 未知
        /// </summary>
        Unkown = 0,
        /// <summary>
        /// UMP客户端
        /// </summary>
        UMPClient = 1,
        /// <summary>
        /// CQC客户端
        /// </summary>
        CQCClient = 11,
        /// <summary>
        /// 许可管理端
        /// </summary>
        LicenseMgr = 12,
        /// <summary>
        /// 告警客户端
        /// </summary>
        AlarmClient = 13,
    }
}
