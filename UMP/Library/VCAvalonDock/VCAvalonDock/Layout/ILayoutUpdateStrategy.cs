//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    55278cd5-1c1e-42be-abfb-e59da4218b88
//        CLR Version:              4.0.30319.18444
//        Name:                     ILayoutUpdateStrategy
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.AvalonDock.Layout
//        File Name:                ILayoutUpdateStrategy
//
//        created by Charley at 2014/7/22 9:43:55
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceCyber.Wpf.AvalonDock.Layout
{
    public interface ILayoutUpdateStrategy
    {
        bool BeforeInsertAnchorable(
            LayoutRoot layout,
            LayoutAnchorable anchorableToShow,
            ILayoutContainer destinationContainer);

        void AfterInsertAnchorable(
            LayoutRoot layout,
            LayoutAnchorable anchorableShown);


        bool BeforeInsertDocument(
            LayoutRoot layout,
            LayoutDocument anchorableToShow,
            ILayoutContainer destinationContainer);

        void AfterInsertDocument(
            LayoutRoot layout,
            LayoutDocument anchorableShown);
    }
}
