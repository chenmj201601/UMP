
using System;

namespace VoiceCyber.UMP.Common31041
{
    public class RecordPlayHistoryInfo
    {
        public int RowNumber { get; set; }
        public long PlayHistoryID { get; set; }
        public long RecordReference { get; set; }
        public long longUserID { get; set; }
        public string UserID { get; set; }
        public DateTime PlayDate { get; set; }
        public double PlayDuration { get; set; }
        public int intType { get; set; }
        public string Type { get; set; }
        public int PlayTimes { get; set; }
        public double StartPosition { get; set; }
        public double StopPosition { get; set; }
    }
}
