//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    3fb9e7c8-a857-4611-97f4-cb29fb805aba
//        CLR Version:              4.0.30319.18408
//        Name:                     ServiceConfigInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPService10
//        File Name:                ServiceConfigInfo
//
//        created by Charley at 2016/6/27 16:12:01
//        http://www.voicecyber.com 
//
//======================================================================

namespace UMPService10
{
    public class ServiceConfigInfo : ResourceConfigInfo
    {
        public const int PRO_HOSTADDRESS = 7;
        public const int PRO_HOSTNAME = 8;
        public const int PRO_HOSTPORT = 9;
        public const int PRO_CONTINENT = 901;
        public const int PRO_COUNTRY = 902;

        public string HostAddress { get; set; }
        public string HostName { get; set; }
        public int HostPort { get; set; }
        public string Continent { get; set; }
        public string Country { get; set; }

        public override string LogInfo()
        {
            string strInfo = base.LogInfo();
            strInfo = string.Format("{0};HostAddress:{1};HostName:{2};HostPort:{3};Continent:{4};Country:{5}",
                strInfo,
                HostAddress,
                HostName,
                HostPort,
                Continent,
                Country);
            return strInfo;
        }
    }
}
