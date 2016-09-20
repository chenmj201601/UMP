//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    1e8e2b14-8aa5-4fd1-89cd-7afd7159c8db
//        CLR Version:              4.0.30319.18444
//        Name:                     RentInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common
//        File Name:                RentInfo
//
//        created by Charley at 2014/8/28 13:20:53
//        http://www.voicecyber.com 
//
//======================================================================

using System.Runtime.Serialization;

namespace VoiceCyber.UMP.Common
{
    /// <summary>
    /// 租户信息
    /// </summary>
    [DataContract]
    public class RentInfo
    {
        /// <summary>
        /// 租户ID
        /// </summary>
        [DataMember]
        public long ID { get; set; }
        /// <summary>
        /// 租户编号，对应数据表名的后五位，（默认是：00000 即本租户）
        /// </summary>
        [DataMember]
        public string Token { get; set; }
        /// <summary>
        /// 租户唯一标识，即域名（ @ 之后的名称 ）
        /// </summary>
        [DataMember]
        public string Domain { get; set; }
        /// <summary>
        /// 租户名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        public override string ToString()
        {
            return string.Format("{0}[{1}]({2})", Name, ID, Token);
        }
    }
}
