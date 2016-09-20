using System.Windows.Input;
namespace UMPS3105.Commands
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
            mShowAboutCommand = new RoutedUICommand();
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
