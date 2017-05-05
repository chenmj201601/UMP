//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    9841fb21-c57e-44bc-8f41-093df34e0c5a
//        CLR Version:              4.0.30319.18444
//        Name:                     CustomColumnItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3102.Models
//        File Name:                CustomColumnItem
//
//        created by Charley at 2014/11/21 16:18:17
//        http://www.voicecyber.com 
//
//======================================================================

using System.ComponentModel;
using VoiceCyber.UMP.Common;

namespace UMPS3102.Models
{
    public class ViewColumnInfoItem:INotifyPropertyChanged
    {
        private string mColumnName;
        private string mDisplay;
        private int mSortID;
        private bool mIsVisible;
        private int mWidth;
        private string mStrIsVisible;

        public string ColumnName
        {
            get { return mColumnName; }
            set { mColumnName = value; }
        }

        public string Display
        {
            get { return mDisplay; }
            set { mDisplay = value; OnPropertyChanged("Display"); }
        }

        public int SortID
        {
            get { return mSortID; }
            set { mSortID = value; OnPropertyChanged("SortID"); }
        }

        public bool IsVisible
        {
            get { return mIsVisible; }
            set { mIsVisible = value; OnPropertyChanged("IsVisible"); }
        }

        public int Width
        {
            get { return mWidth; }
            set { mWidth = value; OnPropertyChanged("Width"); }
        }

        public string StrIsVisible
        {
            get { return mStrIsVisible; }
            set { mStrIsVisible = value; OnPropertyChanged("StrIsVisible"); }
        }

        public ViewColumnInfo ViewColumnInfo { get; set; }

        public ViewColumnInfoItem(ViewColumnInfo viewColumnInfo)
        {
            mColumnName = viewColumnInfo.ColumnName;
            mSortID = viewColumnInfo.SortID;
            mIsVisible = viewColumnInfo.Visibility == "1";
            mWidth = viewColumnInfo.Width;
            ViewColumnInfo = viewColumnInfo;
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
