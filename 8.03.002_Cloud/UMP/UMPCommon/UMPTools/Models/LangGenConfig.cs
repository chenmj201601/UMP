//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    d38d5b88-ae91-4f7a-ba9c-7afe151522fc
//        CLR Version:              4.0.30319.18063
//        Name:                     LangGenConfig
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPTools.Models
//        File Name:                LangGenConfig
//
//        created by Charley at 2015/8/2 14:59:00
//        http://www.voicecyber.com 
//
//======================================================================

using VoiceCyber.UMP.Common;

namespace UMPTools.Models
{
    public class LangGenConfig
    {
        public LangEntityItem Entity { get; set; }
        public LangModuleItem Module { get; set; }
        public DatabaseInfo DBInfo { get; set; }
        public bool IsReplace { get; set; }
        public bool Is2052Only { get; set; }
    }
}
