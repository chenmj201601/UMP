using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace UMPS3604.Commands
{
    public class ContentMenuCommand
    {
        private static RoutedUICommand mNewCommand;
        private static RoutedUICommand mDeleteCommand;
        private static RoutedUICommand mRenameCommand;
        private static RoutedUICommand mAllotCommand;
        private static RoutedUICommand mUploadCommand;
        private static RoutedUICommand mLaosLinkCommand;
        private static RoutedUICommand mBrowseHistoryCommand;

        public static RoutedUICommand NewCommand { get { return mNewCommand; } }
        public static RoutedUICommand DeleteCommand { get { return mDeleteCommand; } }
        public static RoutedUICommand RenameCommand { get { return mRenameCommand; } }
        public static RoutedUICommand AllotCommand { get { return mAllotCommand; } }
        public static RoutedUICommand UploadCommand { get { return mUploadCommand; } }
        public static RoutedUICommand LaosLinkCommand { get { return mLaosLinkCommand; } }
        public static RoutedUICommand BrowseHistoryCommand { get { return mBrowseHistoryCommand; } }

        static ContentMenuCommand()
        {
            mNewCommand=new RoutedUICommand();
            mDeleteCommand=new RoutedUICommand();
            mRenameCommand=new RoutedUICommand();
            mAllotCommand=new RoutedUICommand();
            mUploadCommand = new RoutedUICommand();
            mLaosLinkCommand = new RoutedUICommand();
            mBrowseHistoryCommand = new RoutedUICommand();
        }

    }
}
