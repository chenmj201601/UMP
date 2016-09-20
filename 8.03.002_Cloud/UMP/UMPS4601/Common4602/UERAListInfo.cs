using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common4602
{
    public class UERAListInfo
    {
        public string ID { set; get; }
        public string Name { set; get; }
        public string FullName { set; get; }
        public string OrgID { set; get; }
        public string OrgParentID { set; get; }

        public bool IsCheck { set; get; }
    }
}
