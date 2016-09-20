using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Common2400
{
    public class KeyGenServerEntry
    {
        private string _ResourceID;

        public string ResourceID
        {
            get { return _ResourceID; }
            set { _ResourceID = value; }
        }

        private string _HostAddress;

        public string HostAddress
        {
            get { return _HostAddress; }
            set { _HostAddress = value; }
        }
        private string _HostPort;

        public string HostPort
        {
            get { return _HostPort; }
            set
            {
                _HostPort = value;
            }
        }
        private string _IsEnable;

        public string IsEnable
        {
            get { return _IsEnable; }
            set { _IsEnable = value; }
        }
        private bool _Status;

        public bool Status
        {
            get { return _Status; }
            set
            {
                _Status = value;
            }
        }

     
    }
}
