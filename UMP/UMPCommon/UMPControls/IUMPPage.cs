//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    fe186a15-9ade-490f-b4d5-7556985d4869
//        CLR Version:              4.0.30319.18063
//        Name:                     IUMPPage
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Controls
//        File Name:                IUMPPage
//
//        created by Charley at 2014/8/20 21:12:38
//        http://www.voicecyber.com 
//
//======================================================================

using VoiceCyber.UMP.Common;

namespace VoiceCyber.UMP.Controls
{
    /// <summary>
    /// UMP页面，提供基本切换主题，切换语言的功能
    /// </summary>
    public interface IUMPPage : IThemePage, ILanguagePage
    {
        /// <summary>
        /// 页面名称
        /// </summary>
        string PageName { get; set; }
        /// <summary>
        ///  样式路径
        /// （如：UMPS11021/OUMMainPage.xaml）
        ///  实际对应资源路径为 http://192.168.5.31:8001/Themes/Default/UMPS11021/OUMMainPage.xaml
        /// </summary>
        string StylePath { get; set; }
        /// <summary>
        /// Web服务信息，切换主题或语言时可以从此服务获取资源及语言
        /// </summary>
        AppServerInfo AppServerInfo { get; set; }
    }
}
