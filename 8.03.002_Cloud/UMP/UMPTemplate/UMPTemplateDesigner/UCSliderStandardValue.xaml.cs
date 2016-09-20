//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    de7264db-17af-42b7-8142-86893c102ad1
//        CLR Version:              4.0.30319.18444
//        Name:                     UCSliderStandardValue
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPTemplateDesigner
//        File Name:                UCSliderStandardValue
//
//        created by Charley at 2014/6/17 13:47:45
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
    /// UCSliderStandardValue.xaml 的交互逻辑
    /// </summary>
    public partial class UCSliderStandardValue
    {
        public SliderStandard SliderStandard;

        public UCSliderStandardValue()
        {
            InitializeComponent();
        }

        private void UCSliderStandardValue_OnLoaded(object sender, RoutedEventArgs e)
        {
            DataContext = SliderStandard;

            Init();
        }

        public void Init()
        {
            
        }
    }
}
