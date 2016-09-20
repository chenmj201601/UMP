//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    d270b6a3-65f7-40b5-b930-986c75b372e9
//        CLR Version:              4.0.30319.42000
//        Name:                     OperationLogItem
//        Computer:                 DESKTOP-AH05P0E
//        Organization:             VoiceCyber
//        Namespace:                UMPS1206.Models
//        File Name:                OperationLogItem
//
//        created by Charley at 2016/3/14 15:58:41
//        http://www.voicecyber.com 
//
//======================================================================

using System.ComponentModel;
using VoiceCyber.UMP.Common;


namespace UMPS1206.Models
{
    public class OperationLogItem:INotifyPropertyChanged
    {

        public long LogID { get; set; }

        private string mStrOperation;

        public string StrOperation
        {
            get { return mStrOperation; }
            set { mStrOperation = value; OnPropertyChanged("StrOperation"); }
        }

        private string mStrTime;

        public string StrTime
        {
            get { return mStrTime; }
            set { mStrTime = value; OnPropertyChanged("StrTime"); }
        }

        private string mStrResult;

        public string StrResult
        {
            get { return mStrResult; }
            set { mStrResult = value; OnPropertyChanged("StrResult"); }
        }

        private string mStrUser;

        public string StrUser
        {
            get { return mStrUser; }
            set { mStrUser = value; OnPropertyChanged("StrUser"); }
        }

        private string mStrHost;

        public string StrHost
        {
            get { return mStrHost; }
            set { mStrHost = value; OnPropertyChanged("StrHost"); }
        }

        private string mStrContent;

        public string StrContent
        {
            get { return mStrContent; }
            set { mStrContent = value; OnPropertyChanged("StrContent"); }
        }

        public OperationLogInfo Info { get; set; }

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
