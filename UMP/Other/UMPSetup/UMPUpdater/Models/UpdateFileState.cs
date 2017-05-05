//======================================================================
//
//        Copyright © 2016-2017 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        GUID1:                    474aaa4c-c003-4ac7-8dc9-86ddf740697d
//        CLR Version:              4.0.30319.42000
//        Name:                     UpdateFileState
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                UMPUpdater.Models
//        File Name:                UpdateFileState
//
//        Created by Charley at 2016/9/1 10:27:18
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoiceCyber.UMP.Updates;


namespace UMPUpdater.Models
{
    public class UpdateFileState
    {
        public UpdateStateItem StateItem { get; set; }
        public UpdateFile UpdateFile { get; set; }
        public string SourceDir { get; set; }
        public int FileCount { get; set; }
        public int FileIndex { get; set; }
    }
}
