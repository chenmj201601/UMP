using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common3107
{
    public class TaskDurationRate
    {
        /// <summary>
        /// 自动任务分配ID
        /// </summary>
        public long TaskSettingID { get; set; }
        /// <summary>
        /// 时长下限
        /// </summary>
        public int DurationMin { get; set; }
        /// <summary>
        /// 时长上限
        /// </summary>
        public int DurationMax { get; set; }
        /// <summary>
        /// 分配比率
        /// </summary>
        public double Rate { get; set; }
    }
}
