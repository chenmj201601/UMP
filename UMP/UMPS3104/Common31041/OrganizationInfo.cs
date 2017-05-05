using System;

namespace VoiceCyber.UMP.Common31041
{
    public class OrganizationInfo
    {
        /// <summary>
        /// 机构编号
        /// </summary>
        public long OrgID { get; set; }
        /// <summary>
        /// 机构名称
        /// </summary>
        public string OrgName { get; set; }
        /// <summary>
        /// 机构类型
        /// </summary>
        public int OrgType { get; set; }
        /// <summary>
        /// 父级机构编号
        /// </summary>
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
