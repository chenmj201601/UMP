//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    3ff7e43a-abce-4e2a-b0a9-3fe16521411a
//        CLR Version:              4.0.30319.18444
//        Name:                     IModuleObject
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1110.Models
//        File Name:                IModuleObject
//
//        created by Charley at 2015/4/13 14:50:21
//        http://www.voicecyber.com 
//
//======================================================================

namespace UMPS1110.Models
{
    /// <summary>
    /// 模块对象，某些资源在同一系统中存在多个模块，使用ModuleNumber唯一标识各个模块
    /// </summary>
    public interface IModuleObject
    {
        /// <summary>
        /// 模块编号
        /// </summary>
        int ModuleNumber { get; set; }
    }
}
