//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    5d051988-d152-481f-aa27-799b1d3c2ea9
//        CLR Version:              4.0.30319.18444
//        Name:                     UCTextBoxStandardValue
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPTemplateDesigner
//        File Name:                UCTextBoxStandardValue
//
//        created by Charley at 2014/6/11 11:18:03
//        http://www.voicecyber.com
//
//======================================================================

using System;
using System.Collections.Generic;
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
using VoiceCyber.UMP.ScoreSheets;

namespace UMPTemplateDesigner
{
    /// <summary>
    /// UCTextBoxStandardValue.xaml 的交互逻辑
    /// </summary>
    public partial class UCTextBoxStandardValue
    {
        public NumericStandard NumericStandard;

        public UCTextBoxStandardValue()
        {
            InitializeComponent();
        }

        private void UCTextBoxStandardValue_OnLoaded(object sender, RoutedEventArgs e)
        {
            DataContext = NumericStandard;
        }

        public void Init()
        {
            
        }
    }
}
