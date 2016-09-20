//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    ff6c81d0-3d58-452b-b730-0ea174702a0d
//        CLR Version:              4.0.30319.18444
//        Name:                     ScoreObject
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets
//        File Name:                ScoreObject
//
//        created by Charley at 2014/6/10 13:56:28
//        http://www.voicecyber.com 
//
//======================================================================
//
//======================================================================
//评分对象
//
//评分对象是所有评分相关的类的基类，表示一个评分相关的实体
//属性Type指示评分对象的实际类型
//
//属性编码范围：1 ~ 3
//
//各属性分组下排列序号：
//0：    1 ~ 3
//1:
//2:
//3:
//======================================================================
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace VoiceCyber.UMP.ScoreSheets
{
    /// <summary>
    /// 评分对象
    /// </summary>
    [Description("Score Object")]
    [XmlRoot(Namespace = "http://www.voicecyber.com/UMP/ScoreSheets/")]
    [XmlInclude(typeof(ScoreItem))]
    [XmlInclude(typeof(ControlItem))]
    [XmlInclude(typeof(VisualStyle))]
    [XmlInclude(typeof(CommentItem))]
    [XmlInclude(typeof(Comment))]
    [XmlInclude(typeof(StandardItem))]
    public class ScoreObject
    {
        /// <summary>
        /// 评分对象类别
        /// </summary>
        [Browsable(false)]
        [XmlAttribute]
        public ScoreObjectType Type { get; set; }
        /// <summary>
        /// ID，每个评分对象生成的时候都自动生成一个唯一的ID
        ///评分对象的ID规则是：
        ///301+日期+序号(4位，不足前面加0）
        ///共19位，如：
        ///3011503101323120001
        /// </summary>
        [Browsable(false)]
        [XmlAttribute]
        public long ID { get; set; }
        /// <summary>
        /// 显示文本
        /// </summary>
        [Browsable(false)]
        [XmlIgnore]
        public string Display { get; set; }
        /// <summary>
        /// 属性改变事件
        /// </summary>
        public event EventHandler<PropertyChangedEventArgs> OnPropertyChanged;
        /// <summary>
        /// 属性改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">参数</param>
        public void PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (OnPropertyChanged != null)
            {
                OnPropertyChanged(sender, e);
            }
        }
        /// <summary>
        /// ScoreObject
        /// </summary>
        public ScoreObject()
        {

        }
        /// <summary>
        /// 获取对象的属性列表
        /// </summary>
        /// <param name="listProperties"></param>
        public virtual void GetPropertyList(ref List<ScoreProperty> listProperties)
        {
            listProperties.Add(new ScoreProperty
            {
                ID = 1,
                Name = "ID",
                Display = "ID",
                PropertyName = "ID",
                Description = "Score Object ID",
                Category = 0,
                Flag = ScorePropertyFlag.Normal,
                OrderID = 1,
                DataType = ScorePropertyDataType.Long
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 2,
                Name = "Type",
                Display = "Type",
                PropertyName = "Type",
                Description = "Score Object Type",
                Category = 0,
                Flag = ScorePropertyFlag.Xml,
                OrderID = 2,
                DataType = ScorePropertyDataType.Enum,
                ValueType = typeof(ScoreObjectType)
            });
            listProperties.Add(new ScoreProperty
            {
                ID = 3,
                Name = "Display",
                Display = "Display",
                PropertyName = "Display",
                Description = "Display",
                Category = 0,
                Flag = ScorePropertyFlag.Normal,
                OrderID = 3,
                DataType = ScorePropertyDataType.Enum,
                ValueType = typeof(ScoreObjectType)
            });
        }
        /// <summary>
        /// 获取当前评分对象包含的所有评分对象（不包括自身）
        /// </summary>
        /// <param name="listScoreObjects"></param>
        public virtual void GetAllScoreObject(ref List<ScoreObject> listScoreObjects)
        {
            
        }
        /// <summary>
        /// 复制对象属性
        /// </summary>
        /// <param name="source">源对象</param>
        /// <param name="target">目标对象</param>
        /// <param name="scoreProperty">属性</param>
        public void CopyProperty(ScoreObject source, ScoreObject target, ScoreProperty scoreProperty)
        {
            var value = scoreProperty.GetPropertyValue(source);
            scoreProperty.SetPropertyValue(target, value);
        }
        /// <summary>
        /// 初始化
        /// </summary>
        public virtual void Init()
        {

        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Display))
            {
                return string.Format("{0}", Display);
            }
            return string.Format("({0})", Type);
        }
    }
}
