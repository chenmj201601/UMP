//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    c5b522dc-9688-44b3-8427-107b756340d7
//        CLR Version:              4.0.30319.18063
//        Name:                     FilterInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPLanguageManager.Models
//        File Name:                FilterInfo
//
//        created by Charley at 2015/4/25 16:03:20
//        http://www.voicecyber.com 
//
//======================================================================

namespace UMPLanguageManager.Models
{
    public class FilterInfo
    {
        public const int TYPE_ALLMODULE = 10;
        public const int TYPE_MODULE = 11;
        public const int TYPE_SUBMODULE = 12;
        public const int TYPE_ALLPRIFIX = 20;
        public const int TYPE_PREFIX = 21;
        public const int TYPE_ALLCATEGORY = 30;
        public const int TYPE_CATEGORY = 31;

        /// <summary>
        /// 类型
        /// 10       全部模块
        /// 11       大模块
        /// 12       小模块
        /// 20      全部前缀
        /// 21      前缀
        /// 30      全部类别
        /// 31      类别
        /// </summary>
        public int Type { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Other01 { get; set; }
        public string Other02 { get; set; }
        public string Other03 { get; set; }
        public string Other04 { get; set; }
        public string Other05 { get; set; }
        public int SortID { get; set; }
    }
}
