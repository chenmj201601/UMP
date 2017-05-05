//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    3332d627-1fb0-4588-8740-bd6aedcae884
//        CLR Version:              4.0.30319.18063
//        Name:                     GlobalSettingItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPTools.Models
//        File Name:                GlobalSettingItem
//
//        created by Charley at 2015/6/4 10:38:50
//        http://www.voicecyber.com 
//
//======================================================================

using System.ComponentModel;
using VoiceCyber.UMP.Common;

namespace UMPTools.Models
{
    public class GlobalSettingItem : INotifyPropertyChanged
    {
        private string mKey;

        public string Key
        {
            get { return mKey; }
            set { mKey = value; OnPropertyChanged("Key"); }
        }

        private string mValue;

        public string Value
        {
            get { return mValue; }
            set { mValue = value; OnPropertyChanged("Value"); }
        }

        public GlobalSetting Info { get; set; }

        public static GlobalSettingItem CreateItem(GlobalSetting info)
        {
            if (info == null)
            {
                return null;
            }
            GlobalSettingItem item = new GlobalSettingItem();
            item.Key = info.Key;
            item.Value = info.Value;
            item.Info = info;
            return item;
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
