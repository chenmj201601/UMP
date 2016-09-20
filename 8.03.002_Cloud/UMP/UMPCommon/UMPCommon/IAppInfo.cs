//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    52d14e65-ffe6-4323-995b-5b84fa492c6d
//        CLR Version:              4.0.30319.42000
//        Name:                     IAppInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common
//        File Name:                IAppInfo
//
//        created by Charley at 2016/2/3 10:13:35
//        http://www.voicecyber.com 
//
//======================================================================
using System;

namespace VoiceCyber.UMP.Common
{
    /// <summary>
    /// 表示Application信息
    /// Application是一个应用终端，可以是Wpf应用程序，Wcf服务，Windows服务等
    /// </summary>
    public interface IAppInfo
    {
        /// <summary>
        /// 每个Application应该在创建的时候自动生成一个唯一标识，通常这是Guid串
        /// </summary>
        string SessionID { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        int AppType { get; set; }
        /// <summary>
        /// 表示应用的可识别名称
        /// </summary>
        string AppName { get; set; }
        /// <summary>
        /// 表示应用的标题
        /// </summary>
        string AppTitle { get; set; }
        /// <summary>
        /// 所属模块号，4位小模块号
        /// 这个模块号通常与小模块号对应，但有时候一个Application可能有多个模块，这时通常使用两位的大模块号+00表示
        /// 这个模块号会影响操作日志，文本日志，语言包等功能
        /// </summary>
        int ModuleID { get; set; }
        /// <summary>
        /// 最后活动时间
        /// </summary>
        DateTime LastActiveTime { get; set; }
    }
}
