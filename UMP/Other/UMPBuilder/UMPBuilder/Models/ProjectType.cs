//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    7372e94f-acd9-4557-8e5c-41d63946a3ed
//        CLR Version:              4.0.30319.18063
//        Name:                     ProjectType
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPBuilder.Models
//        File Name:                ProjectType
//
//        created by Charley at 2015/12/21 14:18:39
//        http://www.voicecyber.com 
//
//======================================================================

namespace UMPBuilder.Models
{
    /// <summary>
    /// 项目类型
    /// </summary>
    public enum ProjectType
    {
        /// <summary>
        /// 未知
        /// </summary>
        Unkown = 0,
        /// <summary>
        /// Dll
        /// </summary>
        Library = 1,
        /// <summary>
        /// Wcf服务
        /// </summary>
        WcfService = 2,
        /// <summary>
        /// Windows服务
        /// </summary>
        WinService = 3,
        /// <summary>
        /// Wpf独立应用程序
        /// </summary>
        WpfApp = 4,
        /// <summary>
        /// Wpf浏览器应用程序
        /// </summary>
        WpfBrowser = 5
    }
}
