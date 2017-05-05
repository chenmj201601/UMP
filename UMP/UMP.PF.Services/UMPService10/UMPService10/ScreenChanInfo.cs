//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    e031baf3-6162-4412-96a0-d4bbc4cc03c0
//        CLR Version:              4.0.30319.18408
//        Name:                     ScreenChanInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPService10
//        File Name:                ScreenChanInfo
//
//        created by Charley at 2016/7/7 17:57:14
//        http://www.voicecyber.com 
//
//======================================================================

namespace UMPService10
{
    public class ScreenChanInfo : ResourceConfigInfo
    {
        public const int RESOURCE_SCREENCHANNEL = 232;

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
