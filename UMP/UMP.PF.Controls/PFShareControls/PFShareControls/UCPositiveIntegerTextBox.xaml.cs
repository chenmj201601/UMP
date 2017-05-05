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

namespace PFShareControls
{
    public partial class UCPositiveIntegerTextBox : UserControl
    {
        public UCPositiveIntegerTextBox()
        {
            InitializeComponent();
        }

        public void SetElementData(string AStrDataContext)
        {
            TextBoxPositiveInteger.Text = AStrDataContext;
        }

        public string GetElementData()
        {
            return TextBoxPositiveInteger.Text;
        }

        public void SetMinMaxDefaultValue(int AIntMinValue, int AIntMaxValue, int AIntDefaultValue)
        {
            TextBoxPositiveInteger.SetMinMaxDefaultValue(AIntMinValue, AIntMaxValue, AIntDefaultValue);
        }
    }
}
