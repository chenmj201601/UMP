//======================================================================
//
//        Copyright © 2016-2017 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        GUID1:                    6089875b-8101-4a14-9b42-75db2262b0f9
//        CLR Version:              4.0.30319.42000
//        Name:                     UpdateStateItem
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                UMPUpdater.Models
//        File Name:                UpdateStateItem
//
//        Created by Charley at 2016/8/29 18:06:20
//        http://www.voicecyber.com 
//
//======================================================================

using System.ComponentModel;
using System.Windows.Media;


namespace UMPUpdater.Models
{
    public class UpdateStateItem : INotifyPropertyChanged
    {
        public int State { get; set; }

        public string Name { get; set; }

        private string mDisplay;

        public string Display
        {
            get { return mDisplay; }
            set { mDisplay = value; OnPropertyChanged("Display"); }
        }

        public double Progress { get; set; }

        private string mStrProgress;
        /// <summary>
        /// 0：待执行
        /// 1：正在执行
        /// 2：成功
        /// 3：失败
        /// </summary>
        public string StrProgress
        {
            get { return mStrProgress; }
            set { mStrProgress = value; OnPropertyChanged("StrProgress"); }
        }

        private int mResult;

        public int Result
        {
            get { return mResult; }
            set { mResult = value; OnPropertyChanged("Result"); }
        }

        private string mStrResult;

        public string StrResult
        {
            get { return mStrResult; }
            set { mStrResult = value; OnPropertyChanged("StrResult"); }
        }

        private Brush mForeground;

        public Brush Foreground
        {
            get { return mForeground; }
            set { mForeground = value; OnPropertyChanged("Foreground"); }
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
