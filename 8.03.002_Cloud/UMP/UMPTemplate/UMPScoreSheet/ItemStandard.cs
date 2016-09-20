//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    bfed9eda-7f94-46d7-b180-544972c961cf
//        CLR Version:              4.0.30319.18444
//        Name:                     ItemStandard
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets
//        File Name:                ItemStandard
//
//        created by Charley at 2014/6/10 15:19:04
//        http://www.voicecyber.com 
//
//======================================================================
//
//======================================================================
//多值型评分标准
//
//多值型评分标准是一种特殊的评分标准，用户可以通过选择一个值子项进行打分
//
//属性编码范围：176 ~ 179
//
//各属性分组下排列序号：
//0:    15 ~ 16
//1:    6 ~ 7  
//2:    无
//3:    无
//======================================================================
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace VoiceCyber.UMP.ScoreSheets
{
    /// <summary>
    /// 多项风格的评分标准
    /// </summary>
    [Description("Item Standard")]
    [XmlRoot(Namespace = "http://www.voicecyber.com/UMP/ScoreSheets/")]
    public class ItemStandard : Standard
    {
        /// <summary>
        /// 默认选中项
        /// </summary>
        [Browsable(false)]
        [XmlElement]
        public StandardItem DefaultValue
        {
            get
            {
                if (ValueItems == null || ValueItems.Count < DefaultIndex + 1)
                {
                    return null;
                }
                return ValueItems[DefaultIndex];
            }
            set
            {
                var index = ValueItems.IndexOf(value);
                if (index >= 0)
                {
                    DefaultIndex = index;
                }
            }
        }
        /// <summary>
        /// 默认选中项的序号
        /// </summary>
        [Category("Basic"), Description("Default Index")]
        [XmlAttribute]
        public int DefaultIndex { get; set; }
        /// <summary>
        /// 当前选中项
        /// </summary>
        [Browsable(false)]
        [XmlElement]
        public StandardItem SelectValue { get; set; }
        /// <summary>
        /// 值的子项
        /// </summary>
        [Browsable(false)]
        [XmlArray]
        public List<StandardItem> ValueItems { get; set; }
        /// <summary>
        /// ItemStandard
        /// </summary>
        public ItemStandard()
        {
            ValueItems = new List<StandardItem>();
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
            //检查子得分项的总分，分制是否匹配
            bool usePonitSystem = false;
            ScoreSheet scoreSheet = ScoreSheet;
            if (scoreSheet != null && scoreSheet.UsePointSystem)
            {
                usePonitSystem = true;
            }
            for (int i = 0; i < ValueItems.Count; i++)
            {
                StandardItem item = ValueItems[i];
                if (usePonitSystem && item.Value > PointSystem)
                {
                    result.Code = 111;
                    result.ScoreObject = item;
                    result.Message = string.Format("Child value item value large than PointSystem.");
                    return result;
                }
                if (!usePonitSystem && item.Value > TotalScore)
                {
                    result.Code = 110;
                    result.ScoreObject = item;
                    result.Message = string.Format("Child value item value large than TotalScore.");
                    return result;
                }
            }
            return base.CheckValid();
        }

        public override void GetPropertyList(ref List<ScoreProperty> listProperties)
        {
            base.GetPropertyList(ref listProperties);
            //listProperties.Add(new ScoreProperty
            //{
            //    ID = 176,
            //    Name = "DefaultValue",
            //    Display = "DefaultValue",
            //    PropertyName = "DefaultValue",
            //    Description = "DefaultValue",
            //    Category = 1,
            //    Flag = ScorePropertyFlag.All,
            //    OrderID = 6,
            //    DataType = ScorePropertyDataType.StandardItem
            //});
            listProperties.Add(new ScoreProperty
            {
                ID = 177,
                Name = "DefaultIndex",
                Display = "DefaultIndex",
                PropertyName = "DefaultIndex",
                Description = "DefaultIndex",
                Category = 0,
                Flag = ScorePropertyFlag.Normal,
                OrderID = 15,
                DataType = ScorePropertyDataType.Int
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 178,
                Name = "SelectValue",
                Display = "SelectValue",
                PropertyName = "SelectValue",
                Description = "SelectValue",
                Category = 0,
                Flag = ScorePropertyFlag.Normal,
                OrderID = 16,
                DataType = ScorePropertyDataType.Int
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 179,
                Name = "ValueItems",
                Display = "ValueItems",
                PropertyName = "ValueItems",
                Description = "ValueItems",
                Category = 1,
                Flag = ScorePropertyFlag.All,
                OrderID = 7,
                DataType = ScorePropertyDataType.StandardItem
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
                ValueItems[i].ScoreItem = this;
                ValueItems[i].OrderID = i;
            }
        }

        public override double GetDefaultValue()
        {
            if (DefaultValue != null)
            {
                return DefaultValue.Value;
            }
            return 0;
        }
    }
}
