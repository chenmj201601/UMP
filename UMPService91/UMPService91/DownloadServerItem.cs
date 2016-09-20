//======================================================================
//
//        Copyright © 2016-2017 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        GUID1:                    a1189347-00d9-4af5-8c68-14e9302c7e54
//        CLR Version:              4.0.30319.42000
//        Name:                     DownloadServerItem
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                UMPService91
//        File Name:                DownloadServerItem
//
//        Created by Charley at 2016/8/18 10:59:22
//        http://www.voicecyber.com 
//
//======================================================================

namespace UMPService91
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
