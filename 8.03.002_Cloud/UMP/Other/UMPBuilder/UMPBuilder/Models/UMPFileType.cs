//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    5c4c87fd-9cd9-4032-be7d-3efb222b7fe7
//        CLR Version:              4.0.30319.18063
//        Name:                     UMPFileType
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPBuilder.Models
//        File Name:                UMPFileType
//
//        created by Charley at 2015/12/21 14:26:53
//        http://www.voicecyber.com 
//
//======================================================================

namespace UMPBuilder.Models
{
    /// <summary>
    /// 文件类型
    /// </summary>
    public enum UMPFileType
    {
        /// <summary>
        /// 未知
        /// </summary>
        Unkown = 0,
        /// <summary>
        /// Dll
        /// </summary>
        Libray = 1,
        /// <summary>
        /// Exe
        /// </summary>
        Exe = 2,
        /// <summary>
        /// Svc（Wcf 服务）
        /// </summary>
        Svc = 3,
        /// <summary>
        /// 配置信息
        /// </summary>
        Config = 4,
        /// <summary>
        /// xml配置文件
        /// </summary>
        Xml = 5
    }
}
