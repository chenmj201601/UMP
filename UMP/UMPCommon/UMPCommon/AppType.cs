//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    d5bd0a10-e3a9-4938-956a-1cc6c8a6ca13
//        CLR Version:              4.0.30319.18063
//        Name:                     AppType
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common
//        File Name:                AppType
//
//        created by Charley at 2015/10/10 14:00:36
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.Common
{
    /// <summary>
    /// 应用终端的类型
    /// </summary>
    public enum AppType
    {
        /// <summary>
        /// 未知类型
        /// </summary>
        Unkown = 0,
        /// <summary>
        /// UMP客户端（Wpf浏览器程序）
        /// </summary>
        UMPClient = 1,
        /// <summary>
        /// UMP智能客户端（Wpf独立程序）
        /// </summary>
        UMPIntelligent = 2,
        /// <summary>
        /// UMP Web 服务（Wcf服务）
        /// </summary>
        UMPWebService = 10,
        /// <summary>
        /// UMP Windows 服务
        /// </summary>
        UMPWinService = 20
    }
}
