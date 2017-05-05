//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    5cc6505e-470a-42b6-b52b-c532c6198166
//        CLR Version:              4.0.30319.18444
//        Name:                     SliderStandardValueViewer
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets.Controls.Design
//        File Name:                SliderStandardValueViewer
//
//        created by Charley at 2014/6/18 16:52:43
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

namespace VoiceCyber.UMP.ScoreSheets.Controls.Design
{
    /// <summary>
    /// SliderStandardValueViewer.xaml 的交互逻辑
    /// </summary>
    public partial class SliderStandardValueViewer : IScoreObjectViewer
    {
        /// <summary>
        /// ViewerLoaded
        /// </summary>
        public event Action ViewerLoaded;
        /// <summary>
        /// SliderStandard
        /// </summary>
        public SliderStandard SliderStandard;
        /// <summary>
        /// ViewClassic
        /// </summary>
        public ScoreItemClassic ViewClassic { get; set; }
        /// <summary>
        /// 设置信息
        /// </summary>
        public List<ScoreSetting> Settings { get; set; }
        /// <summary>
        /// 语言信息
        /// </summary>
        public List<ScoreLangauge> Languages { get; set; }
        /// <summary>
        /// 语言类型
        /// </summary>
        public int LangID { get; set; }
        /// <summary>
        /// New SliderStandardValueViewer
        /// </summary>
        public SliderStandardValueViewer()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Init
        /// </summary>
        public void Init()
        {
            if (SliderStandard != null)
            {
                SliderScore.Minimum = SliderStandard.MinValue;
                SliderScore.Maximum = SliderStandard.MaxValue;
                SliderScore.TickFrequency = SliderStandard.Interval;
                SliderScore.Value = SliderStandard.Score;
            }
        }

        private void SliderStandardValueViewer_OnLoaded(object sender, RoutedEventArgs e)
        {
            DataContext = SliderStandard;
            if (SliderStandard != null)
            {
                SliderStandard.OnPropertyChanged += SliderStandard_OnPropertyChanged;
            }
            Init();
        }

        void SliderStandard_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SliderScore.Minimum = SliderStandard.MinValue;
            SliderScore.Maximum = SliderStandard.MaxValue;
            SliderScore.TickFrequency = SliderStandard.Interval;
            SliderScore.Value = SliderStandard.Score;
        }

        private void SliderScore_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SliderStandard.Score = SliderScore.Value;
            SliderStandard.PropertyChanged(SliderStandard, new PropertyChangedEventArgs { PropertyName = "Score" });
        }

        private void SubViewerLoaded()
        {
            if (ViewerLoaded != null)
            {
                ViewerLoaded();
            }
        }
    }
}
