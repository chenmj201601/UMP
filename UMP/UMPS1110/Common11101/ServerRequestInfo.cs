//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    e537abdd-fbbf-452e-9430-76909c0196eb
//        CLR Version:              4.0.30319.18444
//        Name:                     ServerRequestInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                Common11101
//        File Name:                ServerRequestInfo
//
//        created by Charley at 2015/2/3 10:21:49
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.Generic;

namespace VoiceCyber.UMP.Common11101
{
    /// <summary>
    /// 获取服务器信息的请求参数类
    /// </summary>
    public class ServerRequestInfo
    {
        /// <summary>
        /// 服务器地址
        /// </summary>
        public string ServerHost { get; set; }
        /// <summary>
        /// 端口
        /// </summary>
        public int ServerPort { get; set; }
        /// <summary>
        /// 请求命令
        /// </summary>
        public int Command { get; set; }
        /// <summary>
        /// 请求数据
        /// </summary>
        public string Data { get; set; }
        /// <summary>
        /// 请求数据
        /// </summary>
        public List<string> ListData { get; set; }

        public ServerRequestInfo()
        {
            ListData = new List<string>();
        }
    }
}
