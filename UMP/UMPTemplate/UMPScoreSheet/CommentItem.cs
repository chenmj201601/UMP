//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    65aad963-0f8c-40ed-a71c-da65a15ba597
//        CLR Version:              4.0.30319.18444
//        Name:                     CommentItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets
//        File Name:                CommentItem
//
//        created by Charley at 2014/6/10 13:50:12
//        http://www.voicecyber.com 
//
//======================================================================
//
//======================================================================
//备注项子项
//
//备注项子项是多值型备注项的子项
//属性Text指示备注文本
//
//属性编码范围：231 ~ 232
//
//各属性分组下排列序号：
//0：    4
//1:     1
//2:     无
//3:     无
//======================================================================
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace VoiceCyber.UMP.ScoreSheets
{
    /// <summary>
    /// 备注子项
    /// </summary>
    [XmlRoot(Namespace = "http://www.voicecyber.com/UMP/ScoreSheets/")]
    [DefaultProperty("Text"), Description("Comment Item")]
    public class CommentItem : ScoreObject
    {
        /// <summary>
        /// 显示文本
        /// </summary>
        [Category("Basic"), Description("Text")]
        [XmlAttribute]
        public string Text { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        [Browsable(false)]
        [XmlAttribute]
        public int OrderID { get; set; }
        /// <summary>
        /// 显示文本
        /// </summary>
        [Browsable(false)]
        [XmlIgnore]
        public new string Display { get { return Text; } }
        /// <summary>
        /// 备注
        /// </summary>
        [Browsable(false)]
        [XmlIgnore]
        public Comment Comment { get; set; }
        /// <summary>
        /// CommentItem
        /// </summary>
        public CommentItem()
        {

        }

        public override void GetPropertyList(ref List<ScoreProperty> listProperties)
        {
            base.GetPropertyList(ref listProperties);
            listProperties.Add(new ScoreProperty
            {
                ID = 231,
                Name = "Text",
                Display = "Text",
                PropertyName = "Text",
                Description = "Text",
                Category = 1,
                Flag = ScorePropertyFlag.All,
                OrderID = 1,
                DataType = ScorePropertyDataType.MString
            });
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Text))
            {
                return string.Format("{0}", Text);
            }
            return base.ToString();
        }
    }
}
