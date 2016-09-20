//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    eaf8e4ea-6daa-46be-abda-ce37d06421d7
//        CLR Version:              4.0.30319.18063
//        Name:                     AlarmLevelItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS2501.Models
//        File Name:                AlarmLevelItem
//
//        created by Charley at 2015/5/26 10:12:20
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common25011;
using VoiceCyber.UMP.Controls;

namespace UMPS2501.Models
{
    public class AlarmLevelItem : INotifyPropertyChanged
    {
        private int mLevel;

        public int Level
        {
            get { return mLevel; }
            set { mLevel = value; OnPropertyChanged("Level"); }
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

        private bool mIsSelected;

        public bool IsSelected
        {
            get { return mIsSelected; }
            set { mIsSelected = value; OnPropertyChanged("IsSelected"); }
        }

        public override string ToString()
        {
            return string.Format("[{0}]{1}", Level, Display);
        }


        public List<BasicDataInfo> ListBasicDataInfos;
        public UMPApp CurrentApp;

        public void SetDisplay()
        {
            Display = Level.ToString();
            if (ListBasicDataInfos != null)
            {
                var info =
                    ListBasicDataInfos.FirstOrDefault(
                        b => b.InfoID == S2501Consts.BID_ALARM_LEVEL && b.Value == Level.ToString());
                if (info != null)
                {
                    Display =
                        CurrentApp.GetLanguageInfo(
                            string.Format("BID{0}{1}", S2501Consts.BID_ALARM_LEVEL, info.SortID.ToString("000")), info.Icon);
                }
                else
                {
                    if (Level == -1)
                    {
                        Display = CurrentApp.GetLanguageInfo("2501002", "Source Level");
                    }
                }
            }
            Description = string.Format("[{0}]{1}", Level, Display);
        }


        #region PropertyChanged Event

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion
    }
}
