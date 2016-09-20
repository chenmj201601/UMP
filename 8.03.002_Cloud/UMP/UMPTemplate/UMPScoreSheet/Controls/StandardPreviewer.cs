//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    4d174b07-68e6-479c-ad93-082ae6cc9d6c
//        CLR Version:              4.0.30319.18444
//        Name:                     StandardPreviewer
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets.Controls
//        File Name:                StandardPreviewer
//
//        created by Charley at 2014/8/5 17:06:08
//        http://www.voicecyber.com 
//
//======================================================================

using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VoiceCyber.UMP.ScoreSheets.Controls
{
    [TemplatePart(Name = PART_Panel, Type = typeof(Border))]
    [TemplatePart(Name = PART_Value_Table, Type = typeof(Border))]
    [TemplatePart(Name = PART_Value_Tree, Type = typeof(Border))]
    public class StandardPreviewer : ScoreObjectPreViewer
    {
        public static readonly DependencyProperty StandardProperty =
            DependencyProperty.Register("Standard", typeof(Standard), typeof(StandardPreviewer), new PropertyMetadata(default(Standard)));

        public Standard Standard
        {
            get { return (Standard)GetValue(StandardProperty); }
            set { SetValue(StandardProperty, value); }
        }

        public static readonly DependencyProperty ScoreValueWidthProperty =
            DependencyProperty.Register("ScoreValueWidth", typeof(int), typeof(StandardPreviewer), new PropertyMetadata(default(int)));

        public int ScoreValueWidth
        {
            get { return (int)GetValue(ScoreValueWidthProperty); }
            set { SetValue(ScoreValueWidthProperty, value); }
        }

        public static readonly DependencyProperty ScoreTipWidthProperty =
            DependencyProperty.Register("ScoreTipWidth", typeof(int), typeof(StandardPreviewer), new PropertyMetadata(default(int)));

        public int ScoreTipWidth
        {
            get { return (int)GetValue(ScoreTipWidthProperty); }
            set { SetValue(ScoreTipWidthProperty, value); }
        }

        public static readonly DependencyProperty IsNAProperty =
            DependencyProperty.Register("IsNA", typeof(bool), typeof(StandardPreviewer), new PropertyMetadata(false, OnIsNAChanged));

        public bool IsNA
        {
            get { return (bool)GetValue(IsNAProperty); }
            set { SetValue(IsNAProperty, value); }
        }

        private static void OnIsNAChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StandardPreviewer viewer = (StandardPreviewer)d;
            if (viewer != null)
            {
                viewer.OnIsNAChanged((bool)e.OldValue, (bool)e.NewValue);
            }
        }

        private void OnIsNAChanged(bool oldValue, bool newValue)
        {
            if (Standard != null)
            {
                Standard.IsNA = newValue;
            }
        }

        public static readonly DependencyProperty AllowNAProperty =
            DependencyProperty.Register("AllowNA", typeof(bool), typeof(StandardPreviewer), new PropertyMetadata(false));

        public bool AllowNA
        {
            get { return (bool)GetValue(AllowNAProperty); }
            set { SetValue(AllowNAProperty, value); }
        }

        static StandardPreviewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(StandardPreviewer),
                new FrameworkPropertyMetadata(typeof(StandardPreviewer)));
        }

        public override void Init()
        {
            base.Init();
            if (Standard != null)
            {
                Standard.OnPropertyChanged += Standard_OnPropertyChanged;
            }
            CreateStandardValue();
            SetWidth();
            SetPanelStyle();
            SetNA();
        }

        void Standard_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ValueItems")
            {
                CreateStandardValue();
            }
            SetWidth();
            SetPanelStyle();
            SetNA();
        }

        private void CreateStandardValue()
        {
            if (Standard != null)
            {
                if (ViewClassic == ScoreItemClassic.Table)
                {
                    switch (Standard.StandardType)
                    {
                        case StandardType.Numeric:
                            NumericStandardValuePreviewer numericViewer = new NumericStandardValuePreviewer();
                            numericViewer.NumericStandard = Standard as NumericStandard;
                            numericViewer.Settings = Settings;
                            numericViewer.Languages = Languages;
                            numericViewer.LangID = LangID;
                            numericViewer.ViewClassic = ViewClassic;
                            if (mTableValue != null)
                            {
                                mTableValue.Child = numericViewer;
                            }
                            break;
                        case StandardType.YesNo:
                            YesNoStandardValuePreviewer yesNoViewer = new YesNoStandardValuePreviewer();
                            yesNoViewer.YesNoStandard = Standard as YesNoStandard;
                            yesNoViewer.Settings = Settings;
                            yesNoViewer.Languages = Languages;
                            yesNoViewer.LangID = LangID;
                            yesNoViewer.ViewClassic = ViewClassic;
                            if (mTableValue != null)
                            {
                                mTableValue.Child = yesNoViewer;
                            }
                            break;
                        case StandardType.Slider:
                            SliderStandardValuePreviewer sliderViewer = new SliderStandardValuePreviewer();
                            sliderViewer.SliderStandard = Standard as SliderStandard;
                            sliderViewer.Settings = Settings;
                            sliderViewer.Languages = Languages;
                            sliderViewer.LangID = LangID;
                            sliderViewer.ViewClassic = ViewClassic;
                            if (mTableValue != null)
                            {
                                mTableValue.Child = sliderViewer;
                            }
                            break;
                        case StandardType.Item:
                            ItemStandardValuePreviewer itemViewer = new ItemStandardValuePreviewer();
                            itemViewer.ItemStandard = Standard as ItemStandard;
                            itemViewer.Settings = Settings;
                            itemViewer.Languages = Languages;
                            itemViewer.LangID = LangID;
                            itemViewer.ViewClassic = ViewClassic;
                            if (mTableValue != null)
                            {
                                mTableValue.Child = itemViewer;
                            }
                            break;
                    }
                }
                else
                {
                    switch (Standard.StandardType)
                    {
                        case StandardType.Numeric:
                            NumericStandardValuePreviewer numericViewer = new NumericStandardValuePreviewer();
                            numericViewer.NumericStandard = Standard as NumericStandard;
                            numericViewer.Settings = Settings;
                            numericViewer.Languages = Languages;
                            numericViewer.LangID = LangID;
                            numericViewer.ViewClassic = ViewClassic;
                            if (mTreeValue != null)
                            {
                                mTreeValue.Child = numericViewer;
                            }
                            break;
                        case StandardType.YesNo:
                            YesNoStandardValuePreviewer yesNoViewer = new YesNoStandardValuePreviewer();
                            yesNoViewer.YesNoStandard = Standard as YesNoStandard;
                            yesNoViewer.Settings = Settings;
                            yesNoViewer.Languages = Languages;
                            yesNoViewer.LangID = LangID;
                            yesNoViewer.ViewClassic = ViewClassic;
                            if (mTreeValue != null)
                            {
                                mTreeValue.Child = yesNoViewer;
                            }
                            break;
                        case StandardType.Slider:
                            SliderStandardValuePreviewer sliderViewer = new SliderStandardValuePreviewer();
                            sliderViewer.SliderStandard = Standard as SliderStandard;
                            sliderViewer.Settings = Settings;
                            sliderViewer.Languages = Languages;
                            sliderViewer.LangID = LangID;
                            sliderViewer.ViewClassic = ViewClassic;
                            if (mTreeValue != null)
                            {
                                mTreeValue.Child = sliderViewer;
                            }
                            break;
                        case StandardType.Item:
                            ItemStandardValuePreviewer itemViewer = new ItemStandardValuePreviewer();
                            itemViewer.ItemStandard = Standard as ItemStandard;
                            itemViewer.Settings = Settings;
                            itemViewer.Languages = Languages;
                            itemViewer.LangID = LangID;
                            itemViewer.ViewClassic = ViewClassic;
                            if (mTreeValue != null)
                            {
                                mTreeValue.Child = itemViewer;
                            }
                            break;
                    }
                }
            }
        }

        private const string PART_Value_Table = "PART_Value_Table";
        private const string PART_Value_Tree = "PART_Value_Tree";
        private const string PART_Panel = "PART_Panel";

        private Border mTableValue;
        private Border mTreeValue;
        private Border mBorderPanel;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            mTableValue = GetTemplateChild(PART_Value_Table) as Border;
            if (mTableValue != null)
            {

            }
            mTreeValue = GetTemplateChild(PART_Value_Tree) as Border;
            if (mTreeValue != null)
            {

            }
            mBorderPanel = GetTemplateChild(PART_Panel) as Border;
            if (mBorderPanel != null)
            {

            }
        }

        private void SetWidth()
        {
            if (Settings != null)
            {
                ScoreSetting setting = Settings.FirstOrDefault(s => s.Category == "S" && s.Code == "V_WIDTH");
                if (setting != null)
                {
                    int intValue;
                    if (int.TryParse(setting.Value, out intValue))
                    {
                        ScoreValueWidth = intValue;
                    }
                }
                setting = Settings.FirstOrDefault(s => s.Category == "S" && s.Code == "T_WIDTH");
                if (setting != null)
                {
                    int intValue;
                    if (int.TryParse(setting.Value, out intValue))
                    {
                        ScoreTipWidth = intValue;
                    }
                }
            }
        }

        private void SetPanelStyle()
        {
            if (Standard != null)
            {
                VisualStyle style = Standard.PanelStyle;
                if (style != null && mBorderPanel != null)
                {
                    if (style.Width != 0)
                    {
                        mBorderPanel.Width = style.Width;
                    }
                    else
                    {
                        mBorderPanel.ClearValue(WidthProperty);
                    }
                    if (style.Height != 0)
                    {
                        mBorderPanel.Height = style.Height;
                    }
                    else
                    {
                        mBorderPanel.ClearValue(HeightProperty);
                    }
                    mBorderPanel.Background = new SolidColorBrush(style.BackColor);
                }
            }
        }

        private void SetNA()
        {
            if (Standard != null)
            {
                AllowNA = Standard.IsAllowNA;
                if (AllowNA)
                {
                    IsNA = Standard.IsNA;
                }
            }
        }
    }
}
