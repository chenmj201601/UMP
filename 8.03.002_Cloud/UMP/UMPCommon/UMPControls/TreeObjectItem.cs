//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    a124f8fb-ad99-4c6e-9495-72b74044c11f
//        CLR Version:              4.0.30319.18444
//        Name:                     ObjectItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Controls
//        File Name:                ObjectItem
//
//        created by Charley at 2014/11/27 13:30:52
//        http://www.voicecyber.com 
//
//======================================================================

using VoiceCyber.Wpf.CustomControls;

namespace VoiceCyber.UMP.Controls
{
    /// <summary>
    /// 管理对象树中的节点元素
    /// </summary>
    public class TreeObjectItem : CheckableItemBase
    {
        /// <summary>
        /// 对象类型编码
        /// </summary>
        public int ObjType { get; set; }
        /// <summary>
        /// 对象编码
        /// </summary>
        public long ObjID { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 实际数据
        /// </summary>
        public object Data { get; set; }
    }
}
