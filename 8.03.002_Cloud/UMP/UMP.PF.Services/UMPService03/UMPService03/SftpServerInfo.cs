//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    2fd0f7b7-dfd5-4794-a665-6c62895f4df2
//        CLR Version:              4.0.30319.18063
//        Name:                     SftpServerInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPService03
//        File Name:                SftpServerInfo
//
//        created by Charley at 2015/9/4 15:31:12
//        http://www.voicecyber.com 
//
//======================================================================

namespace UMPService03
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
