//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    ffb22d55-16c1-4241-93ad-455d7d7d6fef
//        CLR Version:              4.0.30319.18444
//        Name:                     CustomConditionItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common31021
//        File Name:                CustomConditionItem
//
//        created by Charley at 2014/11/6 17:20:19
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.Common31021
{
    /// <summary>
    /// 自定义查询条件项
    /// </summary>
    public class CustomConditionItem
    {

        public long ID { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 规格
        /// </summary>
        public CustomConditionItemFormat Format { get; set; }

        /// <summary>
        /// 条件类型 (1:普通文件 2:模糊匹配文本 3:逗号间隔多值文本 4:下拉枚举 5:多选 6:复选 10:时间范围 11:时长范围 12:时间综合 13:自动模糊匹配文本)
        /// </summary>
        public CustomConditionItemType Type { get; set; }

        /// <summary>
        /// tabitem索引(0-9内置标签)
        /// </summary>
        public int TabIndex { get; set; }

        /// <summary>
        /// tabitem名称
        /// </summary>
        public string TabName { get; set; }

        /// <summary>
        /// 在一个tabitem下的顺号
        /// </summary>
        public int SortID { get; set; }

        /// <summary>
        /// 显示模式
        /// 0       不显示
        /// 1       显示
        /// 2       必显示
        /// </summary>
        public int ViewMode { get; set; }

        /// <summary>
        /// 1       该条件已经受控，不能勾选或取消勾选
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// 其他参数
        /// </summary>
        public string Param { get; set; }
    }
}
