//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    85c4d660-fd62-44d3-a08f-5248318b160e
//        CLR Version:              4.0.30319.18063
//        Name:                     ScorePropertyInfoItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3101.Models
//        File Name:                ScorePropertyInfoItem
//
//        created by Charley at 2015/11/5 17:45:32
//        http://www.voicecyber.com 
//
//======================================================================

using System.ComponentModel;
using VoiceCyber.UMP.Controls;
using VoiceCyber.UMP.ScoreSheets;
using PropertyChangedEventArgs = System.ComponentModel.PropertyChangedEventArgs;

namespace UMPS3101.Models
{
    public class ScorePropertyInfoItem : INotifyPropertyChanged
    {
        private string mStrPropertyName;

        public string StrPropertyName
        {
            get { return mStrPropertyName; }
            set { mStrPropertyName = value; OnPropertyChanged("StrPropertyName"); }
        }

        private string mStrPropertyDescription;

        public string StrPropertyDescription
        {
            get { return mStrPropertyDescription; }
            set { mStrPropertyDescription = value; OnPropertyChanged("StrPropertyDescription"); }
        }

        private int mGroupID;

        public int GroupID
        {
            get { return mGroupID; }
            set { mGroupID = value; OnPropertyChanged("GroupID"); }
        }

        private string mGroupName;

        public string GroupName
        {
            get { return mGroupName; }
            set { mGroupName = value; OnPropertyChanged("GroupName"); }
        }

        private int mSortID;

        public int SortID
        {
            get { return mSortID; }
            set { mSortID = value; OnPropertyChanged("SortID"); }
        }

        private bool mIsEnabled;
        /// <summary>
        /// 控制属性编辑框是否可用
        /// </summary>
        public bool IsEnabled
        {
            get { return mIsEnabled; }
            set { mIsEnabled = value; OnPropertyChanged("IsEnabled"); }
        }

        public int PropertyID { get; set; }
        public int ObjType { get; set; }
        public ScoreObject ScoreObject { get; set; }
        public ScoreProperty ScoreProperty { get; set; }
        public UCScorePropertyEditor Editor { get; set; }
        public UMPApp CurrentApp;

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
            return string.Format("[{0}]{1}", PropertyID, StrPropertyName);
        }
    }
}
