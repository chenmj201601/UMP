//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    495070fd-b63f-48c1-a304-2d2cc79da6ad
//        CLR Version:              4.0.30319.18444
//        Name:                     ScoreItemTitleViewer
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets.Controls.Design
//        File Name:                ScoreItemTitleViewer
//
//        created by Charley at 2014/6/27 17:12:46
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
using VoiceCyber.UMP.ScoreSheets.Models;

namespace VoiceCyber.UMP.ScoreSheets.Controls.Design
{
    /// <summary>
    /// ScoreItemTitleViewer.xaml 的交互逻辑
    /// </summary>
    public partial class ScoreItemTitleViewer : IScoreObjectViewer
    {
        /// <summary>
        /// ViewerLoaded
        /// </summary>
        public event Action ViewerLoaded;
        /// <summary>
        /// ScoreItem
        /// </summary>
        public ScoreItem ScoreItem;
        /// <summary>
        /// ScoreItemTitleViewer
        /// </summary>
        public ScoreItemTitleViewer()
        {
            InitializeComponent();
        }
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
        /// Init
        /// </summary>
        public void Init()
        {
            CreateItemIcon();
            CreateFlagIcons();
            CreateComments();
            SetTitleStyle(ScoreItem.TitleStyle);
        }

        private void ScoreItemTitleViewer_OnLoaded(object sender, RoutedEventArgs e)
        {
            DataContext = ScoreItem;
            if (ScoreItem != null)
            {
                ScoreItem.OnPropertyChanged += ScoreItem_OnPropertyChanged;
            }
            LbTitle.MouseUp += LbTitle_MouseUp;
            Init();
        }

        void LbTitle_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //PopDescription.IsOpen = true;
        }

        void ScoreItem_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            CreateFlagIcons();
            SetTitleStyle(ScoreItem.TitleStyle);
        }

        private void CreateItemIcon()
        {
            if (ScoreItem == null) { return; }
            PanelIcon.Children.Clear();
            IconButtonItem iconItem;
            Button iconButton;
            string iconPath = string.Empty;
            ScoreSetting setting;
            if (string.IsNullOrEmpty(ScoreItem.IconName))
            {
                if (ScoreItem.Type == ScoreObjectType.ScoreGroup)
                {
                    setting = Settings.FirstOrDefault(s => s.Category == "I" && s.Code == "I_SCOREG");
                    if (setting != null)
                    {
                        iconPath = setting.Value;
                    }
                }
                else
                {
                    setting = Settings.FirstOrDefault(s => s.Category == "I" && s.Code == "I_STANDARD");
                    if (setting != null)
                    {
                        iconPath = setting.Value;
                    }
                }
            }
            else
            {
                setting = Settings.FirstOrDefault(s => s.Category == "I" && s.Code == ScoreItem.IconName);
                if (setting != null)
                {
                    iconPath = setting.Value;
                }
            }
            iconItem = new IconButtonItem();
            iconItem.Name = "Icon";
            iconItem.Display = "Icon";
            iconItem.ToolTip = ScoreItem.Description;
            iconItem.Icon = iconPath;

            iconButton = new Button();
            iconButton.DataContext = iconItem;
            iconButton.SetResourceReference(StyleProperty, "ButtonItemTag");
            iconButton.Click += IconButton_Click;
            PanelIcon.Children.Add(iconButton);
            setting = Settings.FirstOrDefault(s => s.Category == "S" && s.Code == "T_ICON_VIS");
            if (setting != null && setting.Value == "T")
            {
                PanelIcon.Visibility = Visibility.Visible;
            }
            else
            {
                PanelIcon.Visibility = Visibility.Collapsed;
            }
        }

        private void CreateFlagIcons()
        {
            if (ScoreItem == null) { return; }
            PanelFags.Children.Clear();
            IconButtonItem iconItem;
            Button iconButton;
            ScoreSetting setting;
            ScoreLangauge langauge;
            //描述
            if (!string.IsNullOrEmpty(ScoreItem.Description))
            {
                iconItem = new IconButtonItem();
                iconItem.Name = "Description";
                iconItem.Display = "Description";
                iconItem.ToolTip = "Show or Close Description";
                if (Languages != null)
                {
                    langauge =
                   Languages.FirstOrDefault(l => l.LangID == LangID && l.Category == "ScoreViewer" && l.Code == "T_ShowDescription");
                    if (langauge != null)
                    {
                        iconItem.ToolTip = langauge.Display;
                    }
                }
                iconItem.Icon = "/UMPScoreSheet;component/Themes/Default/Images/info.png";
                if (Settings != null)
                {
                    setting = Settings.FirstOrDefault(s => s.Category == "I" && s.Code == "I_DESCRIPTION");
                    if (setting != null)
                    {
                        iconItem.Icon = setting.Value;
                    }
                }

                iconButton = new Button();
                iconButton.DataContext = iconItem;
                iconButton.SetResourceReference(StyleProperty, "ButtonItemTag");
                iconButton.Click += IconButton_Click;
                PanelFags.Children.Add(iconButton);
            }
            //备注
            if (ScoreItem.Comments.Count > 0)
            {
                iconItem = new IconButtonItem();
                iconItem.Name = "Comments";
                iconItem.Display = "Comments";
                iconItem.ToolTip = "Show or Close Comments";
                if (Languages != null)
                {
                    langauge =
                   Languages.FirstOrDefault(l => l.LangID == LangID && l.Category == "ScoreViewer" && l.Code == "T_ShowComment");
                    if (langauge != null)
                    {
                        iconItem.ToolTip = langauge.Display;
                    }
                }
                iconItem.Icon = "/UMPScoreSheet;component/Themes/Default/Images/showcomment.png";
                if (Settings != null)
                {
                    setting = Settings.FirstOrDefault(s => s.Category == "I" && s.Code == "I_COMMENT_ITEM");
                    if (setting != null)
                    {
                        iconItem.Icon = setting.Value;
                    }
                }

                iconButton = new Button();
                iconButton.DataContext = iconItem;
                iconButton.SetResourceReference(StyleProperty, "ButtonItemTag");
                iconButton.Click += IconButton_Click;
                PanelFags.Children.Add(iconButton);
            }
            //关键项
            if (ScoreItem.IsKeyItem)
            {
                iconItem = new IconButtonItem();
                iconItem.Name = "KeyItem";
                iconItem.Display = "KeyItem";
                iconItem.ToolTip = "Key Item";
                if (Languages != null)
                {
                    langauge =
                   Languages.FirstOrDefault(l => l.LangID == LangID && l.Category == "ScoreViewer" && l.Code == "T_KeyItem");
                    if (langauge != null)
                    {
                        iconItem.ToolTip = langauge.Display;
                    }
                }
                iconItem.Icon = "/UMPScoreSheet;component/Themes/Default/Images/keyitem.png";
                if (Settings != null)
                {
                    setting = Settings.FirstOrDefault(s => s.Category == "I" && s.Code == "I_KEY_ITEM");
                    if (setting != null)
                    {
                        iconItem.Icon = setting.Value;
                    }
                }

                iconButton = new Button();
                iconButton.DataContext = iconItem;
                iconButton.SetResourceReference(StyleProperty, "ButtonItemTag");
                iconButton.Click += IconButton_Click;
                PanelFags.Children.Add(iconButton);
            }
            //附加项
            if (ScoreItem.IsAddtionItem)
            {
                iconItem = new IconButtonItem();
                iconItem.Name = "AdditionItem";
                iconItem.Display = "AdditionItem";
                iconItem.ToolTip = "Addition Item";
                if (Languages != null)
                {
                    langauge =
                   Languages.FirstOrDefault(l => l.LangID == LangID && l.Category == "ScoreViewer" && l.Code == "T_AddItem");
                    if (langauge != null)
                    {
                        iconItem.ToolTip = langauge.Display;
                    }
                }
                iconItem.Icon = "/UMPScoreSheet;component/Themes/Default/Images/additionalitem.png";
                if (Settings != null)
                {
                    setting = Settings.FirstOrDefault(s => s.Category == "I" && s.Code == "I_ADD_ITEM");
                    if (setting != null)
                    {
                        iconItem.Icon = setting.Value;
                    }
                }

                iconButton = new Button();
                iconButton.DataContext = iconItem;
                iconButton.SetResourceReference(StyleProperty, "ButtonItemTag");
                iconButton.Click += IconButton_Click;
                PanelFags.Children.Add(iconButton);
            }
            var controlFlag = ScoreItem.ControlFlag;
            //控制源
            if ((controlFlag & 1) != 0)
            {
                iconItem = new IconButtonItem();
                iconItem.Name = "ControlSource";
                iconItem.Display = "ControlSource";
                iconItem.ToolTip = "Control Source Item";
                if (Languages != null)
                {
                    langauge =
                  Languages.FirstOrDefault(l => l.LangID == LangID && l.Category == "ScoreViewer" && l.Code == "T_ControlSource");
                    if (langauge != null)
                    {
                        iconItem.ToolTip = langauge.Display;
                    }
                }
                iconItem.Icon = "/UMPScoreSheet;component/Themes/Default/Images/controlitem.png";
                if (Settings != null)
                {
                    setting = Settings.FirstOrDefault(s => s.Category == "I" && s.Code == "I_CTL_SRC");
                    if (setting != null)
                    {
                        iconItem.Icon = setting.Value;
                    }
                }

                iconButton = new Button();
                iconButton.DataContext = iconItem;
                iconButton.SetResourceReference(StyleProperty, "ButtonItemTag");
                iconButton.Click += IconButton_Click;
                PanelFags.Children.Add(iconButton);
            }
            //控制目标
            if ((controlFlag & 2) != 0)
            {
                iconItem = new IconButtonItem();
                iconItem.Name = "ControlTarget";
                iconItem.Display = "ControlTarget";
                iconItem.ToolTip = "Control Target Item";
                if (Languages != null)
                {
                    langauge =
                   Languages.FirstOrDefault(l => l.LangID == LangID && l.Category == "ScoreViewer" && l.Code == "T_ControlTarget");
                    if (langauge != null)
                    {
                        iconItem.ToolTip = langauge.Display;
                    }
                }
                iconItem.Icon = "/UMPScoreSheet;component/Themes/Default/Images/controltarget.png";
                if (Settings != null)
                {
                    setting = Settings.FirstOrDefault(s => s.Category == "I" && s.Code == "I_CTL_TGT");
                    if (setting != null)
                    {
                        iconItem.Icon = setting.Value;
                    }
                }

                iconButton = new Button();
                iconButton.DataContext = iconItem;
                iconButton.SetResourceReference(StyleProperty, "ButtonItemTag");
                iconButton.Click += IconButton_Click;
                PanelFags.Children.Add(iconButton);
            }
        }

        private void CreateComments()
        {
            if (ScoreItem == null) { return; }
            PanelComments.Children.Clear();
            for (int i = 0; i < ScoreItem.Comments.Count; i++)
            {
                Comment comment = ScoreItem.Comments[i];
                CommentViewer commentViewer = new CommentViewer();
                commentViewer.Comment = comment;
                commentViewer.ViewClassic = ViewClassic;
                PanelComments.Children.Add(commentViewer);
            }
        }

        private void SetTitleStyle(VisualStyle style)
        {
            if (ViewClassic == ScoreItemClassic.Table)
            {
                PanelTitle.HorizontalAlignment = HorizontalAlignment.Center;
            }
            else
            {
                ScoreSheet scoreSheet = ScoreItem as ScoreSheet;
                if (scoreSheet != null)
                {
                    PanelTitle.HorizontalAlignment = HorizontalAlignment.Center;
                }
                else
                {
                    PanelTitle.HorizontalAlignment = HorizontalAlignment.Left;
                }
            }
            if (style != null)
            {
                LbTitle.Foreground = new SolidColorBrush(style.ForeColor);
                BorderPanel.Background = new SolidColorBrush(style.BackColor);
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
        }

        private void IconButton_Click(object sender, RoutedEventArgs e)
        {
            var button = e.Source as Button;
            if (button == null) { return; }
            var iconBtn = button.DataContext as IconButtonItem;
            if (iconBtn == null) { return; }
            switch (iconBtn.Name)
            {
                //case "Icon":
                //    PopDescription.IsOpen = true;
                //    break;
                case "Comments":
                    if (BorderComment.Visibility == Visibility.Visible)
                    {
                        BorderComment.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        BorderComment.Visibility = Visibility.Visible;
                    }
                    break;
                case "Description":
                    PopDescription.IsOpen = true;
                    break;
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
