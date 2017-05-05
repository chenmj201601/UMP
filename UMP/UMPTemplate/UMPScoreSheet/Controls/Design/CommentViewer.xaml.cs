//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    305910b3-a043-4ddf-9450-4a051f781172
//        CLR Version:              4.0.30319.18444
//        Name:                     CommentViewer
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets.Controls.Design
//        File Name:                CommentViewer
//
//        created by Charley at 2014/6/25 15:28:57
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
    /// CommentViewer.xaml 的交互逻辑
    /// </summary>
    public partial class CommentViewer : IScoreObjectViewer
    {
        /// <summary>
        /// ViewerLoaded
        /// </summary>
        public event Action ViewerLoaded;
        /// <summary>
        /// Comment
        /// </summary>
        public Comment Comment;
        /// <summary>
        /// CommentViewer
        /// </summary>
        public CommentViewer()
        {
            InitializeComponent();
        }

        private void CommentViewer_OnLoaded(object sender, RoutedEventArgs e)
        {
            DataContext = Comment;
            Init();
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
            if (Comment == null) { return; }
            switch (Comment.Style)
            {
                case CommentStyle.Text:
                    TextBoxCommentValueViewer textBoxCommentValueViewer = new TextBoxCommentValueViewer();
                    textBoxCommentValueViewer.TextComment = Comment as TextComment;
                    textBoxCommentValueViewer.ViewClassic = ViewClassic;
                    textBoxCommentValueViewer.Settings = Settings;
                    BorderValue.Child = textBoxCommentValueViewer;
                    break;
                case CommentStyle.Item:
                    DropDownItemCommentValueViewer dropDownItemCommentValueViewer = new DropDownItemCommentValueViewer();
                    dropDownItemCommentValueViewer.ItemComment = Comment as ItemComment;
                    dropDownItemCommentValueViewer.ViewClassic = ViewClassic;
                    dropDownItemCommentValueViewer.Settings = Settings;
                    BorderValue.Child = dropDownItemCommentValueViewer;
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
