//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    279619ac-4ff0-40fb-93da-73376bbfa3c5
//        CLR Version:              4.0.30319.18444
//        Name:                     PlayDateTypeItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3102.Models
//        File Name:                PlayDateTypeItem
//
//        created by Charley at 2014/12/2 10:15:43
//        http://www.voicecyber.com 
//
//======================================================================

using System.ComponentModel;

namespace UMPS3102.Models
{
    public class PlayDateTypeItem:INotifyPropertyChanged
    {
        /// <summary>
        /// 0       今天
        /// 1       最近两天
        /// 2       最近一周
        /// 3       最近一月
        /// </summary>
        public int Type { get; set; }

        private string mDisplay;

        public string Display
        {
            get { return mDisplay; }
            set { mDisplay = value; OnPropertyChanged("Display"); }
        }

        public override string ToString()
        {
            return Display;
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
