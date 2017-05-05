//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    1343b6d1-cc2a-413b-b28d-78f5fc393f3f
//        CLR Version:              4.0.30319.18444
//        Name:                     StandardViewer
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets.Controls
//        File Name:                StandardViewer
//
//        created by Charley at 2014/8/12 11:24:48
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
    public class StandardViewer : ScoreObjectViewer
    {
        private const string PART_Value_Table = "PART_Value_Table";
        private const string PART_Value_Tree = "PART_Value_Tree";
        private const string PART_Panel = "PART_Panel";

        private Border mTableValue;
        private Border mTreeValue;
        private Border mBorderPanel;

        public static readonly DependencyProperty StandardProperty =
            DependencyProperty.Register("Standard", typeof(Standard), typeof(StandardViewer), new PropertyMetadata(default(Standard)));

        public Standard Standard
        {
            get { return (Standard)GetValue(StandardProperty); }
            set { SetValue(StandardProperty, value); }
        }

        public static readonly DependencyProperty ScoreValueWidthProperty =
            DependencyProperty.Register("ScoreValueWidth", typeof(int), typeof(StandardViewer), new PropertyMetadata(default(int)));

        public int ScoreValueWidth
        {
            get { return (int)GetValue(ScoreValueWidthProperty); }
            set { SetValue(ScoreValueWidthProperty, value); }
        }

        public static readonly DependencyProperty ScoreTipWidthProperty =
            DependencyProperty.Register("ScoreTipWidth", typeof(int), typeof(StandardViewer), new PropertyMetadata(default(int)));

        public int ScoreTipWidth
        {
            get { return (int)GetValue(ScoreTipWidthProperty); }
            set { SetValue(ScoreTipWidthProperty, value); }
        }

        public static readonly DependencyProperty IsNAProperty =
            DependencyProperty.Register("IsNA", typeof(bool), typeof(StandardViewer), new PropertyMetadata(false, OnIsNAChanged));

        public bool IsNA
        {
            get { return (bool)GetValue(IsNAProperty); }
            set { SetValue(IsNAProperty, value); }
        }

        private static void OnIsNAChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StandardViewer viewer = (StandardViewer)d;
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
            DependencyProperty.Register("AllowNA", typeof(bool), typeof(StandardViewer), new PropertyMetadata(false));

        public bool AllowNA
        {
            get { return (bool)GetValue(AllowNAProperty); }
            set { SetValue(AllowNAProperty, value); }
        }

        public static readonly DependencyProperty CanModifyScoreProperty =
            DependencyProperty.Register("CanModifyScore", typeof(bool), typeof(StandardViewer), new PropertyMetadata(default(bool)));

        public bool CanModifyScore
        {
            get { return (bool)GetValue(CanModifyScoreProperty); }
            set { SetValue(CanModifyScoreProperty, value); }
        }

        static StandardViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(StandardViewer),
                new FrameworkPropertyMetadata(typeof(StandardViewer)));
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
            SetCanModifyScore();
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
            SetCanModifyScore();
        }

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

        private void CreateStandardValue()
        {
            if (Standard != null)
            {
                if (ViewClassic == ScoreItemClassic.Table)
                {
                    switch (Standard.StandardType)
                    {
                        case StandardType.Numeric:
                            NumericStandardValueViewer numericViewer = new NumericStandardValueViewer();
                            numericViewer.NumericStandard = Standard as NumericStandard;
                            numericViewer.Settings = Settings;
                            numericViewer.Languages = Languages;
                            numericViewer.LangID = LangID;
                            numericViewer.ViewClassic = ViewClassic;
                            numericViewer.ViewMode = ViewMode;
                            if (mTableValue != null)
                            {
                                mTableValue.Child = numericViewer;
                            }
                            break;
                        case StandardType.YesNo:
                            YesNoStandardValueViewer yesNoViewer = new YesNoStandardValueViewer();
                            yesNoViewer.YesNoStandard = Standard as YesNoStandard;
                            yesNoViewer.Settings = Settings;
                            yesNoViewer.Languages = Languages;
                            yesNoViewer.LangID = LangID;
                            yesNoViewer.ViewClassic = ViewClassic;
                            yesNoViewer.ViewMode = ViewMode;
                            if (mTableValue != null)
                            {
                                mTableValue.Child = yesNoViewer;
                            }
                            break;
                        case StandardType.Slider:
                            SliderStandardValueViewer sliderViewer = new SliderStandardValueViewer();
                            sliderViewer.SliderStandard = Standard as SliderStandard;
                            sliderViewer.Settings = Settings;
                            sliderViewer.Languages = Languages;
                            sliderViewer.LangID = LangID;
                            sliderViewer.ViewClassic = ViewClassic;
                            sliderViewer.ViewMode = ViewMode;
                            if (mTableValue != null)
                            {
                                mTableValue.Child = sliderViewer;
                            }
                            break;
                        case StandardType.Item:
                            ItemStandardValueViewer itemViewer = new ItemStandardValueViewer();
                            itemViewer.ItemStandard = Standard as ItemStandard;
                            itemViewer.Settings = Settings;
                            itemViewer.Languages = Languages;
                            itemViewer.LangID = LangID;
                            itemViewer.ViewClassic = ViewClassic;
                            itemViewer.ViewMode = ViewMode;
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
                            NumericStandardValueViewer numericViewer = new NumericStandardValueViewer();
                            numericViewer.NumericStandard = Standard as NumericStandard;
                            numericViewer.Settings = Settings;
                            numericViewer.Languages = Languages;
                            numericViewer.LangID = LangID;
                            numericViewer.ViewClassic = ViewClassic;
                            numericViewer.ViewMode = ViewMode;
                            if (mTreeValue != null)
                            {
                                mTreeValue.Child = numericViewer;
                            }
                            break;
                        case StandardType.YesNo:
                            YesNoStandardValueViewer yesNoViewer = new YesNoStandardValueViewer();
                            yesNoViewer.YesNoStandard = Standard as YesNoStandard;
                            yesNoViewer.Settings = Settings;
                            yesNoViewer.Languages = Languages;
                            yesNoViewer.LangID = LangID;
                            yesNoViewer.ViewClassic = ViewClassic;
                            yesNoViewer.ViewMode = ViewMode;
                            if (mTreeValue != null)
                            {
                                mTreeValue.Child = yesNoViewer;
                            }
                            break;
                        case StandardType.Slider:
                            SliderStandardValueViewer sliderViewer = new SliderStandardValueViewer();
                            sliderViewer.SliderStandard = Standard as SliderStandard;
                            sliderViewer.Settings = Settings;
                            sliderViewer.Languages = Languages;
                            sliderViewer.LangID = LangID;
                            sliderViewer.ViewClassic = ViewClassic;
                            sliderViewer.ViewMode = ViewMode;
                            if (mTreeValue != null)
                            {
                                mTreeValue.Child = sliderViewer;
                            }
                            break;
                        case StandardType.Item:
                            ItemStandardValueViewer itemViewer = new ItemStandardValueViewer();
                            itemViewer.ItemStandard = Standard as ItemStandard;
                            itemViewer.Settings = Settings;
                            itemViewer.Languages = Languages;
                            itemViewer.LangID = LangID;
                            itemViewer.ViewClassic = ViewClassic;
                            itemViewer.ViewMode = ViewMode;
                            if (mTreeValue != null)
                            {
                                mTreeValue.Child = itemViewer;
                            }
                            break;
                    }
                }
            }
        }

        private void SetWidth()
        {
            int scoreWidth = 150;
            int tipWidth = 50;
            if (Standard.ScoreSheet != null)
            {
                int intValue;
                if (int.TryParse(Standard.ScoreSheet.ScoreWidth.ToString(), out intValue)
                    &&intValue>0)
                {
                    scoreWidth = intValue;
                }
                if (int.TryParse(Standard.ScoreSheet.TipWidth.ToString(), out intValue)
                    &&intValue>0)
                {
                    tipWidth = intValue;
                }
            }
            ScoreValueWidth = scoreWidth;
            ScoreTipWidth = tipWidth;


            /*  2016-02-26 Waves修改--打分栏、得分栏宽带动态设置
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
            }**/
        }

        private void SetPanelStyle()
        {
            if (Standard != null)
            {
                VisualStyle style = Standard.PanelStyle;
                if (style != null && mBorderPanel != null)
                {
                    if (style.Width > 0)
                    {
                        mBorderPanel.Width = style.Width;
                    }
                    else
                    {
                        mBorderPanel.ClearValue(WidthProperty);
                    }
                    if (style.Height > 0)
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

        private void SetCanModifyScore()
        {
            if (Standard != null)
            {
                bool canModify = true;
                if (Standard.IsAutoStandard)
                {
                    var scoreSheet = Standard.ScoreSheet;
                    if (scoreSheet != null)
                    {
                        if (!scoreSheet.AllowModifyScore)
                        {
                            canModify = false;
                        }
                    }
                }
                else
                {
                    if (ViewMode == 2)
                    {
                        canModify = false;
                    }
                }
                CanModifyScore = canModify;
            }
        }
    }
}
