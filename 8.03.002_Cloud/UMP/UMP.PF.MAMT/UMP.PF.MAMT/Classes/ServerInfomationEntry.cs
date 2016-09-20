using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UMP.PF.MAMT.Classes
{
    /// <summary>
    /// 连接过的服务器信息
    /// </summary>
   public class ServerInfomation
    {
        private string _Host;

        public string Host
        {
            get { return _Host; }
            set { _Host = value; }
        }
        private string _Port;

        public string Port
        {
            get { return _Port; }
            set { _Port = value; }
        }
        private string _UserName;

        public string UserName
        {
            get { return _UserName; }
            set { _UserName = value; }
        }
    }
}
