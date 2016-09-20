//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    c338a806-8d84-481f-ad9f-537071cf0741
//        CLR Version:              4.0.30319.18408
//        Name:                     RegionStateItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS4411.Models
//        File Name:                RegionStateItem
//
//        created by Charley at 2016/7/17 13:23:49
//        http://www.voicecyber.com 
//
//======================================================================

using System.ComponentModel;
using System.Windows.Media;
using VoiceCyber.UMP.Common44101;


namespace UMPS4411.Models
{
    public class RegionStateItem:INotifyPropertyChanged
    {
        public long StateID { get; set; }
        public int StateNumber { get; set; }

        private string mStateName;

        public string StateName
        {
            get { return mStateName; }
            set { mStateName = value; OnPropertyChanged("StateName"); }
        }

        private int mSeatNum;

        public int SeatNum
        {
            get { return mSeatNum; }
            set { mSeatNum = value; OnPropertyChanged("SeatNum"); }
        }

        private Brush mColor;

        public Brush Color
        {
            get { return mColor; }
            set { mColor = value; OnPropertyChanged("Color"); }
        }

        public AgentStateInfo StateInfo { get; set; }

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
            return string.Format("[{0}][{1}][{2}][{3}]", StateID, StateNumber, StateName, SeatNum);
        }
    }
}
