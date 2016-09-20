//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    eb59a7fe-b5ee-4158-8c2a-f4bcc178cf0a
//        CLR Version:              4.0.30319.18444
//        Name:                     TextComment
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets
//        File Name:                TextComment
//
//        created by Charley at 2014/6/10 15:29:30
//        http://www.voicecyber.com 
//
//======================================================================
//
//======================================================================
//文本型备注
//
//文本型备注是一种特殊的备注，可以直接输入备注文本
//属性Text指示备注的文本
//
//属性编码范围：221 ~ 222
//
//各属性分组下排列序号：
//0：    无
//1:     5 ~ 6
//2:     无
//3:     无
//======================================================================
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace VoiceCyber.UMP.ScoreSheets
{
    /// <summary>
    /// 文本型备注项
    /// </summary>
    [Description("Text Comment")]
    [XmlRoot(Namespace = "http://www.voicecyber.com/UMP/ScoreSheets/")]
    public class TextComment : Comment
    {
        /// <summary>
        /// 默认显示文本
        /// </summary>
        [Category("Basic"), Description("Default Text")]
        [XmlAttribute]
        public string DefaultText { get; set; }
        /// <summary>
        /// 备注内容
        /// </summary>
        [Browsable(false)]
        [XmlAttribute]
        public string Text { get; set; }
        /// <summary>
        /// TextComment
        /// </summary>
        public TextComment()
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
                ID = 221,
                Name = "DefaultText",
                Display = "DefaultText",
                PropertyName = "DefaultText",
                Description = "DefaultText",
                Category = 1,
                Flag = ScorePropertyFlag.All,
                OrderID = 5,
                DataType = ScorePropertyDataType.MString
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 222,
                Name = "Text",
                Display = "Text",
                PropertyName = "Text",
                Description = "Text",
                Category = 1,
                Flag = ScorePropertyFlag.All,
                OrderID = 6,
                DataType = ScorePropertyDataType.MString
            });
        }
    }
}
