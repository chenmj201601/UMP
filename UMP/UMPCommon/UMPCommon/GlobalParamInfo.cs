//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    f24ce219-c4ab-48e4-b3d7-80b9dc7e11b2
//        CLR Version:              4.0.30319.18063
//        Name:                     GlobalParamInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common
//        File Name:                GlobalParamInfo
//
//        created by Charley at 2015/6/29 16:29:32
//        http://www.voicecyber.com 
//
//======================================================================

using System.Runtime.Serialization;

namespace VoiceCyber.UMP.Common
{
    /// <summary>
    /// 全局参数信息
    /// </summary>
    [DataContract]
    public class GlobalParamInfo
    {
        /// <summary>
        /// 租户ID
        /// </summary>
        [DataMember]
        public long RentID { get; set; }
        /// <summary>
        /// 模块号
        /// </summary>
        [DataMember]
        public int ModuleID { get; set; }
        /// <summary>
        /// 参数编号
        /// </summary>
        [DataMember]
        public int ParamID { get; set; }
        /// <summary>
        /// 参数所在的组
        /// </summary>
        [DataMember]
        public int GroupID { get; set; }
        /// <summary>
        /// 组内排列序号
        /// </summary>
        [DataMember]
        public int SortID { get; set; }
        /// <summary>
        /// 参数值
        /// </summary>
        [DataMember]
        public string ParamValue { get; set; }

        public override string ToString()
        {
            return string.Format("[{0}][{1}]", ModuleID, ParamID);
        }
    }
}
