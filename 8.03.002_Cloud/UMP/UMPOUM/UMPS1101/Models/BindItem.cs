using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UMPS1101.Models
{
    public class BindItem
    {
        public string Name { get; set; }
        public string Display { get; set; }
        public object Obj { get; set; }

        public BindItem()
        {
            Name = string.Empty;
            Display = string.Empty;
            Obj = null;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Display))
            {
                return Name;
            }
            else
            {
                return string.Format("{0}({1})", Display, Name);
            }
        }
    }
}
