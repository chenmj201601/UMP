using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace UMPS4601.Commands
{
    public class TableListViewCommand
    {
        private static RoutedUICommand mButtonCommand;
        public static RoutedUICommand ButtonCommand { get { return mButtonCommand; } }

        static TableListViewCommand()
        {
            mButtonCommand = new RoutedUICommand();
        }
    }
}
