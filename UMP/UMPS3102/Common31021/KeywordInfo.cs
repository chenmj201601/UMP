//======================================================================
//
//        Copyright © 2016 VoiceCyber Technologies Ltd.
//        All rights reserved
//        guid1:                    a0ce83d2-3f1f-4bd4-9469-b12e9a59c6a2
//        CLR Version:              4.0.30319.42000
//        Name:                     KeywordInfo
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common31021
//        File Name:                KeywordInfo
//
//        Created by Charley at 2016/11/7 17:55:18
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.Common31021
{
    public class KeywordInfo
    {
        public long SerialNo { get; set; }
        public string Name { get; set; }
        public long ContentNo { get; set; }
        public string Content { get; set; }
        public string Icon { get; set; }
        /// <summary>
        /// 0：正常；1：删除；2：禁用
        /// </summary>
        public int State { get; set; }

    }
}
