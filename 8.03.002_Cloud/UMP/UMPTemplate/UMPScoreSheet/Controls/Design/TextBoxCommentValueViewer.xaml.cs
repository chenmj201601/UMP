//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    ee183822-6b68-472a-93b7-d75c0261641b
//        CLR Version:              4.0.30319.18444
//        Name:                     TextBoxCommentValueViewer
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets.Controls.Design
//        File Name:                TextBoxCommentValueViewer
//
//        created by Charley at 2014/6/25 15:32:53
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
    /// TextBoxCommentValueViewer.xaml 的交互逻辑
    /// </summary>
    public partial class TextBoxCommentValueViewer : IScoreObjectViewer
    {
        /// <summary>
        /// ViewerLoaded
        /// </summary>
        public event Action ViewerLoaded;
        /// <summary>
        /// TextComment
        /// </summary>
        public TextComment TextComment;
        /// <summary>
        /// TextBoxCommentValueViewer
        /// </summary>
        public TextBoxCommentValueViewer()
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
            TxtComment.Text = TextComment.Text;
        }

        private void TextBoxCommentValueViewer_OnLoaded(object sender, RoutedEventArgs e)
        {
            DataContext = TextComment;
            Init();
        }

        private void TxtComment_OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (TextComment != null)
            {
                TextComment.Text = TxtComment.Text;
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
