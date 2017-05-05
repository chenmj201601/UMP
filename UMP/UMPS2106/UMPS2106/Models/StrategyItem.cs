//======================================================================
//
//        Copyright © 2016 VoiceCyber Technologies Ltd.
//        All rights reserved
//        guid1:                    108c345b-333d-4513-9ee1-bc66d3afefbd
//        CLR Version:              4.0.30319.42000
//        Name:                     StrategyItem
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                UMPS2106.Models
//        File Name:                StrategyItem
//
//        Created by Charley at 2016/10/19 15:45:53
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.Generic;
using System.ComponentModel;
using VoiceCyber.UMP.Common21061;


namespace UMPS2106.Models
{
    public class StrategyItem : INotifyPropertyChanged
    {

        private long mSerialNo;

        public long SerialNo
        {
            get { return mSerialNo; }
            set { mSerialNo = value; OnPropertyChanged("SerialNo"); }
        }

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

        private string mStrFlag;

        public string StrFlag
        {
            get { return mStrFlag; }
            set { mStrFlag = value; OnPropertyChanged("StrFlag"); }
        }

        public RecoverStrategyInfo Info { get; set; }

        private List<RecoverChannelInfo> mListChannels = new List<RecoverChannelInfo>();

        public List<RecoverChannelInfo> ListChannels
        {
            get { return mListChannels; }
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
