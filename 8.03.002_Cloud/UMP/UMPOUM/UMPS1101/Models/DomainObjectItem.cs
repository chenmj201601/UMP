using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UMPS1101.Models
{
    public class DomainObjectItem : ObjectItem
    {
        public string ParentName { get; set; }

        public string ParentFullName { get; set; }

        public Guid ParentGuid { get; set; }

        public Guid mGuid { get; set; }

        public string PassWords { get; set; }
    }
}
