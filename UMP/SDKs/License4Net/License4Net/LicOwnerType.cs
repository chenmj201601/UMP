//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    5d7ebb08-e318-4c09-8e57-35ade1275fbd
//        CLR Version:              4.0.30319.18063
//        Name:                     LicOwnerType
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.SDKs.Licenses
//        File Name:                LicOwnerType
//
//        created by Charley at 2015/9/14 10:25:42
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.SDKs.Licenses
{
    /// <summary>
    /// License所有者类型
    /// </summary>
    public enum LicOwnerType
    {
        /// <summary>
        /// 未知
        /// </summary>
        Unkown = 0,
        /// <summary>
        /// 独占License
        /// </summary>
        Mono = 1,
        /// <summary>
        /// 共享License
        /// </summary>
        Share = 2
    }
}
