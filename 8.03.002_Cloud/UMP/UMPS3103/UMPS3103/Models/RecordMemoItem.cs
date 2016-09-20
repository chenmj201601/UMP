using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoiceCyber.UMP.Common31031;

namespace UMPS3103.Models
{
    public class RecordMemoItem
    {
        public long ID { get; set; }
        public long RecordSerialID { get; set; }
        public long UserID { get; set; }
        public DateTime MemoTime { get; set; }
        public string Content { get; set; }
        public string State { get; set; }
        public string Source { get; set; }

        public RecordMemoInfo RecordMemoInfo { get; set; }

        public RecordMemoItem(RecordMemoInfo recordMemoInfo)
        {
            ID = recordMemoInfo.ID;
            RecordSerialID = recordMemoInfo.RecordSerialID;
            UserID = recordMemoInfo.UserID;
            MemoTime = recordMemoInfo.MemoTime;
            Content = recordMemoInfo.Content;
            State = recordMemoInfo.State;
            Source = recordMemoInfo.Source;

            RecordMemoInfo = recordMemoInfo;
        }
    }
}
