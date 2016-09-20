﻿//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    3236046f-dbcc-4688-a8fe-9c425f320d11
//        CLR Version:              4.0.30319.18063
//        Name:                     ServiceConfigInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPService07
//        File Name:                ServiceConfigInfo
//
//        created by Charley at 2015/11/24 13:52:05
//        http://www.voicecyber.com 
//
//======================================================================

namespace UMPService07
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
