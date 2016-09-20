//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    91e38d99-23f6-45f5-b2c7-77e10bdc45d9
//        CLR Version:              4.0.30319.18444
//        Name:                     MainWindowCommands
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPScoreSheetDesigner.Commands
//        File Name:                MainWindowCommands
//
//        created by Charley at 2014/7/25 9:56:43
//        http://www.voicecyber.com 
//
//======================================================================

using System.Windows.Input;

namespace UMPScoreSheetDesigner.Commands
{
    public class MainWindowCommands
    {
        private static RoutedUICommand mShowAboutCommand;
        private static RoutedUICommand mSetViewCommand;
        private static RoutedUICommand mCaculateCommand;
        private static RoutedUICommand mSetDragDropCommand;
        private static RoutedUICommand mNewObjectCommand;
        private static RoutedUICommand mSaveLayoutCommand;
        private static RoutedUICommand mResetLayoutCommand;
        private static RoutedUICommand mChildListCommand;

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

        static MainWindowCommands()
        {
            InputGestureCollection inputs = new InputGestureCollection();
            inputs.Add(new KeyGesture(Key.A, ModifierKeys.Control, "Ctrl+A"));
            mShowAboutCommand = new RoutedUICommand("Show About", "ShowAbout", typeof(MainWindowCommands), inputs);
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
