//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    05394687-b2dd-4e33-baaa-9446cdca256e
//        CLR Version:              4.0.30319.18063
//        Name:                     ITreeItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.CustomControls
//        File Name:                ITreeItem
//
//        created by Charley at 2014/4/5 16:01:33
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.Generic;
using System.ComponentModel;

namespace VoiceCyber.CustomControls
{
    /// <summary>
    /// TreeView元素接口
    /// </summary>
    public interface ITreeItem:INotifyPropertyChanged
    {
        /// <summary>
        /// 名称
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// 深度
        /// </summary>
        int Level { get; set; }
        /// <summary>
        /// 父元素
        /// </summary>
        ITreeItem Parent { get; set; }
        /// <summary>
        /// 子元素集合
        /// </summary>
        List<ITreeItem> Children { get;}
        /// <summary>
        /// 初始化，将子元素的父元素设为自身
        /// </summary>
        void Init();
        /// <summary>
        /// 添加一个子元素
        /// </summary>
        /// <param name="child">子元素</param>
        void AddChild(ITreeItem child);
        /// <summary>
        /// 移除一个子元素
        /// </summary>
        /// <param name="child">子元素</param>
        void RemoveChild(ITreeItem child);
    }
}
