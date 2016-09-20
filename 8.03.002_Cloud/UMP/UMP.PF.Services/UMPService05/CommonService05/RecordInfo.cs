using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonService05
{
    public class RecordInfo
    {
        public long RowID { get; set; }
        public long SerialID { get; set; }
        public string RecordReference { get; set; }
        public DateTime StartRecordTime { get; set; }
        public int VoiceID { get; set; }
        public int ChannelID { get; set; }
        public string VoiceIP { get; set; }
        public string Extension { get; set; }
        public string Agent { get; set; }
        public int Duration { get; set; }
        /// <summary>
        /// 0       呼出
        /// 1       呼入
        /// </summary>
        public int Direction { get; set; }
        public string CallerID { get; set; }
        public string CalledID { get; set; }
        public string IsScore { get; set; }

        public DateTime StopRecordTime { get; set; }
        public string TaskUserID { get; set; }
        public string TaskUserName { get; set; }
    }
}
