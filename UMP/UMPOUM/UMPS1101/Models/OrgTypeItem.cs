//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    384357d0-45cd-40df-a89e-461e10bb7be8
//        CLR Version:              4.0.30319.18408
//        Name:                     OrgTypeItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1101.Models
//        File Name:                OrgTypeItem
//
//        created by Charley at 2016/7/28 18:12:31
//        http://www.voicecyber.com 
//
//======================================================================

using System.ComponentModel;
using VoiceCyber.UMP.Common11011;


namespace UMPS1101.Models
{
    public class OrgTypeItem : INotifyPropertyChanged
    {
        private long mObjID;

        public long ObjID
        {
            get { return mObjID; }
            set { mObjID = value; OnPropertyChanged("ObjID"); }
        }

        private int mID;

        public int ID
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

        private int mSortID;

        public int SortID
        {
            get { return mSortID; }
            set { mSortID = value; OnPropertyChanged("SortID"); }
        }

        public OrgTypeInfo Info { get; set; }



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
