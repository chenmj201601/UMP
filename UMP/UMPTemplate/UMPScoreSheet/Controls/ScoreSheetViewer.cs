//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    73da6aa7-7267-4eff-aa3a-96f309a909f0
//        CLR Version:              4.0.30319.18444
//        Name:                     ScoreSheetViewer
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets.Controls
//        File Name:                ScoreSheetViewer
//
//        created by Charley at 2014/8/12 11:58:50
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VoiceCyber.UMP.ScoreSheets.Controls
{
    [TemplatePart(Name = PART_Panel, Type = typeof(Border))]
    [TemplatePart(Name = PART_Table_Items, Type = typeof(ItemsControl))]
    [TemplatePart(Name = PART_Tree_Items, Type = typeof(ItemsControl))]
    public class ScoreSheetViewer : ScoreObjectViewer
    {
        private const string PART_Table_Items = "PART_Table_Items";
        private const string PART_Tree_Items = "PART_Tree_Items";
        private const string PART_Panel = "PART_Panel";

        private ItemsControl mTableItems;
        private ItemsControl mTreeItems;
        private Border mBorderPanel;

        public static readonly DependencyProperty ScoreSheetProperty =
            DependencyProperty.Register("ScoreSheet", typeof (ScoreSheet), typeof (ScoreSheetViewer), new PropertyMetadata(default(ScoreSheet)));

        public ScoreSheet ScoreSheet
        {
            get { return (ScoreSheet) GetValue(ScoreSheetProperty); }
            set { SetValue(ScoreSheetProperty, value); }
        }

        private ObservableCollection<ScoreObjectViewer> mListScoreItems;

        static ScoreSheetViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (ScoreSheetViewer),
                new FrameworkPropertyMetadata(typeof (ScoreSheetViewer)));
        }

        public ScoreSheetViewer()
        {
            mListScoreItems = new ObservableCollection<ScoreObjectViewer>();
        }

        public override void Init()
        {
            base.Init();
            if (ScoreSheet != null)
            {
                ScoreSheet.OnPropertyChanged += ScoreSheet_OnPropertyChanged;
            }
            CreateItems();
            SetPanelStyle();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            mTableItems = GetTemplateChild(PART_Table_Items) as ItemsControl;
            if (mTableItems != null)
            {
                mTableItems.ItemsSource = mListScoreItems;
            }
            mTreeItems = GetTemplateChild(PART_Tree_Items) as ItemsControl;
            if (mTreeItems != null)
            {
                mTreeItems.ItemsSource = mListScoreItems;
            }
            mBorderPanel = GetTemplateChild(PART_Panel) as Border;
            if (mBorderPanel != null)
            {

            }
        }

        private void CreateItems()
        {
            if (ScoreSheet != null)
            {
                mListScoreItems.Clear();
                for (int i = 0; i < ScoreSheet.Items.Count; i++)
                {
                    ScoreItem item = ScoreSheet.Items[i];
                    ScoreGroup scoreGroup = item as ScoreGroup;
                    if (scoreGroup != null)
                    {
                        ScoreGroupViewer scoreGroupViewer = new ScoreGroupViewer();
                        scoreGroupViewer.ScoreGroup = scoreGroup;
                        scoreGroupViewer.Settings = Settings;
                        scoreGroupViewer.Languages = Languages;
                        scoreGroupViewer.LangID = LangID;
                        scoreGroupViewer.ViewClassic = ViewClassic;
                        scoreGroupViewer.ViewMode = ViewMode;
                        if (ViewClassic == ScoreItemClassic.Table)
                        {
                            if (i > 0)
                            {
                                scoreGroupViewer.BorderThickness = new Thickness(1, 1, 0, 0);
                            }
                            else
                            {
                                scoreGroupViewer.BorderThickness = new Thickness(0, 1, 0, 0);
                            }
                        }
                        mListScoreItems.Add(scoreGroupViewer);
                    }
                    Standard standard = item as Standard;
                    if (standard != null)
                    {
                        StandardViewer standardViewer = new StandardViewer();
                        standardViewer.Standard = standard;
                        standardViewer.Settings = Settings;
                        standardViewer.Languages = Languages;
                        standardViewer.LangID = LangID;
                        standardViewer.ViewClassic = ViewClassic;
                        standardViewer.ViewMode = ViewMode;
                        if (ViewClassic == ScoreItemClassic.Table)
                        {
                            if (i > 0)
                            {
                                standardViewer.BorderThickness = new Thickness(1, 1, 0, 0);
                            }
                            else
                            {
                                standardViewer.BorderThickness = new Thickness(0, 1, 0, 0);
                            }
                        }
                        mListScoreItems.Add(standardViewer);
                    }
                }
            }
        }

        void ScoreSheet_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SetPanelStyle();
        }

        private void SetPanelStyle()
        {
            if (ScoreSheet != null)
            {
                VisualStyle style = ScoreSheet.PanelStyle;
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
    }
}
