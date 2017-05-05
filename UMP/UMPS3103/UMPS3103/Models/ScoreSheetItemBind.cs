using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Media;
using VoiceCyber.UMP.Common31031;

namespace UMPS3103.Models
{
     public class ScoreSheetItemBind : INotifyPropertyChanged
    {
       
        public long ScoreSheetID { get; set; }       
        public string Title { get; set; }
        public double TotalScore { get; set; }     
        public Brush Background { get; set; }

        private int mFlag;

        public int Flag
        {
            get { return mFlag; }
            set { mFlag = value; OnPropertyChanged("Flag"); }
        }

         public ScoreSheetInfo ScoreSheetInfo { get; set; }

         public ScoreSheetItemBind(ScoreSheetInfo scoreSheetInfo)
        {
            ScoreSheetID = scoreSheetInfo.ScoreSheetID;         
            Title = scoreSheetInfo.Title;
            TotalScore = scoreSheetInfo.TotalScore;           
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
