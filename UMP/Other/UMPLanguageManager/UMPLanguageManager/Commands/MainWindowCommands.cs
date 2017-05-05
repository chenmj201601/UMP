//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    52297b1f-3f2f-46e8-8880-ed8c9a71c7b7
//        CLR Version:              4.0.30319.18063
//        Name:                     MainWindowCommands
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPLanguageManager.Commands
//        File Name:                MainWindowCommands
//
//        created by Charley at 2015/4/25 13:49:32
//        http://www.voicecyber.com 
//
//======================================================================

using System.Windows.Input;

namespace UMPLanguageManager.Commands
{
    public class MainWindowCommands
    {
        public static RoutedUICommand SettingCommand = new RoutedUICommand();
        public static RoutedUICommand ExportCommand = new RoutedUICommand();
        public static RoutedUICommand SynchronCommand = new RoutedUICommand();
        public static RoutedUICommand SearchCommand = new RoutedUICommand();
        public static RoutedUICommand SearchPreCommand = new RoutedUICommand();
        public static RoutedUICommand SearchNextCommand = new RoutedUICommand();
        public static RoutedUICommand SearchClearCommand = new RoutedUICommand();
        public static RoutedUICommand SaveLayoutCommand = new RoutedUICommand();
        public static RoutedUICommand ResetLayoutCommand = new RoutedUICommand();
        public static RoutedUICommand CopyLangCommand = new RoutedUICommand();
        public static RoutedUICommand AddLangCommand = new RoutedUICommand();
        public static RoutedUICommand SetViewCommand = new RoutedUICommand();
    }
}
