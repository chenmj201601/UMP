//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    1bcb48ac-bee5-4daf-aa41-6d8913fadedb
//        CLR Version:              4.0.30319.18444
//        Name:                     NetworkCardInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1110.Models
//        File Name:                NetworkCardInfo
//
//        created by Charley at 2015/2/3 16:48:48
//        http://www.voicecyber.com 
//
//======================================================================

namespace UMPS1110.Models
{
    public class NetworkCardInfo
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return string.Format("[{0}][{1}][{2}]", ID, Name, Description);
        }
    }
}
