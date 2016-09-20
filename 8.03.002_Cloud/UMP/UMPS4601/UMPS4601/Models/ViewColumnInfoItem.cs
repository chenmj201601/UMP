using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Media;
using VoiceCyber.UMP.Common;

namespace UMPS4601.Models
{
    public class ViewColumnInfoItem : INotifyPropertyChanged
    {
        private string mColumnName;
        private string mDisplay;
        private int mSortID;
        private bool mIsVisible;
        private int mWidth;
        private string mStrIsVisible;
        private int mLangID;

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

        public int LangID
        {
            get { return mLangID; }
            set { mLangID = value; OnPropertyChanged("LangID"); }
        }

        public Brush Background { get; set; }

        public ViewColumnInfo ViewColumnInfo { get; set; }

        public ViewColumnInfoItem(ViewColumnInfo viewColumnInfo)
        {
            mColumnName = viewColumnInfo.ColumnName;
            mSortID = viewColumnInfo.SortID;
            mIsVisible = viewColumnInfo.Visibility == "1";
            mWidth = viewColumnInfo.Width;
            ViewColumnInfo = viewColumnInfo;
            LangID = viewColumnInfo.DataType;
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
