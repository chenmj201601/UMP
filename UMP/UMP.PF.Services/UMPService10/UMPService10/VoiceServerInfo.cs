//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    e331b306-e92e-4bdd-b207-2b62596481d2
//        CLR Version:              4.0.30319.18408
//        Name:                     VoiceServerInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPService10
//        File Name:                VoiceServerInfo
//
//        created by Charley at 2016/7/5 16:59:56
//        http://www.voicecyber.com 
//
//======================================================================

namespace UMPService10
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
