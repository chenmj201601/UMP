//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    04a46326-abee-48c1-896f-e82f86b6d2ab
//        CLR Version:              4.0.30319.18063
//        Name:                     VoiceServerInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPService04
//        File Name:                VoiceServerInfo
//
//        created by Charley at 2015/6/25 10:18:53
//        http://www.voicecyber.com 
//
//======================================================================

namespace UMPService04
{
    public class VoiceServerInfo : ServiceConfigInfo
    {
        public const int RESOURCE_VOICESERVER = 221;

        public const int PRO_NMONPORT = 22;

        public int NMonPort { get; set; }

        public override string LogInfo()
        {
            string strInfo = base.LogInfo();
            strInfo = string.Format("{0};MonPort:{1}",
                strInfo,
                NMonPort);
            return strInfo;
        }
    }
}
