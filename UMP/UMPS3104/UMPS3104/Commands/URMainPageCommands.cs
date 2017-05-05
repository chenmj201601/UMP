using System;
using System.Windows.Input;

namespace UMPS3104.Commands
{
    public class URMainPageCommands
    {

        private static RoutedUICommand mQueryRecordCommand;
        public static RoutedUICommand QueryRecordCommand
        {
            get { return mQueryRecordCommand; }
        }

        private static RoutedUICommand mAgentAppealCommand;
        public static RoutedUICommand AgentAppealCommand
        {
            get { return mAgentAppealCommand; }
        }

        private static RoutedUICommand mViewScoreResultCommand;
        public static RoutedUICommand ViewScoreResultCommand
        {
            get { return mViewScoreResultCommand; }
        }


        private static RoutedUICommand mRecordPlayHistoryCommand;

        public static RoutedUICommand RecordPlayHistoryCommand
        {
            get { return mRecordPlayHistoryCommand; }
        }

        private static RoutedUICommand mLaosLinkCommand;
        public static RoutedUICommand LaosLinkCommand { get { return mLaosLinkCommand; } }

        static URMainPageCommands()
        {
            mQueryRecordCommand = new RoutedUICommand();
            mAgentAppealCommand = new RoutedUICommand();
            mViewScoreResultCommand = new RoutedUICommand();
            mRecordPlayHistoryCommand = new RoutedUICommand();
            mLaosLinkCommand = new RoutedUICommand();
        }
    }
}
