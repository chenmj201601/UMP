using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UMP.PF.MAMT.Classes
{
    public class LoginUserInfo
    {
        private string _UserName;

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName
        {
            get { return _UserName; }
            set { _UserName = value; }
        }

        /// <summary>
        /// 用户ID
        /// </summary>
        private string _UserID;

        public string UserID
        {
            get { return _UserID; }
            set { _UserID = value; }
        }

        private string _TenantID;

        /// <summary>
        /// 租户编号
        /// </summary>
        public string TenantID
        {
            get { return _TenantID; }
            set { _TenantID = value; }
        }

        private string _SessionID;

        /// <summary>
        /// 用户SessionID
        /// </summary>
        public string SessionID
        {
            get { return _SessionID; }
            set { _SessionID = value; }
        }

    }
}
