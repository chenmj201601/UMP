//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    c48da7ae-4ba0-4826-8414-24e5ee79a3f0
//        CLR Version:              4.0.30319.18444
//        Name:                     ControlItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets
//        File Name:                ControlItem
//
//        created by Charley at 2014/6/30 15:41:34
//        http://www.voicecyber.com 
//
//======================================================================
//
//======================================================================
//控制项
//
//控制项可附加到评分表中，表示评分表的子项之间的控制关系
//属性Source指示控制源
//属性Targe指示控制目标
//
//属性编码范围：241 ~ 251
//
//各属性分组下排列序号：
//0：    无
//1:     1 ~ 11
//2:     无
//3:     无
//======================================================================
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;

namespace VoiceCyber.UMP.ScoreSheets
{
    /// <summary>
    /// 控制项
    /// </summary>
    [DefaultProperty("Title"), Description("Control Item")]
    [XmlRoot(Namespace = "http://www.voicecyber.com/UMP/ScoreSheets/")]
    public class ControlItem : ScoreObject
    {
        /// <summary>
        /// 标题
        /// </summary>
        [Category("Basic"), Description("Title")]
        [XmlAttribute]
        public string Title { get; set; }
        /// <summary>
        /// 控制源判定方式
        /// </summary>
        [Category("Basic"), Description("Juge Type")]
        [XmlAttribute]
        public JugeType JugeType { get; set; }
        /// <summary>
        /// 控制源值
        /// </summary>
        [Category("Basic"), Description("Juge Value")]
        [XmlAttribute]
        public double JugeValue1 { get; set; }
        /// <summary>
        /// 控制源值（仅在JugeType为Between时有效）
        /// </summary>
        [Category("Basic"), Description("Juge Value")]
        [XmlAttribute]
        public double JugeValue2 { get; set; }
        /// <summary>
        /// 是否使用分制
        /// </summary>
        [Category("Basic"), Description("Use PointSystem")]
        [XmlAttribute]
        public bool UsePonitSystem { get; set; }
        /// <summary>
        /// 控制源
        /// </summary>
        [Category("Basic"), Description("Control Source")]
        [XmlIgnore]
        public ScoreItem Source
        {
            get
            {
                ScoreSheet scoreSheet = ScoreSheet;
                if (scoreSheet != null)
                {
                    List<ScoreItem> listScoreItems = new List<ScoreItem>();
                    scoreSheet.GetAllScoreItem(ref listScoreItems);
                    ScoreItem scoreItem = listScoreItems.FirstOrDefault(i => i.ID == SourceID);
                    if (scoreItem != null)
                    {
                        return scoreItem;
                    }
                }
                return null;
            }
            set
            {
                if (value != null)
                {
                    SourceItemID = value.ItemID;
                    SourceID = value.ID;
                }

            }
        }
        /// <summary>
        /// 控制源ID
        /// </summary>
        [Browsable(false)]
        [XmlAttribute]
        public long SourceID
        {
            get;
            set;
        }

        /// <summary>
        /// 控制源编码(在当前评分表下的编码)
        /// </summary>
        [Browsable(false)]
        [XmlAttribute]
        public int SourceItemID
        {
            get;
            set;
        }

        /// <summary>
        /// 控制目标
        /// </summary>
        [Category("Basic"), Description("Control Target")]
        [XmlIgnore]
        public ScoreItem Target
        {
            get
            {
                ScoreSheet scoreSheet = ScoreSheet;
                if (scoreSheet != null)
                {
                    List<ScoreItem> listScoreItems = new List<ScoreItem>();
                    scoreSheet.GetAllScoreItem(ref listScoreItems);
                    listScoreItems.Add(scoreSheet);
                    ScoreItem scoreItem = listScoreItems.FirstOrDefault(i => i.ID == TargetID);
                    if (scoreItem != null)
                    {
                        return scoreItem;
                    }
                }
                return null;
            }
            set
            {
                if (value != null)
                {
                    TargetItemID = value.ItemID;
                    TargetID = value.ID;
                }
            }
        }
        /// <summary>
        /// 控制目标ID
        /// </summary>
        [Browsable(false)]
        [XmlAttribute]
        public long TargetID
        {
            get;
            set;
        }
        /// <summary>
        /// 控制目标编码(在当前评分表下的编码)
        /// </summary>
        [Browsable(false)]
        [XmlAttribute]
        public int TargetItemID
        {
            get;
            set;
        }

        /// <summary>
        /// 控制目标的类型
        /// 1       评分表
        /// 2       评分表的子项
        /// </summary>
        [Browsable(false)]
        [XmlAttribute]
        public int TargetType { get; set; }
        /// <summary>
        /// 控制目标控制方式
        /// </summary>
        [Category("Basic"), Description("Change Type")]
        [XmlAttribute]
        public ChangeType ChangeType { get; set; }
        /// <summary>
        /// 控制目标值
        /// </summary>
        [Category("Basic"), Description("Change Value")]
        [XmlAttribute]
        public double ChangeValue { get; set; }
        /// <summary>
        /// 序号
        /// </summary>
        [Browsable(false)]
        [XmlAttribute]
        public int OrderID { get; set; }
        /// <summary>
        /// 显示文本
        /// </summary>
        [Browsable(false)]
        [XmlIgnore]
        public new string Display { get { return Title; } }
        /// <summary>
        /// 所在评分表
        /// </summary>
        [Browsable(false)]
        [XmlIgnore]
        public ScoreSheet ScoreSheet { get; set; }
        /// <summary>
        /// ControlItem
        /// </summary>
        public ControlItem()
        {
            JugeType = JugeType.Equal;
            ChangeType = ChangeType.Equal;
        }
        /// <summary>
        /// 根据控制项改变得分
        /// </summary>
        public void ControlScore()
        {
            ScoreItem sourceItem = Source;
            ScoreItem targetItem = Target;
            if (sourceItem == null || targetItem == null)
            {
                return;
            }
            bool bIsChange = false;
            double score = sourceItem.Score;
            if (sourceItem.IsScoreControled)
            {
                //如果源项已经被其他控制项控制得分了，那么比较分数的时候要使用RealScore，即使用控制后的分数
                score = sourceItem.RealScore;
            }
            switch (JugeType)
            {
                case JugeType.Equal:
                    if (score == JugeValue1)
                    {
                        bIsChange = true;
                    }
                    break;
                case JugeType.Lower:
                    if (score < JugeValue1)
                    {
                        bIsChange = true;
                    }
                    break;
                case JugeType.LowerEqual:
                    if (score <= JugeValue1)
                    {
                        bIsChange = true;
                    }
                    break;
                case JugeType.Upper:
                    if (score > JugeValue1)
                    {
                        bIsChange = true;
                    }
                    break;
                case JugeType.UpperEqual:
                    if (score >= JugeValue1)
                    {
                        bIsChange = true;
                    }
                    break;
                case JugeType.Between:
                    if (score >= JugeValue1 && score <= JugeValue2)
                    {
                        bIsChange = true;
                    }
                    break;
            }
            if (bIsChange)
            {
                switch (ChangeType)
                {
                    case ChangeType.Equal:
                        targetItem.RealScore = ChangeValue;
                        targetItem.IsScoreControled = true;
                        break;
                    case ChangeType.Sum:
                        targetItem.RealScore += ChangeValue;
                        targetItem.IsScoreControled = true;
                        break;
                    case ChangeType.Sub:
                        targetItem.RealScore -= ChangeValue;
                        targetItem.IsScoreControled = true;
                        break;
                    case ChangeType.Multi:
                        targetItem.RealScore *= ChangeValue;
                        targetItem.IsScoreControled = true;
                        break;
                    case ChangeType.Div:
                        targetItem.RealScore /= ChangeValue;
                        targetItem.IsScoreControled = true;
                        break;
                    case ChangeType.NA:
                        targetItem.IsNA = true;
                        targetItem.IsScoreControled = true;
                        break;
                }
            }
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
                ID = 241,
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
                ID = 242,
                Name = "JugeType",
                Display = "JugeType",
                PropertyName = "JugeType",
                Description = "JugeType",
                Category = 1,
                Flag = ScorePropertyFlag.All,
                OrderID = 2,
                DataType = ScorePropertyDataType.Enum,
                ValueType = typeof(JugeType)
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 243,
                Name = "JugeValue1",
                Display = "JugeValue1",
                PropertyName = "JugeValue1",
                Description = "JugeValue1",
                Category = 1,
                Flag = ScorePropertyFlag.All,
                OrderID = 3,
                DataType = ScorePropertyDataType.Double
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 244,
                Name = "JugeValue2",
                Display = "JugeValue2",
                PropertyName = "JugeValue2",
                Description = "JugeValue2",
                Category = 1,
                Flag = ScorePropertyFlag.All,
                OrderID = 4,
                DataType = ScorePropertyDataType.Double
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 245,
                Name = "Source",
                Display = "Source",
                PropertyName = "Source",
                Description = "Source",
                Category = 1,
                Flag = ScorePropertyFlag.Visible | ScorePropertyFlag.Enable | ScorePropertyFlag.Copy,
                OrderID = 5,
                DataType = ScorePropertyDataType.ScoreItem,
                ValueType = typeof(ScoreItem),
                DisplayName = "Title"
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 246,
                Name = "SourceID",
                Display = "SourceID",
                PropertyName = "SourceID",
                Description = "SourceID",
                Category = 1,
                Flag = ScorePropertyFlag.Normal,
                OrderID = 6,
                DataType = ScorePropertyDataType.Double
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 247,
                Name = "Target",
                Display = "Target",
                PropertyName = "Target",
                Description = "Target",
                Category = 1,
                Flag = ScorePropertyFlag.Visible | ScorePropertyFlag.Enable | ScorePropertyFlag.Copy,
                OrderID = 7,
                DataType = ScorePropertyDataType.ScoreItem,
                ValueType = typeof(ScoreItem),
                DisplayName = "Title"
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 248,
                Name = "TargetID",
                Display = "TargetID",
                PropertyName = "TargetID",
                Description = "TargetID",
                Category = 1,
                Flag = ScorePropertyFlag.Normal,
                OrderID = 8,
                DataType = ScorePropertyDataType.Double
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 249,
                Name = "ChangeType",
                Display = "ChangeType",
                PropertyName = "ChangeType",
                Description = "ChangeType",
                Category = 1,
                Flag = ScorePropertyFlag.All,
                OrderID = 9,
                DataType = ScorePropertyDataType.Enum,
                ValueType = typeof(ChangeType)
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 250,
                Name = "ChangeValue",
                Display = "ChangeValue",
                PropertyName = "ChangeValue",
                Description = "ChangeValue",
                Category = 1,
                Flag = ScorePropertyFlag.All,
                OrderID = 10,
                DataType = ScorePropertyDataType.Double
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 251,
                Name = "OrderID",
                Display = "OrderID",
                PropertyName = "OrderID",
                Description = "OrderID",
                Category = 1,
                Flag = ScorePropertyFlag.Normal,
                OrderID = 11,
                DataType = ScorePropertyDataType.Int
            });
        }

        public override void Init()
        {
            base.Init();

            ScoreSheet scoreSheet = ScoreSheet;
            if (scoreSheet != null)
            {
                List<ScoreItem> listScoreItems = new List<ScoreItem>();
                scoreSheet.GetAllScoreItem(ref listScoreItems);
                listScoreItems.Add(scoreSheet);
                ScoreItem scoreItem = listScoreItems.FirstOrDefault(i => i.ID == SourceID);
                if (scoreItem != null)
                {
                    Source = scoreItem;
                }
                scoreItem = listScoreItems.FirstOrDefault(i => i.ID == TargetID);
                if (scoreItem != null)
                {
                    Target = scoreItem;
                }
            }
        }

        public void InitUseItemID()
        {
            ScoreSheet scoreSheet = ScoreSheet;
            if (scoreSheet != null)
            {
                List<ScoreItem> listScoreItems = new List<ScoreItem>();
                scoreSheet.GetAllScoreItem(ref listScoreItems);
                listScoreItems.Add(scoreSheet);
                ScoreItem scoreItem = listScoreItems.FirstOrDefault(i => i.ItemID == SourceItemID);
                if (scoreItem != null)
                {
                    Source = scoreItem;
                }
                scoreItem = listScoreItems.FirstOrDefault(i => i.ItemID == TargetItemID);
                if (scoreItem != null)
                {
                    Target = scoreItem;
                }
            }
        }

        /// <summary>
        /// 获取控制描述信息
        /// </summary>
        /// <returns></returns>
        public string GetControlInfo()
        {
            string str = string.Empty;
            string strSource = string.Empty;
            string strTarget = string.Empty;
            if (Source != null)
            {
                strSource = Source.Title;
            }
            if (Target != null)
            {
                strTarget = Target.Title;
            }
            string strJuge = string.Empty;
            switch (JugeType)
            {
                case JugeType.Equal:
                    strJuge =string.Format(" = {0} ",JugeValue1);
                    break;
                case JugeType.Lower:
                    strJuge = string.Format(" < {0} ", JugeValue1);
                    break;
                case JugeType.LowerEqual:
                    strJuge = string.Format(" <= {0} ", JugeValue1);
                    break;
                case JugeType.Upper:
                    strJuge = string.Format(" > {0} ", JugeValue1);
                    break;
                case JugeType.UpperEqual:
                    strJuge = string.Format(" >= {0} ", JugeValue1);
                    break;
                case JugeType.Between:
                    strJuge = string.Format(" Between {0} And {1} ", JugeValue1, JugeValue2);
                    break;
            }
            string strChange = string.Empty;
            switch (ChangeType)
            {
                case ChangeType.Equal:
                    strChange = string.Format(" = {0} ", ChangeValue);
                    break;
                case ChangeType.Sum:
                    strChange = string.Format(" + {0} ", ChangeValue);
                    break;
                case ChangeType.Sub:
                    strChange = string.Format(" - {0} ", ChangeValue);
                    break;
                case ChangeType.Multi:
                    strChange = string.Format(" * {0} ", ChangeValue);
                    break;
                case ChangeType.Div:
                    strChange = string.Format(" / {0} ", ChangeValue);
                    break;
                case ChangeType.NA:
                    strChange = string.Format(" Is NA ");
                    break;
            }
            str += string.Format("{0}{1}? {2}{3}", strSource, strJuge, strTarget, strChange);
            return str;
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
