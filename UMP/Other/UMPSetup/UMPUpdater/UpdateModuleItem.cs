//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    adde35a5-9b90-4843-adcc-e51a6dc6346d
//        CLR Version:              4.0.30319.18408
//        Name:                     UpdateModuleItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPUpdater
//        File Name:                UpdateModuleItem
//
//        created by Charley at 2016/5/18 14:57:02
//        http://www.voicecyber.com 
//
//======================================================================

using System.ComponentModel;
using VoiceCyber.UMP.Updates;


namespace UMPUpdater
{
    public class UpdateModuleItem : INotifyPropertyChanged
    {

        private string mName;

        public string Name
        {
            get { return mName; }
            set { mName = value; OnPropertyChanged("Name"); }
        }

        private string mTitle;

        public string Title
        {
            get { return mTitle; }
            set { mTitle = value; OnPropertyChanged("Title"); }
        }

        private string mStrType;

        public string StrType
        {
            get { return mStrType; }
            set { mStrType = value; OnPropertyChanged("StrType"); }
        }

        private string mStrLevel;

        public string StrLevel
        {
            get { return mStrLevel; }
            set { mStrLevel = value; OnPropertyChanged("StrLevel"); }
        }

        public UpdateModule Info { get; set; }



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
