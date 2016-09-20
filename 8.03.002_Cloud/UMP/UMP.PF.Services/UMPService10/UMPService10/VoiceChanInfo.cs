//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    2f0e9226-8e06-4044-bbe5-621217ca58c8
//        CLR Version:              4.0.30319.18408
//        Name:                     VoiceChanInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPService10
//        File Name:                VoiceChanInfo
//
//        created by Charley at 2016/7/5 17:02:16
//        http://www.voicecyber.com 
//
//======================================================================

namespace UMPService10
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
