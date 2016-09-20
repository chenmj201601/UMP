//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    4be05d01-716f-45d7-b0c3-0c570b548ca1
//        CLR Version:              4.0.30319.18063
//        Name:                     GridTreeItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.CustomControls.TreeViews.Implementation
//        File Name:                GridTreeItem
//
//        created by Charley at 2014/8/21 14:47:11
//        http://www.voicecyber.com 
//
//======================================================================

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VoiceCyber.Wpf.CustomControls
{
    public class GridTreeItem : TreeViewItem
    {
        static GridTreeItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GridTreeItem),
                new FrameworkPropertyMetadata(typeof(GridTreeItem)));
        }

        private int mLevel = -1;
        /// <summary>
        /// 子项的深度
        /// </summary>
        public int Level
        {
            get
            {
                if (mLevel == -1)
                {
                    GridTreeItem parent =
                        ItemsControlFromItemContainer(this)
                            as GridTreeItem;
                    mLevel = (parent != null) ? parent.Level + 1 : 0;
                }
                return mLevel;
            }
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

        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof (bool?), typeof (GridTreeItem), new PropertyMetadata(default(bool?)));
        /// <summary>
        /// 选中状态
        /// </summary>
        public bool? IsChecked
        {
            get { return (bool?) GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof (ImageSource), typeof (GridTreeItem), new PropertyMetadata(default(ImageSource)));
        /// <summary>
        /// 图标
        /// </summary>
        public ImageSource Icon
        {
            get { return (ImageSource) GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }
    }
}
