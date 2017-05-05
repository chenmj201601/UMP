using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public partial class UCInputType201 : UserControl, S1110Interface
    {
        public event EventHandler<OperationEventArgs> IOperationEvent;
        public string IStrThisName = string.Empty;
        public string StrData1 = string.Empty;
        public string StrData0 = string.Empty;

        public UCInputType201()
        {
            InitializeComponent();
            RadioButton1.Checked += RadioButton_Checked;
            RadioButton0.Checked += RadioButton_Checked;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                OperationEventArgs LEventArgs = new OperationEventArgs();
                LEventArgs.StrObjectTag = IStrThisName;
                
                if (RadioButton1.IsChecked == true)
                {
                    LEventArgs.ObjectSource0 = "1";
                }
                else
                {
                    LEventArgs.ObjectSource0 = "0";
                }
                if (IOperationEvent != null)
                {
                    IOperationEvent(this, LEventArgs);
                }

            }
            catch { }
        }

        public void SetElementData(string AStrDataContext, string AStrThisName)
        {
            foreach (RadioButton LRadioButtonCurrent in StackPanelType201.Children)
            {
                if (LRadioButtonCurrent.DataContext.ToString() == AStrDataContext)
                {
                    LRadioButtonCurrent.IsChecked = true;
                    break;
                }
            }

            RadioButton1.Content = StrData1;
            RadioButton0.Content = StrData0;
            IStrThisName = AStrThisName;
        }

        public string GetElementData()
        {
            string LStrReturnData = string.Empty;

            if (RadioButton1.IsChecked == true) { LStrReturnData = RadioButton1.DataContext.ToString(); }
            if (RadioButton0.IsChecked == true) { LStrReturnData = RadioButton0.DataContext.ToString(); }

            return LStrReturnData;
        }
    }
}
