//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    46d0048c-73d0-4871-b4d8-365a8335b9e5
//        CLR Version:              4.0.30319.18444
//        Name:                     UCScoreSheetViewer
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPTemplateDesigner
//        File Name:                UCScoreSheetViewer
//
//        created by Charley at 2014/6/11 14:08:09
//        http://www.voicecyber.com
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Drawing;
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
using VoiceCyber.UMP.ScoreSheets;

namespace UMPTemplateDesigner
{
    /// <summary>
    /// UCScoreSheetViewer.xaml 的交互逻辑
    /// </summary>
    public partial class UCScoreSheetViewer
    {
        public ScoreSheet ScoreSheet;

        public UCScoreSheetViewer()
        {
            InitializeComponent();
        }

        private void UCScoreSheetViewer_OnLoaded(object sender, RoutedEventArgs e)
        {
            DataContext = ScoreSheet;
            ScoreObject socreObject = ScoreSheet;
            if (socreObject != null)
            {
                socreObject.OnPropertyChanged += ScoreObject_OnPropertyChanged;
            }

            Init();
        }

        void ScoreObject_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ScoreItem scoreItem = ScoreSheet;
            if (scoreItem != null)
            {
                VisualStyle style = scoreItem.TitleStyle;
                if (style != null)
                {
                    LbTitle.Foreground = new SolidColorBrush(style.ForeColor);
                    BorderTitle.Background = new SolidColorBrush(style.BackColor);
                    if (style.FontFamily != null)
                    {
                        LbTitle.FontFamily = style.FontFamily;
                    }
                    if (style.FontSize != 0)
                    {
                        LbTitle.FontSize = style.FontSize;
                    }
                    LbTitle.FontWeight = style.FontWeight;
                }
                style = scoreItem.PanelStyle;
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
                    BorderPanel.Background = new SolidColorBrush(style.BackColor);
                }
            }
        }

        public void Init()
        {
            if (ScoreSheet == null) { return; }
            GridItems.ColumnDefinitions.Clear();
            GridItems.Children.Clear();
            for (int i = 0; i < ScoreSheet.Items.Count; i++)
            {
                ScoreItem item = ScoreSheet.Items[i];
                if (item.Type == ScoreObjectType.ScoreGroup)
                {
                    UCScoreGroupViewer ucScoreGroup = new UCScoreGroupViewer();
                    ucScoreGroup.ScoreGroup = item as ScoreGroup;
                    if (i > 0)
                    {
                        ucScoreGroup.BorderThickness = new Thickness(1, 0, 0, 0);
                    }
                    GridItems.ColumnDefinitions.Add(new ColumnDefinition());
                    GridItems.Children.Add(ucScoreGroup);
                    ucScoreGroup.SetValue(Grid.ColumnProperty, i);
                    ucScoreGroup.Init();
                }
                else
                {
                    UCStandardViewer ucStandard = new UCStandardViewer();
                    ucStandard.Standard = item as Standard;
                    if (i > 0)
                    {
                        ucStandard.BorderThickness = new Thickness(1, 0, 0, 0);
                    }
                    GridItems.ColumnDefinitions.Add(new ColumnDefinition());
                    GridItems.Children.Add(ucStandard);
                    ucStandard.SetValue(Grid.ColumnProperty, i);
                    ucStandard.Init();

                }
            }
            ScoreObject_OnPropertyChanged(this, null);
        }
    }
}
