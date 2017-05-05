using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using VoiceCyber.Wpf.CustomControls;

namespace UMPS4601.Models
{
    public class ObjectItem : CheckableItemBase, INotifyPropertyChanged
    {

        public long ObjID { get; set; }
        public string Name { get; set; }
        public int ObjType { get; set; }
        public string Description { get; set; }
        public object Data { get; set; }

        //tangche add
        public string FullName { get; set; }
        public string ParantID { get; set; }

        //waves 添加 禁止修改
        public bool Isselected { get; set; }

        private bool mIsSingleSelected;

        public bool IsSingleSelected
        {
            get { return mIsSingleSelected; }
            set { mIsSingleSelected = value; OnPropertyChanged("IsSingleSelected"); }
        }
    }
}
