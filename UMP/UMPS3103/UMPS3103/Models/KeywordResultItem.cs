using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using VoiceCyber.UMP.Common31031;

namespace UMPS3103.Models
{
    public class KeywordResultItem : INotifyPropertyChanged
    {
        private string mName;

        public string Name
        {
            get { return mName; }
            set { mName = value; OnPropertyChanged("Name"); }
        }

        private string mDescription;

        public string Description
        {
            get { return mDescription; }
            set { mDescription = value; OnPropertyChanged("Description"); }
        }

        private double mCanvasLeft;

        public double CanvasLeft
        {
            get { return mCanvasLeft; }
            set { mCanvasLeft = value; OnPropertyChanged("CanvasLeft"); }
        }

        public KeywordResultInfo Info { get; set; }

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
