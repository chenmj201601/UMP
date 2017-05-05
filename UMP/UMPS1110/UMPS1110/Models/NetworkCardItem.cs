//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    a73ba34d-d9fd-4733-887a-f05820ebada2
//        CLR Version:              4.0.30319.18444
//        Name:                     NetworkCardItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1110.Models
//        File Name:                NetworkCardItem
//
//        created by Charley at 2015/4/15 14:49:46
//        http://www.voicecyber.com 
//
//======================================================================

using System.ComponentModel;

namespace UMPS1110.Models
{
    public class NetworkCardItem : INotifyPropertyChanged
    {
        private string mID;

        public string ID
        {
            get { return mID; }
            set { mID = value; OnPropertyChanged("ID"); }
        }

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

        public NetworkCardInfo Info { get; set; }

        public override string ToString()
        {
            return string.Format("[{0}][{1}][{2}]", ID, Name, Description);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        public static NetworkCardItem CreateItem(NetworkCardInfo info)
        {
            NetworkCardItem item = new NetworkCardItem();
            item.ID = info.ID;
            item.Name = info.Name;
            item.Description = info.Description;
            item.Info = info;
            return item;
        }
    }
}
