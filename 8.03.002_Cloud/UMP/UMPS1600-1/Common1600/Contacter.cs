using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common1600
{
    /// <summary>
    /// 联系人表
    /// </summary>
    public class Contacter
    {
        private long _UserID;

        /// <summary>
        /// ID
        /// </summary>
        public long UserID
        {
            get { return _UserID; }
            set { _UserID = value; }
        }
        private string _UserName;
        /// <summary>
        /// Name
        /// </summary>
        public string UserName
        {
            get { return _UserName; }
            set { _UserName = value; }
        }
        private string _Status;
        /// <summary>
        /// 在线状态
        /// </summary>
        public string Status
        {
            get { return _Status; }
            set { _Status = value; }
        }

        private long _OrgID;

        /// <summary>
        /// 机构ID
        /// </summary>
        public long OrgID
        {
            get { return _OrgID; }
            set { _OrgID = value; }
        }

        private string _OrgName;

        /// <summary>
        /// 机构名
        /// </summary>
        public string OrgName
        {
            get { return _OrgName; }
            set { _OrgName = value; }
        }

        private string _FullName;

        /// <summary>
        /// 全名
        /// </summary>
        public string FullName
        {
            get { return _FullName; }
            set { _FullName = value; }
        }

        private long _ParentOrgID;

        public long ParentOrgID
        {
            get { return _ParentOrgID; }
            set { _ParentOrgID = value; }
        }
    }
}
