using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoiceCyber.UMP.Common11021
{
    /// <summary>
    /// 基本用户信息
    /// </summary>
    public class BasicUserInfo
    {
        public long UserID { get; set; }
        public string Account { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public int EncryptFlag { get; set; }
        public long OrgID { get; set; }
        public string SourceFlag { get; set; }
        public string IsLocked { get; set; }
        public string LockMethod { get; set; }
        public string IsActived { get; set; }
        public string IsDeleted { get; set; }
        public string State { get; set; }
        public string StrStartTime { get; set; }
        public DateTime StartTime { get; set; }
        public string StrEndTime { get; set; }
        public DateTime EndTime { get; set; }
        public long Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string StrCreateTime { get; set; }
    }
}
