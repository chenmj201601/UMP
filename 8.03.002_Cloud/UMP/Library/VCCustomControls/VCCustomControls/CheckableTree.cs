//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    12b052bc-2b48-4416-bc8c-f9d8ba84274d
//        CLR Version:              4.0.30319.18063
//        Name:                     CheckableTree
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.CustomControls
//        File Name:                CheckableTree
//
//        created by Charley at 2014/4/5 16:43:25
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Windows;
using System.Windows.Controls;

namespace VoiceCyber.CustomControls
{
    /// <summary>
    /// 一种带复选框的TreeView
    /// </summary>
    public class CheckableTree : TreeView
    {
        private readonly ResourceDictionary mResouce = new ResourceDictionary();
        private bool mIsShowIcon;
        /// <summary>
        /// 设置是否显示图标
        /// </summary>
        public bool IsShowIcon
        {
            get { return mIsShowIcon; }
            set { mIsShowIcon = value; }
        }
        /// <summary>
        /// 构造一个带复选框的TreeView控件
        /// </summary>
        public CheckableTree()
        {
            mResouce.Source = new Uri("VCCustomControls;component/CheckableTree.xaml", UriKind.Relative);
            Resources.MergedDictionaries.Add(mResouce);
            HierarchicalDataTemplate itemDataTemplate = (HierarchicalDataTemplate)mResouce["CheckBoxItemTemplate"];
            Style itemStyle = (Style)mResouce["TreeViewItemStyle"];
            ItemTemplate = itemDataTemplate;
            ItemContainerStyle = itemStyle;
            mIsShowIcon = false;
        }
    }
}
