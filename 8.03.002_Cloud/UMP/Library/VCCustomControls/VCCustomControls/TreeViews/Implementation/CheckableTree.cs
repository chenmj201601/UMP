//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    b4974242-ccaa-4530-9fc4-9fe0e66e767b
//        CLR Version:              4.0.30319.18444
//        Name:                     CheckableTree
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.CustomControls.TreeViews.Implementation
//        File Name:                CheckableTree
//
//        created by Charley at 2014/8/22 12:01:35
//        http://www.voicecyber.com 
//
//======================================================================

using System.Windows;
using System.Windows.Controls;

namespace VoiceCyber.Wpf.CustomControls
{
    public class CheckableTree : TreeView
    {
        //static CheckableTree()
        //{
        //    DefaultStyleKeyProperty.OverrideMetadata(typeof (CheckableTree),
        //        new FrameworkPropertyMetadata(typeof (CheckableTree)));
        //}

        /// <summary>
        /// 设定子项的包装类型
        /// </summary>
        /// <returns></returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new CheckableTreeItem();
        }
        /// <summary>
        /// 判断包装类型
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is CheckableTreeItem;
        }

        public static readonly DependencyProperty IsShowIconProperty =
            DependencyProperty.Register("IsShowIcon", typeof (bool), typeof (CheckableTree), new PropertyMetadata(default(bool)));
        /// <summary>
        /// 是否显示图标
        /// </summary>
        public bool IsShowIcon
        {
            get { return (bool) GetValue(IsShowIconProperty); }
            set { SetValue(IsShowIconProperty, value); }
        }
    }
}
