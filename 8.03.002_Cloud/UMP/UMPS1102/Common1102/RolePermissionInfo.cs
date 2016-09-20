using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoiceCyber.UMP.Common11021
{
    public  class RolePermissionInfo
    {
        public long RoleID { get; set; }
        public long PermissionID { get; set; }
        public string IsCanUse { get; set; }
        public string IsCanDownAssign { get; set; }
        public string IsCanCascadeRecycle { get; set; }
        public long ModifyID { get; set; }
        public DateTime ModifyTime { get; set; }
        public string StrModifyTime { get; set; }

        public DateTime EnableTime { get; set; }
        public string StrEnableTime { get; set; }

        public DateTime EndTime { set; get; }
        public String StrEndTime { set; get; }

        public bool IsDelete { set; get; }

    }
}
