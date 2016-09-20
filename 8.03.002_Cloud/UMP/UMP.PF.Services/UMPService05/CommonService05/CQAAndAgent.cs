using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonService05
{
    public class CtrolQA
    {
        public string UserID { set; get; }
        public string UserName { set; get; }
        public string UserFullName { set; get; }
        public string OrgID { set; get; }
    }

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
