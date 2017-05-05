//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    5667209c-a954-4aca-b471-81663cf390f5
//        CLR Version:              4.0.30319.18063
//        Name:                     ResourceConfigInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPService03
//        File Name:                ResourceConfigInfo
//
//        created by Charley at 2015/9/4 15:29:55
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.Generic;

namespace UMPService03
{
    public class ResourceConfigInfo
    {
        public const int PRO_KEY = 1;
        public const int PRO_ID = 2;
        public const int PRO_PARENTOBJID = 3;
        public const int PRO_HOSTADDRESS = 7;
        public const int PRO_HOSTNAME = 8;
        public const int PRO_HOSTPORT = 9;

        public const int PRO_AUTHNAME = 911;
        public const int PRO_AUTHPASSWORD = 912;

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
