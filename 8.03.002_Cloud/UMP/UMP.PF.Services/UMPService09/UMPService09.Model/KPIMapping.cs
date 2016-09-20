using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UMPService09.Model
{

    //对应 T_46_003
    public class KPIMapping
    {
        public long KPIMappingID { set; get; }
        public long KpiID { set; get; }
        public long ObjectID { set; get; }
        public string ActualApplyObjType { set; get; }

        public string ActualApplyCycle { set; get; }
        public string IsStart { set; get; }

        public long StatisticsStartTime { set; get; }
        public long StatisticsStopTime { set; get; }

        public int IsDrop { set; get; }//当为机构时，统计机构下座席时分机是平行还是向下钻取的1平行 2钻取0表示没有这个选项所补充的值
        public string IsApplyAll { set; get; }//是否全应用该指标1是，2否,0表示没有这个选项所补充的值
        public int IsDelete { set; get; }

        public string IsStartGoal1 { set; get; }
        public decimal Goal1
        {
            set;
            get;
        }
        public string CompareSign1 { set; get; }


        public string IsStartGoal2 { set; get; }
        public decimal Goal2
        {
            set;
            get;
        }
        public string CompareSign2 { set; get; }

        public string IsStartGoal3 { set; get; }
        public decimal Goal3
        {
            set;
            get;
        }
        public string CompareSign3 { set; get; }

        public long BelongObject { set; get; }


        public EnumKPISliceType KpiSliceType { set; get; }


    }
}
