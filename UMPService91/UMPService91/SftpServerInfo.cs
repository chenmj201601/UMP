//======================================================================
//
//        Copyright © 2016-2017 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        GUID1:                    3ee73076-0b86-4149-aba3-fc298d6355b2
//        CLR Version:              4.0.30319.42000
//        Name:                     SftpServerInfo
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                UMPService91
//        File Name:                SftpServerInfo
//
//        Created by Charley at 2016/8/18 11:02:45
//        http://www.voicecyber.com 
//
//======================================================================

namespace UMPService91
{
    public class SftpServerInfo : ResourceConfigInfo
    {
        public const int RESOURCE_SFTPSERVER = 219;

        public const int PRO_ROOTDIR = 21;

        public string HostAddress { get; set; }
        public string HostName { get; set; }
        public int HostPort { get; set; }

        public string RootDir { get; set; }

        public string AuthName { get; set; }
        public string AuthPassword { get; set; }

        public override string LogInfo()
        {
            string strInfo = base.LogInfo();
            strInfo =
                string.Format("{0};HostAddress:{1};HostName:{2};HostPort:{3};RootDir:{4};AuthName:{5};AuthPassword:***",
                    strInfo,
                    HostAddress,
                    HostName,
                    HostPort,
                    RootDir,
                    AuthName);
            return strInfo;
        }
    }
}
