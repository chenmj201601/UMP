//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    f141e127-810b-48f0-ae8c-dceeddf63b5c
//        CLR Version:              4.0.30319.18063
//        Name:                     ScoreSheetCommentViewer
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets.Controls
//        File Name:                ScoreSheetCommentViewer
//
//        created by Charley at 2015/10/28 16:14:55
//        http://www.voicecyber.com 
//
//======================================================================

using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VoiceCyber.UMP.ScoreSheets.Controls
{
    [TemplatePart(Name = PART_Panel, Type = typeof(Border))]
    [TemplatePart(Name = PART_Flags, Type = typeof(Panel))]
    [TemplatePart(Name = PART_Comment, Type = typeof(Panel))]
    public class ScoreSheetCommentViewer : ScoreObjectViewer
    {
        private const string PART_Flags = "PART_Flags";
        private const string PART_Comment = "PART_Comment";
        private const string PART_Panel = "PART_Panel";

        private Panel mPanelFlags;
        private Panel mPanelComments;
        private Border mBorderPanel;

        public static readonly DependencyProperty ScoreItemProperty =
            DependencyProperty.Register("ScoreItem", typeof (ScoreItem), typeof (ScoreSheetCommentViewer), new PropertyMetadata(default(ScoreItem)));

        public ScoreItem ScoreItem
        {
            get { return (ScoreItem) GetValue(ScoreItemProperty); }
            set { SetValue(ScoreItemProperty, value); }
        }

        public static readonly DependencyProperty ShowCommentProperty =
            DependencyProperty.Register("ShowComment", typeof (bool), typeof (ScoreSheetCommentViewer), new PropertyMetadata(default(bool)));

        public bool ShowComment
        {
            get { return (bool) GetValue(ShowCommentProperty); }
            set { SetValue(ShowCommentProperty, value); }
        }

        static ScoreSheetCommentViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (ScoreSheetCommentViewer),
                new FrameworkPropertyMetadata(typeof (ScoreSheetCommentViewer)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            mPanelFlags = GetTemplateChild(PART_Flags) as Panel;
            if (mPanelFlags != null)
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
        }

        public override void Init()
        {
            base.Init();
            if (ScoreItem != null)
            {
                ScoreItem.OnPropertyChanged += ScoreItem_OnPropertyChanged;
            }

            CreateFlagIcons();
            CreateComments();
            SetPanelStyle();
        }

        void ScoreItem_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            CreateFlagIcons();
            if (e.PropertyName == "Comments")
            {
                CreateComments();
            }
            SetPanelStyle();
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

        private void CreateFlagIcons()
        {
            if (ScoreItem == null) { return; }
            if (mPanelFlags == null) { return; }
            mPanelFlags.Children.Clear();
            IconButton iconItem;
            ScoreSetting setting;
            ScoreLangauge langauge;
            //备注
            if (ScoreItem.Comments.Count > 0)
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
                case "BtnComments":
                    ShowComment = !ShowComment;
                    break;
            }
        }
    }
}
