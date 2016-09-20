//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    208107f6-821b-4db8-9ddf-0dd64851c6ab
//        CLR Version:              4.0.30319.18444
//        Name:                     UCTScoreSheetViewer
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPTemplateDesigner
//        File Name:                UCTScoreSheetViewer
//
//        created by Charley at 2014/6/18 10:08:55
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
using VoiceCyber.UMP.ScoreSheets;

namespace UMPTemplateDesigner
{
    /// <summary>
    /// UCTScoreSheetViewer.xaml 的交互逻辑
    /// </summary>
    public partial class UCTScoreSheetViewer : UserControl
    {
        public ScoreSheet ScoreSheet;

        public UCTScoreSheetViewer()
        {
            InitializeComponent();
        }

        private void UCTScoreSheetViewer_OnLoaded(object sender, RoutedEventArgs e)
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
            GridItems.RowDefinitions.Clear();
            GridItems.Children.Clear();
            for (int i = 0; i < ScoreSheet.Items.Count; i++)
            {
                ScoreItem item = ScoreSheet.Items[i];
                if (item.Type == ScoreObjectType.ScoreGroup)
                {
                    UCTScoreGroupViewer ucScoreGroup = new UCTScoreGroupViewer();
                    ucScoreGroup.ScoreGroup = item as ScoreGroup;
                    GridItems.RowDefinitions.Add(new RowDefinition());
                    GridItems.Children.Add(ucScoreGroup);
                    ucScoreGroup.SetValue(Grid.RowProperty, i);
                    ucScoreGroup.Init();
                }
                else
                {
                    UCTStandardViewer ucStandard = new UCTStandardViewer();
                    ucStandard.Standard = item as Standard;
                    GridItems.RowDefinitions.Add(new RowDefinition());
                    GridItems.Children.Add(ucStandard);
                    ucStandard.SetValue(Grid.RowProperty, i);
                    ucStandard.Init();

                }
            }
            ScoreObject_OnPropertyChanged(this, null);
        }
    }
}
