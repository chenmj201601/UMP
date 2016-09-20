//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    2ba11438-409d-4f2a-8433-8c6705be4eca
//        CLR Version:              4.0.30319.18444
//        Name:                     LocalHostInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common
//        File Name:                LocalHostInfo
//
//        created by Charley at 2014/9/25 16:30:17
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Runtime.Serialization;

namespace VoiceCyber.UMP.Common
{
    /// <summary>
    /// 机器信息
    /// </summary>
    [DataContract]
    public class LocalHostInfo
    {
        /// <summary>
        /// 机器名
        /// </summary>
        [DataMember]
        public string StrHostName { get; set; }

        /// <summary>
        /// 当前用户的AppData\Local
        /// </summary>
        [DataMember]
        public string StrLocalApplicationData { get; set; }

        /// <summary>
        /// 所有用户的ProgramData
        /// </summary>
        [DataMember]
        public string StrCommonApplicationData { get; set; }

        /// <summary>
        /// 最后一个登录UMP的账号
        /// </summary>
        [DataMember]
        public string StrLastLoginAccount { get; set; }

        public LocalHostInfo()
        {
            StrHostName = Environment.MachineName;
            StrLocalApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            StrCommonApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
        }
    }
}
