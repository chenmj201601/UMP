//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    0d9ecd78-67ee-4ae9-9830-7042d6f242fc
//        CLR Version:              4.0.30319.18408
//        Name:                     RegionSeatItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS4411.Models
//        File Name:                RegionSeatItem
//
//        created by Charley at 2016/6/20 16:26:45
//        http://www.voicecyber.com 
//
//======================================================================

using System.ComponentModel;
using System.Windows.Media;
using VoiceCyber.UMP.Common44101;
using VoiceCyber.UMP.CommonService10;
using VoiceCyber.UMP.Controls;


namespace UMPS4411.Models
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


        #region 状态

        private Brush mBorderBrush;

        public Brush BorderBrush
        {
            get { return mBorderBrush; }
            set { mBorderBrush = value; OnPropertyChanged("BorderBrush"); }
        }

        private Brush mBackground;

        public Brush Background
        {
            get { return mBackground; }
            set { mBackground = value; OnPropertyChanged("Background"); }
        }

        #endregion

        public string Extension { get; set; }
        public string MonID { get; set; }
        public double TimeDeviation { get; set; }
        public RegionSeatInfo Info { get; set; }
        public SeatInfo SeatInfo { get; set; }
        public ExtensionInfo ExtensionInfo { get; set; }
        public MonitorObject MonObject { get; set; }

        public UMPApp CurrentApp;
        public UCSeatStatusViewer Viewer;
        public UCSeatDetail DetailViewer;


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
            return string.Format("[{0}][{1}][{2}][{3}]", RegionID, SeatID, SeatName, Extension);
        }
    }
}
