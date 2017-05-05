//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    ad477050-9512-44c4-afad-e296037c4038
//        CLR Version:              4.0.30319.18444
//        Name:                     ScoreObjectType
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets
//        File Name:                ScoreObjectType
//
//        created by Charley at 2014/6/10 13:57:47
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.ScoreSheets
{
    /// <summary>
    /// 评分对象类别
    /// </summary>
    public enum ScoreObjectType
    {
        /// <summary>
        /// 评分对象
        /// </summary>
        ScoreObject = 0,
        /// <summary>
        /// 评分表
        /// </summary>
        ScoreSheet = 1,
        /// <summary>
        /// 评分项
        /// </summary>
        ScoreItem = 2,
        /// <summary>
        /// 评分项组（评分类别）
        /// </summary>
        ScoreGroup = 3,
        /// <summary>
        /// 评分标准
        /// </summary>
        Standard = 4,
        /// <summary>
        /// 评分标准的子项，部分类型的评分标准（如ItemStandard）有子项
        /// </summary>
        StandardItem = 5,
        /// <summary>
        /// 备注
        /// </summary>
        Comment = 6,
        /// <summary>
        /// 备注子项，部分类型的备注项（如ItemComment）有子项
        /// </summary>
        CommentItem = 7,
        /// <summary>
        /// 样式
        /// </summary>
        VisualStyle = 8,
        /// <summary>
        /// 数值型评分标准
        /// </summary>
        NumericStandard = 9,
        /// <summary>
        /// 是非型评分标准
        /// </summary>
        YesNoStandard = 10,
        /// <summary>
        /// 拖拉型文本标准
        /// </summary>
        SliderStandard = 11,
        /// <summary>
        /// 多值型评分标准
        /// </summary>
        ItemStandard = 12,
        /// <summary>
        /// 文本型备注项
        /// </summary>
        TextComment = 13,
        /// <summary>
        /// 多值型备注项
        /// </summary>
        ItemComment = 14,
        /// <summary>
        /// 控制项
        /// </summary>
        ControlItem = 15
    }
}
