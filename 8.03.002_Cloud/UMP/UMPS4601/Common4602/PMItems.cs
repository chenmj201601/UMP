using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common4602
{
    public class PMItems
    {
        /// <summary>
        /// T46_003.C001,映射表主键ID
        /// </summary>
        public string KPIMappingID { get; set; }

        /// <summary>
        /// T46_003.C002,T46_001.C001
        /// </summary>
        public long KPIID { get; set; }

        /// <summary>
        /// T46_001.C002
        /// </summary>
        public string KPIName { get; set; }

        /// <summary>
        /// 1数值 2百分比 3货币 4时间 5bool(如果该kpi由条件来判断则为bool),T46_001.C010
        /// </summary>
        public int KPIFormat { get; set; }

        /// <summary>
        /// 实际应用类别(1、座席2、分机3、机构4、技能组5、用户6、真实分机 )到时通过 1111111111方式来展示,T46_003.C004
        /// </summary>
        public string KPITypeID { get; set; }

        /// <summary>
        /// 实际应用周期(1、年，2、月 ，3、周，4、天 5、1小时 6、 30分钟 7 、15分钟 9、10分钟  8、 5分钟)  到时通过 1111111111方式来展示,T46_003.C005
        /// </summary>
        public string KPICycle { get; set; }

        /// <summary>
        /// 统计的开始时间，暂时用本地时间,T46_003.C007
        /// </summary>
        public string StartTime { get; set; }

        /// <summary>
        /// 结束时间,T46_003.C008
        /// </summary>
        public string StopTime { get; set; }

        /// <summary>
        /// 当为机构时，统计机构下座席时分机是平行还是向下钻取的1平行 2钻取,T46_003.C009
        /// </summary>
        public string dropDown { get; set; }

        /// <summary>
        /// 同行目标
        /// </summary>
        public string GoalValue1 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string GoalValue2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string GoalValue3 { get; set; }

        /// <summary>
        /// 保存数据库中查出的在List的位置
        /// </summary>
        public int arrInt { get; set; }

        /// <summary>
        /// 临时保存Wcf中查出对象ID
        /// </summary>
        public long ObjectID { get; set; }

        /// <summary>
        /// Kpi应用对象ID
        /// </summary>
        public string KpiObjectID { get; set; }
        
    }
}
