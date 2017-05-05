//======================================================================
//
//        Copyright © 2016 VoiceCyber Technologies Ltd.
//        All rights reserved
//        guid1:                    efd506fd-0c81-41a3-ae1d-e528ffbde782
//        CLR Version:              4.0.30319.42000
//        Name:                     ChannelItem
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                UMPS2106.Models
//        File Name:                ChannelItem
//
//        Created by Charley at 2016/10/19 18:19:44
//        http://www.voicecyber.com 
//
//======================================================================

using System.ComponentModel;
using VoiceCyber.UMP.Common21061;


namespace UMPS2106.Models
{
    public class ChannelItem : INotifyPropertyChanged
    {

        private int mNumber;

        public int Number
        {
            get { return mNumber; }
            set { mNumber = value; OnPropertyChanged("Number"); }
        }

        private string mExtension;

        public string Extension
        {
            get { return mExtension; }
            set { mExtension = value; OnPropertyChanged("Extension"); }
        }

        private int mChannel;

        public int Channel
        {
            get { return mChannel; }
            set { mChannel = value; OnPropertyChanged("Channel"); }
        }

        private string mVoice;

        public string Voice
        {
            get { return mVoice; }
            set { mVoice = value; OnPropertyChanged("Voice"); }
        }

        public RecoverChannelInfo Info { get; set; }


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
