//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    055d0b4e-d3f7-4176-bfe3-d13999e1b5f3
//        CLR Version:              4.0.30319.18063
//        Name:                     ResourceConfigInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPService04
//        File Name:                ResourceConfigInfo
//
//        created by Charley at 2015/6/25 10:17:09
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.Generic;

namespace UMPService04
{
    public class ResourceConfigInfo
    {
        public const int PRO_KEY = 1;
        public const int PRO_ID = 2;
        public const int PRO_PARENTOBJID = 3;


        public long ObjID { get; set; }
        public int ObjType { get; set; }
        public long ParentObjID { get; set; }
        public int Key { get; set; }
        public int ID { get; set; }

        private List<ResourceConfigInfo> mListChildObjects;

        public List<ResourceConfigInfo> ListChildObjects
        {
            get { return mListChildObjects; }
        }

        public ResourceConfigInfo()
        {
            mListChildObjects = new List<ResourceConfigInfo>();
        }

        /// <summary>
        /// 记录详细信息
        /// </summary>
        /// <returns></returns>
        public virtual string LogInfo()
        {
            string strInfo = string.Format("ObjID:{0};ObjType:{1};Parent:{2};Key:{3};ID:{4}",
                ObjID,
                ObjType,
                ParentObjID,
                Key,
                ID);
            return strInfo;
        }
    }
}
