using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoiceCyber.UMP.Common31031;

namespace UMPS3103.Models
{
    public class TaskAssign2QA
    {
        public List<CtrolQA> ListCtrolQA
        {
            set;
            get;
        }

        public int BelongYear { set; get; }
        public int BelongMonth { set; get; }
        public DateTime DealLineTime { set; get; }
        public string TaskName { set; get; }

    }
}
