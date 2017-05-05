//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    baa725be-77c3-4d9f-a934-00f9e56a5263
//        CLR Version:              4.0.30319.18444
//        Name:                     YesNoStandardValueViewer
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets.Controls
//        File Name:                YesNoStandardValueViewer
//
//        created by Charley at 2014/8/12 11:10:27
//        http://www.voicecyber.com 
//
//======================================================================

using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace VoiceCyber.UMP.ScoreSheets.Controls
{
    [TemplatePart(Name = PART_Panel, Type = typeof(Border))]
    [TemplatePart(Name = PART_Yes, Type = typeof(RadioButton))]
    [TemplatePart(Name = PART_No, Type = typeof(RadioButton))]
    public class YesNoStandardValueViewer : ScoreObjectViewer
    {
        private const string PART_Panel = "PART_Panel";
        private const string PART_Yes = "PART_Yes";
        private const string PART_No = "PART_No";

        private RadioButton mRadioYes;
        private RadioButton mRadioNo;

        public static readonly DependencyProperty YesNoStandardProperty =
            DependencyProperty.Register("YesNoStandard", typeof(YesNoStandard), typeof(YesNoStandardValueViewer), new PropertyMetadata(default(YesNoStandard)));

        public YesNoStandard YesNoStandard
        {
            get { return (YesNoStandard)GetValue(YesNoStandardProperty); }
            set { SetValue(YesNoStandardProperty, value); }
        }

        public static readonly DependencyProperty RadioYesContentProperty =
            DependencyProperty.Register("RadioYesContent", typeof(string), typeof(YesNoStandardValueViewer), new PropertyMetadata("Yes"));

        public string RadioYesContent
        {
            get { return (string)GetValue(RadioYesContentProperty); }
            set { SetValue(RadioYesContentProperty, value); }
        }

        public static readonly DependencyProperty RadioNoContentProperty =
            DependencyProperty.Register("RadioNoContent", typeof(string), typeof(YesNoStandardValueViewer), new PropertyMetadata("No"));

        public string RadioNoContent
        {
            get { return (string)GetValue(RadioNoContentProperty); }
            set { SetValue(RadioNoContentProperty, value); }
        }

        static YesNoStandardValueViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(YesNoStandardValueViewer),
                new FrameworkPropertyMetadata(typeof(YesNoStandardValueViewer)));
        }

        public override void Init()
        {
            base.Init();

            SetRadioContent();

            if (YesNoStandard != null)
            {
                bool usePonitSystem = false;
                ScoreSheet scoreSheet = YesNoStandard.ScoreSheet;
                if (scoreSheet != null)
                {
                    if (scoreSheet.UsePointSystem)
                    {
                        usePonitSystem = true;
                    }
                    //如果是修改成绩或查看成绩，显示实际成绩，否则显示默认值
                    if (ViewMode == 1 || ViewMode == 2)
                    {
                        if (usePonitSystem && YesNoStandard.PointSystem == YesNoStandard.Score
                            || !usePonitSystem && YesNoStandard.TotalScore == YesNoStandard.Score)
                        {
                            if (mRadioYes != null)
                            {
                                mRadioYes.IsChecked = true;
                            }
                            if (mRadioNo != null)
                            {
                                mRadioNo.IsChecked = false;
                            }
                        }
                        else
                        {
                            //if (mRadioYes != null)
                            //{
                            //    mRadioYes.IsChecked = !YesNoStandard.DefaultValue;
                            //}
                            //if (mRadioNo != null)
                            //{
                            //    mRadioNo.IsChecked = YesNoStandard.DefaultValue;
                            //}
                            if (YesNoStandard.DefaultValue)
                            {
                                if (mRadioYes != null)
                                {
                                    mRadioYes.IsChecked = !YesNoStandard.DefaultValue;
                                }
                                if (mRadioNo != null)
                                {
                                    mRadioNo.IsChecked = YesNoStandard.DefaultValue;
                                }

                            }
                            else
                            {
                                if (mRadioYes != null)
                                {
                                    mRadioYes.IsChecked = YesNoStandard.DefaultValue;
                                }
                                if (mRadioNo != null)
                                {
                                    mRadioNo.IsChecked = !YesNoStandard.DefaultValue;
                                }
                            }
                        }
                        //查看模式，控件不可用
                        if (ViewMode == 2)
                        {
                            if (mRadioYes != null)
                            {
                                mRadioYes.IsEnabled = false;
                            }
                            if (mRadioNo != null)
                            {
                                mRadioNo.IsEnabled = false;
                            }
                        }
                        return;
                    }
                    else
                    {
                        if (mRadioYes != null)
                        {
                            mRadioYes.IsChecked = YesNoStandard.DefaultValue;
                        }
                        if (mRadioNo != null)
                        {
                            mRadioNo.IsChecked = !YesNoStandard.DefaultValue;
                        }
                    }
                }
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            mRadioYes = GetTemplateChild(PART_Yes) as RadioButton;
            if (mRadioYes != null)
            {
                mRadioYes.Click += Radio_Click;
                mRadioYes.Checked += Radio_Click;
            }
            mRadioNo = GetTemplateChild(PART_No) as RadioButton;
            if (mRadioNo != null)
            {
                mRadioNo.Click += Radio_Click;
                mRadioNo.Checked += Radio_Click;
            }
        }

        private void Radio_Click(object sender, RoutedEventArgs e)
        {
            if (mRadioYes != null && mRadioNo != null && YesNoStandard != null)
            {
                bool usePonitSystem = false;
                ScoreSheet scoreSheet = YesNoStandard.ScoreSheet;
                if (scoreSheet != null)
                {
                    if (scoreSheet.UsePointSystem)
                    {
                        usePonitSystem = true;
                    }
                }
                if (usePonitSystem)
                {
                    YesNoStandard.Score = mRadioYes.IsChecked == true ? YesNoStandard.PointSystem : 0;
                }
                else
                {
                    YesNoStandard.Score = mRadioYes.IsChecked == true ? YesNoStandard.TotalScore : 0;
                }
                PropertyChangedEventArgs args = new PropertyChangedEventArgs();
                args.ScoreObject = YesNoStandard;
                args.PropertyName = "Score";
                YesNoStandard.PropertyChanged(YesNoStandard, args);
            }
        }

        private void SetRadioContent()
        {
            if (Languages != null)
            {
                if (YesNoStandard != null
                    && YesNoStandard.ReverseDisplay)
                {
                    //反转 是 否 显示
                    ScoreLangauge lang =
                        Languages.FirstOrDefault(
                            l => l.LangID == LangID && l.Category == "ScoreViewer" && l.Code == "R102");
                    if (lang != null)
                    {
                        RadioYesContent = lang.Display;
                    }
                    lang =
                        Languages.FirstOrDefault(
                            l => l.LangID == LangID && l.Category == "ScoreViewer" && l.Code == "R101");
                    if (lang != null)
                    {
                        RadioNoContent = lang.Display;
                    }
                }
                else
                {
                    ScoreLangauge lang =
                        Languages.FirstOrDefault(
                            l => l.LangID == LangID && l.Category == "ScoreViewer" && l.Code == "R101");
                    if (lang != null)
                    {
                        RadioYesContent = lang.Display;
                    }
                    lang =
                        Languages.FirstOrDefault(
                            l => l.LangID == LangID && l.Category == "ScoreViewer" && l.Code == "R102");
                    if (lang != null)
                    {
                        RadioNoContent = lang.Display;
                    }
                }
            }
        }
    }
}
