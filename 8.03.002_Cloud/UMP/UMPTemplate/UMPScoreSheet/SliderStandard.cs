//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    128ceaa8-a7aa-4a7e-948d-8238ee3965ef
//        CLR Version:              4.0.30319.18444
//        Name:                     SliderStandard
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets
//        File Name:                SliderStandard
//
//        created by Charley at 2014/6/10 15:17:27
//        http://www.voicecyber.com 
//
//======================================================================
//
//======================================================================
//拖拉型评分标准
//
//拖拉型评分标准是一种特殊的评分标准，用户可以通过拖拉的方式进行打分
//属性Interval指示值的变化频率，可以通过方向键改变值
//
//属性编码范围：171 ~ 174
//
//各属性分组下排列序号：
//0:    无
//1:    6 ~ 9  
//2:    无
//3:    无
//======================================================================
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace VoiceCyber.UMP.ScoreSheets
{
    /// <summary>
    /// 拖拉型评分标准
    /// </summary>
    [Description("Slider Standard")]
    [XmlRoot(Namespace = "http://www.voicecyber.com/UMP/ScoreSheets/")]
    public class SliderStandard : Standard
    {
        /// <summary>
        /// 最小值
        /// </summary>
        [Category("Basic"), Description("Min Value")]
        [XmlAttribute]
        public double MinValue { get; set; }
        /// <summary>
        /// 最大值
        /// </summary>
        [Category("Basic"), Description("Max Value")]
        [XmlAttribute]
        public double MaxValue { get; set; }
        /// <summary>
        /// 默认值
        /// </summary>
        [Category("Basic"), Description("DefaultValue")]
        [XmlAttribute]
        public double DefaultValue { get; set; }
        /// <summary>
        /// 间隔
        /// </summary>
        [Category("Basic"), Description("Interval")]
        [XmlAttribute]
        public double Interval { get; set; }
        /// <summary>
        /// SliderStandard
        /// </summary>
        public SliderStandard()
        {

        }

        public override void GetPropertyList(ref List<ScoreProperty> listProperties)
        {
            base.GetPropertyList(ref listProperties);
            listProperties.Add(new ScoreProperty
            {
                ID = 171,
                Name = "MinValue",
                Display = "MinValue",
                PropertyName = "MinValue",
                Description = "MinValue",
                Category = 1,
                Flag = ScorePropertyFlag.All,
                OrderID = 6,
                DataType = ScorePropertyDataType.Double
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 172,
                Name = "MaxValue",
                Display = "MaxValue",
                PropertyName = "MaxValue",
                Description = "MaxValue",
                Category = 1,
                Flag = ScorePropertyFlag.All,
                OrderID = 7,
                DataType = ScorePropertyDataType.Double
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 173,
                Name = "DefaultValue",
                Display = "DefaultValue",
                PropertyName = "DefaultValue",
                Description = "DefaultValue",
                Category = 1,
                Flag = ScorePropertyFlag.All,
                OrderID = 8,
                DataType = ScorePropertyDataType.Double
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 174,
                Name = "Interval",
                Display = "Interval",
                PropertyName = "Interval",
                Description = "Interval",
                Category = 1,
                Flag = ScorePropertyFlag.All,
                OrderID = 9,
                DataType = ScorePropertyDataType.Double
            });
        }

        public override double GetDefaultValue()
        {
            return DefaultValue;
        }
    }
}
