//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    194ee541-81b8-4c90-82b0-5fbf1d9ea0c1
//        CLR Version:              4.0.30319.18063
//        Name:                     VoiceChanInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPService04
//        File Name:                VoiceChanInfo
//
//        created by Charley at 2015/6/25 10:22:22
//        http://www.voicecyber.com 
//
//======================================================================

namespace UMPService04
{
    public class VoiceChanInfo : ResourceConfigInfo
    {
        public const int RESOURCE_VOICECHANNEL = 225;

        public const int PRO_CHANNAME = 11;
        public const int PRO_EXTENSION = 12;

        public string ChanName { get; set; }
        public string Extension { get; set; }

        public override string LogInfo()
        {
            string strInfo = base.LogInfo();
            strInfo = string.Format("{0};Extension:{1};ChanName:{2}",
                strInfo,
                Extension,
                ChanName);
            return strInfo;
        }
    }
}
