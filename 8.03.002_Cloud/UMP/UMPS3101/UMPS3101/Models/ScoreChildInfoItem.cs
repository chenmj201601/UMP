//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    9d95dd76-515d-4a79-ae95-85e54884b5b7
//        CLR Version:              4.0.30319.18063
//        Name:                     ScoreChildInfoItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3101.Models
//        File Name:                ScoreChildInfoItem
//
//        created by Charley at 2015/11/30 10:52:58
//        http://www.voicecyber.com 
//
//======================================================================

using System.ComponentModel;
using VoiceCyber.UMP.ScoreSheets;
using PropertyChangedEventArgs = System.ComponentModel.PropertyChangedEventArgs;

namespace UMPS3101.Models
{
    public class ScoreChildInfoItem : INotifyPropertyChanged
    {

        public ScoreObject ParentObject { get; set; }
        public ScoreObject Data { get; set; }

        private int mSortID;

        public int SortID
        {
            get { return mSortID; }
            set { mSortID = value; OnPropertyChanged("SortID"); }
        }

        private string mDisplay;

        public string Display
        {
            get { return mDisplay; }
            set { mDisplay = value; OnPropertyChanged("Display"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        public override string ToString()
        {
            return string.Format("[{0}]{1}", SortID, Data);
        }
    }
}
