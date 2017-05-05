//======================================================================
//
//        Copyright © 2016 VoiceCyber Technologies Ltd.
//        All rights reserved
//        guid1:                    77b7d13d-8de1-46d3-bfa7-b44fffee9932
//        CLR Version:              4.0.30319.42000
//        Name:                     KeywordResultItem
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                UMPS3102.Models
//        File Name:                KeywordResultItem
//
//        Created by Charley at 2016/11/8 10:46:42
//        http://www.voicecyber.com 
//
//======================================================================

using System.ComponentModel;
using VoiceCyber.UMP.Common31021;


namespace UMPS3102.Models
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
