//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    ee98a74a-8318-4081-b711-b724f1443b07
//        CLR Version:              4.0.30319.18444
//        Name:                     ObjectItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1110.Models
//        File Name:                ObjectItem
//
//        created by Charley at 2014/12/18 14:50:23
//        http://www.voicecyber.com 
//
//======================================================================

using VoiceCyber.Wpf.CustomControls;

namespace UMPS1110.Models
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
        /// <summary>
        /// 0       配置对象
        /// 1       配置组
        /// </summary>
        public int Type { get; set; }
        public object Data { get; set; }

        private bool mIsSelected;

        public bool IsSelected
        {
            get { return mIsSelected; }
            set { mIsSelected = value; OnPropertyChanged("IsSelected"); }
        }

        private bool mIsMultiSelected;

        public bool IsMultiSelected
        {
            get { return mIsMultiSelected; }
            set { mIsMultiSelected = value; OnPropertyChanged("IsMultiSelected"); }
        }

        public override string ToString()
        {
            return string.Format("[{0}][{1}]", Type, Name);
        }
    }
}
