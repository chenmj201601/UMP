using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoiceCyber.UMP.Common31081
{
    public class StatisticParam:StatisticalParam
    {
        public long OrgID { get; set; }

        public int IsApplyAll { get; set; }

        public string StartTime { get; set; }

        public string EndTime { get; set; }

        public int RowNum { get; set; }

        public long CycleTime { get; set; }

        public int UpdateTime { get; set; }

        public int UpdateUnit { get; set; }

        public int TableType { get; set; }

        public List<string> CycleTimeParam { get; set; }

        public long StatisticKey { get; set; }

        public int StatisticType { get; set; }

        public StatisticParam()
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
    }
}
