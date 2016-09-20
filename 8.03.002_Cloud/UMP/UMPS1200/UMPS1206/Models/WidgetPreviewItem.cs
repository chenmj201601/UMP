//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    de2144d8-fc9a-42c7-866e-5e114930ab6c
//        CLR Version:              4.0.30319.18408
//        Name:                     WidgetPreviewItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1206.Models
//        File Name:                WidgetPreviewItem
//
//        created by Charley at 2016/5/5 17:21:53
//        http://www.voicecyber.com 
//
//======================================================================

using System.ComponentModel;
using VoiceCyber.UMP.Common12001;


namespace UMPS1206.Models
{
    public class WidgetPreviewItem : INotifyPropertyChanged
    {
        public long WidgetID { get; set; }

        private string mName;

        public string Name
        {
            get { return mName; }
            set { mName = value; OnPropertyChanged("Name"); }
        }

        private string mDisplay;

        public string Display
        {
            get { return mDisplay; }
            set { mDisplay = value; OnPropertyChanged("Display"); }
        }

        private string mDescription;

        public string Description
        {
            get { return mDescription; }
            set { mDescription = value; OnPropertyChanged("Description"); }
        }

        private string mIcon;

        public string Icon
        {
            get { return mIcon; }
            set { mIcon = value; OnPropertyChanged("Icon"); }
        }

        public WidgetInfo WidgetInfo { get; set; }


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
