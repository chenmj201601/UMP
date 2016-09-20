//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    b88e7569-ed12-412f-9203-8140a1dc5cec
//        CLR Version:              4.0.30319.18408
//        Name:                     ResourceConfigInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPService10
//        File Name:                ResourceConfigInfo
//
//        created by Charley at 2016/6/27 16:10:21
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.Generic;


namespace UMPService10
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
