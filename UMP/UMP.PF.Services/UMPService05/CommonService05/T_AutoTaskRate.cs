using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonService05
{
    /// <summary>
    /// 任务分配 时长比例
    /// </summary>
    public class T_AutoTaskRate
    {
        /// <summary>
        /// C001 自动任务分配ID
        /// </summary>
        public long AutoTaskSetID { get; set; }
        /// <summary>
        /// C002 时长下限（前闭后开）
        /// </summary>
        public int Min { get; set; }
        /// <summary>
        /// C003 时长上限
        /// </summary>
        public int Max { get; set; }
        /// <summary>
        /// C004 百分比
        /// </summary>
        public int Rate { get; set; }
        /// <summary>
        /// C005 录音数量
        /// </summary>
        public int Num { get; set; }
    }
}
