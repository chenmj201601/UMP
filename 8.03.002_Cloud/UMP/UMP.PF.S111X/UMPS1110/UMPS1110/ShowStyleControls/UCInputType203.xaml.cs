using PFShareControls;
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

namespace UMPS1110.ShowStyleControls
{
    public partial class UCInputType203 : UserControl, PFShareInterface
    {
        public event EventHandler<PFShareControls.OperationEventArgs> IOperationEvent;

        public UCInputType203()
        {
            InitializeComponent();
        }

        public void SetElementData(string AStrDataContext)
        {
            string LStrData = string.Empty;

            LStrData = AStrDataContext;
            if (string.IsNullOrEmpty(LStrData)) { LStrData = "0.0.0.0"; }
            TextBoxIPAddress.SetIP(LStrData);
            TextBoxIPAddress.IOperationEvent += TextBoxIPAddress_IOperationEvent;
        }

        private void TextBoxIPAddress_IOperationEvent(object sender, PFShareControls.OperationEventArgs e)
        {
            if (IOperationEvent != null)
            {
                IOperationEvent(sender, e);
            }
        }

        public string GetElementData()
        {
            return TextBoxIPAddress.GetIP();
        }

        
    }
}
