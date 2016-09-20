//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    be61e013-25c3-4878-8bac-f2ff7a86ded4
//        CLR Version:              4.0.30319.18063
//        Name:                     GridTree
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.CustomControls.TreeViews.Implementation
//        File Name:                GridTree
//
//        created by Charley at 2014/8/21 14:53:48
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace VoiceCyber.Wpf.CustomControls
{
    /// <summary>
    /// 网格型树形控件
    /// </summary>
    public class GridTree : TreeView
    {
        private readonly ResourceDictionary mResouce = new ResourceDictionary();

        static GridTree()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GridTree), new FrameworkPropertyMetadata(typeof(GridTree)));
        }

        public GridTree()
        {
            mResouce.Source = new Uri("VCCustomControls;component/TreeViews/Themes/GridTreeNameColumn.xaml", UriKind.Relative);
            Columns = new GridViewColumnCollection();
            GridViewColumn nameColumn = new GridViewColumn();
            nameColumn.Header = "Name";
            DataTemplate dt = (DataTemplate)mResouce["CellTemplate_Name"];
            if (dt != null)
            {
                nameColumn.CellTemplate = dt;
            }
            else
            {
                nameColumn.DisplayMemberBinding = new Binding("Name");
            }
            Columns.Add(nameColumn);
        }

        /// <summary>
        /// 设定子项的包装类型
        /// </summary>
        /// <returns></returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new GridTreeItem();
        }
        /// <summary>
        /// 判断包装类型
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is GridTreeItem;
        }

        public static readonly DependencyProperty IsShowCheckBoxProperty =
            DependencyProperty.Register("IsShowCheckBox", typeof(bool), typeof(GridTree), new PropertyMetadata(false));
        /// <summary>
        /// 是否显示复选框
        /// </summary>
        public bool IsShowCheckBox
        {
            get { return (bool)GetValue(IsShowCheckBoxProperty); }
            set { SetValue(IsShowCheckBoxProperty, value); }
        }

        public static readonly DependencyProperty IsShowIconProperty =
            DependencyProperty.Register("IsShowIcon", typeof(bool), typeof(GridTree), new PropertyMetadata(default(bool)));

        public bool IsShowIcon
        {
            get { return (bool)GetValue(IsShowIconProperty); }
            set { SetValue(IsShowIconProperty, value); }
        }

        /// <summary>
        /// 初始化列信息
        /// </summary>
        /// <param name="nameHeader"></param>
        /// <param name="nameWidth"></param>
        /// <param name="listColumns"></param>
        public void SetColumns(GridViewColumnHeader nameHeader, int nameWidth, List<GridViewColumn> listColumns)
        {
            if (Columns.Count > 0)
            {
                GridViewColumn nameColumn = Columns[0];
                nameColumn.Header = nameHeader;
                nameColumn.Width = nameWidth;
            }
            for (int i = Columns.Count - 1; i > 0; i--)
            {
                Columns.Remove(Columns[i]);
            }
            for (int i = 0; i < listColumns.Count; i++)
            {
                Columns.Add(listColumns[i]);
            }
        }
        /// <summary>
        /// 初始化列信息
        /// </summary>
        /// <param name="nameColumnTemplate"></param>
        /// <param name="nameHeader"></param>
        /// <param name="nameWidth"></param>
        /// <param name="listColumns"></param>
        public void SetColumns(DataTemplate nameColumnTemplate, GridViewColumnHeader nameHeader, int nameWidth, List<GridViewColumn> listColumns)
        {
            if (Columns.Count > 0)
            {
                GridViewColumn nameColumn = Columns[0];
                nameColumn.CellTemplate = nameColumnTemplate;
            }
            SetColumns(nameHeader, nameWidth, listColumns);
        }

        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register("Columns", typeof(GridViewColumnCollection), typeof(GridTree), new PropertyMetadata(default(GridViewColumnCollection)));

        public GridViewColumnCollection Columns
        {
            get { return (GridViewColumnCollection)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }
    }
}
