using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WCF16001
{
    /// <summary>
    /// 已经登录的用户信息（用户ID、名称、通道）
    /// </summary>
    public class LoginUserInfo
    {
        private long _UserID;

        public long UserID
        {
            get { return _UserID; }
            set { _UserID = value; }
        }

        private string _UserName;

        public string UserName
        {
            get { return _UserName; }
            set { _UserName = value; }
        }

        private IService16001CallBack _CallBack;

        public IService16001CallBack CallBack
        {
            get { return _CallBack; }
            set { _CallBack = value; }
        }
    }
}