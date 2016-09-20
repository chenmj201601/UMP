//======================================================================
//
//        Copyright © 2016-2017 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        GUID1:                    96e5c799-ab3b-4847-b0a7-923731988e9c
//        CLR Version:              4.0.30319.42000
//        Name:                     DownloadParamInfo
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                UMPService91
//        File Name:                DownloadParamInfo
//
//        Created by Charley at 2016/8/18 10:57:58
//        http://www.voicecyber.com 
//
//======================================================================

namespace UMPService91
{
    public class DownloadParamInfo : ResourceConfigInfo
    {
        public const int RESOURCE_DOWNLOADPARAM = 291;

        public const int PRO_METHOD = 11;
        public const int PRO_ADDRESS = 13;
        public const int PRO_PORT = 14;
        public const int PRO_ROOTDIR = 15;
        public const int PRO_SERVERIP = 16;
        public const int PRO_VOCPATHFORMAT = 21;
        public const int PRO_SCRPATHFORMAT = 22;

        public int Method { get; set; }
        public string Address { get; set; }
        public int Port { get; set; }
        public string RootDir { get; set; }
        public string ServerIP { get; set; }
        public string VocPathFormat { get; set; }
        public string ScrPathFormat { get; set; }

        public string AuthName { get; set; }
        public string AuthPassword { get; set; }

        public override string LogInfo()
        {
            string strInfo = base.LogInfo();
            strInfo =
                string.Format(
                    "{0};Method:{1};Address:{2};Port:{3};RootDir:{4};ServerIP:{5};VocPathFormat:{6};ScrPathFormat:{7};AuthName:{8};AuthPassword:***",
                    strInfo,
                    Method,
                    Address,
                    Port,
                    RootDir,
                    ServerIP,
                    VocPathFormat,
                    ScrPathFormat,
                    AuthName);
            return strInfo;
        }
    }
}
