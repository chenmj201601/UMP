//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    3c9e179f-b104-435a-a3c3-858b293eb75a
//        CLR Version:              4.0.30319.18444
//        Name:                     ResourceTypeParam
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                Common11101
//        File Name:                ResourceTypeParam
//
//        created by Charley at 2015/1/12 11:31:03
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.Common11101
{
    /// <summary>
    /// 资源类型参数信息
    /// </summary>
    public class ResourceTypeParam
    {
        /// <summary>
        /// 资源类型ID
        /// </summary>
        public int TypeID { get; set; }
        /// <summary>
        /// 父级类型ID
        /// </summary>
        public int ParentID { get; set; }
        /// <summary>
        /// 排列序号
        /// </summary>
        public int OrderID { get; set; }
        /// <summary>
        /// 是否显示
        /// </summary>
        public bool IsShow { get; set; }
        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 是否主备服务器
        /// </summary>
        public bool IsMasterSlaver { get; set; }
        /// <summary>
        /// 是否需要认证信息
        /// </summary>
        public bool IsAuthention { get; set; }
        /// <summary>
        /// 是否有子级列表
        /// </summary>
        public bool HasChildList { get; set; }
        /// <summary>
        /// 其他附加参数
        /// </summary>
        public int ChildType { get; set; }
        public int IntValue01 { get; set; }
        public int IntValue02 { get; set; }
        public string StringValue01 { get; set; }
        public string StringValue02 { get; set; }

        /// <summary>
        /// Xml文件中节点名称
        /// </summary>
        public string NodeName { get; set; }

        public override string ToString()
        {
            return string.Format("[{0}][{1}][{2}]", TypeID, OrderID, Description);
        }
    }
}
