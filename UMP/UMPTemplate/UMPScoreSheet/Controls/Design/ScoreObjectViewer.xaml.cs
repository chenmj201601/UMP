//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    cc2894c1-019d-4d04-8043-193066b2e6e2
//        CLR Version:              4.0.30319.18444
//        Name:                     ScoreObjectViewer
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets.Controls.Design
//        File Name:                ScoreObjectViewer
//
//        created by Charley at 2014/7/8 17:10:50
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
    /// ScoreObjectViewer.xaml 的交互逻辑
    /// </summary>
    public partial class ScoreObjectViewer : IScoreObjectViewer
    {
        /// <summary>
        /// ViewerLoaded
        /// </summary>
        public event Action ViewerLoaded;
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
        /// ScoreGroup
        /// </summary>
        public ScoreObject ScoreObject;
        /// <summary>
        /// ScoreObjectViewer
        /// </summary>
        public ScoreObjectViewer()
        {
            InitializeComponent();
        }

        private void ScoreObjectViewer_OnLoaded(object sender, RoutedEventArgs e)
        {
            DataContext = ScoreObject;
            Init();
        }

        /// <summary>
        /// Init
        /// </summary>
        public void Init()
        {
            CreateScoreObject();
        }

        private void CreateScoreObject()
        {
            PanelScoreObjects.Children.Clear();
            if (ScoreObject != null)
            {
                ScoreSheet scoreSheet;
                switch (ScoreObject.Type)
                {
                    case ScoreObjectType.ScoreSheet:
                        scoreSheet = ScoreObject as ScoreSheet;
                        if (scoreSheet != null)
                        {
                            ScoreSheetViewer scoreSheetViewer = new ScoreSheetViewer();
                            scoreSheetViewer.ScoreSheet = scoreSheet;
                            scoreSheetViewer.Settings = Settings;
                            scoreSheetViewer.Languages = Languages;
                            scoreSheetViewer.LangID = LangID;
                            scoreSheetViewer.ViewClassic = scoreSheet.ViewClassic;
                            PanelScoreObjects.Children.Add(scoreSheetViewer);
                        }
                        break;
                    case ScoreObjectType.ScoreGroup:
                        ScoreGroup scoreGroup = ScoreObject as ScoreGroup;
                        if (scoreGroup != null)
                        {
                            scoreSheet = scoreGroup.ScoreSheet;
                            ScoreGroupViewer scoreGroupViewer=new ScoreGroupViewer();
                            scoreGroupViewer.ScoreGroup = scoreGroup;
                            scoreGroupViewer.Settings = Settings;
                            scoreGroupViewer.ViewClassic = scoreGroup.ViewClassic;
                            if (scoreSheet != null)
                            {
                                scoreGroupViewer.ViewClassic = scoreSheet.ViewClassic;
                            }
                            scoreGroupViewer.Languages = Languages;
                            scoreGroupViewer.LangID = LangID;
                            PanelScoreObjects.Children.Add(scoreGroupViewer);
                        }
                        break;
                    case ScoreObjectType.NumericStandard:
                    case ScoreObjectType.YesNoStandard:
                    case ScoreObjectType.ItemStandard:
                    case ScoreObjectType.SliderStandard:
                        Standard standard = ScoreObject as Standard;
                        if (standard != null)
                        {
                            scoreSheet = standard.ScoreSheet;
                            StandardViewer standardViewer = new StandardViewer();
                            standardViewer.ViewClassic = standard.ViewClassic;
                            if (scoreSheet != null)
                            {
                                standardViewer.ViewClassic = scoreSheet.ViewClassic;
                            }
                            standardViewer.Standard = standard;
                            standardViewer.Settings = Settings;
                            standardViewer.Languages = Languages;
                            standardViewer.LangID = LangID;
                            PanelScoreObjects.Children.Add(standardViewer);
                        }
                        break;
                    case ScoreObjectType.StandardItem:
                        StandardItem standardItem = ScoreObject as StandardItem;
                        if (standardItem != null)
                        {
                            StandardItemViewer standardItemViewer = new StandardItemViewer();
                            standardItemViewer.StandardItem = standardItem;
                            standardItemViewer.Settings = Settings;
                            standardItemViewer.Languages = Languages;
                            standardItemViewer.LangID = LangID;
                            PanelScoreObjects.Children.Add(standardItemViewer);
                        }
                        break;
                    case ScoreObjectType.TextComment:
                    case ScoreObjectType.ItemComment:
                        Comment comment = ScoreObject as Comment;
                        if (comment != null)
                        {
                            CommentViewer commentViewer = new CommentViewer();
                            commentViewer.Comment = comment;
                            commentViewer.Settings = Settings;
                            commentViewer.Languages = Languages;
                            commentViewer.LangID = LangID;
                            PanelScoreObjects.Children.Add(commentViewer);
                        }
                        break;
                    case ScoreObjectType.CommentItem:
                        CommentItem commentItem = ScoreObject as CommentItem;
                        if (commentItem != null)
                        {
                            CommentItemViewer commentItemViewer = new CommentItemViewer();
                            commentItemViewer.CommentItem = commentItem;
                            commentItemViewer.Settings = Settings;
                            commentItemViewer.Languages = Languages;
                            commentItemViewer.LangID = LangID;
                            PanelScoreObjects.Children.Add(commentItemViewer);
                        }
                        break;
                    case ScoreObjectType.ControlItem:
                        ControlItem controlItem = ScoreObject as ControlItem;
                        if (controlItem != null)
                        {
                            ControlItemViewer controlItemViewer = new ControlItemViewer();
                            controlItemViewer.ControlItem = controlItem;
                            controlItemViewer.Settings = Settings;
                            controlItemViewer.Languages = Languages;
                            controlItemViewer.LangID = LangID;
                            PanelScoreObjects.Children.Add(controlItemViewer);
                        }
                        break;
                }
            }
        }
    }
}
