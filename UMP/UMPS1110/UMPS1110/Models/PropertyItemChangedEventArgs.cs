//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    aeeec381-e4fb-4770-9c3b-58634e8b5898
//        CLR Version:              4.0.30319.18444
//        Name:                     PropertyItemChangedEventArgs
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1110.Models
//        File Name:                PropertyItemChangedEventArgs
//
//        created by Charley at 2015/1/19 11:17:16
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.Generic;

namespace UMPS1110.Models
{
    public class PropertyItemChangedEventArgs
    {
        public ConfigObject ConfigObject { get; set; }

        private List<ConfigObject> mListConfigObjects = new List<ConfigObject>();

        public List<ConfigObject> ListConfigObjects
        {
            get { return mListConfigObjects; }
        }
        public ResourcePropertyInfoItem PropertyItem { get; set; }
    }
}
