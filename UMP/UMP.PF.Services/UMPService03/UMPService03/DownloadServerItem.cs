//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    081e617f-d953-48bc-b937-5c64792fb109
//        CLR Version:              4.0.30319.18063
//        Name:                     DownloadServerItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPService03
//        File Name:                DownloadServerItem
//
//        created by Charley at 2015/9/4 17:45:17
//        http://www.voicecyber.com 
//
//======================================================================

namespace UMPService03
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
