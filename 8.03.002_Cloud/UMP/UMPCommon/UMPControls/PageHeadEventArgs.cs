//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    f4270929-2f4d-4b53-9bc3-82bdea0338b9
//        CLR Version:              4.0.30319.18444
//        Name:                     PageHeadEventArgs
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Controls
//        File Name:                PageHeadEventArgs
//
//        created by Charley at 2014/9/18 9:42:02
//        http://www.voicecyber.com 
//
//======================================================================
using System;

namespace VoiceCyber.UMP.Controls
{
    /// <summary>
    /// PageHead事件参数
    /// </summary>
    public class PageHeadEventArgs : EventArgs
    {
        /// <summary>
        /// 事件代码
        /// 100     主题切换
        /// 110     语言切换
        /// 120     修改密码
        /// 121     关闭打开侧边栏
        /// 200     关闭系统
        /// 201     注销登陆
        /// 202     导航到Home
        /// 203     设置默认页
        /// 300     NetPipe消息
        /// 400     IM消息
        /// 401     打开IM面板
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 文本消息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 其他数据
        /// </summary>
        public object Data { get; set; }

        public const int EVT_THEME_CHANGE = 100;
        public const int EVT_LANG_CHANGE = 110;
        public const int EVT_CHANGE_PASSWORD = 120;
        public const int EVT_LEFT_PANEL = 121;
        public const int EVT_EXIT_SYSTEM = 200;
        public const int EVT_LOGOUT_SYSTEM = 201;
        public const int EVT_NAVIGATE_HOME = 202;
        public const int EVT_DEFAULT_PAGE = 203;
        public const int EVT_NETPIPE = 300;
        public const int EVT_MSG_IM = 400;
        public const int EVT_OPENIM = 401;
    }
}
