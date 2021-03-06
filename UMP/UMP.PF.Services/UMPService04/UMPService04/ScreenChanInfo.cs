﻿//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    d9f34da2-6749-49ea-a993-05d00a54c334
//        CLR Version:              4.0.30319.18063
//        Name:                     ScreenChanInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPService04
//        File Name:                ScreenChanInfo
//
//        created by Charley at 2015/7/28 10:03:56
//        http://www.voicecyber.com 
//
//======================================================================

namespace UMPService04
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
