//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    99881903-91d6-4b64-a6d0-1c21b89377ca
//        CLR Version:              4.0.30319.18408
//        Name:                     ProductItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                LoggingUpdater
//        File Name:                ProductItem
//
//        created by Charley at 2016/7/25 18:39:08
//        http://www.voicecyber.com 
//
//======================================================================

using System.ComponentModel;
using VoiceCyber.UMP.Updates;


namespace LoggingUpdater
{
    public class ProductItem : INotifyPropertyChanged
    {

        private string mName;

        public string Name
        {
            get { return mName; }
            set { mName = value; OnPropertyChanged("Name"); }
        }

        public InstallProduct Info { get; set; }


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
