//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    0781eccc-ca3d-4edd-8420-1f6e4cbaeeca
//        CLR Version:              4.0.30319.18444
//        Name:                     ScoreItemTitleViewer
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets.Controls
//        File Name:                ScoreItemTitleViewer
//
//        created by Charley at 2014/8/12 10:21:07
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;

namespace VoiceCyber.UMP.ScoreSheets.Controls
{
    [TemplatePart(Name = PART_Panel, Type = typeof(Border))]
    [TemplatePart(Name = PART_Flags, Type = typeof(Panel))]
    [TemplatePart(Name = PART_Description, Type = typeof(Popup))]
    [TemplatePart(Name = PART_Comment, Type = typeof(Panel))]
    [TemplatePart(Name = PART_Title, Type = typeof(TextBlock))]
    public class ScoreItemTitleViewer : ScoreObjectViewer
    {
        private const string PART_Flags = "PART_Flags";
        private const string PART_Description = "PART_Description";
        private const string PART_Comment = "PART_Comment";
        private const string PART_Panel = "PART_Panel";
        private const string PART_Title = "PART_Title";

        private Panel mPanelFlags;
        private Popup mPopDescription;
        private Panel mPanelComments;
        private Border mBorderPanel;
        private TextBlock mTextTitle;

        public static readonly DependencyProperty ScoreItemProperty =
            DependencyProperty.Register("ScoreItem", typeof(ScoreItem), typeof(ScoreItemTitleViewer), new PropertyMetadata(default(ScoreItem)));

        public ScoreItem ScoreItem
        {
            get { return (ScoreItem)GetValue(ScoreItemProperty); }
            set { SetValue(ScoreItemProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(ScoreItemTitleViewer), new PropertyMetadata(default(string)));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(ScoreItemTitleViewer), new PropertyMetadata(default(string)));

        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        public static readonly DependencyProperty ShowCommentProperty =
            DependencyProperty.Register("ShowComment", typeof(bool), typeof(ScoreItemTitleViewer), new PropertyMetadata(default(bool)));

        public bool ShowComment
        {
            get { return (bool)GetValue(ShowCommentProperty); }
            set { SetValue(ShowCommentProperty, value); }
        }

        static ScoreItemTitleViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ScoreItemTitleViewer),
                new FrameworkPropertyMetadata(typeof(ScoreItemTitleViewer)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            mPanelFlags = GetTemplateChild(PART_Flags) as Panel;
            if (mPanelFlags != null)
            {

            }
            mPopDescription = GetTemplateChild(PART_Description) as Popup;
            if (mPopDescription != null)
            {

            }
            mPanelComments = GetTemplateChild(PART_Comment) as Panel;
            if (mPanelComments != null)
            {

            }
            mBorderPanel = GetTemplateChild(PART_Panel) as Border;
            if (mBorderPanel != null)
            {

            }
            mTextTitle = GetTemplateChild(PART_Title) as TextBlock;
            if (mTextTitle != null)
            {

            }
        }

        public override void Init()
        {
            base.Init();
            if (ScoreItem != null)
            {
                ScoreItem.OnPropertyChanged += ScoreItem_OnPropertyChanged;
            }

            SetTitleInfo();
            CreateFlagIcons();
            CreateComments();
            SetPanelStyle();
            SetTitleStyle();
        }

        void ScoreItem_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SetTitleInfo();
            CreateFlagIcons();
            if (e.PropertyName == "Comments")
            {
                CreateComments();
            }
            SetPanelStyle();
            SetTitleStyle();
        }

        private void SetTitleInfo()
        {
            if (ScoreItem != null)
            {
                Title = ScoreItem.Title;
                Description = ScoreItem.Description;
            }
        }

        private void SetPanelStyle()
        {
            if (ScoreItem != null)
            {
                VisualStyle style = ScoreItem.TitleStyle;
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

        private void SetTitleStyle()
        {
            if (ScoreItem != null)
            {
                VisualStyle style = ScoreItem.TitleStyle;
                if (style != null && mTextTitle != null)
                {
                    mTextTitle.Foreground = new SolidColorBrush(style.ForeColor);
                    if (style.FontFamily != null)
                    {
                        mTextTitle.FontFamily = style.FontFamily;
                    }
                    if (style.FontSize > 0)
                    {
                        mTextTitle.FontSize = style.FontSize;
                    }
                    mTextTitle.FontWeight = style.FontWeight;
                }
            }
        }

        private void CreateFlagIcons()
        {
            if (ScoreItem == null) { return; }
            if (mPanelFlags == null) { return; }
            mPanelFlags.Children.Clear();
            IconButton iconItem;
            ScoreSetting setting;
            ScoreLangauge langauge;
            //描述
            if (!string.IsNullOrEmpty(ScoreItem.Description))
            {
                iconItem = new IconButton();
                iconItem.Name = "BtnDescription";
                iconItem.Display = "Description";
                iconItem.ToolTip = "Show or Close Description";
                if (Languages != null)
                {
                    langauge =
                   Languages.FirstOrDefault(l => l.LangID == LangID && l.Category == "ScoreViewer" && l.Code == "T102");
                    if (langauge != null)
                    {
                        iconItem.ToolTip = langauge.Display;
                    }
                }
                iconItem.IconPath = "/UMPScoreSheet;component/Themes/Images/info.png";
                if (Settings != null)
                {
                    setting = Settings.FirstOrDefault(s => s.Category == "I" && s.Code == "I_DESCRIPTION");
                    if (setting != null)
                    {
                        iconItem.IconPath = setting.Value;
                    }
                }
                iconItem.Click += IconButton_Click;
                mPanelFlags.Children.Add(iconItem);
            }
            //备注
            if (ScoreItem.Comments.Count > 0
                && !(ScoreItem is ScoreSheet))
            {
                iconItem = new IconButton();
                iconItem.Name = "BtnComments";
                iconItem.Display = "Comments";
                iconItem.ToolTip = "Show or Close Comments";
                if (Languages != null)
                {
                    langauge =
                   Languages.FirstOrDefault(l => l.LangID == LangID && l.Category == "ScoreViewer" && l.Code == "T103");
                    if (langauge != null)
                    {
                        iconItem.ToolTip = langauge.Display;
                    }
                }
                iconItem.IconPath = "/UMPScoreSheet;component/Themes/Images/showcomment.png";
                if (Settings != null)
                {
                    setting = Settings.FirstOrDefault(s => s.Category == "I" && s.Code == "I_COMMENT_ITEM");
                    if (setting != null)
                    {
                        iconItem.IconPath = setting.Value;
                    }
                }
                iconItem.Click += IconButton_Click;
                mPanelFlags.Children.Add(iconItem);
            }
            //关键项
            if (ScoreItem.IsKeyItem)
            {
                iconItem = new IconButton();
                iconItem.Name = "BtnKeyItem";
                iconItem.Display = "KeyItem";
                iconItem.ToolTip = "Key Item";
                if (Languages != null)
                {
                    langauge =
                   Languages.FirstOrDefault(l => l.LangID == LangID && l.Category == "ScoreViewer" && l.Code == "T104");
                    if (langauge != null)
                    {
                        iconItem.ToolTip = langauge.Display;
                    }
                }
                iconItem.IconPath = "/UMPScoreSheet;component/Themes/Images/keyitem.png";
                if (Settings != null)
                {
                    setting = Settings.FirstOrDefault(s => s.Category == "I" && s.Code == "I_KEY_ITEM");
                    if (setting != null)
                    {
                        iconItem.IconPath = setting.Value;
                    }
                }
                iconItem.Click += IconButton_Click;
                mPanelFlags.Children.Add(iconItem);
            }
            //附加项
            if (ScoreItem.IsAddtionItem)
            {
                iconItem = new IconButton();
                iconItem.Name = "BtnAdditionItem";
                iconItem.Display = "AdditionItem";
                iconItem.ToolTip = "Addition Item";
                if (Languages != null)
                {
                    langauge =
                   Languages.FirstOrDefault(l => l.LangID == LangID && l.Category == "ScoreViewer" && l.Code == "T105");
                    if (langauge != null)
                    {
                        iconItem.ToolTip = langauge.Display;
                    }
                }
                iconItem.IconPath = "/UMPScoreSheet;component/Themes/Images/additionalitem.png";
                if (Settings != null)
                {
                    setting = Settings.FirstOrDefault(s => s.Category == "I" && s.Code == "I_ADD_ITEM");
                    if (setting != null)
                    {
                        iconItem.IconPath = setting.Value;
                    }
                }
                iconItem.Click += IconButton_Click;
                mPanelFlags.Children.Add(iconItem);
            }
            //自动评分项
            var standard = ScoreItem as Standard;
            if (standard != null && standard.IsAutoStandard)
            {
                iconItem = new IconButton();
                iconItem.Name = "BtnAutoStandardItem";
                iconItem.Display = "AutoStandardItem";
                iconItem.ToolTip = "AutoStandard Item";
                if (Languages != null)
                {
                    langauge =
                   Languages.FirstOrDefault(l => l.LangID == LangID && l.Category == "ScoreViewer" && l.Code == "T109");
                    if (langauge != null)
                    {
                        iconItem.ToolTip = langauge.Display;
                    }
                }
                iconItem.IconPath = "/UMPScoreSheet;component/Themes/Images/autostandard.png";
                if (Settings != null)
                {
                    setting = Settings.FirstOrDefault(s => s.Category == "I" && s.Code == "I_AUTO_ITEM");
                    if (setting != null)
                    {
                        iconItem.IconPath = setting.Value;
                    }
                }
                iconItem.Click += IconButton_Click;
                mPanelFlags.Children.Add(iconItem);
            }
            var controlFlag = ScoreItem.ControlFlag;
            //控制源
            if ((controlFlag & 1) != 0)
            {
                iconItem = new IconButton();
                iconItem.Name = "BtnControlSource";
                iconItem.Display = "ControlSource";
                iconItem.ToolTip = "Control Source Item";
                if (Languages != null)
                {
                    langauge =
                  Languages.FirstOrDefault(l => l.LangID == LangID && l.Category == "ScoreViewer" && l.Code == "T106");
                    if (langauge != null)
                    {
                        iconItem.ToolTip = langauge.Display;
                    }
                }
                List<ControlItem> listControlItems = new List<ControlItem>();
                ScoreItem.GetControlScoreItem(ref listControlItems);
                if (listControlItems.Count > 0)
                {
                    string strToolTip = string.Empty;
                    for (int i = 0; i < listControlItems.Count; i++)
                    {
                        var item = listControlItems[i];
                        strToolTip += string.Format("\r\n({0}) {1}",i, item.GetControlInfo());
                    }
                    iconItem.ToolTip += strToolTip;
                }
                iconItem.IconPath = "/UMPScoreSheet;component/Themes/Images/controlitem.png";
                if (Settings != null)
                {
                    setting = Settings.FirstOrDefault(s => s.Category == "I" && s.Code == "I_CTL_SRC");
                    if (setting != null)
                    {
                        iconItem.IconPath = setting.Value;
                    }
                }
                iconItem.Click += IconButton_Click;
                mPanelFlags.Children.Add(iconItem);
            }
            //控制目标
            if ((controlFlag & 2) != 0)
            {
                iconItem = new IconButton();
                iconItem.Name = "BtnControlTarget";
                iconItem.Display = "ControlTarget";
                iconItem.ToolTip = "Control Target Item";
                if (Languages != null)
                {
                    langauge =
                   Languages.FirstOrDefault(l => l.LangID == LangID && l.Category == "ScoreViewer" && l.Code == "T107");
                    if (langauge != null)
                    {
                        iconItem.ToolTip = langauge.Display;
                    }
                }
                List<ControlItem> listControledItems = new List<ControlItem>();
                ScoreItem.GetControledScoreItem(ref listControledItems);
                if (listControledItems.Count > 0)
                {
                    string strToolTip = string.Empty;
                    for (int i = 0; i < listControledItems.Count; i++)
                    {
                        var item = listControledItems[i];
                        strToolTip += string.Format("\r\n({0}) {1}", i, item.GetControlInfo());
                    }
                    iconItem.ToolTip += strToolTip;
                }
                iconItem.IconPath = "/UMPScoreSheet;component/Themes/Images/controltarget.png";
                if (Settings != null)
                {
                    setting = Settings.FirstOrDefault(s => s.Category == "I" && s.Code == "I_CTL_TGT");
                    if (setting != null)
                    {
                        iconItem.IconPath = setting.Value;
                    }
                }
                iconItem.Click += IconButton_Click;
                mPanelFlags.Children.Add(iconItem);
            }
        }

        private void CreateComments()
        {
            if (ScoreItem != null && mPanelComments != null)
            {
                mPanelComments.Children.Clear();
                for (int i = 0; i < ScoreItem.Comments.Count; i++)
                {
                    CommentViewer commentViewer = new CommentViewer();
                    commentViewer.Comment = ScoreItem.Comments[i];
                    commentViewer.Settings = Settings;
                    commentViewer.Languages = Languages;
                    commentViewer.LangID = LangID;
                    commentViewer.ViewClassic = ViewClassic;
                    commentViewer.ViewMode = ViewMode;
                    if (i > 0)
                    {
                        commentViewer.BorderThickness = new Thickness(0, 1, 0, 0);
                        commentViewer.BorderBrush = Brushes.LightGray;
                    }
                    mPanelComments.Children.Add(commentViewer);
                }
            }
        }

        private void IconButton_Click(object sender, RoutedEventArgs e)
        {
            var iconBtn = e.Source as IconButton;
            if (iconBtn == null) { return; }
            switch (iconBtn.Name)
            {
                case "BtnDescription":
                    mPopDescription.IsOpen = true;
                    break;
                case "BtnComments":
                    ShowComment = !ShowComment;
                    break;
            }
        }

    }
}
