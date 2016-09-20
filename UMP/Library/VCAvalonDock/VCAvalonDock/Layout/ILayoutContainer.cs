//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    d7e717c8-5041-4f61-ad73-22b9b0b6638c
//        CLR Version:              4.0.30319.18444
//        Name:                     ILayoutContainer
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.AvalonDock.Layout
//        File Name:                ILayoutContainer
//
//        created by Charley at 2014/7/22 9:38:46
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceCyber.Wpf.AvalonDock.Layout
{
    public interface ILayoutContainer : ILayoutElement
    {
        IEnumerable<ILayoutElement> Children { get; }
        void RemoveChild(ILayoutElement element);
        void ReplaceChild(ILayoutElement oldElement, ILayoutElement newElement);
        int ChildrenCount { get; }
    }
}
