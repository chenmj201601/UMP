//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    7e12b047-141b-47b2-9e07-27648e58ed93
//        CLR Version:              4.0.30319.18408
//        Name:                     LoginInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common
//        File Name:                LoginInfo
//
//        created by Charley at 2016/8/9 10:10:54
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Runtime.Serialization;


namespace VoiceCyber.UMP.Common
{
    /// <summary>
    /// 登录信息
    /// </summary>
    [DataContract]
    public class LoginInfo
    {
        /// <summary>
        /// 登录流水号
        /// </summary>
        [DataMember]
        public long LoginID { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        [DataMember]
        public long UserID { get; set; }
        /// <summary>
        /// 登录终端类型
        /// </summary>
        [DataMember]
        public int Terminal { get; set; }
        /// <summary>
        /// 登录机器名称
        /// </summary>
        [DataMember]
        public string Host { get; set; }
        /// <summary>
        /// 登录时间（UTC）
        /// </summary>
        [DataMember]
        public DateTime LoginTime { get; set; }
        /// <summary>
        /// 最后一次在线时间
        /// </summary>
        [DataMember]
        public DateTime OnlineTime { get; set; }

        public override string ToString()
        {
            return string.Format("[{0}][{1}][{2}][{3}][{4}]", LoginID, UserID, Terminal, Host, LoginTime);
        }
    }
}
