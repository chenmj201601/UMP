//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    3fcb6bfa-65d2-4664-b1d4-97d2f93cfc71
//        CLR Version:              4.0.30319.18444
//        Name:                     SliderStandardValuePreviewer
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets.Controls
//        File Name:                SliderStandardValuePreviewer
//
//        created by Charley at 2014/8/6 12:06:48
//        http://www.voicecyber.com 
//
//======================================================================

using System.Windows;
using System.Windows.Controls;

namespace VoiceCyber.UMP.ScoreSheets.Controls
{
    [TemplatePart(Name = PART_Panel, Type = typeof(Border))]
    [TemplatePart(Name = PART_Value, Type = typeof(Slider))]
    public class SliderStandardValuePreviewer : ScoreObjectPreViewer
    {
        public static readonly DependencyProperty SliderStandardProperty =
            DependencyProperty.Register("SliderStandard", typeof(SliderStandard), typeof(SliderStandardValuePreviewer), new PropertyMetadata(default(SliderStandard)));

        public SliderStandard SliderStandard
        {
            get { return (SliderStandard)GetValue(SliderStandardProperty); }
            set { SetValue(SliderStandardProperty, value); }
        }
        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register("MinValue", typeof(double), typeof(SliderStandardValuePreviewer), new PropertyMetadata(default(double)));

        public double MinValue
        {
            get { return (double)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }

        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(double), typeof(SliderStandardValuePreviewer), new PropertyMetadata(default(double)));

        public double MaxValue
        {
            get { return (double)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register("Interval", typeof(double), typeof(SliderStandardValuePreviewer), new PropertyMetadata(default(double)));

        public double Interval
        {
            get { return (double)GetValue(IntervalProperty); }
            set { SetValue(IntervalProperty, value); }
        }

        static SliderStandardValuePreviewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SliderStandardValuePreviewer),
                new FrameworkPropertyMetadata(typeof(SliderStandardValuePreviewer)));
        }

        public override void Init()
        {
            base.Init();

            if (SliderStandard != null)
            {
                MinValue = SliderStandard.MinValue;
                MaxValue = SliderStandard.MaxValue;
                Interval = SliderStandard.Interval;

                if (mValue != null)
                {
                    if (ViewMode == 1 || ViewMode == 2)
                    {
                        mValue.Value = SliderStandard.Score;
                    }
                    else
                    {
                        mValue.Value = SliderStandard.GetDefaultValue();
                    }
                }
            }
        }

        private const string PART_Panel = "PART_Panel";
        private const string PART_Value = "PART_Value";

        private Slider mValue;
        private Border mBorderPanel;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            mValue = GetTemplateChild(PART_Value) as Slider;
            if (mValue != null)
            {
                mValue.ValueChanged += mValue_ValueChanged;
            }
        }

        void mValue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (mValue != null && SliderStandard != null)
            {
                SliderStandard.Score = mValue.Value;
                SliderStandard.PropertyChanged(SliderStandard, new PropertyChangedEventArgs { PropertyName = "Score" });
            }
        }
    }
}
