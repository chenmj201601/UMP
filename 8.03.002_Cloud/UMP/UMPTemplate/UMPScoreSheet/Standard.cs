//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    7789befa-886e-4672-ad56-099a6ebb6476
//        CLR Version:              4.0.30319.18444
//        Name:                     Standard
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets
//        File Name:                Standard
//
//        created by Charley at 2014/6/10 13:48:57
//        http://www.voicecyber.com 
//
//======================================================================
//
//======================================================================
//评分标准
//
//评分标准是评分项组的子项，表示一个打分项目，用户可以对此项进行打分，
//评分标准可以有多种类型和风格
//属性StandardClassic指示评分标准的风格（文本框，单选列表，下拉列表，拖拉条等）
//属性StandardType指示评分标准的类型（数值，是非，拖拉，多值等）
//
//属性编码范围：141 ~ 145
//
//各属性分组下排列序号：
//0:    14
//1:    无  
//2:    7 ~ 8
//3:    无
//======================================================================
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;

namespace VoiceCyber.UMP.ScoreSheets
{
    /// <summary>
    /// 评分标准
    /// </summary>
    [Description("Standard")]
    [XmlRoot(Namespace = "http://www.voicecyber.com/UMP/ScoreSheets/")]
    [XmlInclude(typeof(NumericStandard))]
    [XmlInclude(typeof(YesNoStandard))]
    [XmlInclude(typeof(SliderStandard))]
    [XmlInclude(typeof(ItemStandard))]
    public class Standard : ScoreItem
    {
        /// <summary>
        /// 显示风格
        /// </summary>
        [Category("Basic"), Description("Score Classic")]
        [XmlAttribute]
        public StandardClassic ScoreClassic { get; set; }
        /// <summary>
        /// 评分标准类型
        /// </summary>
        [Category("Basic"), Description("Standard Type")]
        [XmlAttribute]
        public StandardType StandardType { get; set; }
        /// <summary>
        /// 分制
        /// </summary>
        [Category("Extented"), Description("Point System")]
        [XmlAttribute]
        public double PointSystem { get; set; }
        /// <summary>
        /// 是否为自动评分项
        /// </summary>
        [Browsable(false)]
        [XmlAttribute]
        public bool IsAutoStandard { get; set; }
        /// <summary>
        /// 统计标准ID（312）
        /// </summary>
        [Browsable(false)]
        [XmlAttribute]
        public long StatisticalID { get; set; }
        /// <summary>
        /// Standard
        /// </summary>
        public Standard()
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
            bool usePointSystem = false;
            ScoreSheet scoreSheet = ScoreSheet;
            if (scoreSheet != null && scoreSheet.UsePointSystem)
            {
                usePointSystem = true;
            }
            //如果启用分制，检查分制是否有效
            if (usePointSystem)
            {
                if (PointSystem < 0)
                {
                    result.Code = 202;
                    result.ScoreObject = this;
                    result.Message = string.Format("PointSystem invalid.");
                    return result;
                }
            }
            return result;
        }
        /// <summary>
        /// 获取属性列表
        /// </summary>
        /// <param name="listProperties"></param>
        public override void GetPropertyList(ref List<ScoreProperty> listProperties)
        {
            base.GetPropertyList(ref listProperties);
            listProperties.Add(new ScoreProperty
            {
                ID = 141,
                Name = "ScoreClassic",
                Display = "ScoreClassic",
                PropertyName = "ScoreClassic",
                Description = "ScoreClassic",
                Category = 0,
                Flag = ScorePropertyFlag.Normal,
                OrderID = 14,
                DataType = ScorePropertyDataType.Enum,
                ValueType = typeof(StandardClassic)
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 142,
                Name = "StandardType",
                Display = "StandardType",
                PropertyName = "StandardType",
                Description = "StandardType",
                Category = 1,
                Flag = ScorePropertyFlag.Normal,
                OrderID = 6,
                DataType = ScorePropertyDataType.Enum,
                ValueType = typeof(StandardType)
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 143,
                Name = "PointSystem",
                Display = "PointSystem",
                PropertyName = "PointSystem",
                Description = "PointSystem",
                Category = 1,
                Flag = ScorePropertyFlag.All,
                OrderID = 7,
                DataType = ScorePropertyDataType.Double
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 144,
                Name = "IsAutoStandard",
                Display = "IsAutoStandard",
                PropertyName = "IsAutoStandard",
                Description = "IsAutoStandard",
                Category = 1,
                Flag = ScorePropertyFlag.Normal,
                OrderID = 8,
                DataType = ScorePropertyDataType.Bool
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 145,
                Name = "StatisticalID",
                Display = "StatisticalID",
                PropertyName = "StatisticalID",
                Description = "StatisticalID",
                Category = 1,
                Flag = ScorePropertyFlag.Normal,
                OrderID = 9,
                DataType = ScorePropertyDataType.Long
            });
            //设置某些属性的不可见
            ScoreProperty scoreProperty = listProperties.FirstOrDefault(p => p.Name == "ViewClassic");
            if (scoreProperty != null)
            {
                scoreProperty.Flag = ScorePropertyFlag.Xml | ScorePropertyFlag.Copy;
            }
            scoreProperty = listProperties.FirstOrDefault(p => p.Name == "ScoreType");
            if (scoreProperty != null)
            {
                scoreProperty.Flag = ScorePropertyFlag.Xml | ScorePropertyFlag.Copy;
            }
            scoreProperty = listProperties.FirstOrDefault(p => p.Name == "UsePointSystem");
            if (scoreProperty != null)
            {
                scoreProperty.Flag = ScorePropertyFlag.Xml | ScorePropertyFlag.Copy;
            }
        }
        /// <summary>
        /// 获取默认成绩
        /// </summary>
        /// <returns></returns>
        public virtual double GetDefaultValue()
        {
            return Score;
        }

        public override void ResetScore()
        {
            base.ResetScore();
            Score = GetDefaultValue();
        }
    }
}
