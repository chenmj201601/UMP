using System;

using System.Windows.Input;

namespace UMPS3103.Commands
{
    public class URMainPageCommands
    {

        private static RoutedUICommand mTaskAssignQueryCommand;
        public static RoutedUICommand TaskAssignQueryCommand
        {
            get { return mTaskAssignQueryCommand; }
        }

        private static RoutedUICommand mSaveToTaskCommand;
        public static RoutedUICommand SaveToTaskCommand
        {
            get { return mSaveToTaskCommand; }
        }



        private static RoutedUICommand mAddToTaskCommand;
        public static RoutedUICommand AddToTaskCommand
        {
            get { return mAddToTaskCommand; }
        }


        private static RoutedUICommand mModifyTaskFinishTimeCommand;
        public static RoutedUICommand ModifyTaskFinishTimeCommand
        {
            get { return mModifyTaskFinishTimeCommand; }
        }


        static URMainPageCommands()
        {
            mTaskAssignQueryCommand = new RoutedUICommand();
            mSaveToTaskCommand = new RoutedUICommand();
            mAddToTaskCommand = new RoutedUICommand();
            mModifyTaskFinishTimeCommand = new RoutedUICommand();;
        }
    }
}
