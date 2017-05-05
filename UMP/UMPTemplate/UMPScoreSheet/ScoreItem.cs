//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    d41aa6d2-820c-47f2-876b-795c10c137cb
//        CLR Version:              4.0.30319.18444
//        Name:                     ScoreItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets
//        File Name:                ScoreItem
//
//        created by Charley at 2014/6/10 13:47:47
//        http://www.voicecyber.com 
//
//======================================================================
//
//======================================================================
//评分项
//
//评分项是评分类别和评分标准的基类，表示一个评分项目
//Title属性指示评分项的标题，Display属性返回Title值
//每个评分项都有总分，得分以及实际得分
//
//属性编码范围：101 ~ 124
//
//各属性分组下排列序号：
//0:    4 ~ 13
//1:    1 ~ 5   
//2:    1 ~ 6
//3:    1 ~ 2
//======================================================================
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace VoiceCyber.UMP.ScoreSheets
{
    /// <summary>
    /// 评分项
    /// 
    /// </summary>
    [DefaultProperty("Title"), Description("Score Item")]
    [XmlRoot(Namespace = "http://www.voicecyber.com/UMP/ScoreSheets/")]
    [XmlInclude(typeof(ScoreGroup))]
    [XmlInclude(typeof(Standard))]
    public class ScoreItem : ScoreObject
    {
        /// <summary>
        /// 该评分项在整个评分表中的编号
        /// </summary>
        [Category("Basic"), Description("Item ID")]
        [XmlAttribute]
        public int ItemID { get; set; }
        /// <summary>
        /// 图标名称
        /// </summary>
        [Category("Basic"), Description("Icon Name")]
        [XmlAttribute]
        public string IconName { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        [Category("Basic"), Description("Title")]
        [XmlAttribute]
        public string Title { get; set; }
        /// <summary>
        /// 显示风格
        /// </summary>
        [Category("Basic"), Description("View Classic")]
        [XmlAttribute]
        public ScoreItemClassic ViewClassic { get; set; }
        /// <summary>
        /// 分值类型
        /// </summary>
        [Category("Basic"), Description("Score Type")]
        [XmlAttribute]
        public ScoreType ScoreType { get; set; }
        /// <summary>
        /// 总分
        /// </summary>
        [Category("Basic"), Description("Total Score")]
        [XmlAttribute]
        public double TotalScore { get; set; }
        /// <summary>
        /// 实际得分
        /// </summary>
        [Browsable(false)]
        [XmlAttribute]
        public double Score { get; set; }
        /// <summary>
        /// 实际得分（通过控制项计算后的得分）
        /// </summary>
        [Browsable(false)]
        [XmlAttribute]
        public double RealScore { get; set; }
        /// <summary>
        /// 是否忽略计算入总分
        /// </summary>
        [Category("Basic"), Description("Not calculate to total score")]
        [XmlAttribute]
        public bool IsAbortScore { get; set; }
        private int mControlFlag;
        /// <summary>
        /// 控制标志
        /// 00      无控制标志
        /// 1bit    控制项源
        /// 2bit    控制项目标
        /// </summary>
        [Browsable(false)]
        [XmlAttribute]
        public int ControlFlag
        {
            get
            {
                int flag = 0;
                ScoreSheet scoreSheet = ScoreSheet;
                if (scoreSheet == null)
                {
                    return flag;
                }
                for (int i = 0; i < scoreSheet.ControlItems.Count; i++)
                {
                    ControlItem controlItem = scoreSheet.ControlItems[i];
                    if (controlItem.SourceID == ID)
                    {
                        flag = flag | 1;
                    }
                    if (controlItem.TargetID == ID)
                    {
                        flag = flag | 2;
                    }
                }
                mControlFlag = flag;
                return flag;
            }
            set { mControlFlag = value; }
        }
        /// <summary>
        /// 得分是否被控制项控制
        /// </summary>
        [Browsable(false)]
        [XmlAttribute]
        public bool IsScoreControled { get; set; }
        /// <summary>
        /// 是否关键项
        /// </summary>
        [Category("Extented"), Description("Key item or not")]
        [XmlAttribute]
        public bool IsKeyItem { get; set; }
        /// <summary>
        /// 是否允许为空
        /// </summary>
        [Category("Extented"), Description("Allow null value or not")]
        [XmlAttribute]
        public bool IsAllowNA { get; set; }
        /// <summary>
        /// 打分时是否标记为空（只有IsAllowNA为True才有效）
        /// </summary>
        [Browsable(false)]
        [XmlAttribute]
        public bool IsNA { get; set; }
        /// <summary>
        /// 是否跳转项
        /// </summary>
        [Category("Extented"), Description("Jump item or not")]
        [XmlAttribute]
        public bool IsJumpItem { get; set; }
        /// <summary>
        /// 是否附加项
        /// </summary>
        [Category("Extented"), Description("Addtion item or not")]
        [XmlAttribute]
        public bool IsAddtionItem { get; set; }
        /// <summary>
        /// 是否启用分制
        /// </summary>
        [Category("Extented"), Description("Use PointSystem or not")]
        [XmlAttribute]
        public bool UsePointSystem { get; set; }
        /// <summary>
        /// 是否允许修改自动评分项成绩
        /// </summary>
        [Category("Extented"), Description("Allow modify auto standard score")]
        [XmlAttribute]
        public bool AllowModifyScore { get; set; }
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
        /// 序号
        /// </summary>
        [Browsable(false)]
        [XmlAttribute]
        public int OrderID { get; set; }
        /// <summary>
        /// 描述信息
        /// </summary>
        [Category("Basic"), Description("Description")]
        [XmlAttribute]
        public string Description { get; set; }
        /// <summary>
        /// 提示信息
        /// </summary>
        [Category("Basic"), Description("Tip")]
        [XmlAttribute]
        public string Tip { get; set; }

        /// <summary>
        /// 评分项的深度，评分表的深度为0
        /// </summary>
        [Category("Basic"), Description("Level of ScoreItem, for ScoreSheet is 0")]
        [XmlAttribute]
        public int Level { get; set; }
        /// <summary>
        /// 显示文本
        /// </summary>
        [Browsable(false)]
        [XmlIgnore]
        public new string Display
        {
            get { return Title; }
        }
        /// <summary>
        /// 父级评分项
        /// </summary>
        [Browsable(false)]
        [XmlIgnore]
        public ScoreItem Parent { get; set; }
        /// <summary>
        /// 所在的评分表
        /// </summary>
        [Browsable(false)]
        [XmlIgnore]
        public ScoreSheet ScoreSheet { get; set; }
        /// <summary>
        /// 备注列表
        /// </summary>
        [Browsable(false)]
        [XmlArray]
        public List<Comment> Comments { get; set; }
        /// <summary>
        /// ScoreItem
        /// </summary>
        public ScoreItem()
        {
            IsAllowNA = false;
            Score = 0.0;
            RealScore = 0.0;
            Comments = new List<Comment>();
        }
        /// <summary>
        /// 检查完整性，正确性
        /// </summary>
        /// <returns></returns>
        public virtual CheckValidResult CheckValid()
        {
            CheckValidResult result = new CheckValidResult();
            result.Code = 0;
            if (TotalScore < 0)
            {
                result.Code = 201;
                result.ScoreObject = this;
                result.Message = string.Format("TotalScore invalid.\t{0}", ID);
            }
            if (string.IsNullOrEmpty(Title))
            {
                result.Code = CheckValidResultCodes.TITLE_EMPTY;
                result.ScoreObject = this;
                result.Message = string.Format("Title can not be empty.\t{0}", ID);
            }
            return result;
        }
        /// <summary>
        /// 检查输入的有效性
        /// </summary>
        /// <returns></returns>
        public virtual CheckValidResult CheckInputValid()
        {
            CheckValidResult result = new CheckValidResult();
            result.Code = 0;
            return result;
        }
        /// <summary>
        /// 计算得分
        /// </summary>
        public virtual double CaculateScore()
        {
            return Score;
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
                ID = 101,
                Name = "ItemID",
                Display = "ItemID",
                PropertyName = "ItemID",
                Description = "Item ID",
                Category = 0,
                Flag = ScorePropertyFlag.Normal,
                OrderID = 4,
                DataType = ScorePropertyDataType.Int
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 102,
                Name = "IconName",
                Display = "IconName",
                PropertyName = "IconName",
                Description = "Icon Name",
                Category = 0,
                Flag = ScorePropertyFlag.Normal,
                OrderID = 5,
                DataType = ScorePropertyDataType.Icon
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 103,
                Name = "Title",
                Display = "Title",
                PropertyName = "Title",
                Description = "Title",
                Category = 1,
                Flag = ScorePropertyFlag.All,
                OrderID = 1,
                DataType = ScorePropertyDataType.MString
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 104,
                Name = "ViewClassic",
                Display = "ViewClassic",
                PropertyName = "ViewClassic",
                Description = "ViewClassic",
                Category = 1,
                Flag = ScorePropertyFlag.All,
                OrderID = 2,
                DataType = ScorePropertyDataType.Enum,
                ValueType = typeof(ScoreItemClassic)
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 105,
                Name = "ScoreType",
                Display = "ScoreType",
                PropertyName = "ScoreType",
                Description = "ScoreType",
                Category = 1,
                Flag = ScorePropertyFlag.All,
                OrderID = 3,
                DataType = ScorePropertyDataType.Enum,
                ValueType = typeof(ScoreType)
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 106,
                Name = "TotalScore",
                Display = "TotalScore",
                PropertyName = "TotalScore",
                Description = "TotalScore",
                Category = 1,
                Flag = ScorePropertyFlag.All,
                OrderID = 4,
                DataType = ScorePropertyDataType.Double
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 107,
                Name = "Score",
                Display = "Score",
                PropertyName = "Score",
                Description = "Score",
                Category = 0,
                Flag = ScorePropertyFlag.Normal,
                OrderID = 6,
                DataType = ScorePropertyDataType.Double
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 108,
                Name = "RealScore",
                Display = "RealScore",
                PropertyName = "RealScore",
                Description = "RealScore",
                Category = 0,
                Flag = ScorePropertyFlag.Normal,
                OrderID = 7,
                DataType = ScorePropertyDataType.Double
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 109,
                Name = "IsAbortScore",
                Display = "IsAbortScore",
                PropertyName = "IsAbortScore",
                Description = "IsAbortScore",
                Category = 2,
                Flag = ScorePropertyFlag.All,
                OrderID = 1,
                DataType = ScorePropertyDataType.Bool
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 110,
                Name = "ControlFlag",
                Display = "ControlFlag",
                PropertyName = "ControlFlag",
                Description = "ControlFlag",
                Category = 0,
                Flag = ScorePropertyFlag.Normal,
                OrderID = 8,
                DataType = ScorePropertyDataType.Int
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 111,
                Name = "IsScoreControled",
                Display = "IsScoreControled",
                PropertyName = "IsScoreControled",
                Description = "IsScoreControled",
                Category = 0,
                Flag = ScorePropertyFlag.Normal,
                OrderID = 9,
                DataType = ScorePropertyDataType.Bool
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 112,
                Name = "IsKeyItem",
                Display = "IsKeyItem",
                PropertyName = "IsKeyItem",
                Description = "IsKeyItem",
                Category = 2,
                Flag = ScorePropertyFlag.All,
                OrderID = 2,
                DataType = ScorePropertyDataType.Bool
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 113,
                Name = "IsAllowNA",
                Display = "IsAllowNA",
                PropertyName = "IsAllowNA",
                Description = "IsAllowNA",
                Category = 2,
                Flag = ScorePropertyFlag.All,
                OrderID = 3,
                DataType = ScorePropertyDataType.Bool
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 114,
                Name = "IsNA",
                Display = "IsNA",
                PropertyName = "IsNA",
                Description = "IsNA",
                Category = 0,
                Flag = ScorePropertyFlag.Normal,
                OrderID = 10,
                DataType = ScorePropertyDataType.Bool
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 115,
                Name = "IsJumpItem",
                Display = "IsJumpItem",
                PropertyName = "IsJumpItem",
                Description = "IsJumpItem",
                Category = 2,
                Flag = ScorePropertyFlag.Normal,
                OrderID = 4,
                DataType = ScorePropertyDataType.Bool
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 116,
                Name = "IsAddtionItem",
                Display = "IsAddtionItem",
                PropertyName = "IsAddtionItem",
                Description = "IsAddtionItem",
                Category = 2,
                Flag = ScorePropertyFlag.All,
                OrderID = 5,
                DataType = ScorePropertyDataType.Bool
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 117,
                Name = "UsePointSystem",
                Display = "UsePointSystem",
                PropertyName = "UsePointSystem",
                Description = "UsePointSystem",
                Category = 2,
                Flag = ScorePropertyFlag.Normal,
                OrderID = 6,
                DataType = ScorePropertyDataType.Bool
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 118,
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
                ID = 119,
                Name = "PanelStyle",
                Display = "PanelStyle",
                PropertyName = "PanelStyle",
                Description = "PanelStyle",
                Category = 3,
                Flag = ScorePropertyFlag.All,
                OrderID = 2,
                DataType = ScorePropertyDataType.Style
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 120,
                Name = "OrderID",
                Display = "OrderID",
                PropertyName = "OrderID",
                Description = "OrderID",
                Category = 0,
                Flag = ScorePropertyFlag.Normal,
                OrderID = 11,
                DataType = ScorePropertyDataType.Int
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 121,
                Name = "Description",
                Display = "Description",
                PropertyName = "Description",
                Description = "Description",
                Category = 1,
                Flag = ScorePropertyFlag.All,
                OrderID = 5,
                DataType = ScorePropertyDataType.MString
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 122,
                Name = "Tip",
                Display = "Tip",
                PropertyName = "Tip",
                Description = "Tip",
                Category = 0,
                Flag = ScorePropertyFlag.Normal,
                OrderID = 12,
                DataType = ScorePropertyDataType.MString
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 123,
                Name = "Level",
                Display = "Level",
                PropertyName = "Level",
                Description = "Level",
                Category = 0,
                Flag = ScorePropertyFlag.Xml,
                OrderID = 13,
                DataType = ScorePropertyDataType.Double
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 124,
                Name = "AllowModifyScore",
                Display = "AllowModifyScore",
                PropertyName = "AllowModifyScore",
                Description = "AllowModifyScore",
                Category = 2,
                Flag = ScorePropertyFlag.All,
                OrderID = 7,
                DataType = ScorePropertyDataType.Bool
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
            for (int i = 0; i < Comments.Count; i++)
            {
                var comment = Comments[i];
                listScoreObjects.Add(comment);
                comment.GetAllScoreObject(ref listScoreObjects);
            }
        }

        /// <summary>
        /// 获取此项控制的控制项列表
        /// </summary>
        /// <param name="listControlItems"></param>
        public void GetControlScoreItem(ref List<ControlItem> listControlItems)
        {
            if ((ControlFlag & 1) > 0)
            {
                var scoreSheet = ScoreSheet;
                if (scoreSheet == null) { return; }
                for (int i = 0; i < scoreSheet.ControlItems.Count; i++)
                {
                    var controlItem = scoreSheet.ControlItems[i];
                    if (controlItem.SourceID == ID)
                    {
                        listControlItems.Add(controlItem);
                    }
                }
            }
        }
        /// <summary>
        /// 获取此项被控制的控制项列表
        /// </summary>
        /// <param name="listControlItems"></param>
        public void GetControledScoreItem(ref List<ControlItem> listControlItems)
        {
            if ((ControlFlag & 2) > 0)
            {
                var scoreSheet = ScoreSheet;
                if (scoreSheet == null) { return; }
                for (int i = 0; i < scoreSheet.ControlItems.Count; i++)
                {
                    var controlItem = scoreSheet.ControlItems[i];
                    if (controlItem.TargetID == ID)
                    {
                        listControlItems.Add(controlItem);
                    }
                }
            }
        }

        /// <summary>
        /// Init
        /// </summary>
        public override void Init()
        {
            base.Init();
            for (int i = 0; i < Comments.Count; i++)
            {
                Comments[i].ScoreItem = this;
                Comments[i].ScoreSheet = ScoreSheet;
                Comments[i].OrderID = i;
                Comments[i].Init();
            }
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
        /// <summary>
        /// 重置成绩
        /// </summary>
        public virtual void ResetScore()
        {
            Score = 0;
        }
        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
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
