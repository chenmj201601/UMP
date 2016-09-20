//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    7f9f5ab6-38cc-4fc1-b7c5-000e8aa5f609
//        CLR Version:              4.0.30319.18444
//        Name:                     ScorePropertyDataType
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets
//        File Name:                ScorePropertyDataType
//
//        created by Charley at 2014/7/8 11:01:05
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.ScoreSheets
{
    /// <summary>
    /// 属性数据类型
    /// </summary>
    public enum ScorePropertyDataType
    {
        #region 基本类型

        /// <summary>
        ///  文本
        /// </summary>
        SString = 0,
        /// <summary>
        /// int (>=0)
        /// </summary>
        Int = 1,
        /// <summary>
        /// long (>=0)
        /// </summary>
        Long = 2,
        /// <summary>
        /// double (>=0)
        /// </summary>
        Double = 3,
        /// <summary>
        /// bool
        /// </summary>
        Bool = 4,
        /// <summary>
        /// Datetime
        /// </summary>
        DateTime = 5,
        /// <summary>
        /// 枚举，需要指定DataSource
        /// </summary>
        Enum = 6,
        /// <summary>
        /// 列表类型,需要指定对象类型及数据源以及显示字段
        /// </summary>
        List = 7,
        /// <summary>
        /// 自定义对象，需要指定DataSource及DisplayName
        /// </summary>
        CustomObject,

        #endregion

        #region 扩展类型
        /// <summary>
        /// 多行文本
        /// </summary>
        MString = 10,
        /// <summary>
        /// 带负数的Int
        /// </summary>
        SInt = 11,
        /// <summary>
        /// 带负数的Long
        /// </summary>
        SLong = 12,
        /// <summary>
        /// 带负数的Double
        /// </summary>
        SDouble = 13,

        #endregion

        #region 特殊类型
        /// <summary>
        /// 图标类型
        /// </summary>
        Icon = 100,
        /// <summary>
        /// 颜色
        /// </summary>
        Color = 101,
        /// <summary>
        /// 字体
        /// </summary>
        FontFamily = 102,
        /// <summary>
        /// 字体样式
        /// </summary>
        FontWeight = 103,
        /// <summary>
        /// 样式
        /// </summary>
        Style = 104,
        /// <summary>
        /// 多项
        /// </summary>
        Item = 105,
        /// <summary>
        /// 评分项
        /// </summary>
        ScoreItem = 106,
        /// <summary>
        /// 评分标准子项
        /// </summary>
        StandardItem = 107,
        /// <summary>
        /// 备注子项
        /// </summary>
        CommentItem = 108,
        /// <summary>
        /// 控制项子项
        /// </summary>
        ControlItem = 109,

        #endregion

    }
}
