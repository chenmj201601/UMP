//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    834f4721-6a26-4d80-81f0-e60106fba400
//        CLR Version:              4.0.30319.18444
//        Name:                     ScoreSheetViewer
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets.Controls.Design
//        File Name:                ScoreSheetViewer
//
//        created by Charley at 2014/6/18 16:34:18
//        http://www.voicecyber.com
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
using Color = System.Drawing.Color;

namespace VoiceCyber.UMP.ScoreSheets.Controls.Design
{
    /// <summary>
    /// ScoreSheetViewer.xaml 的交互逻辑
    /// </summary>
    public partial class ScoreSheetViewer : IScoreObjectViewer
    {
        /// <summary>
        /// ViewerLoaded
        /// </summary>
        public event Action ViewerLoaded;
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
        /// ScoreSheet
        /// </summary>
        public ScoreSheet ScoreSheet;
        /// <summary>
        /// ViewClassic
        /// </summary>
        public ScoreItemClassic ViewClassic { get; set; }
        /// <summary>
        /// New ScoreSheetViewer
        /// </summary>
        public ScoreSheetViewer()
        {
            InitializeComponent();
        }

        private void ScoreSheetViewer_OnLoaded(object sender, RoutedEventArgs e)
        {
            DataContext = ScoreSheet;
            ScoreObject socreObject = ScoreSheet;
            if (socreObject != null)
            {
                socreObject.OnPropertyChanged += ScoreObject_OnPropertyChanged;
            }

            Init();
        }
        /// <summary>
        /// Init
        /// </summary>
        public void Init()
        {
            CreateTitle();
            CreateItems();
            SetPanelStyle(ScoreSheet.PanelStyle);
            ShowLanguage();
        }

        void ScoreObject_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SetPanelStyle(ScoreSheet.PanelStyle);
            if (e.PropertyName == "ViewClassic")
            {
                ViewClassic = (ScoreItemClassic)e.NewValue;
                CreateTitle();
                CreateItems();
            }
        }

        private void SetPanelStyle(VisualStyle style)
        {
            if (style != null)
            {
                if (style.Width != 0)
                {
                    BorderPanel.Width = style.Width;
                }
                else
                {
                    BorderPanel.ClearValue(WidthProperty);
                }
                if (style.Height != 0)
                {
                    BorderPanel.Height = style.Height;
                }
                else
                {
                    BorderPanel.ClearValue(HeightProperty);
                }
                BorderPanel.Background = new SolidColorBrush(style.BackColor);
            }
        }

        private void CreateTitle()
        {
            if (ScoreSheet != null)
            {
                ScoreItemTitleViewer titleViewer = new ScoreItemTitleViewer();
                titleViewer.ScoreItem = ScoreSheet;
                titleViewer.ViewClassic = ViewClassic;
                titleViewer.Settings = Settings;
                titleViewer.LangID = LangID;
                titleViewer.Languages = Languages;
                BorderTitleTable.Child = titleViewer;
                titleViewer = new ScoreItemTitleViewer();
                titleViewer.ScoreItem = ScoreSheet;
                titleViewer.ViewClassic = ViewClassic;
                titleViewer.Settings = Settings;
                titleViewer.LangID = LangID;
                titleViewer.Languages = Languages;
                BorderTitleTree.Child = titleViewer;
            }
        }

        private void CreateItems()
        {
            if (ScoreSheet == null) { return; }
            if (ViewClassic == ScoreItemClassic.Table)
            {
                GridTable.Visibility = Visibility.Visible;
                GridTree.Visibility = Visibility.Collapsed;
                GridItemsTable.ColumnDefinitions.Clear();
                GridItemsTable.Children.Clear();
                for (int i = 0; i < ScoreSheet.Items.Count; i++)
                {
                    ScoreItem item = ScoreSheet.Items[i];
                    if (item.Type == ScoreObjectType.ScoreGroup)
                    {
                        ScoreGroupViewer scoreGroupViewer = new ScoreGroupViewer();
                        scoreGroupViewer.ViewClassic = ViewClassic;
                        scoreGroupViewer.Settings = Settings;
                        scoreGroupViewer.LangID = LangID;
                        scoreGroupViewer.Languages = Languages;
                        scoreGroupViewer.ScoreGroup = item as ScoreGroup;
                        scoreGroupViewer.BorderBrush = System.Windows.Media.Brushes.LightGray;
                        if (i > 0)
                        {
                            scoreGroupViewer.BorderThickness = new Thickness(1, 0, 0, 0);
                        }
                        GridItemsTable.ColumnDefinitions.Add(new ColumnDefinition());
                        GridItemsTable.Children.Add(scoreGroupViewer);
                        scoreGroupViewer.SetValue(Grid.ColumnProperty, i);
                    }
                    else
                    {
                        StandardViewer ucStandard = new StandardViewer();
                        ucStandard.ViewClassic = ViewClassic;
                        ucStandard.Settings = Settings;
                        ucStandard.LangID = LangID;
                        ucStandard.Languages = Languages;
                        ucStandard.Standard = item as Standard;
                        ucStandard.BorderBrush = System.Windows.Media.Brushes.LightGray;
                        if (i > 0)
                        {
                            ucStandard.BorderThickness = new Thickness(1, 0, 0, 0);
                        }
                        GridItemsTable.ColumnDefinitions.Add(new ColumnDefinition());
                        GridItemsTable.Children.Add(ucStandard);
                        ucStandard.SetValue(Grid.ColumnProperty, i);
                    }
                }
            }
            else
            {
                GridTable.Visibility = Visibility.Collapsed;
                GridTree.Visibility = Visibility.Visible;
                GridItemsTree.RowDefinitions.Clear();
                GridItemsTree.Children.Clear();
                for (int i = 0; i < ScoreSheet.Items.Count; i++)
                {
                    ScoreItem item = ScoreSheet.Items[i];
                    if (item.Type == ScoreObjectType.ScoreGroup)
                    {
                        ScoreGroupViewer scoreGroupViewer = new ScoreGroupViewer();
                        scoreGroupViewer.ViewClassic = ViewClassic;
                        scoreGroupViewer.Settings = Settings;
                        scoreGroupViewer.LangID = LangID;
                        scoreGroupViewer.Languages = Languages;
                        scoreGroupViewer.ScoreGroup = item as ScoreGroup;
                        GridItemsTree.RowDefinitions.Add(new RowDefinition());
                        GridItemsTree.Children.Add(scoreGroupViewer);
                        scoreGroupViewer.SetValue(Grid.RowProperty, i);
                    }
                    else
                    {
                        StandardViewer ucStandard = new StandardViewer();
                        ucStandard.ViewClassic = ViewClassic;
                        ucStandard.Settings = Settings;
                        ucStandard.LangID = LangID;
                        ucStandard.Languages = Languages;
                        ucStandard.Standard = item as Standard;
                        GridItemsTree.RowDefinitions.Add(new RowDefinition());
                        GridItemsTree.Children.Add(ucStandard);
                        ucStandard.SetValue(Grid.RowProperty, i);
                    }
                }
            }
        }

        private void ShowLanguage()
        {
            if(Languages==null){return;}
            ScoreLangauge lang = Languages.FirstOrDefault(l => l.LangID == LangID && l.Category == "ScoreViewer" && l.Code == "L_Creator");
            if (lang != null)
            {
                LbCreatorTitleTable.Content = lang.Display;
                LbCreatorTitleTree.Content = lang.Display;
            }
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
