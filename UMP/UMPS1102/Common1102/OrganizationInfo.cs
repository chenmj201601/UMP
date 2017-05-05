using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoiceCyber.UMP.Common11021
{
    public class OrganizationInfo
    {
        public long OrgID { get; set; }
        public string OrgName { get; set; }
        public int OrgType { get; set; }
        public long ParentID { get; set; }
        public string IsActived { get; set; }
        public string IsDeleted { get; set; }
        public string State { get; set; }
        public string StrStartTime { get; set; }
        public DateTime StartTime { get; set; }
        public string StrEndTime { get; set; }
        public DateTime EndTime { get; set; }
        public long Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Description { get; set; }
    }
}
