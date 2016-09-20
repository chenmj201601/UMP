//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    9f67d97f-a116-471e-b91f-9b1eb1f48d00
//        CLR Version:              4.0.30319.18444
//        Name:                     ItemComment
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets
//        File Name:                ItemComment
//
//        created by Charley at 2014/6/10 15:30:24
//        http://www.voicecyber.com 
//
//======================================================================
//
//======================================================================
//多值型备注项
//
//多值型备注项是一种特殊的备注项，可以从列出多个文本选择一个作为备注文本
//属性ValueItems指示备注文本列表
//
//属性编码范围：226 ~ 229
//
//各属性分组下排列序号：
//0：    4 ~ 5
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
    /// 多项风格的备注项
    /// </summary>
    [Description("Item Comment")]
    [XmlRoot(Namespace = "http://www.voicecyber.com/UMP/ScoreSheets/")]
    public class ItemComment : Comment
    {
        /// <summary>
        /// 默认选中项的序号
        /// </summary>
        [Category("Basic"), Description("DefaultIndex")]
        [XmlAttribute]
        public int DefaultIndex { get; set; }
        /// <summary>
        /// 默认选中项
        /// </summary>
        [Browsable(false)]
        [XmlElement]
        public CommentItem DefaultItem { get; set; }
        /// <summary>
        /// 当前项
        /// </summary>
        [Browsable(false)]
        [XmlElement]
        public CommentItem SelectItem { get; set; }
        /// <summary>
        /// 子项
        /// </summary>
        [Browsable(false)]
        [XmlArray]
        public List<CommentItem> ValueItems { get; set; }
        /// <summary>
        /// ItemComment
        /// </summary>
        public ItemComment()
        {
            ValueItems = new List<CommentItem>();
        }

        public override void GetPropertyList(ref List<ScoreProperty> listProperties)
        {
            base.GetPropertyList(ref listProperties);
            listProperties.Add(new ScoreProperty
            {
                ID = 226,
                Name = "DefaultIndex",
                Display = "DefaultIndex",
                PropertyName = "DefaultIndex",
                Description = "DefaultIndex",
                Category = 1,
                Flag = ScorePropertyFlag.All,
                OrderID = 5,
                DataType = ScorePropertyDataType.Int
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 227,
                Name = "DefaultItem",
                Display = "DefaultItem",
                PropertyName = "DefaultItem",
                Description = "DefaultItem",
                Category = 0,
                Flag = ScorePropertyFlag.Normal,
                OrderID = 4,
                DataType = ScorePropertyDataType.MString
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 228,
                Name = "SelectValue",
                Display = "SelectValue",
                PropertyName = "SelectValue",
                Description = "SelectValue",
                Category = 0,
                Flag = ScorePropertyFlag.Normal,
                OrderID = 5,
                DataType = ScorePropertyDataType.MString
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 229,
                Name = "ValueItems",
                Display = "ValueItems",
                PropertyName = "ValueItems",
                Description = "ValueItems",
                Category = 1,
                Flag = ScorePropertyFlag.All,
                OrderID = 6,
                DataType = ScorePropertyDataType.CommentItem
            });
        }

        public override void GetAllScoreObject(ref List<ScoreObject> listScoreObjects)
        {
            base.GetAllScoreObject(ref listScoreObjects);
            for (int i = 0; i < ValueItems.Count; i++)
            {
                var item = ValueItems[i];
                listScoreObjects.Add(item);
                item.GetAllScoreObject(ref listScoreObjects);
            }
        }

        public override void Init()
        {
            base.Init();
            for (int i = 0; i < ValueItems.Count; i++)
            {
                ValueItems[i].Comment = this;
                ValueItems[i].OrderID = i;
            }
        }
    }
}
