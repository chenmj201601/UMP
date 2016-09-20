//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    c78f3ca1-85b2-40a2-b5b1-8bf7f75a2215
//        CLR Version:              4.0.30319.18444
//        Name:                     OrgUserItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3101.Models
//        File Name:                OrgUserItem
//
//        created by Charley at 2014/10/24 16:50:10
//        http://www.voicecyber.com 
//
//======================================================================
using VoiceCyber.Wpf.CustomControls;

namespace UMPS3602.Models
{
    public class OrgUserItem : CheckableItemBase
    {
        public long ObjID { get; set; }
        public string Name { get; set; }
        public int ObjType { get; set; }
        public string Description { get; set; }
        public object Data { get; set; }
    }
}