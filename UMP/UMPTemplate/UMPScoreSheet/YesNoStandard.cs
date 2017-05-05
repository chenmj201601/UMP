//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    eeb6ebe7-64a3-48c5-822a-6a0583b5ad43
//        CLR Version:              4.0.30319.18444
//        Name:                     YesNoStandard
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets
//        File Name:                YesNoStandard
//
//        created by Charley at 2014/6/10 15:13:10
//        http://www.voicecyber.com 
//
//======================================================================
//
//======================================================================
//是非型评分标准
//
//是非型评分标准是一种特殊的评分标准，用户可以通过选择Yes，No打分
//
//属性编码范围：166
//
//各属性分组下排列序号：
//0:    无
//1:    6  
//2:    无
//3:    无
//======================================================================
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace VoiceCyber.UMP.ScoreSheets
{
    /// <summary>
    /// YesNoStandard
    /// </summary>
    [Description("YesNo Standard")]
    [XmlRoot(Namespace = "http://www.voicecyber.com/UMP/ScoreSheets/")]
    public class YesNoStandard : Standard
    {
        /// <summary>
        /// 默认值
        /// </summary>
        [Category("Basic"), Description("Default Value")]
        [XmlAttribute]
        public bool DefaultValue { get; set; }
        /// <summary>
        /// 反转“是” 和 “否” 显示
        /// </summary>
        public bool ReverseDisplay { get; set; }
        /// <summary>
        /// YesNoStandard
        /// </summary>
        public YesNoStandard()
        {

        }
        /// <summary>
        /// GetPropertyList
        /// </summary>
        /// <param name="listProperties"></param>
        public override void GetPropertyList(ref List<ScoreProperty> listProperties)
        {
            base.GetPropertyList(ref listProperties);
            listProperties.Add(new ScoreProperty
            {
                ID = 166,
                Name = "DefaultValue",
                Display = "DefaultValue",
                PropertyName = "DefaultValue",
                Description = "DefaultValue",
                Category = 1,
                Flag = ScorePropertyFlag.All,
                OrderID = 6,
                DataType = ScorePropertyDataType.Bool
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 167,
                Name = "ReverseDisplay",
                Display = "ReverseDisplay",
                PropertyName = "ReverseDisplay",
                Description = "ReverseDisplay",
                Category = 1,
                Flag = ScorePropertyFlag.All,
                OrderID = 7,
                DataType = ScorePropertyDataType.Bool
            });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override double GetDefaultValue()
        {
            bool usePonitSystem = ScoreSheet != null&&ScoreSheet.UsePointSystem;
            if (usePonitSystem&&DefaultValue)
            {
                return PointSystem;
            }
            if (!usePonitSystem && DefaultValue)
            {
                return TotalScore;
            }
            return 0;
        }
    }
}
