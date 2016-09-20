//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    b384c6a8-18cf-4efd-a7a5-44129001ba94
//        CLR Version:              4.0.30319.18408
//        Name:                     LangInstallMode
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Updates
//        File Name:                LangInstallMode
//
//        created by Charley at 2016/5/27 09:38:28
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.Updates
{
    /// <summary>
    /// 语言包安装模式
    /// </summary>
    public enum LangInstallMode
    {
        /// <summary>
        /// 未知
        /// </summary>
        Unkown = 0,
        /// <summary>
        /// 追加，已经存在的不替换
        /// </summary>
        Additional = 1,
        /// <summary>
        /// 追加并替换，已经存在的会替换
        /// </summary>
        AddReplace = 2,
        /// <summary>
        /// 重置，删掉所有语言，重新初始化
        /// </summary>
        Reset = 3,
    }
}
