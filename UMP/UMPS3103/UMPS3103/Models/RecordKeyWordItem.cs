using System;
using System.ComponentModel;
using System.Windows.Media;
using VoiceCyber.UMP.Common31031;

namespace UMPS3103.Models
{
    public class RecordKeyWordItem : INotifyPropertyChanged
    {
        public long RowID { set; get; }
        public long SerialID { get; set; }
        public long RecordID { get; set; }
        public string RecordReference { set; get; }

        public string ConfidenceLeve { set; get; }
        public string PictureName { set; get; }//关键词图标
        public string PicturePath { set; get; } //关键词路径
        public string ImageURL { set; get; }

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



        public RecordKeyWordInfo RecordKeyInfo { get; set; }

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

        public RecordKeyWordItem(RecordKeyWordInfo recordKeyWordInfo)
        {
            SerialID = recordKeyWordInfo.SerialID;
            RecordID = recordKeyWordInfo.RecordID;
            Offset = recordKeyWordInfo.Offset;
            Duration = recordKeyWordInfo.Duration;
            Title = recordKeyWordInfo.Title;
            Content = recordKeyWordInfo.Content;
            PictureName = recordKeyWordInfo.PictureName;
            PicturePath = recordKeyWordInfo.PicturePath;

            RecordKeyInfo = recordKeyWordInfo;
        }

        public void SetValues()
        {
            if (RecordKeyInfo != null)
            {
                RecordKeyInfo.Offset = Offset;
                RecordKeyInfo.Duration = Duration;
                RecordKeyInfo.Title = Title;
                RecordKeyInfo.Content = Content;
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
