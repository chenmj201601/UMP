//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    8c74c8a8-5af3-49c8-9e78-d0b78e83f50e
//        CLR Version:              4.0.30319.18444
//        Name:                     ResourceGroupParam
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                Common11101
//        File Name:                ResourceGroupParam
//
//        created by Charley at 2015/1/15 16:12:38
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.Common11101
{
    /// <summary>
    /// 资源分组信息
    /// </summary>
    public class ResourceGroupParam
    {
        /// <summary>
        /// 资源类型ID
        /// </summary>
        public int TypeID { get; set; }
        /// <summary>
        /// 分组编号
        /// </summary>
        public int GroupID { get; set; }
        /// <summary>
        /// 父级分组
        /// </summary>
        public int ParentGroup { get; set; }
        /// <summary>
        /// 同一层次下的排列序号
        /// </summary>
        public int SortID { get; set; }
        /// <summary>
        /// 是否显示
        /// </summary>
        public bool IsShow { get; set; }
        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }
        /// <summary>
        /// 分组类型
        /// </summary>
        public ResourceGroupType GroupType { get; set; }
        /// <summary>
        /// 其他附加参数
        /// </summary>
        public int ChildType { get; set; }
        public int IntValue01 { get; set; }
        public int IntValue02 { get; set; }
        public string StringValue01 { get; set; }
        public string StringValue02 { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// xml文件中节点名称
        /// </summary>
        public string NodeName { get; set; }
        /// <summary>
        /// 对于ChildList类型的组，是否统计子对象的个数，并作为Count属性值
        /// </summary>
        public bool IsCaculateCount { get; set; }

        public override string ToString()
        {
            return string.Format("[{0}][{1}][{2}]", TypeID, GroupID, Description);
        }
    }
}
