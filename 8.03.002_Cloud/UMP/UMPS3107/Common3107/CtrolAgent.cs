using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common3107
{
    public class CtrolAgent
    {
        public string AgentID { set; get; }
        public string AgentName { set; get; }
        public string AgentFullName { set; get; }
        public string AgentOrgID { set; get; }
        public bool IsCheck { set; get; }

        //用作存储每个座席多少条
        public int EveryAgentNum { set; get; }
    }     
}
