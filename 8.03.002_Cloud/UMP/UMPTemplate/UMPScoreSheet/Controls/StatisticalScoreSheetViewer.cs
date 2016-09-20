//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    74d14ced-cd13-4d09-b4a4-a95359f708a9
//        CLR Version:              4.0.30319.18444
//        Name:                     StatisticalScoreSheetViewer
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets.Controls
//        File Name:                StatisticalScoreSheetViewer
//
//        created by Charley at 2015/1/8 18:04:38
//        http://www.voicecyber.com 
//
//======================================================================

using System.Windows;
using System.Windows.Controls;

namespace VoiceCyber.UMP.ScoreSheets.Controls
{
    public class StatisticalScoreSheetViewer : ScoreObjectViewer
    {
        static StatisticalScoreSheetViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (StatisticalScoreSheetViewer),
                new FrameworkPropertyMetadata(typeof (StatisticalScoreSheetViewer)));
        }

        public static readonly DependencyProperty ScoreSheetProperty =
            DependencyProperty.Register("ScoreSheet", typeof (ScoreSheet), typeof (StatisticalScoreSheetViewer), new PropertyMetadata(default(ScoreSheet)));

        public ScoreSheet ScoreSheet
        {
            get { return (ScoreSheet) GetValue(ScoreSheetProperty); }
            set { SetValue(ScoreSheetProperty, value); }
        }

        public override void Init()
        {
            base.Init();

            if(ScoreSheet==null){return;}
            if (mBorderScoreSheetViewer != null)
            {
                ScoreSheetViewer viewer=new ScoreSheetViewer();
                viewer.ViewMode = ViewMode;
                viewer.Settings = Settings;
                viewer.Languages = Languages;
                viewer.LangID = LangID;
                viewer.ViewClassic = ViewClassic;
                viewer.ScoreSheet = ScoreSheet;
                mBorderScoreSheetViewer.Child = viewer;
            }
            if (mBorderStatisticsViewer != null)
            {
                ScoreGroupStatisticsViewer viewer=new ScoreGroupStatisticsViewer();
                viewer.ViewMode = ViewMode;
                viewer.Settings = Settings;
                viewer.Languages = Languages;
                viewer.LangID = LangID;
                viewer.ViewClassic = ViewClassic;
                viewer.ScoreSheet = ScoreSheet;
                mBorderStatisticsViewer.Child = viewer;
            }
        }

        #region Public Functions

        public void CaculateScore()
        {
            if (mBorderStatisticsViewer != null)
            {
                ScoreGroupStatisticsViewer viewer = new ScoreGroupStatisticsViewer();
                viewer.ViewMode = ViewMode;
                viewer.Settings = Settings;
                viewer.Languages = Languages;
                viewer.LangID = LangID;
                viewer.ViewClassic = ViewClassic;
                viewer.ScoreSheet = ScoreSheet;
                mBorderStatisticsViewer.Child = viewer;
                mBorderStatisticsViewer.Visibility = Visibility.Visible;
            }
        }

        #endregion

        #region Template

        private const string PART_ScoreSheetViewer = "PART_ScoreSheetViewer";
        private const string PART_StatisticsViewer = "PART_StatisticsViewer";

        private Border mBorderScoreSheetViewer;
        private Border mBorderStatisticsViewer;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            mBorderScoreSheetViewer = GetTemplateChild(PART_ScoreSheetViewer) as Border;
            if (mBorderScoreSheetViewer != null)
            {
                
            }
            mBorderStatisticsViewer = GetTemplateChild(PART_StatisticsViewer) as Border;
            if (mBorderStatisticsViewer != null)
            {
                
            }
        }

        #endregion
    }
}
