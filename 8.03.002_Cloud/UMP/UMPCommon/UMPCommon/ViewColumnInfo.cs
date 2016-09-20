//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    2e45715b-00b9-4241-ac3d-735753adbc3c
//        CLR Version:              4.0.30319.18444
//        Name:                     ViewColumnInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common
//        File Name:                ViewColumnInfo
//
//        created by Charley at 2014/9/24 15:30:11
//        http://www.voicecyber.com 
//
//======================================================================

using System.Runtime.Serialization;

namespace VoiceCyber.UMP.Common
{
    /// <summary>
    /// 视图列信息
    /// </summary>
    [DataContract]
    public class ViewColumnInfo
    {
        /// <summary>
        /// 视图编号（4位小模块编号+3位顺序编号）
        /// </summary>
        [DataMember]
        public long ViewID { get; set; }
        /// <summary>
        /// 列名称
        /// </summary>
        [DataMember]
        public string ColumnName { get; set; }
        /// <summary>
        /// 显示文本
        /// </summary>
        [DataMember]
        public string Display { get; set; }
        /// <summary>
        /// 数据类型
        /// </summary>
        [DataMember]
        public int DataType { get; set; }
        /// <summary>
        /// 列宽度
        /// </summary>
        [DataMember]
        public int Width { get; set; }
        /// <summary>
        /// 对齐方式（L、R、C等）
        /// </summary>
        [DataMember]
        public string Align { get; set; }
        /// <summary>
        /// 格式化文本
        /// </summary>
        [DataMember]
        public string Format { get; set; }
        /// <summary>
        /// 可见性（0、1等）
        /// </summary>
        [DataMember]
        public string Visibility { get; set; }
        /// <summary>
        /// 排序编号
        /// </summary>
        [DataMember]
        public int SortID { get; set; }
        /// <summary>
        /// 描述信息（鼠标悬停文本）
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        public override string ToString()
        {
            return string.Format("[{0}]{1}", ViewID, ColumnName);
        }
    }
}
