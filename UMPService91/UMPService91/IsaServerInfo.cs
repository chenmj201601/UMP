//======================================================================
//
//        Copyright © 2016-2017 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        GUID1:                    510cc6ee-813d-46e6-9d52-20ecf6d2399b
//        CLR Version:              4.0.30319.42000
//        Name:                     IsaServerInfo
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                UMPService91
//        File Name:                IsaServerInfo
//
//        Created by Charley at 2016/8/18 11:00:02
//        http://www.voicecyber.com 
//
//======================================================================

namespace UMPService91
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
