//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    308fdcee-1843-4bf4-8919-c081c2102c8e
//        CLR Version:              4.0.30319.18444
//        Name:                     SSDPageCommands
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3101.Commands
//        File Name:                SSDPageCommands
//
//        created by Charley at 2014/10/14 17:25:43
//        http://www.voicecyber.com 
//
//======================================================================

using System.Windows.Input;

namespace UMPS3101.Commands
{
    public class SSDPageCommands
    {
        private static RoutedUICommand mNavigateCommand;
        private static RoutedUICommand mShowAboutCommand;
        private static RoutedUICommand mSetViewCommand;
        private static RoutedUICommand mCaculateCommand;
        private static RoutedUICommand mSetDragDropCommand;
        private static RoutedUICommand mNewObjectCommand;
        private static RoutedUICommand mSaveLayoutCommand;
        private static RoutedUICommand mResetLayoutCommand;
        private static RoutedUICommand mChildListCommand;

        public static RoutedUICommand NavigateCommand
        {
            get { return mNavigateCommand; }
        }

        public static RoutedUICommand ShowAboutCommand
        {
            get { return mShowAboutCommand; }
        }

        public static RoutedUICommand SetViewCommand
        {
            get { return mSetViewCommand; }
        }

        public static RoutedUICommand CaculateCommand
        {
            get { return mCaculateCommand; }
        }

        public static RoutedUICommand SetDragDropCommand
        {
            get { return mSetDragDropCommand; }
        }

        public static RoutedUICommand NewObjectCommand
        {
            get { return mNewObjectCommand; }
        }

        public static RoutedUICommand SaveLayoutCommand
        {
            get { return mSaveLayoutCommand; }
        }

        public static RoutedUICommand ResetLayoutCommand
        {
            get { return mResetLayoutCommand; }
        }

        public static RoutedUICommand ChildListCommand
        {
            get { return mChildListCommand; }
        }

        static SSDPageCommands()
        {
            mNavigateCommand = new RoutedUICommand();
            mShowAboutCommand=new RoutedUICommand();
            mSetViewCommand = new RoutedUICommand();
            mCaculateCommand = new RoutedUICommand();
            mSetDragDropCommand = new RoutedUICommand();
            mNewObjectCommand = new RoutedUICommand();
            mSaveLayoutCommand = new RoutedUICommand();
            mResetLayoutCommand = new RoutedUICommand();
            mChildListCommand = new RoutedUICommand();
        }

    }
}
