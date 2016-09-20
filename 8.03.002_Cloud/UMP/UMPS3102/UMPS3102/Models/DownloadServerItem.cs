//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    75b5a4e9-b647-4a2d-93b9-fe59d5fd4946
//        CLR Version:              4.0.30319.18063
//        Name:                     DownloadServerItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3102.Models
//        File Name:                DownloadServerItem
//
//        created by Charley at 2015/5/7 20:22:34
//        http://www.voicecyber.com 
//
//======================================================================

namespace UMPS3102.Models
{
    public class DownloadServerItem
    {
        /// <summary>
        /// 方式
        /// 0       SftpServer
        /// 1       DownloadParam（NAS）
        /// </summary>
        public int Type { get; set; }
        public object Info { get; set; }
    }
}
