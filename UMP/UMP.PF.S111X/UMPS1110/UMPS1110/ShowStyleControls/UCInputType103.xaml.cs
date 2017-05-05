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
    public partial class UCInputType103 : UserControl
    {
        public UCInputType103()
        {
            InitializeComponent();
        }

        public void SetElementData(string AStrDataContext)
        {
            TextBoxPositiveInteger.SetElementData(AStrDataContext);
        }

        public string GetElementData()
        {
            return TextBoxPositiveInteger.GetElementData();
        }

        public void SetMinMaxDefaultValue(int AIntMinValue, int AIntMaxValue, int AIntDefaultValue)
        {
            TextBoxPositiveInteger.SetMinMaxDefaultValue(AIntMinValue, AIntMaxValue, AIntDefaultValue);
        }
    }
}
