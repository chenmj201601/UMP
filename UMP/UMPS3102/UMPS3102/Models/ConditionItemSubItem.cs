//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    2ef9d9cd-6b70-4249-b198-e6cc8184933c
//        CLR Version:              4.0.30319.18444
//        Name:                     ConditionItemSubItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3102.Models
//        File Name:                ConditionItemSubItem
//
//        created by Charley at 2014/12/2 17:10:16
//        http://www.voicecyber.com 
//
//======================================================================

using System.ComponentModel;

namespace UMPS3102.Models
{
    public class ConditionItemSubItem : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public int Value { get; set; }

        public long QueryValue { get; set; }

        private bool mIsChecked;
        //表示复选框的的勾选状态的
        public bool IsChecked
        {
            get { return mIsChecked; }
            set { mIsChecked = value; OnPropertyChanged("IsChecked"); }
        }

        private string mDisplay;
        //这个是在界面上显示的内容Display
        public string Display
        {
            get { return mDisplay; }
            set { mDisplay = value; OnPropertyChanged("Display"); }
        }

        ////这个是Combox默认选择的Tag
        //private int mSelectTag;
        //public int SelectTag
        //{
        //    get { return mSelectTag; }
        //    set { mSelectTag = value; OnPropertyChanged("SelectTag"); }
        //}

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
