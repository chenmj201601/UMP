//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    ff17215d-0473-4d65-9c1f-06ef984476bf
//        CLR Version:              4.0.30319.18444
//        Name:                     ScoreGroup
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets
//        File Name:                ScoreGroup
//
//        created by Charley at 2014/6/10 14:08:18
//        http://www.voicecyber.com 
//
//======================================================================
//
//======================================================================
//评分项组
//
//评分项组也叫评分类别，表示含有一个或多个子项的评分项的集合
//评分项组有总分，但总分应该是所有子项总分的和
//评分项组的得分应该是所有子项得分之和
//属性IsAvg指示此组下的子项是否平均分配总分
//
//属性编码范围：131 ~ 133
//
//各属性分组下排列序号：
//0:    无
//1:    无  
//2:    7 ~ 9
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
    /// 评分项组
    /// </summary>
    [DefaultProperty("Title"), Description("Score Group")]
    [XmlRoot(Namespace = "http://www.voicecyber.com/UMP/ScoreSheets/")]
    [XmlInclude(typeof(ScoreSheet))]
    public class ScoreGroup : ScoreItem
    {
        /// <summary>
        /// 平均分配分值
        /// </summary>
        [Category("Basic"), Description("Avarage score or not")]
        [XmlAttribute]
        public bool IsAvg { get; set; }
        /// <summary>
        /// 是否包含自动评分项
        /// </summary>
        [Category("Extend"), Description("Has auto standard")]
        [XmlAttribute]
        public bool HasAutoStandard { get; set; }
        /// <summary>
        /// 子项
        /// </summary>
        [Browsable(false)]
        [XmlArray]
        public List<ScoreItem> Items { get; set; }
        /// <summary>
        /// ScoreGroup
        /// </summary>
        public ScoreGroup()
        {
            Items = new List<ScoreItem>();
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
                //检查所有子项总分之和是否与本项总分相同
                double totalScore = 0.0;
                for (int i = 0; i < Items.Count; i++)
                {
                    result = Items[i].CheckValid();
                    if (result.Code != 0)
                    {
                        return result;
                    }
                    if (Items[i].IsAddtionItem) { continue; }   //附加分不计入总分
                    if (Items[i].IsAbortScore) { continue; }    //不计入总分
                    Decimal score1 = new Decimal(totalScore);
                    Decimal score2 = new Decimal(Items[i].TotalScore);
                    totalScore = Convert.ToDouble(score1 + score2);
                    //totalScore += Items[i].TotalScore;
                }
                if (totalScore != TotalScore)
                {
                    result.Code = 110;
                    result.ScoreObject = this;
                    result.Message = string.Format("TotalScore invalid.");
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.Code = 999;
                result.Message = ex.Message;
            }
            return result;
        }

        public override CheckValidResult CheckInputValid()
        {
            CheckValidResult result = new CheckValidResult();
            result.Code = 0;
            try
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    result = Items[i].CheckInputValid();
                    if (result.Code != 0)
                    {
                        return result;
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
        /// 计算分数
        /// </summary>
        /// <returns></returns>
        public override double CaculateScore()
        {
            //如果得分被控制项控制了，就不用计算此项的得分了
            if (IsScoreControled)
            {
                return RealScore;
            }
            //如果该项不计入总分，返回0
            if (IsAbortScore)
            {
                return 0.0;
            }
            double doubleValue = 0.0;
            bool usePonitSystem = false;
            ScoreSheet scoreSheet = ScoreSheet;
            if (scoreSheet != null && scoreSheet.UsePointSystem)
            {
                usePonitSystem = true;
            }
            //将NA项的分数累加记下
            double naScore = 0.0;
            for (int i = 0; i < Items.Count; i++)
            {
                ScoreItem scoreItem = Items[i];
                if (scoreItem.IsAllowNA && scoreItem.IsNA)
                {
                    naScore += scoreItem.TotalScore;
                }
            }
            //t     本项总分
            //a     子项得分
            //b     NA项多出的分值（即naScore）
            //子项实际得分的计算公式： x = (a * t) / (t - b)
            for (int i = 0; i < Items.Count; i++)
            {
                ScoreItem scoreItem = Items[i];
                double t = TotalScore;
                double b = naScore;
                double a = 0.0;
                ScoreGroup scoreGroup = scoreItem as ScoreGroup;
                if (scoreGroup != null)
                {
                    a = scoreGroup.CaculateScore();
                }
                else
                {
                    Standard standard = Items[i] as Standard;
                    if (standard != null && !standard.IsNA)
                    {
                        //如果此标准被控制项控制了，就不用计算得分，直接返回得分
                        if (standard.IsScoreControled)
                        {
                            a = standard.RealScore;
                        }
                        else
                        {
                            if (standard.IsAbortScore)
                            {
                                a = 0.0;
                            }
                            else
                            {
                                if (usePonitSystem)
                                {
                                    a = (standard.Score / standard.PointSystem) * standard.TotalScore;
                                }
                                else
                                {
                                    a = standard.Score;
                                }
                            }
                        }
                    }
                }
                //注意t-b不能为0，否则除数为0就出错了
                if (t == b)
                {
                    doubleValue = 0.0;
                }
                else
                {
                    doubleValue += (a * t) / (t - b);
                }
            }
            Score = doubleValue;
            RealScore = Score;
            return doubleValue;
        }
        /// <summary>
        /// 获取所有子评分标准
        /// </summary>
        /// <param name="listStandard"></param>
        public virtual void GetAllStandards(ref List<Standard> listStandard)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                ScoreGroup scoreGroup = Items[i] as ScoreGroup;
                if (scoreGroup != null)
                {
                    scoreGroup.GetAllStandards(ref listStandard);
                }
                Standard standard = Items[i] as Standard;
                if (standard != null)
                {
                    listStandard.Add(standard);
                }
            }
        }
        /// <summary>
        /// 获取所有评分项（包括评分项组）
        /// </summary>
        /// <param name="listScoreItem"></param>
        public virtual void GetAllScoreItem(ref List<ScoreItem> listScoreItem)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                listScoreItem.Add(Items[i]);
                ScoreGroup group = Items[i] as ScoreGroup;
                if (group != null)
                {
                    group.GetAllScoreItem(ref listScoreItem);
                }
            }
        }

        public override void GetPropertyList(ref List<ScoreProperty> listProperties)
        {
            base.GetPropertyList(ref listProperties);
            listProperties.Add(new ScoreProperty
            {
                ID = 131,
                Name = "IsAvg",
                Display = "IsAvg",
                PropertyName = "IsAvg",
                Description = "IsAvg",
                Category = 2,
                Flag = ScorePropertyFlag.All,
                OrderID = 7,
                DataType = ScorePropertyDataType.Bool
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 132,
                Name = "Items",
                Display = "Items",
                PropertyName = "Items",
                Description = "Items",
                Category = 2,
                Flag = ScorePropertyFlag.Normal,
                OrderID = 8,
                DataType = ScorePropertyDataType.ScoreItem
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 133,
                Name = "HasAutoStandard",
                Display = "HasAutoStandard",
                PropertyName = "HasAutoStandard",
                Description = "HasAutoStandard",
                Category = 2,
                Flag = ScorePropertyFlag.Normal,
                OrderID = 9,
                DataType = ScorePropertyDataType.Bool
            });
            //设置特定属性的不可见
            ScoreProperty scoreProperty = listProperties.FirstOrDefault(p => p.Name == "ViewClassic");
            if (scoreProperty != null)
            {
                scoreProperty.Flag = ScorePropertyFlag.Normal;
            }
            scoreProperty = listProperties.FirstOrDefault(p => p.Name == "ScoreType");
            if (scoreProperty != null)
            {
                scoreProperty.Flag = ScorePropertyFlag.Normal;
            }
            scoreProperty = listProperties.FirstOrDefault(p => p.Name == "IsKeyItem");
            if (scoreProperty != null)
            {
                scoreProperty.Flag = ScorePropertyFlag.Normal;
            }
            scoreProperty = listProperties.FirstOrDefault(p => p.Name == "IsAllowNA");
            if (scoreProperty != null)
            {
                scoreProperty.Flag = ScorePropertyFlag.Normal;
            }
            scoreProperty = listProperties.FirstOrDefault(p => p.Name == "UsePointSystem");
            if (scoreProperty != null)
            {
                scoreProperty.Flag = ScorePropertyFlag.Normal;
            }
        }

        public override void GetAllScoreObject(ref List<ScoreObject> listScoreObjects)
        {
            base.GetAllScoreObject(ref listScoreObjects);
            for (int i = 0; i < Items.Count; i++)
            {
                var item = Items[i];
                listScoreObjects.Add(item);
                item.GetAllScoreObject(ref listScoreObjects);
            }
        }

        public override void Init()
        {
            base.Init();
            for (int i = 0; i < Items.Count; i++)
            {
                Items[i].Parent = this;
                Items[i].ScoreSheet = ScoreSheet;
                Items[i].OrderID = i;
                Items[i].Init();
            }
        }

        public override void ResetScore()
        {
            base.ResetScore();
            for (int i = 0; i < Items.Count; i++)
            {
                Items[i].ResetScore();
            }
            Score = CaculateScore();
        }

        /// <summary>
        /// 重新设置子项的深度
        /// </summary>
        public virtual void SetItemLevel()
        {
            int level = Level;
            for (int i = 0; i < Items.Count; i++)
            {
                ScoreItem item = Items[i];
                item.Level = level + 1;
                ScoreGroup group = item as ScoreGroup;
                if (group != null)
                {
                    group.SetItemLevel();
                }
            }
        }
    }
}
