//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    d9f507da-a9e5-4030-b2e7-7bedc6902a3d
//        CLR Version:              4.0.30319.18444
//        Name:                     ScoreSheet
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets
//        File Name:                ScoreSheet
//
//        created by Charley at 2014/6/10 13:47:18
//        http://www.voicecyber.com 
//
//======================================================================
//
//======================================================================
//评分表
//
//评分表是一种特殊的评分项组，具有独立性
//DateFrom和DateTo属性指示评分表的有效期限
//评分表提供了控制项的集合，可在整个评分表里使用控制项做得分控制
//
//属性编码范围：151 ~ 157
//
//各属性分组下排列序号：
//0:    14 ~ 17
//1:    6  
//2:    9 ~ 12
//3:    无
//======================================================================
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;

namespace VoiceCyber.UMP.ScoreSheets
{
    /// <summary>
    /// 评分表
    /// </summary>
    [Description("Score Sheet")]
    [XmlRoot(Namespace = "http://www.voicecyber.com/UMP/ScoreSheets/")]
    public class ScoreSheet : ScoreGroup
    {
        private static int mFlagNum = 3;
        /// <summary>
        /// 创建者
        /// </summary>
        [Browsable(false)]
        [XmlAttribute]
        public long Creator { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        [Browsable(false)]
        [XmlAttribute]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        [Browsable(false)]
        [XmlAttribute]
        public string Status { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        [Category("Extented"), Description("Date From")]
        [XmlAttribute]
        public DateTime DateFrom { get; set; }
        /// <summary>
        /// 失效时间
        /// </summary>
        [Category("Extented"), Description("Date To")]
        [XmlAttribute]
        public DateTime DateTo { get; set; }
        /// <summary>
        /// 使用标志
        /// </summary>
        [Browsable(false)]
        [XmlAttribute]
        public long UseTag { get; set; }
        /// <summary>
        /// 合格线
        /// </summary>
        [Category("Basic"), Description("Qualified Line")]
        [XmlAttribute]
        public double QualifiedLine { get; set; }
        /// <summary>
        /// 是否将附加项分数计入总分
        /// </summary>
        [Category("Extented"), Description("Caculate AdditionalItem's score or not")]
        [XmlAttribute]
        public bool CalAdditionalItem { get; set; }
        /// <summary>
        /// 打分栏的宽度
        /// </summary>
        [Category("Extented"), Description("Score Width")]
        [XmlAttribute]
        public double ScoreWidth { get; set; }
        /// <summary>
        /// 得分栏的宽度
        /// </summary>
        [Category("Extented"), Description("Tip Width")]
        [XmlAttribute]
        public double TipWidth { get; set; }
        /// <summary>
        /// 标志（右起）
        /// 0bit    评分表是否完整
        /// 1bit    是否修改成绩
        /// 2bit    是否查看成绩
        /// </summary>
        [Browsable(false)]
        [XmlAttribute]
        public int Flag { get; set; }
        /// <summary>
        /// 控制项
        /// </summary>
        [Browsable(false)]
        [XmlArray]
        public List<ControlItem> ControlItems { get; set; }
        /// <summary>
        /// ScoreSheet
        /// </summary>
        public ScoreSheet()
        {
            ControlItems = new List<ControlItem>();
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
            try
            {
                //纯是非的评分表，其评分标准只能是是非的
                if (ScoreType == ScoreType.YesNo)
                {
                    List<Standard> listStandards = new List<Standard>();
                    GetAllStandards(ref listStandards);
                    for (int i = 0; i < listStandards.Count; i++)
                    {
                        if (listStandards[i].StandardType != StandardType.YesNo)
                        {
                            result.Code = 100;
                            result.ScoreObject = listStandards[i];
                            result.Message = string.Format("AllYesNoScoreSheet has not YesNoStandard.");
                            return result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.Code = 999;
                result.Message = ex.Message;
                return result;
            }
            return result;
        }
        /// <summary>
        /// 计算得分
        /// </summary>
        /// <returns></returns>
        public override double CaculateScore()
        {
            //清除pingfenbiao被控制标志
            IsScoreControled = false;
            RealScore = 0;
            //清除子项被控制标志
            List<ScoreItem> listScoreItems = new List<ScoreItem>();
            GetAllScoreItem(ref listScoreItems);
            for (int i = 0; i < listScoreItems.Count; i++)
            {
                listScoreItems[i].IsScoreControled = false;
                listScoreItems[i].RealScore = 0;
            }
            //预计算得分（不考虑控制项,此时子项的IsScoreControled应该为false)
            base.CaculateScore();
            //控制得分
            for (int i = 0; i < ControlItems.Count; i++)
            {
                ControlItem controlItem = ControlItems[i];
                controlItem.ControlScore();
            }
            //再次计算得分（被控制项的IsScoreControled已经为True了)，得到最终的得分
            double score = base.CaculateScore();
            if (CalAdditionalItem)
            {
                Score = score;
                return score;
            }
            //如果计算的总分超出评分表的总分，并且选择了不计算附加项，则总分取评分表的总分
            if (score > TotalScore)
            {
                score = TotalScore;
            }
            Score = score;
            return score;
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
                ID = 151,
                Name = "Creator",
                Display = "Creator",
                PropertyName = "Creator",
                Description = "Creator",
                Category = 0,
                Flag = ScorePropertyFlag.Normal,
                OrderID = 14,
                DataType = ScorePropertyDataType.Double
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 152,
                Name = "CreateTime",
                Display = "CreateTime",
                PropertyName = "CreateTime",
                Description = "CreateTime",
                Category = 0,
                Flag = ScorePropertyFlag.Xml | ScorePropertyFlag.Copy,
                OrderID = 15,
                DataType = ScorePropertyDataType.DateTime
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 153,
                Name = "Status",
                Display = "Status",
                PropertyName = "Status",
                Description = "Status",
                Category = 0,
                Flag = ScorePropertyFlag.Xml | ScorePropertyFlag.Copy,
                OrderID = 16,
                DataType = ScorePropertyDataType.SString
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 154,
                Name = "DateFrom",
                Display = "DateFrom",
                PropertyName = "DateFrom",
                Description = "DateFrom",
                Category = 2,
                Flag = ScorePropertyFlag.All,
                OrderID = 9,
                DataType = ScorePropertyDataType.DateTime
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 155,
                Name = "DateTo",
                Display = "DateTo",
                PropertyName = "DateTo",
                Description = "DateTo",
                Category = 2,
                Flag = ScorePropertyFlag.All,
                OrderID = 10,
                DataType = ScorePropertyDataType.DateTime
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 156,
                Name = "UseTag",
                Display = "UseTag",
                PropertyName = "UseTag",
                Description = "UseTag",
                Category = 0,
                Flag = ScorePropertyFlag.Xml | ScorePropertyFlag.Copy,
                OrderID = 17,
                DataType = ScorePropertyDataType.Double
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 157,
                Name = "QualifiedLine",
                Display = "QualifiedLine",
                PropertyName = "QualifiedLine",
                Description = "QualifiedLine",
                Category = 1,
                Flag = ScorePropertyFlag.All,
                OrderID = 6,
                DataType = ScorePropertyDataType.Double
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 158,
                Name = "CalAdditionalItem",
                Display = "CalAdditionalItem",
                PropertyName = "CalAdditionalItem",
                Description = "CalAdditionalItem",
                Category = 2,
                Flag = ScorePropertyFlag.All,
                OrderID = 11,
                DataType = ScorePropertyDataType.Bool
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 159,
                Name = "ControlItems",
                Display = "ControlItems",
                PropertyName = "ControlItems",
                Description = "ControlItems",
                Category = 2,
                Flag = ScorePropertyFlag.All,
                OrderID = 12,
                DataType = ScorePropertyDataType.ControlItem
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 191,
                Name = "ScoreWidth",
                Display = "ScoreWidth",
                PropertyName = "ScoreWidth",
                Description = "ScoreWidth",
                Category = 2,
                Flag = ScorePropertyFlag.All,
                OrderID = 13,
                DataType = ScorePropertyDataType.Double
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 192,
                Name = "TipWidth",
                Display = "TipWidth",
                PropertyName = "TipWidth",
                Description = "TipWidth",
                Category = 2,
                Flag = ScorePropertyFlag.All,
                OrderID = 14,
                DataType = ScorePropertyDataType.Double
            });
            //设置某些属性的可见性
            ScoreProperty scoreProperty = listProperties.FirstOrDefault(p => p.Name == "ViewClassic");
            if (scoreProperty != null)
            {
                scoreProperty.Flag = ScorePropertyFlag.All;
            }
            scoreProperty = listProperties.FirstOrDefault(p => p.Name == "ScoreType");
            if (scoreProperty != null)
            {
                scoreProperty.Flag = ScorePropertyFlag.All;
            }
            scoreProperty = listProperties.FirstOrDefault(p => p.Name == "UsePointSystem");
            if (scoreProperty != null)
            {
                scoreProperty.Flag = ScorePropertyFlag.All;
            }
            scoreProperty = listProperties.FirstOrDefault(p => p.Name == "IsAddtionItem");
            if (scoreProperty != null)
            {
                scoreProperty.Flag = ScorePropertyFlag.Normal;
            }
            scoreProperty = listProperties.FirstOrDefault(p => p.Name == "IsAbortScore");
            if (scoreProperty != null)
            {
                scoreProperty.Flag = ScorePropertyFlag.Normal;
            }
            scoreProperty = listProperties.FirstOrDefault(p => p.Name == "IsAvg");
            if (scoreProperty != null)
            {
                scoreProperty.Flag = ScorePropertyFlag.Normal;
            }
        }

        public override void GetAllScoreObject(ref List<ScoreObject> listScoreObjects)
        {
            base.GetAllScoreObject(ref listScoreObjects);
            for (int i = 0; i < ControlItems.Count; i++)
            {
                var item = ControlItems[i];
                listScoreObjects.Add(item);
                item.GetAllScoreObject(ref listScoreObjects);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Init()
        {
            base.Init();
            for (int i = 0; i < ControlItems.Count; i++)
            {
                ControlItems[i].ScoreSheet = ScoreSheet;
                ControlItems[i].OrderID = i;
                ControlItems[i].Init();
            }
        }

        public void InitUseItemID()
        {
            for (int i = 0; i < ControlItems.Count; i++)
            {
                ControlItems[i].ScoreSheet = this;
                ControlItems[i].InitUseItemID();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public void ResetFlag(int index)
        {
            Flag = Flag & ((int)Math.Pow(2, mFlagNum) - (int)Math.Pow(2, index) - 1);
        }

        /// <summary>
        /// 
        /// </summary>
        public void ResetFlag()
        {
            Flag = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public void SetFlag(int index)
        {
            Flag = Flag | (int)Math.Pow(2, index);
        }
        /// <summary>
        /// 获取下一评分项的ItemID
        /// </summary>
        /// <returns></returns>
        public int GetNewItemID()
        {
            int intReturn = 0;
            List<ScoreItem> listItems = new List<ScoreItem>();
            GetAllScoreItem(ref listItems);
            int i = 1;
            while (i < int.MaxValue)
            {
                ScoreItem item = listItems.FirstOrDefault(l => l.ItemID == i);
                if (item == null)
                {
                    intReturn = i;
                    break;
                }
                i++;
            }
            return intReturn;
        }

        /// <summary>
        /// 重新设置子项的深度
        /// </summary>
        public override void SetItemLevel()
        {
            Level = 0;
            base.SetItemLevel();
        }
    }
}
