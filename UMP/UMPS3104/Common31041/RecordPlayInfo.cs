using System;
using System.Collections.Generic;
using System.Linq;

namespace VoiceCyber.UMP.Common31041
{
    public class RecordPlayInfo
    {
        public long SerialID { get; set; }
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
