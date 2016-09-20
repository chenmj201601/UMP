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

using System.Runtime.Serialization;

namespace VoiceCyber.UMP.Common
{
    /// <summary>
    /// WCF服务信息
    /// </summary>
    [DataContract]
    public class AppServerInfo
    {
        /// <summary>
        /// 协议类型（http , https）
        /// </summary>
        [DataMember]
        public string Protocol { get; set; }
        /// <summary>
        /// 服务地址
        /// </summary>
        [DataMember]
        public string Address { get; set; }
        /// <summary>
        /// 端口
        /// </summary>
        [DataMember]
        public int Port { get; set; }
        /// <summary>
        /// 是否支持https
        /// </summary>
        [DataMember]
        public bool SupportHttps { get; set; }
        /// <summary>
        /// 是否支持NetTcp
        /// </summary>
        [DataMember]
        public bool SupportNetTcp { get; set; }

        public override string ToString()
        {
            return string.Format("{0}://{1}:{2}[{3},{4}]", Protocol, Address, Port, SupportHttps, SupportNetTcp);
        }
    }
}
