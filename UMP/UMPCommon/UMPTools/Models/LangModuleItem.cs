//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    460e80ba-e936-4639-ac74-4720aa1e9c5a
//        CLR Version:              4.0.30319.18063
//        Name:                     LangModuleItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPTools.Models
//        File Name:                LangModuleItem
//
//        created by Charley at 2015/8/2 13:40:12
//        http://www.voicecyber.com 
//
//======================================================================

namespace UMPTools.Models
{
    public class LangModuleItem
    {
        /// <summary>
        /// 4位的模块编号或3位的资源编号
        /// </summary>
        public int Module { get; set; }
        public string Name { get; set; }
        public string Display { get; set; }
        /// <summary>
        /// 两位的大模块号，资源编码的大模块号为0
        /// </summary>
        public int Main { get; set; }
    }
}
