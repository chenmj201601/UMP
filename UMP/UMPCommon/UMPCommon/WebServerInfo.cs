//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    5b4faf0f-99b4-44e3-8a6a-1e2a3802eb3e
//        CLR Version:              4.0.30319.18063
//        Name:                     WebServerInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common
//        File Name:                WebServerInfo
//
//        created by Charley at 2014/8/20 15:19:36
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceCyber.UMP.Common
{
    /// <summary>
    /// Web服务信息
    /// </summary>
    public class WebServerInfo
    {
        /// <summary>
        /// 协议类型（http , https）
        /// </summary>
        public string Protocol { get; set; }
        /// <summary>
        /// 服务地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }
    }
}
