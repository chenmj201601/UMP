//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    1eba2ff2-1336-46dd-970f-e95945c5aed3
//        CLR Version:              4.0.30319.18444
//        Name:                     OrgTypeInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common11011
//        File Name:                OrgTypeInfo
//
//        created by Charley at 2014/9/3 14:58:25
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.Common11011
{
    public class OrgTypeInfo
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int SortID { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
