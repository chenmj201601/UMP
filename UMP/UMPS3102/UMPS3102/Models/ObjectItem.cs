//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    86e7b1cd-fee6-4ac9-b361-95f7e1a28464
//        CLR Version:              4.0.30319.18444
//        Name:                     ObjectItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3102.Models
//        File Name:                ObjectItem
//
//        created by Charley at 2014/11/12 14:33:36
//        http://www.voicecyber.com 
//
//======================================================================

using VoiceCyber.Wpf.CustomControls;

namespace UMPS3102.Models
{
    public class ObjectItem : CheckableItemBase
    {
        public long ObjID { get; set; }
        public string Name { get; set; }
        public int ObjType { get; set; }
        public string Description { get; set; }
        public object Data { get; set; }

        //tangche add
        public string FullName { get; set; }

    }
}
