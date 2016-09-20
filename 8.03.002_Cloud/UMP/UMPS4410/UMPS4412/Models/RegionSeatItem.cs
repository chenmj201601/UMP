//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    220ec341-4fa2-454d-8f73-718fd5992722
//        CLR Version:              4.0.30319.18408
//        Name:                     RegionSeatItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS4412.Models
//        File Name:                RegionSeatItem
//
//        created by Charley at 2016/6/15 10:53:38
//        http://www.voicecyber.com 
//
//======================================================================

using System.ComponentModel;
using VoiceCyber.UMP.Common44101;
using VoiceCyber.UMP.Controls;


namespace UMPS4412.Models
{
    public class RegionSeatItem : INotifyPropertyChanged
    {
        public long RegionID { get; set; }
        public long SeatID { get; set; }
        private string mSeatName;

        public string SeatName
        {
            get { return mSeatName; }
            set { mSeatName = value; OnPropertyChanged("SeatName"); }
        }

        private int mLeft;

        public int Left
        {
            get { return mLeft; }
            set { mLeft = value; OnPropertyChanged("Left"); }
        }

        private int mTop;

        public int Top
        {
            get { return mTop; }
            set { mTop = value; OnPropertyChanged("Top"); }
        }

        public RegionSeatInfo Info { get; set; }
        public SeatInfo SeatInfo { get; set; }

        public UCRegionSeatSetting PageParent;
        public UCDragableSeat SeatPanel;
        public UMPApp CurrentApp;


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
