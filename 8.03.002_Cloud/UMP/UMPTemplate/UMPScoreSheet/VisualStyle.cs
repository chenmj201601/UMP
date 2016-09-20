//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    9139d40a-d47e-4084-8950-58e3341afeaa
//        CLR Version:              4.0.30319.18444
//        Name:                     VisualStyle
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets
//        File Name:                VisualStyle
//
//        created by Charley at 2014/6/10 13:51:33
//        http://www.voicecyber.com 
//
//======================================================================
//
//======================================================================
//样式
//
//样式可附加到所有评分对象上，表示评分对象的显示样式
//设置字体，字体大小，宽高，颜色
//
//属性编码范围：261 ~ 267
//
//各属性分组下排列序号：
//0：    无
//1:     1 ~ 7
//2:     无
//3:     无
//======================================================================
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Xml.Serialization;

namespace VoiceCyber.UMP.ScoreSheets
{
    /// <summary>
    /// 显示样式
    /// </summary>
    [Description("View Style"), DefaultProperty("FontSize")]
    [XmlRoot(Namespace = "http://www.voicecyber.com/UMP/ScoreSheets/")]
    public class VisualStyle : ScoreObject
    {
        /// <summary>
        /// 字体大小
        /// </summary>
        [CategoryAttribute("Basic"), Description("Font Size")]
        [XmlAttribute]
        public int FontSize { get; set; }
        /// <summary>
        /// 字体类型
        /// </summary>
        [CategoryAttribute("Basic"), Description("Font Weight")]
        [XmlIgnore]
        public FontWeight FontWeight { get; set; }
        /// <summary>
        /// 字体类型
        /// </summary>
        [Browsable(false)]
        [XmlAttribute]
        public string StrFontWeight
        {
            get { return FontWeight.ToString(); }
            set
            {
                var convertFromString = TypeDescriptor.GetConverter(typeof(FontWeight)).ConvertFromString(value);
                if (convertFromString != null)
                    FontWeight = (FontWeight)convertFromString;
            }
        }
        /// <summary>
        /// 字体名称
        /// </summary>
        [CategoryAttribute("Basic"), Description("Font Family")]
        [XmlIgnore]
        public FontFamily FontFamily { get; set; }
        /// <summary>
        /// 字体名称
        /// </summary>
        [XmlAttribute]
        [Browsable(false)]
        public string StrFontFamily
        {
            get { return FontFamily == null ? string.Empty : FontFamily.ToString(); }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    var convertFromString = TypeDescriptor.GetConverter(typeof(FontFamily)).ConvertFromString(value);
                    if (convertFromString != null)
                        FontFamily = (FontFamily)convertFromString;
                }
            }
        }
        /// <summary>
        /// 前景色
        /// </summary>
        [CategoryAttribute("Basic"), Description("Fore Color")]
        [XmlIgnore]
        public Color ForeColor { get; set; }
        /// <summary>
        /// 前景色
        /// </summary>
        [XmlAttribute]
        [Browsable(false)]
        public string StrForeColor
        {
            get { return ForeColor.ToString(); }
            set
            {
                var convertFromString = TypeDescriptor.GetConverter(typeof(Color)).ConvertFromString(value);
                if (convertFromString != null)
                    ForeColor = (Color)convertFromString;
            }
        }
        /// <summary>
        /// 背景色
        /// </summary>
        [CategoryAttribute("Basic"), Description("Back Color")]
        [XmlIgnore]
        public Color BackColor { get; set; }
        /// <summary>
        /// 背景色
        /// </summary>
        [XmlAttribute]
        [Browsable(false)]
        public string StrBackColor
        {
            get { return BackColor.ToString(); }
            set
            {
                var convertFromString = TypeDescriptor.GetConverter(typeof(Color)).ConvertFromString(value);
                if (convertFromString != null)
                    BackColor = (Color)convertFromString;
            }
        }
        /// <summary>
        /// 宽度
        /// </summary>
        [CategoryAttribute("Basic"), Description("Width")]
        [XmlAttribute]
        public int Width { get; set; }
        /// <summary>
        /// 高度
        /// </summary>
        [CategoryAttribute("Basic"), Description("Height")]
        [XmlAttribute]
        public int Height { get; set; }
        /// <summary>
        /// 显示文本
        /// </summary>
        [Browsable(false)]
        [XmlIgnore]
        public new string Display
        {
            get
            {
                string strInfo = string.Empty;
                strInfo += FontSize + "-";
                strInfo += FontWeight + "-";
                strInfo += FontFamily + "-";
                strInfo += ForeColor + "-";
                strInfo += BackColor + "-";
                strInfo += Width + "-";
                strInfo += Height;
                return strInfo;
            }
        }
        /// <summary>
        /// 评分对象
        /// </summary>
        [Browsable(false)]
        [XmlIgnore]
        public ScoreObject ScoreObject { get; set; }
        /// <summary>
        /// 评分表
        /// </summary>
        [Browsable(false)]
        [XmlIgnore]
        public ScoreSheet ScoreSheet { get; set; }
       
        /// <summary>
        /// VisualStyle
        /// </summary>
        public VisualStyle()
        {

        }
        public override string ToString()
        {
            string strInfo = string.Empty;
            strInfo += FontSize + "-";
            strInfo += FontWeight + "-";
            strInfo += FontFamily + "-";
            strInfo += ForeColor + "-";
            strInfo += BackColor + "-";
            strInfo += Width + "-";
            strInfo += Height;
            return strInfo;
            //return string.Empty;
        }
        public override void GetPropertyList(ref List<ScoreProperty> listProperties)
        {
            base.GetPropertyList(ref listProperties);
            listProperties.Add(new ScoreProperty
            {
                ID =261,
                Name = "FontSize",
                Display = "FontSize",
                PropertyName = "FontSize",
                Description = "FontSize",
                Category = 1,
                Flag = ScorePropertyFlag.All,
                OrderID = 1,
                DataType = ScorePropertyDataType.Int,
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 262,
                Name = "FontWeight",
                Display = "FontWeight",
                PropertyName = "FontWeight",
                Description = "FontWeight",
                Category = 1,
                Flag = ScorePropertyFlag.All,
                OrderID = 2,
                DataType = ScorePropertyDataType.FontWeight,
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 263,
                Name = "FontFamily",
                Display = "FontFamily",
                PropertyName = "FontFamily",
                Description = "FontFamily",
                Category = 1,
                Flag = ScorePropertyFlag.All,
                OrderID = 3,
                DataType = ScorePropertyDataType.FontFamily,
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 264,
                Name = "ForeColor",
                Display = "ForeColor",
                PropertyName = "ForeColor",
                Description = "ForeColor",
                Category = 1,
                Flag = ScorePropertyFlag.All,
                OrderID = 4,
                DataType = ScorePropertyDataType.Color,
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 265,
                Name = "BackColor",
                Display = "BackColor",
                PropertyName = "BackColor",
                Description = "BackColor",
                Category = 1,
                Flag = ScorePropertyFlag.All,
                OrderID = 5,
                DataType = ScorePropertyDataType.Color,
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 266,
                Name = "Width",
                Display = "Width",
                PropertyName = "Width",
                Description = "Width",
                Category = 1,
                Flag = ScorePropertyFlag.All,
                OrderID = 6,
                DataType = ScorePropertyDataType.Int,
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 267,
                Name = "Height",
                Display = "Height",
                PropertyName = "Height",
                Description = "Height",
                Category = 1,
                Flag = ScorePropertyFlag.All,
                OrderID = 7,
                DataType = ScorePropertyDataType.Int,
            });
        }
    }
}
