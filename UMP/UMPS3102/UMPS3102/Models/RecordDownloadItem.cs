//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    ce9b98a2-bcf4-499c-9b4e-98357b98960d
//        CLR Version:              4.0.30319.18444
//        Name:                     RecordDownloadItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3102.Models
//        File Name:                RecordDownloadItem
//
//        created by Charley at 2015/3/30 16:10:35
//        http://www.voicecyber.com 
//
//======================================================================

using System.ComponentModel;
using System.Windows.Media;

namespace UMPS3102.Models
{
    public class RecordDownloadItem : INotifyPropertyChanged
    {
        public int RowNumber { get; set; }
        public long SerialID { get; set; }
        public string RecordLength { get; set; }
        private string mState;

        public string State
        {
            get { return mState; }
            set { mState = value; OnPropertyChanged("State"); }
        }

        private string mError;

        public string Error
        {
            get { return mError; }
            set { mError = value; OnPropertyChanged("Error"); }
        }

        private Brush mForeground;

        public Brush Foreground
        {
            get { return mForeground; }
            set { mForeground = value; OnPropertyChanged("Foreground"); }
        }

        public RecordInfoItem RecordItem { get; set; }

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
