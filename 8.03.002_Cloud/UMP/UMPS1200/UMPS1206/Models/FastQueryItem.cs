//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    fca3ac4e-0d50-48a3-918f-31aaa5a146d5
//        CLR Version:              4.0.30319.42000
//        Name:                     FastQueryItem
//        Computer:                 DESKTOP-AH05P0E
//        Organization:             VoiceCyber
//        Namespace:                UMPS1206.Models
//        File Name:                FastQueryItem
//
//        created by Charley at 2016/3/24 10:25:45
//        http://www.voicecyber.com 
//
//======================================================================

using System.ComponentModel;
using System.Windows.Media;
using VoiceCyber.UMP.Common31021;


namespace UMPS1206.Models
{
    public class FastQueryItem : INotifyPropertyChanged
    {

        public long QueryID { get; set; }

        private string mQueryName;

        public string QueryName
        {
            get { return mQueryName; }
            set { mQueryName = value; OnPropertyChanged("QueryName"); }
        }

        private string mDescription;

        public string Description
        {
            get { return mDescription; }
            set { mDescription = value; OnPropertyChanged("Description"); }
        }

        private int mUseCount;

        public int UseCount
        {
            get { return mUseCount; }
            set { mUseCount = value; OnPropertyChanged("UseCount"); }
        }

        private int mRecordCount;

        public int RecordCount
        {
            get { return mRecordCount; }
            set { mRecordCount = value; OnPropertyChanged("RecordCount"); }
        }

        private Brush mBackground;

        public Brush Background
        {
            get { return mBackground; }
            set { mBackground = value; OnPropertyChanged("Background"); }
        }

        public QueryCondition Info { get; set; }


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
