using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Common3105;

namespace UMPS3105.Models
{
    public class AppealInfoItems : INotifyPropertyChanged
    {
        public int RowNumber { get; set; }
        public long RecordReference { get; set; }
        public int Appealint { get; set; }
        public string AppealState { get; set; }
        public long ReferenceID { get; set; }
        public DateTime AppealTime { get; set; }
        /// <summary>
        /// 获取数据库中查询到的座席的唯一主键
        /// </summary>
        public long AgentNum { get; set; }
        /// <summary>
        /// 座席ID
        /// </summary>
        public string AgentID { get; set; }
        public string AgentName { get; set; }
        public long TemplateID { get; set; }
        public string TemplateName { get; set; }
        public double Score { get; set; }
        public long ScoreID { get; set; }
        public Brush Background { get; set; }

        public AppealInfo AppealInfo { get; set; }

        public AppealInfoItems()
        {}

        public AppealInfoItems(AppealInfo appealInfo)
        {
            RowNumber = appealInfo.RowNumber;
            RecordReference = appealInfo.RecordReference;
            Appealint = appealInfo.Appealint;
            AppealState = appealInfo.AppealState;
            ReferenceID = appealInfo.ReferenceID;
            AppealTime = appealInfo.AppealTime;
            AgentNum = appealInfo.AgentID;
            AgentID = AgentNum.ToString();
            AgentName = appealInfo.AgentName;
            TemplateID = appealInfo.TemplateID;
            TemplateName = appealInfo.TemplateName;
            Score = appealInfo.Score;
            ScoreID = appealInfo.ScoreID;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
