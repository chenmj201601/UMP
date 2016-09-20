//======================================================================
//
//        Copyright © 2016-2017 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        GUID1:                    a1129e40-3a82-49fd-ba68-e9aa0652c71f
//        CLR Version:              4.0.30319.42000
//        Name:                     ResourceConfigInfo
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                UMPService91
//        File Name:                ResourceConfigInfo
//
//        Created by Charley at 2016/8/18 10:58:32
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.Generic;


namespace UMPService91
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
