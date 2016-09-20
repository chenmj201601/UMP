//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    98355d45-e737-4955-9516-4b2c45a350ef
//        CLR Version:              4.0.30319.18444
//        Name:                     IRibbonSizeChangedSink
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Fluent.Extensibility
//        File Name:                IRibbonSizeChangedSink
//
//        created by Charley at 2014/5/27 17:31:06
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.Ribbon.Extensibility
{
    public interface IRibbonSizeChangedSink
    {
        void OnSizePropertyChanged(RibbonControlSize previous, RibbonControlSize current);
    }
}
