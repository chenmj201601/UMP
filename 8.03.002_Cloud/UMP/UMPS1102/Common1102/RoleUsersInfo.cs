using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoiceCyber.UMP.Common11021
{
    public class RoleUsersInfo
    {
        public long NumberID { set; get; }
        public long ParentID { set; get; }
        public long RoleID { set; get; }
        public long UserID { set; get; }
        public DateTime StartTime { set; get; }
        public string StrStartTime { set; get; }
        public DateTime EndTime { set; get; }
        public string StrEndTime { set; get; }

        public bool IsDelete { set; get; }
    }
}
