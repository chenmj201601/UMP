using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceCyber.Wpf.CustomControls;

namespace Common1111
{
    public class NodeData : CheckableItemBase, INotifyPropertyChanged
    {
        private string mName;
        public string Name
        {
            get { return mName; }
            set { mName = value; OnPropertyChanged("Name"); }
        }

        private string mDescription;
        public string Description
        {
            get { return mDescription; }
            set { mDescription = value; OnPropertyChanged("Description"); }
        }
        /// <summary>
        /// 对象编码
        /// </summary>
        public long ObjID { get; set; }
        /// <summary>
        /// 0       配置对象
        /// 1       配置组
        /// </summary>
        public int Type { get; set; }

        public object Data { get; set; }

        private bool mIsSelected;
        public bool IsSelected
        {
            get { return mIsSelected; }
            set { mIsSelected = value; OnPropertyChanged("IsSelected"); }
        }

        public override string ToString()
        {
            return string.Format("[{0}][{1}]", Type, Name);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
