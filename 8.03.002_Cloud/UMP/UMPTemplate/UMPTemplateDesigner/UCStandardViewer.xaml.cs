//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    912ba7cb-2bba-4ae7-a765-c00f85a7f595
//        CLR Version:              4.0.30319.18444
//        Name:                     UCStandardView
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPTemplateDesigner
//        File Name:                UCStandardView
//
//        created by Charley at 2014/6/11 11:25:28
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
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VoiceCyber.UMP.ScoreSheets;

namespace UMPTemplateDesigner
{
    /// <summary>
    /// UCStandardView.xaml 的交互逻辑
    /// </summary>
    public partial class UCStandardViewer
    {
        public Standard Standard;

        public UCStandardViewer()
        {
            InitializeComponent();
        }

        private void UCStandardView_OnLoaded(object sender, RoutedEventArgs e)
        {
            DataContext = Standard;
            ScoreObject socreObject = Standard;
            if (socreObject != null)
            {
                socreObject.OnPropertyChanged += ScoreObject_OnPropertyChanged;
            }
            Init();
        }

        public void Init()
        {
            if (Standard == null) { return; }
            switch (Standard.StandardType)
            {
                case StandardType.Numeric:
                    NumericStandard numericStandard = Standard as NumericStandard;
                    if (numericStandard == null) { return; }
                    UCTextBoxStandardValue ucTextBoxStandardValue = new UCTextBoxStandardValue();
                    ucTextBoxStandardValue.NumericStandard = numericStandard;
                    BorderValue.Child = ucTextBoxStandardValue;
                    ucTextBoxStandardValue.Init();
                    break;
                case StandardType.YesNo:
                    YesNoStandard yesNoStandard = Standard as YesNoStandard;
                    if (yesNoStandard == null) { return; }
                    UCYesNoStandardValue ucYesNoStandardValue = new UCYesNoStandardValue();
                    ucYesNoStandardValue.YesNoStandard = yesNoStandard;
                    BorderValue.Child = ucYesNoStandardValue;
                    ucYesNoStandardValue.Init();
                    break;
                case StandardType.Item:
                    ItemStandard itemStandard = Standard as ItemStandard;
                    if(itemStandard==null){return;}
                    UCComboStandardValue ucComboStandardValue=new UCComboStandardValue();
                    ucComboStandardValue.ItemStandard = itemStandard;
                    BorderValue.Child = ucComboStandardValue;
                    ucComboStandardValue.Init();
                    break;
                case StandardType.Slider:
                    SliderStandard sliderStandard = Standard as SliderStandard;
                    if(sliderStandard==null){return;}
                    UCSliderStandardValue ucSliderStandardValue=new UCSliderStandardValue();
                    ucSliderStandardValue.SliderStandard = sliderStandard;
                    BorderValue.Child = ucSliderStandardValue;
                    ucSliderStandardValue.Init();
                    break;
            }
            ScoreObject_OnPropertyChanged(this, null);
        }

        void ScoreObject_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ScoreItem scoreItem = Standard;
            if (scoreItem != null)
            {
                VisualStyle style = scoreItem.TitleStyle;
                if (style != null)
                {
                    LbTitle.Foreground = new SolidColorBrush(style.ForeColor);
                    BorderTitle.Background = new SolidColorBrush(style.BackColor);
                    if (style.FontFamily != null)
                    {
                        LbTitle.FontFamily = style.FontFamily;
                    }
                    if (style.FontSize != 0)
                    {
                        LbTitle.FontSize = style.FontSize;
                    }
                    LbTitle.FontWeight = style.FontWeight;
                }
                style = scoreItem.PanelStyle;
                if (style != null)
                {
                    if (style.Width != 0)
                    {
                        BorderPanel.Width = style.Width;
                    }
                    else
                    {
                        BorderPanel.ClearValue(WidthProperty);
                    }
                    BorderPanel.Background = new SolidColorBrush(style.BackColor);
                }
            }
        }
    }
}
