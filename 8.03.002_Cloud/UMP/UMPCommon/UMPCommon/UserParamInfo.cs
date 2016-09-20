//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    01749b77-bdda-4cc9-929c-a1b7e3b094d8
//        CLR Version:              4.0.30319.18063
//        Name:                     UserParamInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common
//        File Name:                UserParamInfo
//
//        created by Charley at 2015/7/5 17:08:41
//        http://www.voicecyber.com 
//
//======================================================================

using System.Runtime.Serialization;

namespace VoiceCyber.UMP.Common
{
    /// <summary>
    /// 用户参数信息
    /// </summary>
    [DataContract]
    public class UserParamInfo
    {
        /// <summary>
        /// 用户编码
        /// </summary>
        [DataMember]
        public long UserID { get; set; }
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
        /// <summary>
        /// 参数数据类型
        /// </summary>
        [DataMember]
        public DBDataType DataType { get; set; }

        public override string ToString()
        {
            return string.Format("[{0}][{1}]", UserID, ParamID);
        }
    }
}
