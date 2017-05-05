//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    9459126e-f04f-4109-ade7-59a4e320e47b
//        CLR Version:              4.0.30319.18408
//        Name:                     DomainInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common
//        File Name:                DomainInfo
//
//        created by Charley at 2016/8/9 10:18:31
//        http://www.voicecyber.com 
//
//======================================================================

using System.Runtime.Serialization;

namespace VoiceCyber.UMP.Common
{
    /// <summary>
    /// 域信息
    /// </summary>
    [DataContract]
    public class DomainInfo
    {
        /// <summary>
        /// 域编码
        /// </summary>
        [DataMember]
        public long DomainID { get; set; }
        /// <summary>
        /// 域名
        /// </summary>
        [DataMember]
        public string Name { get; set; }
        /// <summary>
        /// 全名或描述
        /// </summary>
        [DataMember]
        public string FullName { get; set; }
        /// <summary>
        /// 是否允许自动登录
        /// </summary>
        [DataMember]
        public bool AllowAutoLogin { get; set; }

        public override string ToString()
        {
            return string.Format("[{0}][{1}][{2}][{3}]", DomainID, Name, FullName, AllowAutoLogin);
        }
    }
}
