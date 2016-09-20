//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    22142b60-0909-46ad-8ae4-ce30b3966807
//        CLR Version:              4.0.30319.18444
//        Name:                     RibbonCommands
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Ribbon
//        File Name:                RibbonCommands
//
//        created by Charley at 2014/5/27 22:14:26
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace VoiceCyber.Ribbon
{
    public static class RibbonCommands
    {
        private static readonly RoutedUICommand openBackstage = new RoutedUICommand("Open backstage", "OpenBackstage", typeof(RibbonCommands));

        /// <summary>
        /// Gets the value that represents the Open Backstage command
        /// </summary>
        public static RoutedCommand OpenBackstage
        {
            get { return openBackstage; }
        }
    }
}
