//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    560834a6-6247-4cbb-8327-7133dafd5596
//        CLR Version:              4.0.30319.18063
//        Name:                     IsaServerInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPService03
//        File Name:                IsaServerInfo
//
//        created by Charley at 2015/10/12 11:35:54
//        http://www.voicecyber.com 
//
//======================================================================

namespace UMPService03
{
    public class IsaServerInfo : ResourceConfigInfo
    {
        public const int RESOURCE_ISASERVER = 233;

        public const int PRO_ACEESS_TOKEN = 11;

        public string HostAddress { get; set; }
        public string HostName { get; set; }
        public int HostPort { get; set; }

        public string AccessToken { get; set; }

        public override string LogInfo()
        {
            string strInfo = base.LogInfo();
            strInfo = string.Format("{0};HostAddress:{1};HostName:{2};HostPort:{3};AccessToken:{4}",
                strInfo,
                HostAddress,
                HostName,
                HostPort,
                AccessToken);
            return strInfo;
        }
    }
}
