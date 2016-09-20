//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    6cf2d06b-dfe0-4b90-b8e3-fefb8e13043f
//        CLR Version:              4.0.30319.18444
//        Name:                     DatabaseInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1110.Models
//        File Name:                DatabaseInfo
//
//        created by Charley at 2015/1/24 14:18:32
//        http://www.voicecyber.com 
//
//======================================================================

namespace UMPS1110.Models
{
    public class DatabaseInfo
    {
        public int DBType { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string DBName { get; set; }
        public string LoginUser { get; set; }
        public string LoginPassword { get; set; }

        public string Token
        {
            get
            {
                return string.Format("{0}-{1}:{2}-{3}"
                    , DBType
                    , Host
                    , Port
                    , DBName);
            }
        }
    }
}
