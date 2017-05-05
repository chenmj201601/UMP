using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UMPS3104.Models
{
    public class PanelItem
    {
        public string Name { get; set; }
        public string ContentID { get; set; }
        public string Title { get; set; }
        public string Icon { get; set; }

        public bool IsVisible { get; set; }
        public bool CanClose { get; set; }
    }
}
