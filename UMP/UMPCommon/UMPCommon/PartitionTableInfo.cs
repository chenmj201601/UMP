//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    c54a4fe4-f84c-4fbd-b72c-f03ae5577cb9
//        CLR Version:              4.0.30319.18444
//        Name:                     PartitionTableInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common
//        File Name:                PartitionTableInfo
//
//        created by Charley at 2014/12/4 10:55:54
//        http://www.voicecyber.com 
//
//======================================================================

using System.Runtime.Serialization;

namespace VoiceCyber.UMP.Common
{
    /// <summary>
    /// 分表信息
    /// </summary>
    [DataContract]
    public class PartitionTableInfo
    {
        /// <summary>
        /// 表名称
        /// </summary>
        [DataMember]
        public string TableName { get; set; }
        /// <summary>
        /// 划分方式
        /// </summary>
        [DataMember]
        public TablePartType PartType { get; set; }
        /// <summary>
        /// 其他参数(通常划分分区依据的表中的字段名）
        /// </summary>
        [DataMember]
        public string Other1 { get; set; }
        /// <summary>
        /// 其他参数
        /// </summary>
        [DataMember]
        public string Other2 { get; set; }
        /// <summary>
        /// 其他参数
        /// </summary>
        [DataMember]
        public string Other3 { get; set; }

        public override string ToString()
        {
            return string.Format("[{0}]{1}", PartType, TableName);
        }
    }
    /// <summary>
    /// 分表的划分方式
    /// </summary>
    public enum TablePartType
    {
        /// <summary>
        /// 未知
        /// </summary>
        Unkown = 0,
        /// <summary>
        /// 按日期范围划分
        /// </summary>
        DatetimeRange = 1,
        /// <summary>
        /// 按录音服务器编号划分
        /// </summary>
        VoiceID = 2
    }
}
