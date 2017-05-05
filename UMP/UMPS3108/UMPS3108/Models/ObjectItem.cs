using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using VoiceCyber.Wpf.CustomControls;

namespace UMPS3108.Models
{
    public class ObjectItem : CheckableItemBase,INotifyPropertyChanged
    {
        private string mName;
        public string Name
        {
            get { return mName; }
            set { mName = value; OnPropertyChanged("Name"); }
        }
        public int ObjType { get; set; }
        public string ObjID { get; set; }
        private string mDescription;
        public string Description
        {
            get { return mDescription; }
            set { mDescription = value; OnPropertyChanged("Description"); }
        }
        public object Data { get; set; }
        public string ObjParentID { set; get; }
        public string ItemID { get; set; }
        
        private bool mIsSelected;
        public bool IsSelected
        {
            get { return mIsSelected; }
            set { mIsSelected = value; OnPropertyChanged("IsSelected"); }
        }

        public ObjectItem()
        {
            Name = "";
            ObjType = 0;
            ObjID = "0";
            Description = "";
            Data = null;
            ObjParentID = "0";
            ItemID = "0";
            IsSelected = false;
        }

        public override string ToString()
        {
            return string.Format("{0}", Name);
        }
    }
}
