//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    9e881758-8e54-4e80-ad2a-3215ac486560
//        CLR Version:              4.0.30319.18408
//        Name:                     DECServerInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPService10
//        File Name:                DECServerInfo
//
//        created by Charley at 2016/6/27 16:11:29
//        http://www.voicecyber.com 
//
//======================================================================

namespace UMPService10
{
    public class DECServerInfo : ServiceConfigInfo
    {
        public const int RESOURCE_DATAEXCHANGECENTER = 212;

        public override string LogInfo()
        {
            string strInfo = base.LogInfo();
            strInfo = string.Format("{0};",
                strInfo);
            return strInfo;
        }
    }
}
