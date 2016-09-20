//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    44fc8c13-55d0-40ec-9aa1-e5e05345f635
//        CLR Version:              4.0.30319.18063
//        Name:                     BasicDataInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common
//        File Name:                BasicDataInfo
//
//        created by Charley at 2015/5/22 14:49:08
//        http://www.voicecyber.com 
//
//======================================================================

using System.Runtime.Serialization;

namespace VoiceCyber.UMP.Common
{
    /// <summary>
    /// 基础数据信息，通常是一些枚举信息
    /// </summary>
    [DataContract]
    public class BasicDataInfo
    {
        /// <summary>
        /// 编号，ModuleID+00000
        /// </summary>
        [DataMember]
        public int InfoID { get; set; }
        /// <summary>
        /// 排列序号
        /// </summary>
        [DataMember]
        public int SortID { get; set; }
        /// <summary>
        /// 父级编号
        /// </summary>
        [DataMember]
        public int ParentID { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        [DataMember]
        public bool IsEnable { get; set; }
        /// <summary>
        /// 加密版本，0表示不加密
        /// </summary>
        [DataMember]
        public int EncryptVersion { get; set; }
        /// <summary>
        /// 实际值
        /// </summary>
        [DataMember]
        public string Value { get; set; }
        /// <summary>
        /// 图标（或备注）
        /// </summary>
        [DataMember]
        public string Icon { get; set; }

        public override string ToString()
        {
            return string.Format("[{0}][{1}][{2}]", InfoID, SortID, Value);
        }
    }
}
