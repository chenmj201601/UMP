//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    02127669-9521-48cb-8909-1b5a0469977f
//        CLR Version:              4.0.30319.18063
//        Name:                     WinServiceInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPSetup.Models
//        File Name:                WinServiceInfo
//
//        created by Charley at 2015/12/30 14:40:49
//        http://www.voicecyber.com 
//
//======================================================================

namespace UMPSetup.Models
{
    /// <summary>
    /// Windows服务信息
    /// </summary>
    public class WinServiceInfo
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 服务名
        /// </summary>
        public string ServiceName { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }
    }
}
