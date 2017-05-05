//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    7df94bf7-a45e-48dd-9b9c-b8461a24f949
//        CLR Version:              4.0.30319.18444
//        Name:                     RadioYesNoStandardValueViewer
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets.Controls.Design
//        File Name:                RadioYesNoStandardValueViewer
//
//        created by Charley at 2014/6/18 16:52:08
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
    /// RadioYesNoStandardValueViewer.xaml 的交互逻辑
    /// </summary>
    public partial class RadioYesNoStandardValueViewer : IScoreObjectViewer
    {
        /// <summary>
        /// ViewerLoaded
        /// </summary>
        public event Action ViewerLoaded;
        /// <summary>
        /// YesNoStandard
        /// </summary>
        public YesNoStandard YesNoStandard;
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
        /// New RadioYesNoStandardValueViewer
        /// </summary>
        public RadioYesNoStandardValueViewer()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Init
        /// </summary>
        public void Init()
        {
            if (YesNoStandard == null) { return; }
            bool usePonitSystem = false;
            ScoreSheet scoreSheet = YesNoStandard.ScoreSheet;
            if (scoreSheet != null)
            {
                if (scoreSheet.UsePointSystem)
                {
                    usePonitSystem = true;
                }
            }
            if (usePonitSystem && YesNoStandard.PointSystem == YesNoStandard.Score
                ||!usePonitSystem&&YesNoStandard.TotalScore==YesNoStandard.Score)
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

        private void RadioYesNoStandardValueViewer_OnLoaded(object sender, RoutedEventArgs e)
        {
            DataContext = YesNoStandard;

            Init();
        }

        private void RadioBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (YesNoStandard == null) { return; }
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
                YesNoStandard.Score = RadioYes.IsChecked == true ? YesNoStandard.PointSystem : 0;
            }
            else
            {
                YesNoStandard.Score = RadioYes.IsChecked == true ? YesNoStandard.TotalScore : 0;
            }
            PropertyChangedEventArgs args = new PropertyChangedEventArgs();
            args.ScoreObject = YesNoStandard;
            args.PropertyName = "Score";
            YesNoStandard.PropertyChanged(YesNoStandard, args);
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
