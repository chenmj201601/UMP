//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    5253f4e9-3b82-4d11-841d-ca35066e01b6
//        CLR Version:              4.0.30319.18063
//        Name:                     DownloadParamInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common31021
//        File Name:                DownloadParamInfo
//
//        created by Charley at 2015/5/7 18:36:21
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.Common31021
{
    public class DownloadParamInfo
    {
        public long ObjID { get; set; }
        public int ID { get; set; }
        public bool IsEnabled { get; set; }
        public int Method { get; set; }
        public int VoiceID { get; set; }
        public string VoiceAddress { get; set; }
        public string Address { get; set; }
        public int Port { get; set; }
        public string RootDir { get; set; }
        public string VocPathFormat { get; set; }
        public string ScrPathFormat { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
