//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    4b06724a-6a42-476f-91fe-04b0bbe0ff06
//        CLR Version:              4.0.30319.18063
//        Name:                     ConfigNameItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPTools.Models
//        File Name:                ConfigNameItem
//
//        created by Charley at 2015/12/1 10:46:13
//        http://www.voicecyber.com 
//
//======================================================================

using System.ComponentModel;

namespace UMPTools.Models
{
    public class ConfigNameItem : INotifyPropertyChanged
    {
        private string mName;

        public string Name
        {
            get { return mName; }
            set { mName = value; OnPropertyChanged("Name"); }
        }

        private string mDescription;

        public string Description
        {
            get { return mDescription; }
            set { mDescription = value; OnPropertyChanged("Description"); }
        }

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
            return Name;
        }
    }
}
