//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    22b113d9-f24a-40a0-8d1f-56a7162ec809
//        CLR Version:              4.0.30319.18444
//        Name:                     DiagramTree
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.CustomControls.TreeViews.Implementation
//        File Name:                DiagramTree
//
//        created by Charley at 2014/9/15 14:18:34
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace VoiceCyber.Wpf.CustomControls
{
    public class DiagramTree : TreeView
    {
        static DiagramTree()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DiagramTree),
                new FrameworkPropertyMetadata(typeof(DiagramTree)));
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new DiagramTreeItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is DiagramTreeItem;
        }
    }
}
