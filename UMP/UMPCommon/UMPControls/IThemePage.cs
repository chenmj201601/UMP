//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    75c59811-170c-4965-ad64-b88fb20a0d1c
//        CLR Version:              4.0.30319.18063
//        Name:                     IThemePage
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Controls
//        File Name:                IThemePage
//
//        created by Charley at 2014/8/20 21:11:12
//        http://www.voicecyber.com 
//
//======================================================================

using VoiceCyber.UMP.Common;

namespace VoiceCyber.UMP.Controls
{
    /// <summary>
    /// 可以切换主题的页面或用户控件
    /// </summary>
    public interface IThemePage
    {
        /// <summary>
        /// 主题
        /// </summary>
        ThemeInfo ThemeInfo { get; set; }
        /// <summary>
        /// 切换主题
        /// </summary>
        void ChangeTheme();
    }
}
