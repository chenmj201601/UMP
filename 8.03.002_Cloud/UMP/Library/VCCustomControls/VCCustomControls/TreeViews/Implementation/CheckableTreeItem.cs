//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    9173c201-adf9-4458-b36f-5e92db21ad63
//        CLR Version:              4.0.30319.18444
//        Name:                     CheckableTreeItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.CustomControls.TreeViews.Implementation
//        File Name:                CheckableTreeItem
//
//        created by Charley at 2014/8/22 11:57:21
//        http://www.voicecyber.com 
//
//======================================================================

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VoiceCyber.Wpf.CustomControls
{
    public class CheckableTreeItem:TreeViewItem
    {
        static CheckableTreeItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (CheckableTreeItem),
                new FrameworkPropertyMetadata(typeof (CheckableTreeItem)));
        }

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

        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof (bool?), typeof (CheckableTreeItem), new PropertyMetadata(default(bool?)));
        /// <summary>
        /// 选中状态
        /// </summary>
        public bool? IsChecked
        {
            get { return (bool?) GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof (ImageSource), typeof (CheckableTreeItem), new PropertyMetadata(default(ImageSource)));
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
