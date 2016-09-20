//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    e87e7bde-5e21-4fa0-a11c-90615ca23d90
//        CLR Version:              4.0.30319.18063
//        Name:                     DownloadParamInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPService03
//        File Name:                DownloadParamInfo
//
//        created by Charley at 2015/9/4 15:31:43
//        http://www.voicecyber.com 
//
//======================================================================

namespace UMPService03
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
