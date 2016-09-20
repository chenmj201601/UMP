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

namespace UMPS1110
{
    public partial class UCValidationData : UserControl, S1110Interface
    {
        private string IStrOperationID = string.Empty;

        public event EventHandler<OperationEventArgs> IOperationEvent;

        public UCValidationData(string AStrOperationID)
        {
            InitializeComponent();
            IStrOperationID = AStrOperationID;
            GridValidationDataPanel.PreviewMouseLeftButtonDown += GridValidationDataPanel_PreviewMouseLeftButtonDown;
        }

        public void ShowOperationDetails(OperationParameters AParameters)
        {
            TextBlockOperationName.Text = App.GetDisplayCharater("FO" + IStrOperationID);
        }

        private void GridValidationDataPanel_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (IOperationEvent == null) { return; }
            OperationEventArgs LEventArgs = new OperationEventArgs();
            LEventArgs.StrObjectTag = IStrOperationID;
            IOperationEvent(this, LEventArgs);
        }
    }
}
