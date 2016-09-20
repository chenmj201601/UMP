//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    96675058-322c-43ed-bd72-de19907f37a0
//        CLR Version:              4.0.30319.18444
//        Name:                     Comment
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets
//        File Name:                Comment
//
//        created by Charley at 2014/6/10 13:49:51
//        http://www.voicecyber.com 
//
//======================================================================
//
//======================================================================
//备注项
//
//备注项可附加在评分项中，用来对评分项做备注
//同一评分项下可有多个备注项
//属性Style指示备注项的样式（文本型，多值型）
//
//属性编码范围：211 ~ 216
//
//各属性分组下排列序号：
//0：    无
//1:     1 ~ 4
//2:     无
//3:     1 ~ 2
//======================================================================
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace VoiceCyber.UMP.ScoreSheets
{
    /// <summary>
    /// 备注信息
    /// </summary>
    [DefaultProperty("Title"), Description("Comment")]
    [XmlRoot(Namespace = "http://www.voicecyber.com/UMP/ScoreSheets/")]
    [XmlInclude(typeof(TextComment))]
    [XmlInclude(typeof(ItemComment))]
    public class Comment : ScoreObject
    {
        /// <summary>
        /// 备注风格
        /// </summary>
        [Browsable(false)]
        [XmlAttribute]
        public CommentStyle Style { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        [Category("Basic"), Description("Title")]
        [XmlAttribute]
        public string Title { get; set; }
        /// <summary>
        /// 描述信息
        /// </summary>
        [Category("Basic"), Description("Description")]
        [XmlAttribute]
        public string Description { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        [Browsable(false)]
        [XmlAttribute]
        public int OrderID { get; set; }
        /// <summary>
        /// 标题样式
        /// </summary>
        [Category("Style"), Description("Title Style")]
        [XmlElement]
        public VisualStyle TitleStyle { get; set; }
        /// <summary>
        /// 面板样式
        /// </summary>
        [Category("Style"), Description("Panel Style")]
        [XmlElement]
        public VisualStyle PanelStyle { get; set; }
        /// <summary>
        /// 显示文本
        /// </summary>
        [Browsable(false)]
        [XmlIgnore]
        public new string Display { get { return Title; } }
        /// <summary>
        /// 评分项
        /// </summary>
        [Browsable(false)]
        [XmlIgnore]
        public ScoreItem ScoreItem { get; set; }
        /// <summary>
        /// 评分表
        /// </summary>
        [Browsable(false)]
        [XmlIgnore]
        public ScoreSheet ScoreSheet { get; set; }
        /// <summary>
        /// Comment
        /// </summary>
        public Comment()
        {

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
                ID = 211,
                Name = "Style",
                Display = "Style",
                PropertyName = "Style",
                Description = "Style",
                Category = 1,
                Flag = ScorePropertyFlag.Normal,
                OrderID = 1,
                DataType = ScorePropertyDataType.Enum,
                ValueType = typeof(CommentStyle)
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 212,
                Name = "Title",
                Display = "Title",
                PropertyName = "Title",
                Description = "Title",
                Category = 1,
                Flag = ScorePropertyFlag.All,
                OrderID = 2,
                DataType = ScorePropertyDataType.MString
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 213,
                Name = "Description",
                Display = "Description",
                PropertyName = "Description",
                Description = "Description",
                Category = 1,
                Flag = ScorePropertyFlag.All,
                OrderID = 3,
                DataType = ScorePropertyDataType.MString
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 214,
                Name = "OrderID",
                Display = "OrderID",
                PropertyName = "OrderID",
                Description = "OrderID",
                Category = 1,
                Flag = ScorePropertyFlag.Xml,
                OrderID = 4,
                DataType = ScorePropertyDataType.Int
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 215,
                Name = "TitleStyle",
                Display = "TitleStyle",
                PropertyName = "TitleStyle",
                Description = "TitleStyle",
                Category = 3,
                Flag = ScorePropertyFlag.All,
                OrderID = 1,
                DataType = ScorePropertyDataType.Style
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 216,
                Name = "PanelStyle",
                Display = "PanelStyle",
                PropertyName = "PanelStyle",
                Description = "PanelStyle",
                Category = 3,
                Flag = ScorePropertyFlag.All,
                OrderID = 2,
                DataType = ScorePropertyDataType.Style
            });
        }

        public override void GetAllScoreObject(ref List<ScoreObject> listScoreObjects)
        {
            base.GetAllScoreObject(ref listScoreObjects);
            if (TitleStyle != null)
            {
                listScoreObjects.Add(TitleStyle);
                TitleStyle.GetAllScoreObject(ref listScoreObjects);
            }
            if (PanelStyle != null)
            {
                listScoreObjects.Add(PanelStyle);
                PanelStyle.GetAllScoreObject(ref listScoreObjects);
            }
        }

        public override void Init()
        {
            base.Init();

            if (TitleStyle != null)
            {
                TitleStyle.ScoreObject = this;
                TitleStyle.ScoreSheet = ScoreSheet;
                TitleStyle.Init();
            }
            if (PanelStyle != null)
            {
                PanelStyle.ScoreObject = this;
                PanelStyle.ScoreSheet = ScoreSheet;
                PanelStyle.Init();
            }
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Title))
            {
                return string.Format("{0}", Title);
            }
            return base.ToString();
        }
    }
}
