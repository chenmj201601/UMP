//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    8f38240f-5709-4723-a598-eba0ca6a9f0f
//        CLR Version:              4.0.30319.18444
//        Name:                     RecordBookmarkItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3102.Models
//        File Name:                RecordBookmarkItem
//
//        created by Charley at 2014/12/10 10:21:52
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.ComponentModel;
using System.Windows.Media;
using VoiceCyber.UMP.Common31021;

namespace UMPS3102.Models
{
    public class RecordBookmarkItem : INotifyPropertyChanged
    {
        public long SerialID { get; set; }
        public long RecordID { get; set; }
        private int mOffset;

        public int Offset
        {
            get { return mOffset; }
            set { mOffset = value; OnPropertyChanged("Offset"); }
        }

        private int mDuration;

        public int Duration
        {
            get { return mDuration; }
            set { mDuration = value; OnPropertyChanged("Duration"); }
        }
        private string mTitle;

        public string Title
        {
            get { return mTitle; }
            set { mTitle = value; OnPropertyChanged("Title"); }
        }

        private string mTooltipDisplay;

        public string TooltipDisplay
        {
            get { return mTooltipDisplay; }
            set { mTooltipDisplay = value; OnPropertyChanged("TooltipDisplay"); }
        }

        
        private string mContent;

        public string Content
        {
            get { return mContent; }
            set { mContent = value; OnPropertyChanged("Content"); }
        }
        public long RankID { get; set; }
        public long MarkerID { get; set; }
        public DateTime MarkTime { get; set; }


        public string IsHaveBookMarkRecord { get; set; }
        /// <summary>
        /// 音频类标签的时长
        /// </summary>
        public string BookmarkTimesLength { get; set; }

        /// <summary>
        /// 音频类标签的文件名
        /// </summary>
        public string BookMarkVoiceName { get; set; }

        /// <summary>
        /// 音频类标签的路径
        /// </summary>
        public string BookMarkVoicePath { get; set; }

        public string Teminal { get; set; }

        public RecordBookmarkInfo BookmarkInfo { get; set; }

        private double mCanvasLeft;

        public double CanvasLeft
        {
            get { return mCanvasLeft; }
            set { mCanvasLeft = value; OnPropertyChanged("CanvasLeft"); }
        }

        private double mLineWidth;

        public double LineWidth
        {
            get { return mLineWidth; }
            set { mLineWidth = value; OnPropertyChanged("LineWidth"); }
        }

        private Brush mBackground;

        public Brush Background
        {
            get { return mBackground; }
            set { mBackground = value; OnPropertyChanged("Background"); }
        }

        private bool mIsSelected;

        public bool IsSelected
        {
            get { return mIsSelected; }
            set { mIsSelected = value; OnPropertyChanged("IsSelected"); }
        }

        public RecordBookmarkItem(RecordBookmarkInfo bookmarkInfo)
        {
            SerialID = bookmarkInfo.SerialID;
            RecordID = bookmarkInfo.RecordID;
            Offset = bookmarkInfo.Offset;
            Duration = bookmarkInfo.Duration;
            Title = bookmarkInfo.Title;
            Content = bookmarkInfo.Content;
            RankID = bookmarkInfo.RankID;
            MarkerID = bookmarkInfo.MarkerID;
            MarkTime = bookmarkInfo.MarkTime;
            IsHaveBookMarkRecord = bookmarkInfo.IsHaveBookMarkRecord;
            BookmarkTimesLength = bookmarkInfo.BookmarkTimesLength;
            Teminal = bookmarkInfo.Teminal;

            if (bookmarkInfo.Teminal == "K")
            {
                TooltipDisplay = "K:" + bookmarkInfo.Title;
            }
            else 
            {
                TooltipDisplay = bookmarkInfo.Title;
            }

            BookmarkInfo = bookmarkInfo;
        }

        public void SetValues()
        {
            if (BookmarkInfo != null)
            {
                BookmarkInfo.Offset = Offset;
                BookmarkInfo.Duration = Duration;
                BookmarkInfo.Title = Title;
                BookmarkInfo.RankID = RankID;
                BookmarkInfo.Content = Content;
                BookmarkInfo.IsHaveBookMarkRecord = IsHaveBookMarkRecord;
                BookmarkInfo.BookmarkTimesLength = BookmarkTimesLength;
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
