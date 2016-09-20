//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    679fd992-bb7d-493e-a307-19ca8e9d8424
//        CLR Version:              4.0.30319.18408
//        Name:                     StateSeatItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS4411.Models
//        File Name:                StateSeatItem
//
//        created by Charley at 2016/7/4 16:57:22
//        http://www.voicecyber.com 
//
//======================================================================

using System.ComponentModel;
using VoiceCyber.UMP.Common44101;
using VoiceCyber.UMP.CommonService10;


namespace UMPS4411.Models
{
    public class StateSeatItem : INotifyPropertyChanged
    {

        public long ObjID { get; set; }
        private string mSeatName;

        public string SeatName
        {
            get { return mSeatName; }
            set { mSeatName = value; OnPropertyChanged("SeatName"); }
        }

        private string mDescription;

        public string Description
        {
            get { return mDescription; }
            set { mDescription = value; OnPropertyChanged("Description"); }
        }

        public int Number { get; set; }

        public string Extension { get; set; }
        public string MonID { get; set; }
        public double TimeDeviation { get; set; }
        public RegionSeatInfo Info { get; set; }
        public SeatInfo SeatInfo { get; set; }
        public ExtensionInfo ExtensionInfo { get; set; }
        public MonitorObject MonObject { get; set; }
        public AgentStateItem StateItem { get; set; }


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
            return string.Format("[{0}][{1}][{2}][{3}][{4}]", ObjID, SeatName, Extension,Number, MonID);
        }
    }
}
