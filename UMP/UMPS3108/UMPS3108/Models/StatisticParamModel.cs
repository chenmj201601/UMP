using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using VoiceCyber.UMP.Common31081;

namespace UMPS3108.Models
{
    public class StatisticParamModel :INotifyPropertyChanged
    {
        /// <summary>
        /// 统计分析参数ID
        /// </summary>
        public long StatisticalParamID { get; set; }
        /// <summary>
        /// 统计分析参数名
        /// </summary>
        private string mStatisticalParamName;
        public string StatisticalParamName
        {
            get { return mStatisticalParamName; }
            set { mStatisticalParamName = value; OnPropertyChanged("StatisticalParamName"); }
        }
        /// <summary>
        /// 是否启用
        /// </summary>
        public string IsUsed { get; set; }
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
            set { mDescription = value; OnPropertyChanged("Description"); }
        }

        public long OrgID { get; set; }

        public int IsApplyAll { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public int RowNum { get; set; }

        public long CycleTime { get; set; }

        public int UpdateTime { get; set; }

        public int UpdateUnit { get; set; }

        public int TableType { get; set; }

        public List<string> CycleTimeParam { get; set; }

        public long StatisticKey { get; set; }

        public int StatisticType { get; set; }

        public StatisticParamModel(StatisticParam SP)
        {
            StatisticalParamID = SP.StatisticalParamID;
            StatisticalParamName = SP.StatisticalParamName;
            TableType = SP.TableType;
            Description = SP.Description;
            OrgID = SP.OrgID;
            IsApplyAll = SP.IsApplyAll;
            StatisticType = SP.StatisticType;
            RowNum = SP.RowNum;
            CycleTime = SP.CycleTime;
            UpdateTime = SP.UpdateTime;
            UpdateUnit = SP.UpdateUnit;
            StatisticKey = SP.StatisticKey;
            CycleTimeParam = SP.CycleTimeParam;
            IsUsed = SP.IsUsed;
            IsCombine = SP.IsCombine;
            StartTime =  Convert.ToDateTime(SP.StartTime);
            EndTime =  Convert.ToDateTime(SP.EndTime);
        }

        public StatisticParamModel()
        {
            StatisticalParamID = 0;
            StatisticalParamName = null;
            TableType = 0;
            Description = null;
            OrgID = 0;
            IsApplyAll = 0;
            StatisticType = 0;
            RowNum = 0;
            CycleTime = 0;
            UpdateTime = 0;
            UpdateUnit = 0;
            StatisticKey = 0;
            CycleTimeParam = new List<string>();
        }

        public override string ToString()
        {
            return StatisticalParamName;
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
