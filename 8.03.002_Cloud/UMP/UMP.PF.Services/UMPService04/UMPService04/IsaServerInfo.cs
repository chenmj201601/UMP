//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    6bfd9ed9-4358-43c1-a8a3-bee1c4f7930b
//        CLR Version:              4.0.30319.18063
//        Name:                     IsaServerInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPService04
//        File Name:                IsaServerInfo
//
//        created by Charley at 2015/10/25 16:28:19
//        http://www.voicecyber.com 
//
//======================================================================

namespace UMPService04
{
    public class IsaServerInfo : ServiceConfigInfo
    {
        public const int RESOURCE_ISASERVER = 233;

        public const int PRO_ACCESSTOKEN = 11;

        public string AccessToken { get; set; }

        public override string LogInfo()
        {
            string strInfo = base.LogInfo();
            strInfo = string.Format("{0};AccessToken:{1}",
                strInfo,
                AccessToken);
            return strInfo;
        }
    }
}
