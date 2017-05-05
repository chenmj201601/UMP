using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Common4602
{
    /// <summary>
    /// PM数据
    /// </summary>
    public class PMShowDataItems
    {
        /// <summary>
        /// T_46_1?.C001,T_46_003.C001
        /// </summary>
        public long KPIMappingID { get; set; }
        /// <summary>
        /// T46_011.C015,T46_01?.C117
        /// </summary>
        public long KPIID { get; set; }
        
        /// <summary>
        /// KPI名字，,T_46_001.C002
        /// </summary>
        public string KPIName { get; set; }

        /// <summary>
        /// 对象ID
        /// </summary>
        public long UERAId { get; set; }

        /// <summary>
        /// 对象全名
        /// </summary>
        public string UERName { get; set; }

        public long StartUTCTime { get; set; }

        public long StartLocalTime { get; set; }

        public DateTime dtLocalTime { get; set; }

        public int pmYear { get; set; }

        public int pmMonth { get; set; }

        public int pmDay { get; set; }

        /// <summary>
        /// 所属对象ID
        /// </summary>
        public long BelongsId { get; set; }

        /// <summary>
        /// 实际值,T_46_1?.C101
        /// </summary>
        public double ActualValue { get; set; }
        /// <summary>
        /// 目标 ,T_46_1?.C102
        /// </summary>
        public string Goal1 { get; set; }
        /// <summary>
        /// 趋势1   趋势（根据配置连续3或4期 与上一个周期比较的值形成趋势）1、上升 2 、N/A  -1、下降  0 持平,T_46_1?.C104
        /// </summary>
        public string Trend1 { get; set; }
        /// <summary>
        ///同比, 和上一个周期相比较，是升高还是下降，如果值是正的则表示上升了多少  如果值是负的则表下降了多少,T_46_1?.C109
        /// </summary>
        public string Compare { get; set; }
        /// <summary>
        /// 实际值/目标1
        /// </summary>
        public string ActualGoal1 { get; set; }
        /// <summary>
        ///得分,T_46_1?.C111
        /// </summary>
        public string BoundaryShow1 { get; set; }
        /// <summary>
        /// 同行目标
        /// </summary>
        public string Goal2 { get; set; }
        /// <summary>
        /// 趋势2
        /// </summary>
        public double Trend2 { get; set; }
        /// <summary>
        /// 实际值/目标2
        /// </summary>
        public string ActualGoal2 { get; set; }
        /// <summary>
        /// 界面显示2
        /// </summary>
        public string BoundaryShow2 { get; set; }
        /// <summary>
        /// 目标3
        /// </summary>
        public string Goal3 { get; set; }
        /// <summary>
        /// 趋势3
        /// </summary>
        public double Trend3 { get; set; }
        /// <summary>
        /// 实际值/目标3
        /// </summary>
        public string ActualGoal3 { get; set; }
        /// <summary>
        /// 界面显示
        /// </summary>
        public string BoundaryShow3 { get; set; }

    }
}
