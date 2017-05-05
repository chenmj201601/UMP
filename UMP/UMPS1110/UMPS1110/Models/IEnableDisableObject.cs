//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    624e3a85-ef39-45c2-a11f-42ba86fad3ec
//        CLR Version:              4.0.30319.18444
//        Name:                     IEnableDisableObject
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1110.Models
//        File Name:                IEnableDisableObject
//
//        created by Charley at 2015/4/13 15:35:32
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;

namespace UMPS1110.Models
{
    public interface IEnableDisableObject
    {
        bool IsEnabled { get; set; }
    }
}
