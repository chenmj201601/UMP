using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMPService06
{
    public class ClientOpArgsAndReturn
    {
        public List<string> LListArguments = new List<string>();
        public bool LBoolReturn = true;
        public long LLongClientSessionID = 0;
        public DateTime LDateTimeOperation = DateTime.UtcNow;
        public string LStrReturnCode = string.Empty;
        public string LStrReturnMessage = string.Empty;
    }
}
