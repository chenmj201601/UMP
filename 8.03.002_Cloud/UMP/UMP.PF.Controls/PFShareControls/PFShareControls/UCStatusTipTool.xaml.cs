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
    public partial class UCStatusTipTool : UserControl
    {
        public UCStatusTipTool(SolidColorBrush AColorBrush)
        {
            InitializeComponent();
            LabelStatusTip.Foreground = AColorBrush;
        }

        public void ShowStatusTipTool(string AStrStatusTip)
        {
            LabelStatusTip.Content = AStrStatusTip;
        }
    }
}
