using Common3105;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Media;
namespace UMPS3105.Models
{
    public class BasicScoreSheetItem : INotifyPropertyChanged
    {
        public long ScoreResultID { get; set; }
        public long ScoreSheetID { get; set; }
        public long RecordSerialID { get; set; }
        public long UserID { get; set; }
        public int RowNumber { get; set; }
        public string Title { get; set; }
        public double TotalScore { get; set; }
        public double Score { get; set; }

        public Brush Background { get; set; }

        private int mFlag;

        public int Flag
        {
            get { return mFlag; }
            set { mFlag = value; OnPropertyChanged("Flag"); }
        }

        public BasicScoreSheetInfo ScoreSheetInfo { get; set; }

        public BasicScoreSheetItem(BasicScoreSheetInfo scoreSheetInfo)
        {
            ScoreResultID = scoreSheetInfo.ScoreResultID;
            ScoreSheetID = scoreSheetInfo.ScoreSheetID;
            RecordSerialID = scoreSheetInfo.RecordSerialID;
            Title = scoreSheetInfo.Title;
            TotalScore = scoreSheetInfo.TotalScore;
            Score = scoreSheetInfo.Score;
            UserID = scoreSheetInfo.UserID;

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
