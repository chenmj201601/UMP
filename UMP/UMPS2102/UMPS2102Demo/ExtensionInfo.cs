//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    0f7fea1d-7abc-4cf0-a0aa-c15c3f8070be
//        CLR Version:              4.0.30319.18063
//        Name:                     ExtensionInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS2102Demo
//        File Name:                ExtensionInfo
//
//        created by Charley at 2015/7/10 14:41:02
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;

namespace UMPS2102Demo
{
    class ExtensionInfo
    {
        public long ObjID { get; set; }
        public int ID { get; set; }
        public long ServerObjID { get; set; }
        public long ChanObjID { get; set; }
        public int ChanID { get; set; }
        public string Extension { get; set; }
        public string ChanName { get; set; }
    }
}
