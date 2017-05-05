//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    07991c23-5f97-4a6a-891d-1494ca251a74
//        CLR Version:              4.0.30319.18444
//        Name:                     NumericStandard
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets
//        File Name:                NumericStandard
//
//        created by Charley at 2014/6/10 15:11:26
//        http://www.voicecyber.com 
//
//======================================================================
//
//======================================================================
//数值型评分标准
//
//数值型评分标准是一种特殊的评分标准，用户可以直接指定得分值
//
//属性编码范围：161 ~ 163
//
//各属性分组下排列序号：
//0:    无
//1:    6 ~ 8  
//2:    无
//3:    无
//======================================================================
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace VoiceCyber.UMP.ScoreSheets
{
    /// <summary>
    /// 数值型的评分标准
    /// </summary>
    [DefaultProperty("DefaultValue"), Description("Numeric Standard")]
    [XmlRoot(Namespace = "http://www.voicecyber.com/UMP/ScoreSheets/")]
    public class NumericStandard : Standard
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
        [Category("Basic"), Description("Default Value")]
        [XmlAttribute]
        public double DefaultValue { get; set; }

        /// <summary>
        /// NumericStandard
        /// </summary>
        public NumericStandard()
        {

        }
        /// <summary>
        /// 检查完整性，正确性
        /// </summary>
        /// <returns></returns>
        public override CheckValidResult CheckValid()
        {
            CheckValidResult result = base.CheckValid();
            if (result.Code != 0)
            {
                return result;
            }
            //检查最大值是否超出范围
            bool usePonitSystem = false;
            ScoreSheet scoreSheet = ScoreSheet;
            if (scoreSheet != null && scoreSheet.UsePointSystem)
            {
                usePonitSystem = true;
            }
            if (usePonitSystem && (MaxValue > PointSystem))
            {
                result.Code = 300;
                result.ScoreObject = this;
                result.Message = string.Format("MaxValue larger than PointSystem value.");
                return result;
            }
            if (!usePonitSystem && (MaxValue > TotalScore))
            {
                result.Code = 300;
                result.ScoreObject = this;
                result.Message = string.Format("MaxValue larger than total value.");
                return result;
            }
            if (DefaultValue < MinValue || DefaultValue > MaxValue)//如果默认值大于最大值小于最小值，直接设置为最小值
            {
                DefaultValue = MinValue;
            }
            return result;
        }

        public override CheckValidResult CheckInputValid()
        {
            CheckValidResult result = base.CheckInputValid();
            if (result.Code != 0)
            {
                return result;
            }
            //检查得分是否超过最大值，最小值
            if (Score > MaxValue || Score < MinValue)
            {
                result.Code = 300;
                result.ScoreObject = this;
                result.Message = string.Format("Score value invalid");
                return result;
            }
            return result;
        }

        public override void GetPropertyList(ref List<ScoreProperty> listProperties)
        {
            base.GetPropertyList(ref listProperties);
            listProperties.Add(new ScoreProperty
            {
                ID = 161,
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
                ID = 162,
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
                ID = 163,
                Name = "DefaultValue",
                Display = "DefaultValue",
                PropertyName = "DefaultValue",
                Description = "DefaultValue",
                Category = 1,
                Flag = ScorePropertyFlag.All,
                OrderID = 8,
                DataType = ScorePropertyDataType.Double
            });
        }

        public override double GetDefaultValue()
        {
            return DefaultValue;
        }

    }
}
