using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UMPS0001
{
    public partial class UCObjectOperationCreateDB : UserControl, S0001Interface
    {
        private string IStrOperationID = string.Empty;
        OperationParameters IOperationParameters = new OperationParameters();

        public event EventHandler<OperationEventArgs> IOperationEvent;

        public UCObjectOperationCreateDB(string AStrOperationID)
        {
            InitializeComponent();
            IStrOperationID = AStrOperationID;
            GridCreateDatabase.PreviewMouseLeftButtonDown += GridCreateDatabase_PreviewMouseLeftButtonDown;
        }

        public void ShowOperationDetails(OperationParameters AParameters)
        {
            IOperationParameters = AParameters;
            TextBlockOperationName.Text = App.GetDisplayCharater("FO" + IStrOperationID);
        }

        private void GridCreateDatabase_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (IOperationEvent == null) { return; }
            OperationEventArgs LEventArgs = new OperationEventArgs();
            LEventArgs.StrObjectTag = IStrOperationID;
            LEventArgs.ObjectSource0 = IOperationParameters;
            IOperationEvent(this, LEventArgs);
        }

        
    }
}
