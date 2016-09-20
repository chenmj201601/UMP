//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    ffeaa1df-b88d-4187-8d93-d655c5bc1dd9
//        CLR Version:              4.0.30319.18444
//        Name:                     ILayoutPane
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.AvalonDock.Layout
//        File Name:                ILayoutPane
//
//        created by Charley at 2014/7/22 9:41:36
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceCyber.Wpf.AvalonDock.Layout
{
    public interface ILayoutPane : ILayoutContainer, ILayoutElementWithVisibility
    {
        void MoveChild(int oldIndex, int newIndex);

        void RemoveChildAt(int childIndex);
    }
}
