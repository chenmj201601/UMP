using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonService05
{
    /// <summary>
    /// 运行频率设置 26 S
    /// </summary>
    public class T_RunFrequency
    {
        /// <summary>
        /// C001 主键自增，运行周期ID
        /// </summary>
        public long FrequencyID { get; set; }
        /// <summary>
        /// C002 运行周期 D:天,W:周,P:旬,M:月,S:季 Y:年  O:只运行一次
        /// </summary>
        public string RunFreq { get; set; }
        /// <summary>
        /// C003 运行时间(02:00:00)
        /// </summary>
        public string DayTime { get; set; }
        /// <summary>
        /// C004 每周的星期几运行 1~7
        /// </summary>
        public int? DayOfWeek { get; set; }
        /// <summary>
        /// C005 每月的第几天运行1~31,那个月没有29，30，31则那个月的最后一天运行
        /// </summary>
        public int? DayOfMonth { get; set; }
        /// <summary>
        /// C006 是否分别设置旬运行 Y旬分开设置 N统一旬设置
        /// </summary>
        public string IsUniteSetOfPeriod { get; set; }
        /// <summary>
        /// C007 统一设置的时间 1~11, 如果那个旬没有9，10，11则旬的最后一天运行
        /// </summary>
        public int? UniteSetPeriod { get; set; }
        /// <summary>
        /// C008 上旬的第几天运行1~10
        /// </summary>
        public int? DayOfFirstPeriod { get; set; }
        /// <summary>
        /// C009 中旬的第几天运行1~10
        /// </summary>
        public int? DayOfSecondPeriod { get; set; }
        /// <summary>
        /// C010 下旬的第几天运行1~11，下旬没有9，10，11的，按最后一天运行
        /// </summary>
        public int? DayOfThirdPeriod { get; set; }
        /// <summary>
        /// C011 是否分别设置旬运行 Y季分开设置 N统一季设置
        /// </summary>
        public string IsUniteSetOfSeason { get; set; }
        /// <summary>
        /// C012 统一设置的时间 0~92, 如果那个季没有90，91，92，则取那个季的最后一天运行
        /// </summary>
        public int? UniteSetSeason { get; set; }
        /// <summary>
        /// C013 第一季的第几天
        /// </summary>
        public int? DayOfFirstSeaSon { get; set; }
        /// <summary>
        /// C014 第二季的第几天
        /// </summary>
        public int? DayOfSecondSeason { get; set; }
        /// <summary>
        /// C015 第三季的第几天
        /// </summary>
        public int? DayOfThirdSeason { get; set; }
        /// <summary>
        /// C016 第四季的第几天
        /// </summary>
        public int? DayOfFourSeason { get; set; }
        /// <summary>
        /// C017 年的第几天运行（0~366）
        /// </summary>
        public int? DayOfYear { get; set; }
    }
}
