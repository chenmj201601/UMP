//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    3ddbef25-67a0-4b29-9a2a-d0d6c85c9f53
//        CLR Version:              4.0.30319.18063
//        Name:                     ScreenServerInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPService04
//        File Name:                ScreenServerInfo
//
//        created by Charley at 2015/7/28 9:44:11
//        http://www.voicecyber.com 
//
//======================================================================

namespace UMPService04
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
