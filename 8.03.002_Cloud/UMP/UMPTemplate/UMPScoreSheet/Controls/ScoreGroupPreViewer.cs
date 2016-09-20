//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    758ffa9c-0bda-4cf9-936e-5c382e517c88
//        CLR Version:              4.0.30319.18444
//        Name:                     ScoreGroupPreViewer
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets.Controls
//        File Name:                ScoreGroupPreViewer
//
//        created by Charley at 2014/8/5 15:15:56
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
    public class ScoreGroupPreViewer : ScoreObjectPreViewer
    {
        public static readonly DependencyProperty ScoreGroupProperty =
            DependencyProperty.Register("ScoreGroup", typeof(ScoreGroup), typeof(ScoreGroupPreViewer), new PropertyMetadata(default(ScoreGroup)));

        public ScoreGroup ScoreGroup
        {
            get { return (ScoreGroup)GetValue(ScoreGroupProperty); }
            set { SetValue(ScoreGroupProperty, value); }
        }

        private ObservableCollection<ScoreObjectPreViewer> mListScoreItems;

        static ScoreGroupPreViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ScoreGroupPreViewer),
                new FrameworkPropertyMetadata(typeof(ScoreGroupPreViewer)));
        }

        public ScoreGroupPreViewer()
        {
            mListScoreItems = new ObservableCollection<ScoreObjectPreViewer>();
        }

        public override void Init()
        {
            base.Init();
            if (ScoreGroup != null)
            {
                ScoreGroup.OnPropertyChanged += ScoreGroup_OnPropertyChanged;
            }
            CreateItems();
            SetPanelStyle();
        }

        void ScoreGroup_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SetPanelStyle();
        }

        private void CreateItems()
        {
            if (ScoreGroup != null)
            {
                mListScoreItems.Clear();
                for (int i = 0; i < ScoreGroup.Items.Count; i++)
                {
                    ScoreItem item = ScoreGroup.Items[i];
                    ScoreGroup scoreGroup = item as ScoreGroup;
                    if (scoreGroup != null)
                    {
                        ScoreGroupPreViewer scoreGroupViewer = new ScoreGroupPreViewer();
                        scoreGroupViewer.ScoreGroup = scoreGroup;
                        scoreGroupViewer.Settings = Settings;
                        scoreGroupViewer.Languages = Languages;
                        scoreGroupViewer.LangID = LangID;
                        scoreGroupViewer.ViewClassic = ViewClassic;
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
                        StandardPreviewer standardViewer = new StandardPreviewer();
                        standardViewer.Standard = standard;
                        standardViewer.Settings = Settings;
                        standardViewer.Languages = Languages;
                        standardViewer.LangID = LangID;
                        standardViewer.ViewClassic = ViewClassic;
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

        private const string PART_Table_Items = "PART_Table_Items";
        private const string PART_Tree_Items = "PART_Tree_Items";
        private const string PART_Panel = "PART_Panel";

        private ItemsControl mTableItems;
        private ItemsControl mTreeItems;
        private Border mBorderPanel;

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

        private void SetPanelStyle()
        {
            if (ScoreGroup != null)
            {
                VisualStyle style = ScoreGroup.PanelStyle;
                if (style != null && mBorderPanel != null)
                {
                    if (style.Width != 0)
                    {
                        mBorderPanel.Width = style.Width;
                    }
                    else
                    {
                        mBorderPanel.ClearValue(WidthProperty);
                    }
                    if (style.Height != 0)
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
