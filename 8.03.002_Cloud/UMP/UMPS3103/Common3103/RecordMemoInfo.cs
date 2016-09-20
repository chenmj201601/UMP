using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoiceCyber.UMP.Common31031
{
    /// <summary>
    /// 记录备注信息
    /// </summary>
    public class RecordMemoInfo
    {
        public long ID { get; set; }
        public long RecordSerialID { get; set; }
        public long UserID { get; set; }
        public DateTime MemoTime { get; set; }
        public string Content { get; set; }
        public string State { get; set; }
        public long LastModifyUserID { get; set; }
        public DateTime LastModifyTime { get; set; }
        public long DeleteUserID { get; set; }
        public DateTime DeleteTime { get; set; }
        public string Source { get; set; }
    }
}
