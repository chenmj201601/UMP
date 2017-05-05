using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UMPService09.Model
{
    public class KPIStatisticsData
    {
        public long KPIMappingID { set; get; }
        public long KPIID { set; get; }

        
        public int RowID { set; get; }//T_46_011表专有 小于天切片的数据用
        public long ObjectID { set; get; }

        public int SliceType { set; get; }//T_46_011表专有 片的段类型（5、1小时 6、 30分钟 7 、15分钟 9、10分钟  8、 5分钟)）
        public int SliceInOrder { set; get; }//1年这天的顺序，1年里这周的顺序，1年里这月的顺序 1天里小时的顺序

        public long StartTimeUTC { set; get; }
        public long StartTimeLocal { set; get; }
        public DateTime UpdateTime { set; get; }
        public int Year { set; get; }
        public int Month { set; get; }
        public int Day { set; get; }

        public long TrueStartTimeLocal { set; get; }
        public long TrueStopTimeLocal { set; get; }
        public long BelongObjectID { set; get; }

        public decimal ActualValue { set; get; }
        public decimal Goal1 { set; get; }
        public string CompareSign1 { set; get; }
        public decimal Trend1 { set; get; }//C104
        public decimal ComparePrior { set; get; } //C109和上一个周期相比较是增加下是下降多少       
        public decimal ActualCompareGoal1 { set; get; }//和目标相比较
        public string Show1 { set; get; }

        public decimal Goal2 { set; get; }

        public int ColumnOrder { set; get; }//1~12  特别用于存T_46_011表的数据



    }
}
