//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    af288cdb-f6df-4596-b61c-05bea94b9100
//        CLR Version:              4.0.30319.18444
//        Name:                     ResourcePropertyInfoItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1110.Models
//        File Name:                ResourcePropertyInfoItem
//
//        created by Charley at 2015/1/14 11:19:07
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.Generic;
using System.ComponentModel;
using VoiceCyber.UMP.Common11101;
using VoiceCyber.UMP.Controls;

namespace UMPS1110.Models
{
    public class ResourcePropertyInfoItem:INotifyPropertyChanged
    {
        private string mStrPropertyName;

        public string StrPropertyName
        {
            get { return mStrPropertyName; }
            set { mStrPropertyName = value; OnPropertyChanged("StrPropertyName"); }
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
       
        private bool mIsProp6Enabled;
        /// <summary>
        /// 控制属性行是否可用
        /// </summary>
        public bool IsProp6Enabled
        {
            get { return mIsProp6Enabled; }
            set { mIsProp6Enabled = value; OnPropertyChanged("IsProp6Enabled"); }
        }

        private bool mIsHidden;
        /// <summary>
        /// 控制属性行是否隐藏
        /// </summary>
        public bool IsHidden
        {
            get { return mIsHidden; }
            set { mIsHidden = value; OnPropertyChanged("IsHidden"); }
        }

        public int PropertyID { get; set; }
        public bool IsKeyProperty { get; set; }
        public int ObjType { get; set; }
        public ConfigObject ConfigObject { get; set; }
        public ResourceProperty ResourceProperty { get; set; }
        public ObjectPropertyInfo PropertyInfo { get; set; }
        public List<ResourceProperty> ListPropertyValues { get; set; }
        public List<ConfigObject> ListConfigObjects { get; set; } 
        public List<BasicUserInfo> ListSftpUsers { get; set; }
        public List<BasicInfoData> ListBasicInfoDatas { get; set; } 

        public ResourceMainView MainPage { get; set; }
        public UMPApp CurrentApp { get; set; }
        public UCResourcePropertyEditor Editor { get; set; }

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
            return StrPropertyName;
        }
    }
}
