//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    bf1a9698-8d68-423f-a110-08964dab651a
//        CLR Version:              4.0.30319.18444
//        Name:                     ScoreGroupStatisticsViewer
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets.Controls
//        File Name:                ScoreGroupStatisticsViewer
//
//        created by Charley at 2015/1/8 18:24:41
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace VoiceCyber.UMP.ScoreSheets.Controls
{
    public class ScoreGroupStatisticsViewer : ScoreObjectViewer
    {
        static ScoreGroupStatisticsViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (ScoreGroupStatisticsViewer),
                new FrameworkPropertyMetadata(typeof (ScoreGroupStatisticsViewer)));
        }

        public ScoreGroupStatisticsViewer()
        {
            mListScoreGroups = new ObservableCollection<ScoreGroup>();
        }

        #region DependencyProperties

        public static readonly DependencyProperty ScoreSheetProperty =
            DependencyProperty.Register("ScoreSheet", typeof(ScoreSheet), typeof(ScoreGroupStatisticsViewer), new PropertyMetadata(default(ScoreSheet)));

        public ScoreSheet ScoreSheet
        {
            get { return (ScoreSheet)GetValue(ScoreSheetProperty); }
            set { SetValue(ScoreSheetProperty, value); }
        }

        public static readonly DependencyProperty LbStatisticsProperty =
            DependencyProperty.Register("LbStatistics", typeof(string), typeof(ScoreGroupStatisticsViewer), new PropertyMetadata(default(string)));

        public string LbStatistics
        {
            get { return (string)GetValue(LbStatisticsProperty); }
            set { SetValue(LbStatisticsProperty, value); }
        }

        public static readonly DependencyProperty LbScoreItemProperty =
            DependencyProperty.Register("LbScoreItem", typeof(string), typeof(ScoreGroupStatisticsViewer), new PropertyMetadata(default(string)));

        public string LbScoreItem
        {
            get { return (string)GetValue(LbScoreItemProperty); }
            set { SetValue(LbScoreItemProperty, value); }
        }

        public static readonly DependencyProperty LbTotalScoreProperty =
            DependencyProperty.Register("LbTotalScore", typeof(string), typeof(ScoreGroupStatisticsViewer), new PropertyMetadata(default(string)));

        public string LbTotalScore
        {
            get { return (string)GetValue(LbTotalScoreProperty); }
            set { SetValue(LbTotalScoreProperty, value); }
        }

        public static readonly DependencyProperty LbScoreProperty =
            DependencyProperty.Register("LbScore", typeof(string), typeof(ScoreGroupStatisticsViewer), new PropertyMetadata(default(string)));

        public string LbScore
        {
            get { return (string)GetValue(LbScoreProperty); }
            set { SetValue(LbScoreProperty, value); }
        }

        #endregion


        public override void Init()
        {
            base.Init();

            InitScoreGroupList();
            ShowLanguages();
        }

        private void InitScoreGroupList()
        {
            mListScoreGroups.Clear();
            if(ScoreSheet==null){return;}
            ScoreSheet.SetItemLevel();
            List<ScoreItem> listItems = new List<ScoreItem>();
            ScoreSheet.GetAllScoreItem(ref listItems);
            for (int i = 0; i < listItems.Count; i++)
            {
                ScoreItem item = listItems[i];
                ScoreGroup group = item as ScoreGroup;
                if (group != null)
                {
                    mListScoreGroups.Add(group);
                }
            }
            mListScoreGroups.Add(ScoreSheet);
        }

        private void ShowLanguages()
        {
            if (Languages != null)
            {
                ScoreLangauge lang =
                    Languages.FirstOrDefault(l => l.LangID == LangID && l.Category == "ScoreViewer" && l.Code == "L102");
                if (lang != null)
                {
                    LbStatistics = lang.Display;
                }
                else
                {
                    LbStatistics = "Statistics";
                }
                lang =
                    Languages.FirstOrDefault(l => l.LangID == LangID && l.Category == "ScoreViewer" && l.Code == "L103");
                if (lang != null)
                {
                    LbScoreItem = lang.Display;
                }
                else
                {
                    LbScoreItem = "Score Item";
                }
                lang =
                    Languages.FirstOrDefault(l => l.LangID == LangID && l.Category == "ScoreViewer" && l.Code == "L104");
                if (lang != null)
                {
                    LbTotalScore = lang.Display;
                }
                else
                {
                    LbTotalScore = "TotalScore";
                }
                lang =
                    Languages.FirstOrDefault(l => l.LangID == LangID && l.Category == "ScoreViewer" && l.Code == "L105");
                if (lang != null)
                {
                    LbScore = lang.Display;
                }
                else
                {
                    LbScore = "Score";
                }
            }
        }

        #region Members

        private ObservableCollection<ScoreGroup> mListScoreGroups; 

        #endregion

        #region Template

        private const string PART_ListBoxStatistics = "PART_ListBoxStatistics";

        private ItemsControl mListBoxStatistics;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            mListBoxStatistics = GetTemplateChild(PART_ListBoxStatistics) as ItemsControl;
            if (mListBoxStatistics != null)
            {
                mListBoxStatistics.ItemsSource = mListScoreGroups;
            }
        }

        #endregion
    }
}
