//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    1b3950c0-088c-4f8e-9415-ff6e56eb0e24
//        CLR Version:              4.0.30319.18444
//        Name:                     OperationInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common
//        File Name:                OperationInfo
//
//        created by Charley at 2014/9/24 15:29:28
//        http://www.voicecyber.com 
//
//======================================================================

using System.Runtime.Serialization;

namespace VoiceCyber.UMP.Common
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
        /// 0       M       Menu（作为菜单，默认类型）
        /// 1       B       Button（作为普通按钮）
        /// 2       T       ToggleButton（作为开关）
        /// 3       H       Hide（隐藏的，不会出现在操作权限分配界面）
        /// </summary>
        [DataMember]
        public int Type { get; set; }

        public override string ToString()
        {
            return string.Format("{0}[{1}]", Display, ID);
        }
    }
}
