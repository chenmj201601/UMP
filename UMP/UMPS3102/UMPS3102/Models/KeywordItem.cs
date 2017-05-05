//======================================================================
//
//        Copyright © 2016 VoiceCyber Technologies Ltd.
//        All rights reserved
//        guid1:                    7cdf3cc3-c1a8-493b-8c28-34b50977b0f6
//        CLR Version:              4.0.30319.42000
//        Name:                     KeywordItem
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                UMPS3102.Models
//        File Name:                KeywordItem
//
//        Created by Charley at 2016/11/7 18:59:30
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.Generic;
using VoiceCyber.UMP.Common31021;
using VoiceCyber.Wpf.CustomControls;


namespace UMPS3102.Models
{
    public class KeywordItem : CheckableItemBase
    {
        public long ObjID { get; set; }
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

        private List<KeywordInfo> mListKeywordInfos = new List<KeywordInfo>();

        public List<KeywordInfo> ListKeywordInfos
        {
            get { return mListKeywordInfos; }
        }
    }
}
