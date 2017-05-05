//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    f224aac2-9474-464a-831a-919cbe98e423
//        CLR Version:              4.0.30319.18408
//        Name:                     ILeftView
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPUpdater
//        File Name:                ILeftView
//
//        created by Charley at 2016/8/8 16:13:20
//        http://www.voicecyber.com 
//
//======================================================================

using VoiceCyber.UMP.Updates;


namespace UMPUpdater
{
    public interface ILeftView : IChildView
    {
        UpdateWindow PageParent { get; set; }
        InstallState InstallState { get; set; }
        UpdateInfo UpdateInfo { get; set; }
    }
}
