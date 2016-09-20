//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    6b02ff68-cf0d-4aeb-8e0b-6b452c6d5006
//        CLR Version:              4.0.30319.18063
//        Name:                     ChanConfigInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPService07
//        File Name:                ChanConfigInfo
//
//        created by Charley at 2015/11/24 13:54:43
//        http://www.voicecyber.com 
//
//======================================================================

namespace UMPService07
{
    public class ChanConfigInfo : ResourceConfigInfo
    {
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
