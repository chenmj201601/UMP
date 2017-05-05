//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    45f3e443-6aac-4858-bdeb-0053e6ae12b0
//        CLR Version:              4.0.30319.18063
//        Name:                     StatisticalItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPBuilder.Models
//        File Name:                StatisticalItem
//
//        created by Charley at 2015/12/24 18:10:34
//        http://www.voicecyber.com 
//
//======================================================================

using System.ComponentModel;

namespace UMPBuilder.Models
{
    public class StatisticalItem : INotifyPropertyChanged
    {
        private string mBeginTime;

        public string BeginTime
        {
            get { return mBeginTime; }
            set { mBeginTime = value; OnPropertyChanged("BeginTime"); }
        }

        private string mEndTime;

        public string EndTime
        {
            get { return mEndTime; }
            set { mEndTime = value; OnPropertyChanged("EndTime"); }
        }

        private string mDuration;

        public string Duration
        {
            get { return mDuration; }
            set { mDuration = value; OnPropertyChanged("Duration"); }
        }

        private string mProjectCount;

        public string ProjectCount
        {
            get { return mProjectCount; }
            set { mProjectCount = value; OnPropertyChanged("ProjectCount"); }
        }

        private string mFileCount;

        public string FileCount
        {
            get { return mFileCount; }
            set { mFileCount = value; OnPropertyChanged("FileCount"); }
        }

        private string mPackageCount;

        public string PackageCount
        {
            get { return mPackageCount; }
            set { mPackageCount = value; OnPropertyChanged("PackageCount"); }
        }

        private string mPackageFile;

        public string PackageFile
        {
            get { return mPackageFile; }
            set { mPackageFile = value; OnPropertyChanged("PackageFile"); }
        }

        private string mPackageSize;

        public string PackageSize
        {
            get { return mPackageSize; }
            set { mPackageSize = value; OnPropertyChanged("PackageSize"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        public void ClearValue()
        {
            BeginTime =
                EndTime =
                    Duration = ProjectCount = FileCount = PackageCount = PackageSize = PackageFile = string.Empty;
        }
    }
}
