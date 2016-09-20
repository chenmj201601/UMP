using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using VoiceCyber.UMP.Common31081;
using VoiceCyber.UMP.Controls;

namespace UMPS3108.Models
{
    public class StatisticalParamItemModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 统计分析参数子项ID
        /// </summary>
        public long StatisticalParamItemID { get; set; }

        /// <summary>
        /// 统计分析参数名
        /// </summary>
        private string mStatisticalParamItemName;
        public string StatisticalParamItemName
        {
            get { return mStatisticalParamItemName; }
            set { mStatisticalParamItemName = value; OnPropertyChanged("Display"); }
        }

        /// <summary>
        /// 是否启用
        /// </summary>
        private bool mIsUsed;
        public bool IsUsed
        {
            get { return mIsUsed; }
            set { mIsUsed = value; OnPropertyChanged("Display"); }
        }
        /// <summary>
        /// 是否能组合
        /// </summary>
        public string IsCombine { get; set; }
        /// <summary>
        /// 统计参数在界面上显示的东西,和语言包相关
        /// </summary>
        private string mDescription;
        public string Description
        {
            get { return mDescription; }
            set { mDescription = value; OnPropertyChanged("Display"); }
        }
        /// <summary>
        /// 规格
        /// </summary>
        public CombStatiParaItemsFormat Formart { get; set; }

        /// <summary>
        /// 统计参数的类型
        /// </summary>
        public StatisticalParamItemType Type { get; set; }

        /// <summary>
        /// 统计分析ID (如果是0，则为可供添加的条件)
        /// </summary>
        public long StatisticalParamID { get; set; }
        /// <summary>
        /// tabitem索引,可组合的统计条件大项---这个对应的是服务态度(0)和专业水平(1)
        /// </summary>
        public int TabIndex { get; set; }
        /// <summary>
        /// tabitem名称,可组合的统计条件大项---服务态度和专业水平
        /// </summary>
        public string TabName { get; set; }
        /// <summary>
        /// 顺序
        /// </summary>
        public int SortID { get; set; }

        /// <summary>
        /// 统计时长
        /// </summary>
        public string StatisticDuration { get; set; }

        /// <summary>
        /// 统计单位
        /// </summary>
        public int StatisticUnit { get; set; }

        /// <summary>
        /// 小项的值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 小项的值类型
        /// </summary>
        public string ValueType { get; set; }

        public StatisticalParamItem ConditionItem { get; set; }

        public ParamItemViewItem ParamItemItem { get; set; }

        public ABCDItem abcdItem { get; set; }

        public void Apply()
        {
            if (ConditionItem != null)
            {
                ConditionItem.SortID = SortID;
            }
        }

        public UMPApp CurrentApp { get; set; }

        public StatisticalParamItemModel(StatisticalParamItem paramItem, UMPApp currentApp)
        {
            StatisticDuration = paramItem.StatisticDuration;
            StatisticUnit = paramItem.StatisticUnit;
            IsCombine = paramItem.IsCombine;
            IsUsed = paramItem.IsUsed;
            Description = paramItem.Description;
            StatisticalParamID = paramItem.StatisticalParamID;
            StatisticalParamItemID = paramItem.StatisticalParamItemID;
            StatisticalParamItemName = paramItem.StatisticalParamItemName;
            TabIndex = paramItem.TabIndex;
            TabName = paramItem.TabName;
            SortID = paramItem.SortID;
            Formart = paramItem.Formart;
            Type = paramItem.Type;
            ConditionItem = paramItem;
            Value = paramItem.Value;
            ValueType = paramItem.ValueType;
            CurrentApp = currentApp;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
