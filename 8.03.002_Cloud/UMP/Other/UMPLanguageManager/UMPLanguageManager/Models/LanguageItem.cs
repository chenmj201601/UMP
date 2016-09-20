//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    1291b5a2-b09e-479d-8ff3-fa6539a8dd39
//        CLR Version:              4.0.30319.18063
//        Name:                     LanguageItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPLanguageManager.Models
//        File Name:                LanguageItem
//
//        created by Charley at 2015/6/4 15:41:16
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.ComponentModel;
using VoiceCyber.UMP.Common;

namespace UMPLanguageManager.Models
{
    public class LanguageItem : INotifyPropertyChanged
    {
        public int LangID { get; set; }
        public string Category { get; set; }
        public string Name { get; set; }
        public int ModuleID { get; set; }
        public int SubModuleID { get; set; }

        private string mDisplay;

        public string Display
        {
            get { return mDisplay; }
            set
            {
                mDisplay = value;
                State = State | LangItemState.ValueChanged;
                OnPropertyChanged("Display");
            }
        }

        private LangItemState mState;

        /// <summary>
        /// 状态
        /// </summary>
        public LangItemState State
        {
            get { return mState; }
            set { mState = value; OnPropertyChanged("State"); }
        }

        private bool mIsSelected;

        public bool IsSelected
        {
            get { return mIsSelected; }
            set
            {
                mIsSelected = value;
                if (value)
                {
                    State = State | LangItemState.Current;
                }
                else
                {
                    State = State & (~LangItemState.Current);
                }
                OnPropertyChanged("IsSelected");
            }
        }

        public LanguageInfo Info { get; set; }

        public static LanguageItem CreateItem(LanguageInfo info)
        {
            if (info == null)
            {
                return null;
            }
            LanguageItem item = new LanguageItem();
            item.LangID = info.LangID;
            item.Name = info.Name;
            item.Display = info.Display;
            item.ModuleID = info.Module;
            item.SubModuleID = info.SubModule;
            item.Info = info;
            item.State = LangItemState.None;
            return item;
        }

        public void UpdateLangInfo()
        {
            if (Info != null)
            {
                Info.Display = Display;
            }
        }

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
