//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    e6c00b97-12ea-4dc1-8b52-6b7f807346d0
//        CLR Version:              4.0.30319.18063
//        Name:                     ViewColumnItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPTools.Models
//        File Name:                ViewColumnItem
//
//        created by Charley at 2015/8/24 15:19:33
//        http://www.voicecyber.com 
//
//======================================================================

using System.ComponentModel;
using VoiceCyber.UMP.Common;

namespace UMPTools.Models
{
    public class ViewColumnItem : INotifyPropertyChanged
    {
        public long ViewID { get; set; }
        public string ColumnName { get; set; }

        private string mDisplay;

        public string Display
        {
            get { return mDisplay; }
            set { mDisplay = value; OnPropertyChanged("Display"); }
        }

        private int mWidth;

        public int Width
        {
            get { return mWidth; }
            set { mWidth = value; OnPropertyChanged("Width"); }
        }

        public string Visibility { get; set; }

        private string mStrVisibility;

        public string StrVisibility
        {
            get { return mStrVisibility; }
            set { mStrVisibility = value; OnPropertyChanged("StrVisibility"); }
        }

        private int mSortID;

        public int SortID
        {
            get { return mSortID; }
            set { mSortID = value; OnPropertyChanged("SortID"); }
        }

        public ViewColumnInfo Info { get; set; }


        public static ViewColumnItem CreateItem(ViewColumnInfo info)
        {
            ViewColumnItem item=new ViewColumnItem();
            item.ViewID = info.ViewID;
            item.ColumnName = info.ColumnName;
            item.Display = info.Display;
            item.SortID = info.SortID;
            item.Visibility = info.Visibility;
            item.Width = info.Width;
            return item;
        }

        public void UpdateState()
        {
           
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
