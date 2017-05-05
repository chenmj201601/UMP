//======================================================================
//
//        Copyright © 2016 VoiceCyber Technologies Ltd.
//        All rights reserved
//        guid1:                    7bccf169-7d87-41b1-b94b-d61d6374922b
//        CLR Version:              4.0.30319.42000
//        Name:                     S2106Config
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                UMPS2106.Models
//        File Name:                S2106Config
//
//        Created by Charley at 2016/10/19 16:24:36
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.Generic;
using System.Xml.Serialization;


namespace UMPS2106.Models
{
    [XmlRoot]
    public class S2106Config
    {
        public const string FILE_NAME = "S2106Config.xml";

        private List<RecoverStrategyConfig> mListStrategies = new List<RecoverStrategyConfig>();

        [XmlArray(ElementName = "Strategies")]
        [XmlArrayItem(ElementName = "Strategy")]
        public List<RecoverStrategyConfig> ListStrategies
        {
            get { return mListStrategies; }
        } 
    }
}
