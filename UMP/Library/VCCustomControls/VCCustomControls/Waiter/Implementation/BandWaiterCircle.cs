//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    090f9db3-017f-4f8a-a1e9-44ed087072fa
//        CLR Version:              4.0.30319.18444
//        Name:                     BandWaiterCircle
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.CustomControls.Waiter.Implementation
//        File Name:                BandWaiterCircle
//
//        created by Charley at 2014/8/25 9:42:21
//        http://www.voicecyber.com 
//
//======================================================================

using System.Windows;
using System.Windows.Controls;

namespace VoiceCyber.Wpf.CustomControls
{
    public class BandWaiterCircle:Control
    {
        static BandWaiterCircle()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (BandWaiterCircle),
                new FrameworkPropertyMetadata(typeof (BandWaiterCircle)));
        }
    }
}
