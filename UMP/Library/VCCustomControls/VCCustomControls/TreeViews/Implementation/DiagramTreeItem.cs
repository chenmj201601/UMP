//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    a8268995-0db6-41fd-aeee-5f215e751d18
//        CLR Version:              4.0.30319.18444
//        Name:                     DiagramTreeItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.CustomControls.TreeViews.Implementation
//        File Name:                DiagramTreeItem
//
//        created by Charley at 2014/9/15 14:19:17
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VoiceCyber.Wpf.CustomControls
{
    public class DiagramTreeItem : TreeViewItem
    {
        static DiagramTreeItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DiagramTreeItem),
                new FrameworkPropertyMetadata(typeof(DiagramTreeItem)));
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new DiagramTreeItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is DiagramTreeItem;
        }

        public static readonly DependencyProperty NameProperty =
            DependencyProperty.Register("Name", typeof(string), typeof(DiagramTreeItem), new PropertyMetadata(default(string)));

        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(DiagramTreeItem), new PropertyMetadata(default(string)));

        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(ImageSource), typeof(DiagramTreeItem), new PropertyMetadata(default(ImageSource)));

        public ImageSource Icon
        {
            get { return (ImageSource)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }
    }
}
