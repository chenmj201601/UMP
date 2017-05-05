using System;

namespace VoiceCyber.UMP.Common31041
{
    /// <summary>
    /// 基本用户信息
    /// </summary>
    public class BasicUserInfo
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        public long UserID { get; set; }
        /// <summary>
        /// 登录帐号
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 用户全名
        /// </summary>
        public string FullName { get; set; }
        public string Password { get; set; }
        public int EncryptFlag { get; set; }
        /// <summary>
        /// 所属机构
        /// </summary>
        public long OrgID { get; set; }
        /// <summary>
        /// 用户来源，U：手工加入；L：LDAP；S：系统初始化数据
        /// </summary>
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
