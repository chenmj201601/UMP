//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    e56c5c5e-c4b2-4fd2-ab3d-e41dabce4ea3
//        CLR Version:              4.0.30319.18063
//        Name:                     UserInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common
//        File Name:                UserInfo
//
//        created by Charley at 2014/8/20 15:28:56
//        http://www.voicecyber.com 
//
//======================================================================

using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace VoiceCyber.UMP.Common
{
    /// <summary>
    /// 用户信息
    /// </summary>
    [DataContract]
    public class UserInfo
    {
        /// <summary>
        /// 用户编码
        /// </summary>
        [DataMember]
        public long UserID { get; set; }
        /// <summary>
        /// 帐号
        /// </summary>
        [DataMember]
        public string Account { get; set; }
        /// <summary>
        /// 用户全名
        /// </summary>
        [DataMember]
        public string UserName { get; set; }
        /// <summary>
        /// 登录密码，通常是已经加密的密码
        /// </summary>
        [DataMember]
        public string Password { get; set; }
        /// <summary>
        /// 实际密码，这个密码不能序列化传输，是未加密的原始密码
        /// </summary>
        [XmlIgnore]
        public string RealPassword { get; set; }

        public override string ToString()
        {
            return string.Format("{0}[{1}]", Account, UserID);
        }
    }
}
