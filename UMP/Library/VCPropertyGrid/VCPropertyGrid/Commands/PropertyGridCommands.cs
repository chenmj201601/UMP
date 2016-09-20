//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    7af4c7d0-887c-41c3-867b-6d07ab128bf4
//        CLR Version:              4.0.30319.18444
//        Name:                     PropertyGridCommands
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.PropertyGrids.Commands
//        File Name:                PropertyGridCommands
//
//        created by Charley at 2014/7/23 11:54:40
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace VoiceCyber.Wpf.PropertyGrids.Commands
{
    public class PropertyGridCommands
    {
        private static RoutedCommand _clearFilterCommand = new RoutedCommand();
        public static RoutedCommand ClearFilter
        {
            get
            {
                return _clearFilterCommand;
            }
        }
    }
}
