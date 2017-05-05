//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    acf4bfa5-7a36-42e0-93f6-6825894b8412
//        CLR Version:              4.0.30319.18063
//        Name:                     LangItemState
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPLanguageManager.Models
//        File Name:                LangItemState
//
//        created by Charley at 2015/6/5 13:21:20
//        http://www.voicecyber.com 
//
//======================================================================
using System;

namespace UMPLanguageManager.Models
{
    /// <summary>
    /// 语言项的状态，按位
    /// </summary>
    [Flags]
    public enum LangItemState
    {
        /// <summary>
        /// 无状态
        /// </summary>
        None = 0,
        /// <summary>
        /// 值改变了
        /// </summary>
        ValueChanged = 1,
        /// <summary>
        /// 在查找列表中
        /// </summary>
        Searched = 2,
        /// <summary>
        /// 焦点
        /// </summary>
        Current = 4,
        /// <summary>
        /// 查找的焦点
        /// </summary>
        SearchCurrent = Searched | Current
    }
}
