//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    2bc02313-8ac6-4fda-8666-b686d4941fc5
//        CLR Version:              4.0.30319.18444
//        Name:                     OUMMainPageCommands
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1101.Commands
//        File Name:                OUMMainPageCommands
//
//        created by Charley at 2014/8/27 13:36:55
//        http://www.voicecyber.com 
//
//======================================================================

using System.Windows.Input;

namespace UMPS1101.Commands
{
    public class OUMMainPageCommands
    {
        private static RoutedUICommand mAddOrgCommand;
        private static RoutedUICommand mDeleteOrgCommand;
        private static RoutedUICommand mModifyOrgCommand;
        private static RoutedUICommand mAddUserCommand;
        private static RoutedUICommand mDeleteUserCommand;
        private static RoutedUICommand mModifyUserCommand;
        private static RoutedUICommand mSetUserRoleCommand;
        private static RoutedUICommand mSetUserManagementCommand;
        private static RoutedUICommand mSetUserResourceManagementCommand;
        private static RoutedUICommand mImportUserDataCommand;
        private static RoutedUICommand mImportAgentDataCommand;
        private static RoutedUICommand mABCDConfigCommand;
        private static RoutedUICommand mLDAPCommand;

        public static RoutedUICommand AddOrgCommand
        {
            get { return mAddOrgCommand; }
        }

        public static RoutedUICommand DeleteOrgCommand
        {
            get { return mDeleteOrgCommand; }
        }

        public static RoutedUICommand ModifyOrgCommand
        {
            get { return mModifyOrgCommand; }
        }

        public static RoutedUICommand AddUserCommand
        {
            get { return mAddUserCommand; }
        }

        public static RoutedUICommand DeleteUserCommand
        {
            get { return mDeleteUserCommand; }
        }

        public static RoutedUICommand ModifyUserCommand
        {
            get { return mModifyUserCommand; }
        }

        public static RoutedUICommand SetUserRoleCommand
        {
            get { return mSetUserRoleCommand; }
        }

        public static RoutedUICommand SetUserManagementCommand
        {
            get { return mSetUserManagementCommand; }
        }

        public static RoutedUICommand SetUserResourceManagementCommand
        {
            get { return mSetUserResourceManagementCommand; }
        }

        public static RoutedUICommand ImportUserDataCommand
        {
            get { return mImportUserDataCommand; }
        }

        public static RoutedUICommand ImportAgentDataCommand
        {
            get { return mImportAgentDataCommand; }
        }

        public static RoutedUICommand ABCDConfigCommand
        {
            get { return mABCDConfigCommand; }
        }

        public static RoutedUICommand LDAPCommand
        {
            get { return mLDAPCommand; }
        }
        static OUMMainPageCommands()
        {
            mAddOrgCommand = new RoutedUICommand();
            mDeleteOrgCommand = new RoutedUICommand();
            mModifyOrgCommand = new RoutedUICommand();
            mAddUserCommand = new RoutedUICommand();
            mDeleteUserCommand = new RoutedUICommand();
            mModifyUserCommand = new RoutedUICommand();
            mSetUserRoleCommand = new RoutedUICommand();
            mSetUserManagementCommand = new RoutedUICommand();
            mSetUserResourceManagementCommand = new RoutedUICommand();
            mImportUserDataCommand = new RoutedUICommand();
            mImportAgentDataCommand = new RoutedUICommand();
            mABCDConfigCommand = new RoutedUICommand();
            mLDAPCommand = new RoutedUICommand();
        }

        public static RoutedUICommand mABCDCommand { get; set; }
    }
}
