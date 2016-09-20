//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    743edb5c-8cbf-4334-a7db-95b71ded8627
//        CLR Version:              4.0.30319.18408
//        Name:                     AgentStateInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common44101
//        File Name:                AgentStateInfo
//
//        created by Charley at 2016/6/22 09:33:59
//        http://www.voicecyber.com 
//
//======================================================================

using System;

namespace VoiceCyber.UMP.Common44101
{
    public class AgentStateInfo
    {
        public long ObjID { get; set; }
        public int Number { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public int State { get; set; }
        public string Color { get; set; }
        public string Icon { get; set; }
        public bool IsWorkTime { get; set; }
        public int Value { get; set; }
        public string Description { get; set; }
        public long Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public long Modifier { get; set; }
        public DateTime ModifyTime { get; set; }

        public override string ToString()
        {
            return string.Format("[{0}][{1}][{2}][{3}]", ObjID, Name, Type, Value);
        }
    }
}
