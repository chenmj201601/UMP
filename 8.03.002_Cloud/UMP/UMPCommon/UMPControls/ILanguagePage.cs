//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    5f67f668-1a90-4650-894e-9700d60ffd89
//        CLR Version:              4.0.30319.18063
//        Name:                     ILanguagePage
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Controls
//        File Name:                ILanguagePage
//
//        created by Charley at 2014/8/20 21:11:55
//        http://www.voicecyber.com 
//
//======================================================================

using VoiceCyber.UMP.Common;

namespace VoiceCyber.UMP.Controls
{
    /// <summary>
    /// 切换语言的页面
    /// </summary>
    public interface ILanguagePage
    {
        //int LangID { get; set; }
        LangTypeInfo LangTypeInfo { get; set; }
        /// <summary>
        /// 切换语言
        /// </summary>
        void ChangeLanguage();
    }
}
