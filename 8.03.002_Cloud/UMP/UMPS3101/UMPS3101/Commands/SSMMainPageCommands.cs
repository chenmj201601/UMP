//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    74ae0b23-8fd6-45b7-a94b-86d57a7e021f
//        CLR Version:              4.0.30319.18444
//        Name:                     SSMMainPageCommands
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3101.Commands
//        File Name:                SSMMainPageCommands
//
//        created by Charley at 2014/10/14 10:33:57
//        http://www.voicecyber.com 
//
//======================================================================

using System.Windows.Input;

namespace UMPS3101.Commands
{
    public class SSMMainPageCommands
    {
        private static RoutedUICommand mCreateScoreSheetCommand;
        private static RoutedUICommand mModifyScoreSheetCommand;
        private static RoutedUICommand mDeleteScoreSheetCommand;
        private static RoutedUICommand mSetManageUserCommand;
        private static RoutedUICommand mImportScoreSheetCommand;
        private static RoutedUICommand mExportScoreSheetCommand;

        public static RoutedUICommand CreateScoreSheetCommand
        {
            get { return mCreateScoreSheetCommand; }
        }

        public static RoutedUICommand ModifyScoreSheetCommand
        {
            get { return mModifyScoreSheetCommand; }
        }

        public static RoutedUICommand DeleteScoreSheetCommand
        {
            get { return mDeleteScoreSheetCommand; }
        }

        public static RoutedUICommand SetManageUserCommand
        {
            get { return mSetManageUserCommand; }
        }

        public static RoutedUICommand ImportScoreSheetCommand
        {
            get { return mImportScoreSheetCommand; }
        }

        public static RoutedUICommand ExportScoreSheetCommand
        {
            get { return mExportScoreSheetCommand; }
        }

        static SSMMainPageCommands()
        {
            mCreateScoreSheetCommand = new RoutedUICommand();
            mModifyScoreSheetCommand = new RoutedUICommand();
            mDeleteScoreSheetCommand = new RoutedUICommand();
            mSetManageUserCommand = new RoutedUICommand();
            mImportScoreSheetCommand = new RoutedUICommand();
            mExportScoreSheetCommand = new RoutedUICommand();
        }
    }
}
