//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    db005bb2-79c8-4582-92ca-da03d37a4175
//        CLR Version:              4.0.30319.18063
//        Name:                     GridTree
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.CustomControls
//        File Name:                GridTree
//
//        created by Charley at 2014/4/1 17:20:42
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace VoiceCyber.CustomControls
{
    /// <summary>
    /// 一种网格化的TreeView控件
    /// </summary>
    public class GridTree : TreeView
    {
        private readonly ResourceDictionary mResouce = new ResourceDictionary();

        #region Properties
        private bool mIsShowCheckBox;
        /// <summary>
        /// 是否显示复选框
        /// </summary>
        public bool IsShowCheckBox
        {
            get { return mIsShowCheckBox; }
            set { mIsShowCheckBox = value; }
        }
        private bool mIsShowIcon;
        /// <summary>
        /// 设置是否显示图标
        /// </summary>
        public bool IsShowIcon
        {
            get { return mIsShowIcon; }
            set { mIsShowIcon = value; }
        }
        #endregion
        /// <summary>
        /// 构造一个网格化的TreeView控件
        /// </summary>
        public GridTree()
        {
            mResouce.Source = new Uri("VCCustomControls;component/GridTree.xaml", UriKind.Relative);
            Resources.MergedDictionaries.Add(mResouce);
            HierarchicalDataTemplate itemDataTemplate = (HierarchicalDataTemplate)mResouce["TreeViewDataTemplate"];
            Style treeViewStyle = (Style)mResouce["GridTreeStyle"];
            Style treeViewItemStyle = (Style)mResouce["TreeViewItemStyle"];
            ItemTemplate = itemDataTemplate;
            Style = treeViewStyle;
            ItemContainerStyle = treeViewItemStyle;
            mIsShowCheckBox = false;
        }
        /// <summary>
        /// 配置GridView的列
        /// </summary>
        /// <param name="nameHeader">Name列模版</param>
        /// <param name="nameWidth">Name列宽度</param>
        /// <param name="listColumns">列集合</param>
        public void SetColumns(GridViewColumnHeader nameHeader, int nameWidth, List<GridViewColumn> listColumns)
        {
            GridViewColumnCollection gvcc = (GridViewColumnCollection)mResouce["Gvcc"];
            GridViewColumn gvcName = gvcc[0];
            gvcName.Header = nameHeader;
            gvcName.Width = nameWidth;
            for (int i = gvcc.Count - 1; i > 0; i--)
            {
                gvcc.Remove(gvcc[i]);
            }
            for (int i = 0; i < listColumns.Count; i++)
            {
                gvcc.Add(listColumns[i]);
            }
        }
    }
  
}
