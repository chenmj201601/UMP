//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    6e4372f9-7082-4bbf-9251-687b2aa3f10e
//        CLR Version:              4.0.30319.18408
//        Name:                     ScreenServerInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPService10
//        File Name:                ScreenServerInfo
//
//        created by Charley at 2016/7/7 17:56:05
//        http://www.voicecyber.com 
//
//======================================================================

namespace UMPService10
{
    public class ScreenServerInfo : ServiceConfigInfo
    {
        public const int RESOURCE_SCREENSERVER = 231;

        public const int PRO_MONPORT = 14;

        public int MonPort { get; set; }

        public override string LogInfo()
        {
            string strInfo = base.LogInfo();
            strInfo = string.Format("{0};MonPort:{1}",
                strInfo,
                MonPort);
            return strInfo;
        }
    }
}
