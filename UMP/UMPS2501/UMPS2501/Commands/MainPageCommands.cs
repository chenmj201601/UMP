//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    31f51cc3-a856-44df-90b5-65ff27ae9b36
//        CLR Version:              4.0.30319.18063
//        Name:                     MainPageCommands
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS2501.Commands
//        File Name:                MainPageCommands
//
//        created by Charley at 2015/5/22 17:03:38
//        http://www.voicecyber.com 
//
//======================================================================

using System.Windows.Input;

namespace UMPS2501.Commands
{
    public class MainPageCommands
    {
        public static RoutedUICommand EnableCommand = new RoutedUICommand();
        public static RoutedUICommand DisableCommand = new RoutedUICommand();
        public static RoutedUICommand TreeItemCheckCommand = new RoutedUICommand();
        public static RoutedUICommand TreeItemDoubleClickCommand = new RoutedUICommand();
    }
}
