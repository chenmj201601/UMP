//======================================================================
//
//        Copyright © 2016 VoiceCyber Technologies Ltd.
//        All rights reserved
//        guid1:                    6215a238-eec0-4235-b59c-57a25875aa98
//        CLR Version:              4.0.30319.42000
//        Name:                     ObjectItem
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                UMPS2106.Models
//        File Name:                ObjectItem
//
//        Created by Charley at 2016/10/24 16:17:41
//        http://www.voicecyber.com 
//
//======================================================================

using VoiceCyber.Wpf.CustomControls;


namespace UMPS2106.Models
{
    public class ObjectItem : CheckableItemBase
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

        public object Data { get; set; }

        private bool mIsSelected;

        public bool IsSelected
        {
            get { return mIsSelected; }
            set { mIsSelected = value; OnPropertyChanged("IsSelected"); }
        }
    }
}
