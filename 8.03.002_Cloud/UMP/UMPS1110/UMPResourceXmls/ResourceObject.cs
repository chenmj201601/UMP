//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    2ac0a0c2-ef97-423f-b5fe-ab62830e8243
//        CLR Version:              4.0.30319.18444
//        Name:                     ResourceObject
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ResourceXmls
//        File Name:                ResourceObject
//
//        created by Charley at 2015/2/13 9:39:41
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.Generic;
using VoiceCyber.UMP.Common11101;

namespace VoiceCyber.UMP.ResourceXmls
{
    public class ResourceObject
    {
        public long ObjID { get; set; }
        public int ObjType { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public int Key { get; set; }
        public int ID { get; set; }
        public long ParentID { get; set; }
        public int ModuleNumber { get; set; }
        public string HostAddress { get; set; }
        public string HostName { get; set; }
        public int HostPort { get; set; }

        private List<ResourceProperty> mListPropertyValues;

        public List<ResourceProperty> ListPropertyValues
        {
            get { return mListPropertyValues; }
        }

        public ResourceObject()
        {
            mListPropertyValues = new List<ResourceProperty>();
        }
    }
}
