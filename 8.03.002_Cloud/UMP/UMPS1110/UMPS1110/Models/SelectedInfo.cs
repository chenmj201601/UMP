using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoiceCyber.Wpf.CustomControls;

namespace UMPS1110.Models
{
    class SelectedInfo
    {
        public int ObjType { get; set; }

        public ICheckableItem Parent { get; set; }

        private List<ObjectItem> mListItems;

        public List<ObjectItem> ListItems
        {
            get { return mListItems; }
        }

        public SelectedInfo()
        {
            mListItems = new List<ObjectItem>();
        }
    }
}
