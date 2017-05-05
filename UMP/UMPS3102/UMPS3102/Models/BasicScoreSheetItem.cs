//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    dbd0e30b-20d6-4c18-aa70-1f787429477b
//        CLR Version:              4.0.30319.18444
//        Name:                     BasicScoreSheetItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3102.Models
//        File Name:                BasicScoreSheetItem
//
//        created by Charley at 2014/11/16 14:27:05
//        http://www.voicecyber.com 
//
//======================================================================

using System.ComponentModel;
using System.Windows.Media;
using VoiceCyber.UMP.Common31021;
using VoiceCyber.UMP.Controls;

namespace UMPS3102.Models
{
    public class BasicScoreSheetItem : INotifyPropertyChanged
    {
        public long ScoreResultID { get; set; }
        public long ScoreSheetID { get; set; }
        public long RecordSerialID { get; set; }
        public long UserID { get; set; }
        public long AgentID { get; set; }
        public long OrgID { get; set; }
        public int RowNumber { get; set; }
        public string Title { get; set; }
        public double TotalScore { get; set; }
        public double Score { get; set; }
        public bool IsFinalScore { get; set; }
        /// <summary>
        /// 修改成绩前的评分成绩ID，新评分的此字段为0
        /// </summary>
        public long OldScoreResultID { get; set; }

        public Brush Background { get; set; }

        private int mFlag;
        /// <summary>
        /// Flag为0 表示增加按钮;Flag为1 表示修改按钮
        /// </summary>
        public int Flag
        {
            get { return mFlag; }
            set { mFlag = value; OnPropertyChanged("Flag"); }
        }

        public double WasteTime { get; set; }

        private bool mIsVisible;

        public bool IsVisible
        {
            get { return mIsVisible; }
            set { mIsVisible = value; OnPropertyChanged("IsVisible"); }
        }

        private string mStrScore;

        public string StrScore
        {
            get { return mStrScore; }
            set { mStrScore = value; OnPropertyChanged("StrScore"); }
        }

        public BasicScoreSheetInfo ScoreSheetInfo { get; set; }
        public UMPApp CurrentApp;

        public BasicScoreSheetItem(BasicScoreSheetInfo scoreSheetInfo)
        {
            ScoreResultID = scoreSheetInfo.ScoreResultID;
            ScoreSheetID = scoreSheetInfo.ScoreSheetID;
            RecordSerialID = scoreSheetInfo.RecordSerialID;
            Title = scoreSheetInfo.Title;
            TotalScore = scoreSheetInfo.TotalScore;
            Score = scoreSheetInfo.Score;
            UserID = scoreSheetInfo.UserID;
            AgentID = scoreSheetInfo.AgentID;
            OrgID = scoreSheetInfo.OrgID;
            IsFinalScore = scoreSheetInfo.IsFinalScore;
            //Flag = scoreSheetInfo.Flag;
            //if (Flag == 0)
            //{
            //    StrScore = CurrentApp.GetLanguageInfo("3102TIP001IsScored0", "No Score");
            //}
            //else
            //{
            //    StrScore = Score.ToString();
            //}
            //WasteTime = scoreSheetInfo.WasteTime;
            ScoreSheetInfo = scoreSheetInfo;
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
