//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    677577ac-e3d3-4516-ae7f-a125ed1adfd5
//        CLR Version:              4.0.30319.18408
//        Name:                     MonPlaybackItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS4411.Models
//        File Name:                MonPlaybackItem
//
//        created by Charley at 2016/7/6 13:27:13
//        http://www.voicecyber.com 
//
//======================================================================

using System.ComponentModel;
using VoiceCyber.UMP.CommonService10;
using VoiceCyber.UMP.Controls;


namespace UMPS4411.Models
{
    public class MonPlaybackItem : INotifyPropertyChanged
    {
        public long SeatID { get; set; }
        private string mSeatName;

        public string SeatName
        {
            get { return mSeatName; }
            set { mSeatName = value; OnPropertyChanged("SeatName"); }
        }

        public string Extension { get; set; }
        public string MonID { get; set; }
        public double TimeDeviation { get; set; }
        public ExtensionInfo ExtensionInfo { get; set; }
        public MonitorObject MonObject { get; set; }

        public UMPApp CurrentApp;
        public UCSeatPlayback PlaybackViewer;


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
            return string.Format("[{0}][{1}][{2}][{3}]", SeatID, SeatName, Extension, MonID);
        }
    }
}
