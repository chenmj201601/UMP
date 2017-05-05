//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    a42fb8fb-75f5-43f7-97f0-fed33f4bb71a
//        CLR Version:              4.0.30319.18444
//        Name:                     ChildObjectItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1110.Models
//        File Name:                ChildObjectItem
//
//        created by Charley at 2015/1/30 14:25:25
//        http://www.voicecyber.com 
//
//======================================================================

using System.ComponentModel;
using System.Windows.Media;

namespace UMPS1110.Models
{
    public class ChildObjectItem : INotifyPropertyChanged
    {
        public long ObjID { get; set; }
        public int Key { get; set; }
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

        public string Icon { get; set; }
        public ConfigObject ConfigObject { get; set; }
        public Brush Background { get; set; }

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
            return string.Format("[{0}][{1}]", ObjID, Name);
        }
    }
}
