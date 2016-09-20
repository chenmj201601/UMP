//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    9244d53c-a993-4508-b7af-33f6d3064645
//        CLR Version:              4.0.30319.18444
//        Name:                     CommentPreviewer
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets.Controls
//        File Name:                CommentPreviewer
//
//        created by Charley at 2014/8/6 14:49:44
//        http://www.voicecyber.com 
//
//======================================================================

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VoiceCyber.UMP.ScoreSheets.Controls
{
    /// <summary>
    /// 
    /// </summary>
    [TemplatePart(Name = PART_Panel, Type = typeof(Border))]
    [TemplatePart(Name = PART_Title, Type = typeof(TextBlock))]
    [TemplatePart(Name = PART_Value, Type = typeof(Border))]
    public class CommentPreviewer : ScoreObjectPreViewer
    {
        public static readonly DependencyProperty CommentProperty =
            DependencyProperty.Register("Comment", typeof(Comment), typeof(CommentPreviewer), new PropertyMetadata(default(Comment)));

        public Comment Comment
        {
            get { return (Comment)GetValue(CommentProperty); }
            set { SetValue(CommentProperty, value); }
        }
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(CommentPreviewer), new PropertyMetadata(default(string)));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        static CommentPreviewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CommentPreviewer),
                new FrameworkPropertyMetadata(typeof(CommentPreviewer)));
        }

        public override void Init()
        {
            base.Init();
            if (Comment != null)
            {
                Comment.OnPropertyChanged += Comment_OnPropertyChanged;
            }
            SetCommentTitle();
            CreateCommentValue();
            SetTitleStyle();
            SetPanelStyle();
        }

        void Comment_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SetCommentTitle();
            SetTitleStyle();
            SetPanelStyle();
        }

        private void CreateCommentValue()
        {
            if (Comment != null)
            {
                switch (Comment.Style)
                {
                    case CommentStyle.Text:
                        TextBoxCommentValuePreviewer textCommentPreviewer = new TextBoxCommentValuePreviewer();
                        textCommentPreviewer.TextComment = Comment as TextComment;
                        textCommentPreviewer.Settings = Settings;
                        textCommentPreviewer.Languages = Languages;
                        textCommentPreviewer.LangID = LangID;
                        textCommentPreviewer.ViewClassic = ViewClassic;
                        if (mValue != null)
                        {
                            mValue.Child = textCommentPreviewer;
                        }
                        break;
                    case CommentStyle.Item:
                        ItemCommentValuePreviewer itemCommentPreviewer = new ItemCommentValuePreviewer();
                        itemCommentPreviewer.ItemComment = Comment as ItemComment;
                        itemCommentPreviewer.Settings = Settings;
                        itemCommentPreviewer.Languages = Languages;
                        itemCommentPreviewer.LangID = LangID;
                        itemCommentPreviewer.ViewClassic = ViewClassic;
                        if (mValue != null)
                        {
                            mValue.Child = itemCommentPreviewer;
                        }
                        break;
                }
            }
        }

        private void SetCommentTitle()
        {
            if (Comment != null)
            {
                Title = Comment.Title;
            }
        }

        private const string PART_Value = "PART_Value";
        private const string PART_Title = "PART_Title";
        private const string PART_Panel = "PART_Panel";

        private Border mValue;
        private TextBlock mTextTitle;
        private Border mBorderPanel;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            mBorderPanel = GetTemplateChild(PART_Panel) as Border;
            if (mBorderPanel != null)
            {

            }
            mValue = GetTemplateChild(PART_Value) as Border;
            if (mValue != null)
            {
                //默认状态下，是不显示备注的，所以OnApplyTemplate不会执行，这时Init就引用不到mValue，所以在这里调用一下Init
                Init();
            }
            mTextTitle = GetTemplateChild(PART_Title) as TextBlock;
            if (mTextTitle != null)
            {
                Init();
            }
        }

        private void SetTitleStyle()
        {
            if (Comment != null)
            {
                VisualStyle style = Comment.TitleStyle;
                if (style != null && mTextTitle != null)
                {
                    mTextTitle.Foreground = new SolidColorBrush(style.ForeColor);
                    if (style.FontFamily != null)
                    {
                        mTextTitle.FontFamily = style.FontFamily;
                    }
                    if (style.FontSize != 0)
                    {
                        mTextTitle.FontSize = style.FontSize;
                    }
                    mTextTitle.FontWeight = style.FontWeight;
                }
            }
        }

        private void SetPanelStyle()
        {
            if (Comment != null)
            {
                VisualStyle style = Comment.PanelStyle;
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
