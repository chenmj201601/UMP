//======================================================================
//
//        Copyright © 2016-2017 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        GUID1:                    d0a201b2-2663-4c27-aa8c-3180f7f9f0ff
//        CLR Version:              4.0.30319.42000
//        Name:                     BackupState
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                UMPUpdater.Models
//        File Name:                BackupState
//
//        Created by Charley at 2016/8/31 13:22:19
//        http://www.voicecyber.com 
//
//======================================================================

using VoiceCyber.SharpZips.Zip;


namespace UMPUpdater.Models
{
    public class BackupState
    {
        public string Dir { get; set; }
        public string Name { get; set; }
        public string ProductName { get; set; }
        public ZipOutputStream Stream { get; set; }
        public long TotalSize { get; set; }
        public long CurrentSize { get; set; }
        public int ProductCount { get; set; }
        public int ProductIndex { get; set; }
        public UpdateStateItem StateItem { get; set; }
    }
}
