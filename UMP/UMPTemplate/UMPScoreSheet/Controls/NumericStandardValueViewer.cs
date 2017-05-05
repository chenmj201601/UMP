//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    f500a13f-2f1f-4fd6-b0c4-15c3eb75a528
//        CLR Version:              4.0.30319.18444
//        Name:                     NumericStandardValueViewer
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets.Controls
//        File Name:                NumericStandardValueViewer
//
//        created by Charley at 2014/8/12 11:07:04
//        http://www.voicecyber.com 
//
//======================================================================

using System.Linq;
using System.Windows;
using System.Windows.Controls;
using VoiceCyber.Wpf.CustomControls;

namespace VoiceCyber.UMP.ScoreSheets.Controls
{
    /// <summary>
    /// 
    /// </summary>
    [TemplatePart(Name = PART_Panel, Type = typeof(Border))]
    [TemplatePart(Name = PART_Value, Type = typeof(DoubleUpDown))]
    public class NumericStandardValueViewer : ScoreObjectViewer
    {
        private const string PART_Panel = "PART_Panel";
        private const string PART_Value = "PART_Value";
        private bool mIsSkipChange;

        private DoubleUpDown mValue;

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty NumericStandardProperty =
            DependencyProperty.Register("NumericStandard", typeof(NumericStandard), typeof(NumericStandardValueViewer), new PropertyMetadata(default(NumericStandard)));

        /// <summary>
        /// 
        /// </summary>
        public NumericStandard NumericStandard
        {
            get { return (NumericStandard)GetValue(NumericStandardProperty); }
            set { SetValue(NumericStandardProperty, value); }
        }

        static NumericStandardValueViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericStandardValueViewer),
                new FrameworkPropertyMetadata(typeof(NumericStandardValueViewer)));
        }

        /// <summary>
        /// 
        /// </summary>
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
                        if (ViewMode == 1
                            || ViewMode == 2)
                        {
                            mIsSkipChange = true;
                            mValue.Value = NumericStandard.Score;
                            mIsSkipChange = false;
                            //查看模式，控件不可用
                            if (ViewMode == 2)
                            {
                                mValue.IsEnabled = false;
                            }
                            return;
                        }
                    }
                    mValue.Value = NumericStandard.DefaultValue;
                }
            }
        }

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
            try
            {
                if (mIsSkipChange) { return; }
                if (e.NewValue == null) { return; }  //当用户按退格键删除文本框中所有数字，这里的NewValue可能为Null，此时不能强制转为double，所以这里判断一下
                double newValue = (double)e.NewValue;
                if (NumericStandard != null)
                {
                    string invalidMsg = "Input Invalid";
                    string title = "UMP ScoreSheet Designer";
                    if (Languages != null)
                    {
                        ScoreLangauge lang =
                            Languages.FirstOrDefault(
                                l => l.LangID == LangID && l.Category == "ScoreViewer" && l.Code == "T108");
                        if (lang != null)
                        {
                            title = lang.Display;
                        }
                        lang =
                            Languages.FirstOrDefault(
                                l => l.LangID == LangID && l.Category == "ScoreViewer" && l.Code == "N101");
                        if (lang != null)
                        {
                            invalidMsg = lang.Display;
                        }
                    }
                    if (newValue > NumericStandard.MaxValue || newValue < NumericStandard.MinValue)
                    {
                        MessageBox.Show(invalidMsg, title, MessageBoxButton.OK, MessageBoxImage.Error);
                        if (mValue != null)
                        {
                            mIsSkipChange = true;
                            mValue.Value = NumericStandard.Score;
                            mIsSkipChange = false;
                        }
                        return;
                    }
                    NumericStandard.Score = newValue;
                    NumericStandard.Score = newValue;
                    PropertyChangedEventArgs args = new PropertyChangedEventArgs();
                    args.ScoreObject = NumericStandard;
                    args.PropertyName = "Score";
                    NumericStandard.PropertyChanged(NumericStandard, args);
                }
            }
            catch { }
        }
    }
}
