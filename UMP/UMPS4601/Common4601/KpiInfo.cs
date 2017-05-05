using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoiceCyber.UMP.Common4601
{
    /// <summary>
    /// KPI基本信息表  从T_46_001拿 也是KPI里面的默认信息
    /// </summary>
    public class KpiInfo
    {
        public string KpiID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Creator { get; set; }
        public string CreateTime { get; set; }
        /// <summary>
        /// 表示可以应用对象 1座席 2应用于分机 3机构 4技能组 5用户到时通过 1111111111方式来展示(当应用录音，成绩时，专写入成绩表和录音表)
        /// </summary>
        public string UseType { get; set; }
        /// <summary>
        /// 1 质检类Kpi 2 录音类kpi  3 cti类kpi  4 acd类kpi 5 wfm类kpi  10其它类kpi
        /// </summary>
        public string KpiType { get; set; }
        /// <summary>
        /// 来源 1.系统自带即UMP（不允许客户修改） 2.手动添加的
        /// </summary>
        public string SourceType { get; set; }
        /// <summary>
        /// 是否启用 1、为启用 2为不启用
        /// </summary>
        public string Active { get; set; }
        /// <summary>
        /// 1数值 2百分比 3货币 4时间 5bool(如果该kpi由条件来判断则为bool)
        /// </summary>
        public string ValueFormat { get; set; }
        /// <summary>
        /// 可应用周期
        /// </summary>
        public string ApplyCycle { get; set; }
        /// <summary>
        /// 是否启用目标1  1、为启用 2为不启用
        /// </summary>
        public string IsStart1 { get; set; }
        /// <summary>
        /// 目标1的值 ，比如实际目标
        /// </summary>
        public string GoalValue1 { get; set; }
        /// <summary>
        /// 目标1的比较符号
        /// </summary>
        public string GoalOperator1 { get; set; }
        /// <summary>
        /// 目标1是否启用多区段
        /// </summary>
        public string IsStartMultiRegion1 { get; set; }
        /// <summary>
        /// 是否启用目标2  1、为启用 2为不启用
        /// </summary>
        public string IsStart2{ get; set; }
        /// <summary>
        /// 目标2的值 ，比如实际目标
        /// </summary>
        public string GoalValue2{ get; set; }
        /// <summary>
        /// 目标2的比较符号
        /// </summary>
        public string GoalOperator2{ get; set; }
        /// <summary>
        /// 目标2是否启用多区段
        /// </summary>
        public string IsStartMultiRegion2 { get; set; }
        /// <summary>
        /// 默认符号
        /// </summary>
        public string DefaultSymbol { get; set; }
    }
}
