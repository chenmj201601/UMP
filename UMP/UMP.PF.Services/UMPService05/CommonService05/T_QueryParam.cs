using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonService05
{
    /// <summary>
    /// 查询参数 T24 F
    /// </summary>
    public class T_QueryParam
    {
        /// <summary>
        /// C001 主键自增，查询ID
        /// </summary>
        public long QueryID { get; set; }
        /// <summary>
        /// C002 是否启用 1启用，0禁用
        /// </summary>
        public string IsValid { get; set; }
        /// <summary>
        /// C003 查询条件名称
        /// </summary>
        public string QueryName { get; set; }
        /// <summary>
        /// C004 条件有效期起始时间 yyyy-MM-dd HH:mm:ss
        /// </summary>
        public string ValitdTimeStart { get; set; }
        /// <summary>
        /// C005 条件有效期结束时间 yyyy-MM-dd HH:mm:ss
        /// </summary>
        public string ValitdTimeEnd { get; set; }
        /// <summary>
        /// C006 是否启用最近时间 Y则启用 N则启用开始时间~结束时间
        /// </summary>
        public string IsRecentTime { get; set; }
        /// <summary>
        /// C007 Y:年 M:月  D:天  H:小时
        /// </summary>
        public string TimeType { get; set; }
        /// <summary>
        /// C008 几年，几月，几天，几小时
        /// </summary>
        public int TimeNum { get; set; }
        /// <summary>
        /// C009 录音开始时间 UTC yyyy-MM-dd HH:mm:ss
        /// </summary>
        public string StartRecordTime { get; set; }
        /// <summary>
        /// C010 录音结束时间 UTC yyyy-MM-dd HH:mm:ss
        /// </summary>
        public string StopRecordTime { get; set; }
        /// <summary>
        /// C011 呼叫方向  2 呼入和呼出  1呼入 0呼出
        /// </summary>
        public string CallDirection { get; set; }
        /// <summary>
        /// C012 A 全部的 Y有录屏 N无录屏
        /// </summary>
        public string Screen { get; set; }
        /// <summary>
        /// C013 是否有情绪分析标志 Y有标志N无标志
        /// </summary>
        public string IsEmtion { get; set; }
        /// <summary>
        /// C014 是否带关键字标志  Y有标志N无标志
        /// </summary>
        public string IsKeyWord { get; set; }
        /// <summary>
        /// C015 是否带录音标签  Y有标志N无标志
        /// </summary>
        public string IsBookMark { get; set; }
        /// <summary>
        /// C016 A 全部的 Y被评完分 N未被评过分
        /// </summary>
        public string IsScore { get; set; }
        /// <summary>
        /// C017 坐席录音分配类型 0:每天分配N条 1:每天分配N% 2:按查询时间分配M条 3:按查询时间分配M%
        /// </summary>
        public string AgentAssType { get; set; }
        /// <summary>
        /// C018 每个坐席分配数量
        /// </summary>
        public int? AgentAssNum { get; set; }
        /// <summary>
        /// C019 每个坐席分配百分比
        /// </summary>
        public int? AgentAssPer { get; set; }
        /// <summary>
        /// C020 座席工号,以逗号隔开（1）
        /// </summary>
        public string AgentsIDOne { get; set; }
        /// <summary>
        /// C021 座席工号,以逗号隔开（2）
        /// </summary>
        public string AgentsIDTwo { get; set; }
        /// <summary>
        /// C022 座席工号,以逗号隔开（3）
        /// </summary>
        public string AgentsIDThree { get; set; }
        /// <summary>
        /// C023 时长下限 eg 大于等于0
        /// </summary>
        public int DurationMin { get; set; }
        /// <summary>
        /// C024 时长上限 eg  小于等于100
        /// </summary>
        public int DurationMax { get; set; }
        /// <summary>
        /// C025 ABCD条件
        /// </summary>
        public string ABCDCondition { get; set; }


        /// <summary>
        /// bookmark 查询
        /// </summary>
        public string BookMarkStr { get; set; }
    }
}
