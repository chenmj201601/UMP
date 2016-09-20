//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    6fea4914-c04a-4db6-ae7b-c9667315406e
//        CLR Version:              4.0.30319.18444
//        Name:                     ILayoutElementWithVisibility
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.AvalonDock.Layout
//        File Name:                ILayoutElementWithVisibility
//
//        created by Charley at 2014/7/22 9:40:24
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceCyber.Wpf.AvalonDock.Layout
{
    public interface ILayoutElementWithVisibility
    {
        void ComputeVisibility();
    }
}
