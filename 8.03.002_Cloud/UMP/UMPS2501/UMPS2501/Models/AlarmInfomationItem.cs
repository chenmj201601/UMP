//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    223f9fe2-aa09-417e-ab34-b236ff66ac14
//        CLR Version:              4.0.30319.18063
//        Name:                     AlarmInfomationItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS2501.Models
//        File Name:                AlarmInfomationItem
//
//        created by Charley at 2015/5/21 16:57:29
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common25011;
using VoiceCyber.UMP.Controls;

namespace UMPS2501.Models
{
    public class AlarmInfomationItem : INotifyPropertyChanged
    {
        public int RowNumber { get; set; }
        public long SerialID { get; set; }
        public long MessageID { get; set; }

        private int mType;

        public int Type
        {
            get { return mType; }
            set { mType = value; OnPropertyChanged("Type"); }
        }

        private int mLevel;

        public int Level
        {
            get { return mLevel; }
            set { mLevel = value; OnPropertyChanged("Level"); }
        }

        private bool mIsEnabled;

        public bool IsEnabled
        {
            get { return mIsEnabled; }
            set { mIsEnabled = value; OnPropertyChanged("IsEnabled"); }
        }

        private DateTime mCreateTime;

        public DateTime CreateTime
        {
            get { return mCreateTime; }
            set { mCreateTime = value; OnPropertyChanged("CreateTime"); }
        }

        private string mStrCreateTime;

        public string StrCreateTime
        {
            get { return mStrCreateTime; }
            set { mStrCreateTime = value; OnPropertyChanged("StrCreateTime"); }
        }

        private string mName;

        public string Name
        {
            get { return mName; }
            set { mName = value; OnPropertyChanged("Name"); }
        }

        private string mStrMessage;

        public string StrMessage
        {
            get { return mStrMessage; }
            set { mStrMessage = value; OnPropertyChanged("StrMessage"); }
        }

        public AlarmInfomationInfo Info { get; set; }
        public AlarmMessageInfo Message { get; set; }

        public List<BasicDataInfo> ListBasicDataInfos;
        public List<AlarmMessageInfo> ListAlarmMessageInfos;
        public UMPApp CurrentApp;

        private string mStrIsEnabled;

        public string StrIsEnabled
        {
            get { return mStrIsEnabled; }
            set { mStrIsEnabled = value; OnPropertyChanged("StrIsEnabled"); }
        }

        private string mStrType;

        public string StrType
        {
            get { return mStrType; }
            set { mStrType = value; OnPropertyChanged("StrType"); }
        }

        private string mStrLevel;

        public string StrLevel
        {
            get { return mStrLevel; }
            set { mStrLevel = value; OnPropertyChanged("StrLevel"); }
        }


        #region CreateItem

        public static AlarmInfomationItem CreateItem(AlarmInfomationInfo info,UMPApp currentApp)
        {
            if (info == null)
            {
                return null;
            }
            AlarmInfomationItem item = new AlarmInfomationItem();
            item.CurrentApp = currentApp;
            item.SerialID = info.SerialID;
            item.MessageID = info.MessageID;
            item.Level = info.Level;
            item.IsEnabled = info.IsEnabled;
            item.CreateTime = info.CreateTime;
            item.Name = info.Name;
            item.Info = info;

            item.StrIsEnabled = currentApp.GetLanguageInfo(item.IsEnabled ? "2501001" : "2501000",
                item.IsEnabled ? "Enabled" : "Disabled");
            item.StrType =
                currentApp.GetLanguageInfo(string.Format("{0}{1}", S2501Consts.BID_ALARM_TYPE, item.Type.ToString("000")),
                    item.Type.ToString());
            item.StrLevel =
                currentApp.GetLanguageInfo(string.Format("{0}{1}", S2501Consts.BID_ALARM_LEVEL, item.Level.ToString("000")),
                    item.Level.ToString());
            item.StrCreateTime = item.CreateTime.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
            return item;
        }

        #endregion


        public void InitItem()
        {
            if (ListAlarmMessageInfos != null)
            {
                var info = ListAlarmMessageInfos.FirstOrDefault(i => i.SerialID == MessageID);
                if (info != null)
                {
                    Message = info;
                    Type = Message.AlarmType;
                }
            }
            SetPropertyDisplay();
        }

        public void UpdateInfo()
        {
            if (Info != null)
            {
                Info.Name = Name;
                Info.Description = Name;
                Info.MessageID = MessageID;
                Info.Level = Level;
                Info.IsEnabled = IsEnabled;
                Info.CreateTime = CreateTime;
            }
        }


        public void SetPropertyDisplay()
        {
            StrIsEnabled = CurrentApp.GetLanguageInfo(IsEnabled ? "2501001" : "2501000",
                IsEnabled ? "Enabled" : "Disabled");
            StrType = Type.ToString();
            if (ListBasicDataInfos != null)
            {
                var info =
                    ListBasicDataInfos.FirstOrDefault(
                        b => b.InfoID == S2501Consts.BID_ALARM_TYPE && b.Value == Type.ToString());
                if (info != null)
                {
                    StrType =
                        CurrentApp.GetLanguageInfo(string.Format("BID{0}{1}", S2501Consts.BID_ALARM_TYPE, info.SortID.ToString("000")),
                            info.Icon);
                }
            }
            StrLevel = Level.ToString();
            if (Level == -1)
            {
                StrLevel = CurrentApp.GetLanguageInfo("2501002", "Source Level");
            }
            if (ListBasicDataInfos != null)
            {
                var info =
                    ListBasicDataInfos.FirstOrDefault(
                        b => b.InfoID == S2501Consts.BID_ALARM_LEVEL && b.Value == Level.ToString());
                if (info != null)
                {
                    StrLevel =
                        CurrentApp.GetLanguageInfo(string.Format("BID{0}{1}", S2501Consts.BID_ALARM_LEVEL, info.SortID.ToString("000")),
                            info.Icon);
                }
            }
            StrMessage = MessageID.ToString();
            if (Message != null)
            {
                StrMessage = CurrentApp.GetLanguageInfo(string.Format("2501MSG{0}", Message.SerialID), Message.Description);
                //全部状态
                if (Message.StatusID == 0)
                {
                    StrMessage = string.Format("{0}[{1}]", StrMessage, CurrentApp.GetLanguageInfo("2501004", "All Status"));
                }
            }
            StrCreateTime = CreateTime.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
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
