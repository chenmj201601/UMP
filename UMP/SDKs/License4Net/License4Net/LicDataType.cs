//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    ed9fe29a-6322-452d-83ce-d0d5732ad660
//        CLR Version:              4.0.30319.18063
//        Name:                     LicDataType
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.SDKs.Licenses
//        File Name:                LicDataType
//
//        created by Charley at 2015/9/14 10:29:17
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceCyber.SDKs.Licenses
{
    /// <summary>
    /// License数据类型
    /// </summary>
    public enum LicDataType
    {
        /// <summary>
        /// 未知
        /// </summary>
        Unkown = 0,
        /// <summary>
        /// 数字
        /// </summary>
        Number = 1,
        /// <summary>
        /// 字符串
        /// </summary>
        String = 2
    }
}
