using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common2400
{
    public class EncryptEmail
    {
        private string _RentID;

        public string RentID
        {
            get { return _RentID; }
            set { _RentID = value; }
        }

        private string _IsEnableEmail;

        public string IsEnableEmail
        {
            get { return _IsEnableEmail; }
            set { _IsEnableEmail = value; }
        }
        private string _SMTP;

        public string SMTP
        {
            get { return _SMTP; }
            set { _SMTP = value; }
        }
        private string _SMTPPort;

        public string SMTPPort
        {
            get { return _SMTPPort; }
            set { _SMTPPort = value; }
        }
        private string _NeedSSL;

        public string NeedSSL
        {
            get { return _NeedSSL; }
            set { _NeedSSL = value; }
        }
        private string _Account;

        public string Account
        {
            get { return _Account; }
            set { _Account = value; }
        }
        private string _Pwd;

        public string Pwd
        {
            get { return _Pwd; }
            set { _Pwd = value; }
        }
        private string _NeedAuthentication;

        public string NeedAuthentication
        {
            get { return _NeedAuthentication; }
            set { _NeedAuthentication = value; }
        }
        private string _EmailAddress;

        public string EmailAddress
        {
            get { return _EmailAddress; }
            set { _EmailAddress = value; }
        }
    }
}
