//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    9b8572c5-f4f6-4e32-bbf2-0000aba80d9a
//        CLR Version:              4.0.30319.18444
//        Name:                     StandardItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets
//        File Name:                StandardItem
//
//        created by Charley at 2014/6/10 13:49:23
//        http://www.voicecyber.com 
//
//======================================================================
//
//======================================================================
//评分标准子项
//
//评分标准子项是多值型评分标准的的子项
//属性Value指示该评分标准子项的分值
//
//属性编码范围：201 ~ 202
//
//各属性分组下排列序号：
//0：    无
//1:     1 ~ 2
//2:     无
//3:     无
//======================================================================
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace VoiceCyber.UMP.ScoreSheets
{
    /// <summary>
    /// 评分标准子项
    /// </summary>
    [DefaultProperty("Display"), Description("Standard Item")]
    [XmlRoot(Namespace = "http://www.voicecyber.com/UMP/ScoreSheets/")]
    public class StandardItem : ScoreObject
    {
        /// <summary>
        /// 分值
        /// </summary>
        [Category("Basic"), Description("Value")]
        [XmlAttribute]
        public double Value { get; set; }
        /// <summary>
        /// 显示信息
        /// </summary>
        [Category("Basic"), Description("Display")]
        [XmlAttribute]
        public new string Display { get; set; }
        /// <summary>
        /// 序号
        /// </summary>
        [Browsable(false)]
        [XmlAttribute]
        public int OrderID { get; set; }
        /// <summary>
        /// 评分标准
        /// </summary>
        [Browsable(false)]
        [XmlIgnore]
        public ScoreItem ScoreItem { get; set; }
        /// <summary>
        /// StandardItem
        /// </summary>
        public StandardItem()
        {
            
        }

        /// <summary>
        /// 获取对象的属性列表
        /// </summary>
        /// <param name="listProperties"></param>
        public override void GetPropertyList(ref List<ScoreProperty> listProperties)
        {
            base.GetPropertyList(ref listProperties);
            listProperties.Add(new ScoreProperty
            {
                ID = 201,
                Name = "Display",
                Display = "Display",
                PropertyName = "Display",
                Description = "Display",
                Category = 1,
                Flag = ScorePropertyFlag.All,
                OrderID = 1,
                DataType = ScorePropertyDataType.MString
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 202,
                Name = "Value",
                Display = "Value",
                PropertyName = "Value",
                Description = "Value",
                Category = 1,
                Flag = ScorePropertyFlag.All,
                OrderID = 2,
                DataType = ScorePropertyDataType.Double
            });
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Display))
            {
                return string.Format("{0}", Display);
            }
            return base.ToString();
        }
    }
}
