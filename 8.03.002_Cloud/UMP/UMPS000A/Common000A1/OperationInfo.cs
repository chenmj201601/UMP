//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    2954a754-a038-46a1-a0b7-1696630670fd
//        CLR Version:              4.0.30319.18063
//        Name:                     OperationInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common000A1
//        File Name:                OperationInfo
//
//        created by Charley at 2015/7/30 10:28:32
//        http://www.voicecyber.com 
//
//======================================================================

using System.Runtime.Serialization;

namespace VoiceCyber.UMP.Common000A1
{
    /// <summary>
    /// 操作信息
    /// </summary>
    [DataContract]
    public class OperationInfo
    {
        /// <summary>
        /// 操作编号
        /// </summary>
        [DataMember]
        public long ID { get; set; }
        /// <summary>
        /// 显示文本
        /// </summary>
        [DataMember]
        public string Display { get; set; }
        /// <summary>
        /// 描述信息（鼠标悬停文本）
        /// </summary>
        [DataMember]
        public string Description { get; set; }
        /// <summary>
        /// 图标路径
        /// </summary>
        [DataMember]
        public string Icon { get; set; }
        /// <summary>
        /// 上级操作ID
        /// </summary>
        [DataMember]
        public long ParentID { get; set; }
        /// <summary>
        /// 排列序号
        /// </summary>
        [DataMember]
        public int SortID { get; set; }
        /// <summary>
        /// 0       M       Menu
        /// 1       B       Button
        /// 2       T       ToggleButton
        /// </summary>
        [DataMember]
        public int Type { get; set; }
        /// <summary>
        /// 其他信息（跳转路径）
        /// </summary>
        [DataMember]
        public string Other01 { get; set; }
        /// <summary>
        /// 其他信息（跳转参数）
        /// </summary>
        [DataMember]
        public string Other02 { get; set; }
        /// <summary>
        /// 其他信息
        /// </summary>
        [DataMember]
        public string Other03 { get; set; }
        /// <summary>
        /// 其他信息
        /// </summary>
        [DataMember]
        public string Other04 { get; set; }
        /// <summary>
        /// 其他信息
        /// </summary>
        [DataMember]
        public string Other05 { get; set; }

        public override string ToString()
        {
            return string.Format("{0}[{1}]", Display, ID);
        }
    }
}
