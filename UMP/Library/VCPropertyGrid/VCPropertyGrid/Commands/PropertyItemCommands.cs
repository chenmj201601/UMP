//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    6fe56ef3-a03a-4e3c-99aa-91abacf6cc1a
//        CLR Version:              4.0.30319.18444
//        Name:                     PropertyItemCommands
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.PropertyGrids.Commands
//        File Name:                PropertyItemCommands
//
//        created by Charley at 2014/7/23 11:54:59
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace VoiceCyber.Wpf.PropertyGrids.Commands
{
    public static class PropertyItemCommands
    {
        private static RoutedCommand _resetValueCommand = new RoutedCommand();
        public static RoutedCommand ResetValue
        {
            get
            {
                return _resetValueCommand;
            }
        }
    }
}
