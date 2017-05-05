//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    bcc568cc-40d1-467c-9f24-c86935546b9a
//        CLR Version:              4.0.30319.42000
//        Name:                     IAppControlService
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common
//        File Name:                IAppControlService
//
//        created by Charley at 2016/2/3 10:27:53
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.Generic;

namespace VoiceCyber.UMP.Common
{
    /// <summary>
    /// 子应用控制器
    /// </summary>
    public interface IAppControlService
    {
        /// <summary>
        /// 子应用列表
        /// </summary>
        IList<IAppConfigInfo> ListAppConfigs { get; } 
    }
}
