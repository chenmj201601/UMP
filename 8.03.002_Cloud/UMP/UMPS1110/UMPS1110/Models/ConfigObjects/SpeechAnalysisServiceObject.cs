//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    40ffa1f0-1f09-4857-9d2d-1e1e5b51b2b7
//        CLR Version:              4.0.30319.18063
//        Name:                     SpeechAnalysisServiceObject
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1110.Models.ConfigObjects
//        File Name:                SpeechAnalysisServiceObject
//
//        created by Charley at 2015/11/1 16:02:29
//        http://www.voicecyber.com 
//
//======================================================================

namespace UMPS1110.Models.ConfigObjects
{
    public class SpeechAnalysisServiceObject : ServiceObject
    {
        public const int PRO_PCMDEVICE = 11;

        public const int PRO_ANALYSISENABLE = 21;
        public const int PRO_KEYWORDENABLE = 31;
        public const int PRO_SPEECHTOTXTENABLE = 41;

        public const int PRO_ANALYSISSOURCEACCESS = 22;
        public const int PRO_KEYWORDSOURCEACCESS = 32;
        public const int PRO_SPEECHTOTXTSOURCEACCESS = 42;
        //public const int PRO_SPEECHTOTXTTARGETACCESS = 52;

        public const int PRO_ANALYSISSOURCEDEVICE = 23;
        public const int PRO_KEYWORDSOURCEDEVICE = 33;
        public const int PRO_SPEECHTOTXTSOURCEDEVICE = 43;
        public const int PRO_SPEECHTOTXTTARGETDEVICE = 47;
    }
}
