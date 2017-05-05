//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    b8e0851b-8213-4e56-b87d-4a8ced61ba8e
//        CLR Version:              4.0.30319.18063
//        Name:                     ModuleInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPTools.Models
//        File Name:                ModuleInfo
//
//        created by Charley at 2015/8/24 11:06:38
//        http://www.voicecyber.com 
//
//======================================================================

namespace UMPTools.Models
{
    /// <summary>
    /// 模块信息，也包括资源编号信息
    /// </summary>
    public class ModuleInfo
    {
        /// <summary>
        /// 2位或4位的模块编号或3位的资源编号
        /// </summary>
        public int Module { get; set; }
        public string Name { get; set; }
        public string Display { get; set; }
        /// <summary>
        /// 两位的大模块号，资源编码的大模块号为0或1位
        /// </summary>
        public int Main { get; set; }

        public override string ToString()
        {
            return string.Format("[{0}]{1}", Module, Name);
        }
    }
}
