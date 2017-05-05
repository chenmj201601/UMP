
using System.Windows.Input;

namespace UMPS1102.Commands
{
    public class URMainPageCommands
    {
        private static RoutedUICommand mAddRoleCommand;
        private static RoutedUICommand mDeleteRoleCommand;
        private static RoutedUICommand mModifyRoleCommand;
        private static RoutedUICommand mSubmitRolePermissionsCommand;
        private static RoutedUICommand mSubmitRoleUsersUsersCommand;

        public static RoutedUICommand AddRoleCommand
        {
            get { return mAddRoleCommand; }
        }

        public static RoutedUICommand DeleteRoleCommand
        {
            get { return mDeleteRoleCommand; }
        }

        public static RoutedUICommand ModifyRoleCommand
        {
            get { return mModifyRoleCommand; }
        }

        public static RoutedUICommand SubmitRolePermissionsCommand
        {
            get { return mSubmitRolePermissionsCommand; }
        }

        public static RoutedUICommand SubmitRoleUsersCommand
        {
            get { return mSubmitRoleUsersUsersCommand; }
        }

        static URMainPageCommands()
        {
            mAddRoleCommand = new RoutedUICommand();
            mDeleteRoleCommand = new RoutedUICommand();
            mModifyRoleCommand = new RoutedUICommand();
            mSubmitRolePermissionsCommand = new RoutedUICommand();
            mSubmitRoleUsersUsersCommand = new RoutedUICommand();
        }
    }
}
