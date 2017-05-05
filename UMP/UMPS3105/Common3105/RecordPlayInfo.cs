using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common3105
{
    /// <summary>
    /// 播放历史信息
    /// </summary>
    public class RecordPlayInfo
    {
        public long SerialID { get; set; }
        public long RecordID { get; set; }
        public string RecordReference { get; set; }
        public double StartPosition { get; set; }
        public double StopPosition { get; set; }
        public double Duration { get; set; }
        public int PlayTimes { get; set; }
        public long Player { get; set; }
        public DateTime PlayTime { get; set; }
        /// <summary>
        /// 1       UMP QM
        /// 2       CQC
        /// </summary>
        public int PlayTerminal { get; set; }
    }
}
