//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    fef7d7d1-6fda-492b-a32c-c25c3b56d3c5
//        CLR Version:              4.0.30319.18444
//        Name:                     RecordInfoItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3102.Models
//        File Name:                RecordInfoItem
//
//        created by Charley at 2014/11/10 11:38:44
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;
using VoiceCyber.UMP.Common31021;

namespace UMPS3102.Models
{
    public class RecordInfoItem : INotifyPropertyChanged
    {
        public int RowNumber { get; set; }
        public long RowID { get; set; }
        public long SerialID { get; set; }
        public string RecordReference { get; set; }
        public DateTime StartRecordTime { get; set; }
        public int VoiceID { get; set; }
        public int ChannelID { get; set; }
        public string VoiceIP { get; set; }
        public string Extension { get; set; }
        public string Agent { get; set; }
        public int Duration { get; set; }
        /// <summary>
        /// 0       呼出
        /// 1       呼入
        /// </summary>
        public int Direction { get; set; }
        public string CallerID { get; set; }
        public string CalledID { get; set; }

        public string EncryptFlag { get; set; }

        public string SkillGroup { get; set; }
        public string RealExtension { get; set; }
        public string ParticipantNum { get; set; }

        /// <summary>
        /// CTI流水号
        /// </summary>
        public string CTIReference { get; set; }
        /// <summary>
        /// 主叫DTMF
        /// </summary>
        public string CallerDTMF { get; set; }
        /// <summary>
        /// 被叫DTMF
        /// </summary>
        public string CalledDTMF { get; set; }
        /// <summary>
        /// 2    只有一条最终分
        /// 1    已评分
        /// 0    未评分
        /// </summary>
        public int IsScored { get; set; }
        /// <summary>
        /// 分数[当该录音只有一条有效评分的时候 这个才会有值]
        /// </summary>
        public string Score { get; set; }
        /// <summary>
        /// 服务态度
        /// 1 好
        /// 2 差
        /// </summary>
        public string ServiceAttitude { get; set; }
        /// <summary>
        /// 录音时长异常
        /// 1正常
        /// 2异常
        /// </summary>
        public string RecordDurationExcept { get; set; }
        /// <summary>
        /// 异常分数
        /// 1正常
        /// 2异常
        /// </summary>
        public string ExceptionScore { get; set; }
        /// <summary>
        /// 亊后处理时长异常
        /// 1正常
        /// 2异常
        /// </summary>
        public string AfterDealDurationExcept { get; set; }
        /// <summary>
        /// 坐席/客人讲话时长比例异常
        /// 1正常
        /// 2异常
        /// </summary>
        public string ACSpeExceptProportion { get; set; }
        public string RepeatedCall { get; set; }

        public string CallPeak { get; set; }

        public string ProfessionalLevel { get; set; }

        public string StrServiceAttitude { get; set; }
        public string StrRecordDurationExcept { get; set; }
        public string StrRepeatedCall { get; set; }
        public string StrCallPeak { get; set; }
        public string StrProfessionalLevel { get; set; }
        public string StrExceptionScore { get; set; }
        public string StrAfterDealDurationExcept { get; set; }
        public string StrACSpeExceptProportion { get; set; }
        /// <summary>
        /// 是否有录音标签或者关键词[1,有 0,没有]
        /// </summary>
        public string IsHaveBookMark { get; set; }
        /// <summary>
        /// 是否有备注[1,有 0,没有]
        /// </summary>
        public string IsHaveMemo { get; set; }
        /// <summary>
        /// 通道名称
        /// </summary>
        public string ChannelName { get; set; }

        /// <summary>
        /// 是否有语音标签[1.有 0.没有]
        /// </summary>
        public string IsHaveVoiceBookMark { get; set; }

        private string mStrIsScored;

        public string StrIsScored
        {
            get { return mStrIsScored; }
            set
            {
                mStrIsScored = value;
                OnProeprtyChanged("StrIsScored");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnProeprtyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        private string mStrDirection;
        public string StrDirection
        {
            get { return mStrDirection; }
            set
            {
                mStrDirection = value;
                OnProeprtyChanged("StrDirection");
            }
        }

        private string mStrIsHaveBookMark;
        public string StrIsHaveBookMark
        {
            get { return mStrIsHaveBookMark; }
            set 
            {
                mStrIsHaveBookMark = value;
                OnProeprtyChanged("StrIsHaveBookMark");
            }
        }

        private string mStrIsHaveMemo;
        public string StrIsHaveMemo
        {
            get { return mStrIsHaveMemo; }
            set 
            {
                mStrIsHaveMemo = value;
                OnProeprtyChanged("StrIsHaveMemo");
            }
        }

        private string mStrIsHaveVoiceBookMark;
        public string StrIsHaveVoiceBookMark
        {
            get { return mStrIsHaveVoiceBookMark; }
            set 
            {
                mStrIsHaveVoiceBookMark = value;
                OnProeprtyChanged("StrIsHaveVoiceBookMark");
            }
        }
        /// <summary>
        /// 初检任务号
        /// </summary>
        public long TaskNumber { get; set; }

        public int MediaType { get; set; }

        public DateTime StopRecordTime { get; set; }

        public Brush Background { get; set; }

        public RecordInfo RecordInfo { get; set; }

        public DateTime LocalStartRecordTime { get; set; }

        public DateTime LocalStopRecordTime { get; set; }

        private List<RecordInfo> mListRelativeInfos;

        public List<RecordInfo> ListRelativeInfos
        {
            get { return mListRelativeInfos; }
        }

        public RecordInfoItem()
        {
            mListRelativeInfos = new List<RecordInfo>();
        }

        public RecordInfoItem(RecordInfo recordInfo): this()
        {
            RowID = recordInfo.RowID;
            SerialID = recordInfo.SerialID;
            RecordReference = recordInfo.RecordReference;
            StartRecordTime = recordInfo.StartRecordTime;

            VoiceID = recordInfo.VoiceID;
            ChannelID = recordInfo.ChannelID;
            VoiceIP = recordInfo.VoiceIP;
            Extension = recordInfo.Extension;
            Agent = recordInfo.Agent;
            Duration = recordInfo.Duration;
            Direction = recordInfo.Direction;
            CallerID = recordInfo.CallerID;
            CalledID = recordInfo.CalledID;
            StopRecordTime = recordInfo.StopRecordTime;
            EncryptFlag = recordInfo.EncryptFlag;

            SkillGroup = recordInfo.SkillGroup;
            RealExtension = recordInfo.RealExtension;
            ParticipantNum = recordInfo.ParticipantNum;

            RecordInfo = recordInfo;

            LocalStartRecordTime = StartRecordTime.ToLocalTime();
            LocalStopRecordTime = StopRecordTime.ToLocalTime();

            ServiceAttitude = recordInfo.ServiceAttitude;
            RecordDurationExcept = recordInfo.RecordDuritionExcept;
            Score = recordInfo.Score;
            IsScored = recordInfo.IsScored;
            MediaType = recordInfo.MediaType;
            CallerDTMF = recordInfo.CallerDTMF;
            CalledDTMF = recordInfo.CalledDTMF;
            CTIReference = recordInfo.CTIReference;
            RepeatedCall = recordInfo.RepeatedCall;
            CallPeak = recordInfo.CallPeak;
            ProfessionalLevel = recordInfo.ProfessionalLevel;
            ExceptionScore = recordInfo.ExceptionScore;
            AfterDealDurationExcept = recordInfo.AfterDealDurationExcept;
            ACSpeExceptProportion = recordInfo.ACSpeExceptProportion;

            IsHaveBookMark = recordInfo.IsHaveBookMark;
            IsHaveMemo = recordInfo.IsHaveMemo;
            ChannelName = recordInfo.ChannelName;
            IsHaveVoiceBookMark = recordInfo.IsHaveVoiceBookMark;
            #region 客户化字段
            CustomField01 = recordInfo.CustomField01;
            CustomField02 = recordInfo.CustomField02;
            CustomField03 = recordInfo.CustomField03;
            CustomField04 = recordInfo.CustomField04;
            CustomField05 = recordInfo.CustomField05;
            CustomField06 = recordInfo.CustomField06;
            CustomField07 = recordInfo.CustomField07;
            CustomField08 = recordInfo.CustomField08;
            CustomField09 = recordInfo.CustomField09;
            CustomField10 = recordInfo.CustomField10;
            CustomField11 = recordInfo.CustomField11;
            CustomField12 = recordInfo.CustomField12;
            CustomField13 = recordInfo.CustomField13;
            CustomField14 = recordInfo.CustomField14;
            CustomField15 = recordInfo.CustomField15;
            CustomField16 = recordInfo.CustomField16;
            CustomField17 = recordInfo.CustomField17;
            CustomField18 = recordInfo.CustomField18;
            CustomField19 = recordInfo.CustomField19;
            CustomField20 = recordInfo.CustomField20;
            #endregion  
        }



        //客户化字段
        public string CustomField01 { get; set; }
        public string CustomField02 { get; set; }
        public string CustomField03 { get; set; }
        public string CustomField04 { get; set; }
        public string CustomField05 { get; set; }
        public string CustomField06 { get; set; }
        public string CustomField07 { get; set; }
        public string CustomField08 { get; set; }
        public string CustomField09 { get; set; }
        public string CustomField10 { get; set; }
        public string CustomField11 { get; set; }
        public string CustomField12 { get; set; }
        public string CustomField13 { get; set; }
        public string CustomField14 { get; set; }
        public string CustomField15 { get; set; }
        public string CustomField16 { get; set; }
        public string CustomField17 { get; set; }
        public string CustomField18 { get; set; }
        public string CustomField19 { get; set; }
        public string CustomField20 { get; set; }
    }
}
