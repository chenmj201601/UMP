//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    5b1d6721-3d8f-4827-900c-ad4d5ecfbf8c
//        CLR Version:              4.0.30319.18444
//        Name:                     UCYesNoStandardValue
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPTemplateDesigner
//        File Name:                UCYesNoStandardValue
//
//        created by Charley at 2014/6/11 14:58:34
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
    /// UCYesNoStandardValue.xaml 的交互逻辑
    /// </summary>
    public partial class UCYesNoStandardValue
    {
        public YesNoStandard YesNoStandard;

        public UCYesNoStandardValue()
        {
            InitializeComponent();
        }

        private void UCYesNoStandardValue_OnLoaded(object sender, RoutedEventArgs e)
        {
            DataContext = YesNoStandard;

            Init();
        }

        public void Init()
        {
            if (YesNoStandard == null) { return; }
            if (YesNoStandard.TotalScore == YesNoStandard.Score)
            {
                RadioYes.IsChecked = true;
                RadioNo.IsChecked = false;
            }
            else
            {
                RadioYes.IsChecked = false;
                RadioNo.IsChecked = true;
            }
        }

        private void RadioYes_OnClick(object sender, RoutedEventArgs e)
        {
            YesNoStandard.Value = RadioYes.IsChecked == true;
            YesNoStandard.Score = RadioYes.IsChecked == true ? YesNoStandard.TotalScore : 0;
        }
    }
}
