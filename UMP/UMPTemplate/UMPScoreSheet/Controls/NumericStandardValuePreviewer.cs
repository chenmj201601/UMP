//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    8006226e-e03d-4510-92a3-acd19abcd220
//        CLR Version:              4.0.30319.18444
//        Name:                     NumericStandardValuePreviewer
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets.Controls
//        File Name:                NumericStandardValuePreviewer
//
//        created by Charley at 2014/8/5 17:54:57
//        http://www.voicecyber.com 
//
//======================================================================

using System.Linq;
using System.Windows;
using System.Windows.Controls;
using VoiceCyber.Wpf.CustomControls;

namespace VoiceCyber.UMP.ScoreSheets.Controls
{
    [TemplatePart(Name = PART_Panel, Type = typeof(Border))]
    [TemplatePart(Name = PART_Value, Type = typeof(DoubleUpDown))]
    public class NumericStandardValuePreviewer : ScoreObjectPreViewer
    {
        public static readonly DependencyProperty NumericStandardProperty =
            DependencyProperty.Register("NumericStandard", typeof(NumericStandard), typeof(NumericStandardValuePreviewer), new PropertyMetadata(default(NumericStandard)));

        public NumericStandard NumericStandard
        {
            get { return (NumericStandard)GetValue(NumericStandardProperty); }
            set { SetValue(NumericStandardProperty, value); }
        }

        static NumericStandardValuePreviewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericStandardValuePreviewer),
                new FrameworkPropertyMetadata(typeof(NumericStandardValuePreviewer)));
        }

        public override void Init()
        {
            base.Init();

            if (NumericStandard != null)
            {
                if (mValue != null)
                {
                    ScoreSheet scoreSheet = NumericStandard.ScoreSheet;
                    if (scoreSheet != null)
                    {
                        //如果是修改成绩或查看成绩，显示实际成绩，否则显示默认值
                        //if ((scoreSheet.Flag & 2) != 0
                        //    || (scoreSheet.Flag & 4) != 0)
                        //{
                        //    mValue.Value = NumericStandard.Score;
                        //    return;
                        //}
                        if (ViewMode == 1
                            || ViewMode == 2)
                        {
                            mValue.Value = NumericStandard.Score;
                            return;
                        }
                    }
                    mValue.Value = NumericStandard.DefaultValue;
                }
            }
        }

        private const string PART_Panel = "PART_Panel";
        private const string PART_Value = "PART_Value";

        private Border mBorderPanel;
        private DoubleUpDown mValue;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            mValue = GetTemplateChild(PART_Value) as DoubleUpDown;
            if (mValue != null)
            {
                mValue.ValueChanged += mValue_ValueChanged;
            }
        }

        void mValue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (NumericStandard != null)
            {
                string invalidMsg = "Input Invalid";
                string title = "UMP ScoreSheet Designer";
                if (Languages != null)
                {
                    ScoreLangauge lang =
                        Languages.FirstOrDefault(l => l.LangID == LangID && l.Category == "ScoreViewer" && l.Code == "T_AppName");
                    if (lang != null)
                    {
                        title = lang.Display;
                    }
                    lang =
                        Languages.FirstOrDefault(l => l.LangID == LangID && l.Category == "ScoreViewer" && l.Code == "N_001");
                    if (lang != null)
                    {
                        invalidMsg = lang.Display;
                    }
                }
                if (mValue != null)
                {
                    double doubleValue;
                    if (!double.TryParse(mValue.Text, out doubleValue))
                    {
                        MessageBox.Show(invalidMsg, title, MessageBoxButton.OK, MessageBoxImage.Error);
                        mValue.Value = NumericStandard.Score;
                        return;
                    }
                    if (doubleValue > NumericStandard.MaxValue || doubleValue < NumericStandard.MinValue)
                    {
                        MessageBox.Show(invalidMsg, title, MessageBoxButton.OK, MessageBoxImage.Error);
                        mValue.Value = NumericStandard.Score;
                        return;
                    }
                    NumericStandard.Score = doubleValue;
                    PropertyChangedEventArgs args = new PropertyChangedEventArgs();
                    args.ScoreObject = NumericStandard;
                    args.PropertyName = "Score";
                    NumericStandard.PropertyChanged(NumericStandard, args);
                }

            }
        }
    }
}
