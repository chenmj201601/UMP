//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    51c55e79-8195-44c5-8e2c-2912140098bd
//        CLR Version:              4.0.30319.18408
//        Name:                     LicModelItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                LicenseMonitor
//        File Name:                LicModelItem
//
//        created by Charley at 2016/4/26 12:43:08
//        http://www.voicecyber.com 
//
//======================================================================

using VoiceCyber.Wpf.CustomControls;


namespace LicenseMonitor
{
    public class LicModelItem : CheckableItemBase
    {

        private string mName;

        public string Name
        {
            get { return mName; }
            set { mName = value; OnPropertyChanged("Name"); }
        }

        private int mLicNo;

        public int LicNo
        {
            get { return mLicNo; }
            set { mLicNo = value; OnPropertyChanged("LicNo"); }
        }
  
        private long mLicID;

        public long LicID
        {
            get { return mLicID; }
            set { mLicID = value; OnPropertyChanged("LicID"); }
        }

        private string mStrValue;

        public string StrValue
        {
            get { return mStrValue; }
            set { mStrValue = value; OnPropertyChanged("StrValue"); }
        }

        private string mStrLicType;

        public string StrLicType
        {
            get { return mStrLicType; }
            set { mStrLicType = value; OnPropertyChanged("StrLicType"); }
        }

        private string mStrExpireTime;

        public string StrExpireTime
        {
            get { return mStrExpireTime; }
            set { mStrExpireTime = value; OnPropertyChanged("StrExpireTime"); }
        }

        public LicModel Info { get; set; }
    }
}
