//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    3b3fe8f9-60d9-4d46-9969-8b593e736d3b
//        CLR Version:              4.0.30319.18063
//        Name:                     OrgInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common
//        File Name:                OrgInfo
//
//        created by Charley at 2015/10/12 9:44:56
//        http://www.voicecyber.com 
//
//======================================================================

using System.Runtime.Serialization;

namespace VoiceCyber.UMP.Common
{
    /// <summary>
    /// 机构信息
    /// </summary>
    [DataContract]
    public class OrgInfo
    {
        /// <summary>
        /// 机构ID
        /// </summary>
        [DataMember]
        public long OrgID { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [DataMember]
        public string Description { get; set; }
        /// <summary>
        /// 机构类型编码
        /// </summary>
        [DataMember]
        public long OrgTypeID { get; set; }

        public override string ToString()
        {
            return string.Format("[{0}][{1}]", OrgID, Name);
        }
    }
}
