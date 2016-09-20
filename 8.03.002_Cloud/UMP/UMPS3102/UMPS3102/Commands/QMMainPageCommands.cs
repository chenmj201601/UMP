//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    43da6166-e594-4003-bfe1-9a214cd5cac0
//        CLR Version:              4.0.30319.18444
//        Name:                     QMMainPageCommands
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3102.Commands
//        File Name:                QMMainPageCommands
//
//        created by Charley at 2014/11/13 16:59:18
//        http://www.voicecyber.com 
//
//======================================================================

using System.Windows.Input;

namespace UMPS3102.Commands
{
    public class QMMainPageCommands
    {
        private static RoutedUICommand mDeletePlayItemCommand;
        private static RoutedUICommand mAddScoreCommand;
        private static RoutedUICommand mModifyScoreCommand;
        private static RoutedUICommand mGridViewColumnHeaderCommand;

        public static RoutedUICommand DeletePlayItemCommand
        {
            get { return mDeletePlayItemCommand; }
        }

        public static RoutedUICommand AddScoreCommand
        {
            get { return mAddScoreCommand; }
        }

        public static RoutedUICommand ModifyScoreCommand
        {
            get { return mModifyScoreCommand; }
        }

        public static RoutedUICommand GridViewColumnHeaderCommand
        {
            get { return mGridViewColumnHeaderCommand; }
        }

        static QMMainPageCommands()
        {
            mDeletePlayItemCommand = new RoutedUICommand();
            mAddScoreCommand = new RoutedUICommand();
            mModifyScoreCommand = new RoutedUICommand();
            mGridViewColumnHeaderCommand = new RoutedUICommand();
        }
    }
}
