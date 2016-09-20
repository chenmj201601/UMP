using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UMPS1110
{
    public class DECServerListColumnDefine
    {
        public long LongServerID { get; set; }
        public string StrServerName { get; set; }
        public UInt16 UIntNumber { get; set; }
        public UInt16 UIntMaxApps { get; set; }

        public string VServerIcoPath { get; set; }
        public string VServerIP { get; set; }
        public string VServerPort { get; set; }
        public string VUsedSSL { get; set; }

        public DECServerListColumnDefine(long ALongServerID, string AServerIcoPath, string AServerIP, string AServerPort, string AServerName, UInt16 AUIntNumber)
        {
            LongServerID = ALongServerID;
            VServerIcoPath = AServerIcoPath;
            VServerIP = AServerIP;
            VServerPort = AServerPort;
            StrServerName = AServerName;
            UIntNumber = AUIntNumber;

            VUsedSSL = App.GetDisplayCharater("UCResourceType212A", "SSLStatus1");
            UIntMaxApps = 256;
        }
    }
}
