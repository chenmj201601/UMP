using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common3107
{
    public class QuerySettingItems
    {
        #region 查询设置
        public int RowNumber { get; set; }
        /// <summary>
        /// 主键
        /// </summary>
        public long QuerySettingID { get; set; }
        /// <summary>
        /// 是否使用 Y启用，N禁用
        /// </summary>
        public string IsUsed { get; set; }
        public string strUsed { get; set; }
        public string QuerySettingName { get; set; }
        /// <summary>
        /// 查询设置生效时间
        /// </summary>
        public string QueryStartTime { get; set; }
        /// <summary>
        /// 查询设置失效时间
        /// </summary>
        public string QueryStopTime { get; set; }
        /// <summary>
        /// 是否启用最近时间 Y启用最近时间，N启用生效-失效时间
        /// </summary>
        public string IsRecentTime { get; set; }
        /// <summary>
        /// Y:年 M:月  D:天  H:小时
        /// </summary>
        public string RecentTimeType { get; set; }

        public string RecentTimeNum { get; set; }
        /// <summary>
        /// 录音开始时间
        /// </summary>
        public string StartRecordTime { get; set; }
        /// <summary>
        /// 录音结束时间
        /// </summary>
        public string StopRecordTime { get; set; }
        /// <summary>
        /// 呼叫方向  2 呼入和呼出  0呼出 1呼入
        /// </summary>
        public string CallDirection { get; set; }
        public string StrCall { get; set; }

        /// <summary>
        /// 是否有录屏  A 全部的 Y有录屏 N无录屏
        /// </summary>
        public string HasScreen { get; set; }
        /// <summary>
        /// 是否有情绪分析标志 Y有标志N无标志
        /// </summary>
        public string IsEmtion { get; set; }
        /// <summary>
        /// 是否带关键字标志  Y有标志N无标志
        /// </summary>
        public string IsKeyWord { get; set; }
        /// <summary>
        /// 是否带录音标签  Y有标志N无标志
        /// </summary>
        public string IsBookMark { get; set; }
        /// <summary>
        /// A 全部的 Y被评完分 N未被评过分
        /// </summary>
        public string IsScore { get; set; }
        /// <summary>
        /// 坐席录音分配类型 0:每天分配N条 1:每天分配N% 2:按查询时间分配M条 3:按查询时间分配M%
        /// </summary>
        public int AgentAssType { get; set; }
        public string StrAssT { get; set; }

        /// <summary>
        /// 每个坐席分配数量
        /// </summary>
        public string AgentAssNum { get; set; }
        /// <summary>
        /// 每个坐席分配百分比
        /// </summary>
        public string AgentAssRate { get; set; }
        /// <summary>
        /// 座席工号,以逗号隔开,长度限制2000
        /// </summary>
        public string AgentsIDOne { get; set; }
        public string AgentsIDTwo { get; set; }
        public string AgentsIDThree { get; set; }
        /// <summary>
        /// 录音时长下限
        /// </summary>
        public int DurationMin { get; set; }
        /// <summary>
        /// 录音时长上限
        /// </summary>
        public int DurationMax { get; set; }
        /// <summary>
        /// abcd SQL串
        /// </summary>
        public string ABCDSql { get; set; }
        /// <summary>
        /// 保存ABCD查询配置，abcdID--所选机构ID--0/1/2（好/坏/全部）
        /// </summary>
        public string ABCDSetting { get; set; }

        #endregion
    }
}
