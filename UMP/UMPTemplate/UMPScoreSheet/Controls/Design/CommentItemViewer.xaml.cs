//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    1986c87c-f4c5-449c-ad76-35f1eb9f6e03
//        CLR Version:              4.0.30319.18444
//        Name:                     CommentItemViewer
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets.Controls.Design
//        File Name:                CommentItemViewer
//
//        created by Charley at 2014/6/25 16:08:39
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
    /// CommentItemViewer.xaml 的交互逻辑
    /// </summary>
    public partial class CommentItemViewer : IScoreObjectViewer
    {
        /// <summary>
        /// ViewerLoaded
        /// </summary>
        public event Action ViewerLoaded;
        /// <summary>
        /// CommentItem
        /// </summary>
        public CommentItem CommentItem;
        /// <summary>
        /// CommentItemViewer
        /// </summary>
        public CommentItemViewer()
        {
            InitializeComponent();
        }

        private void CommentItemViewer_OnLoaded(object sender, RoutedEventArgs e)
        {
            DataContext = CommentItem;
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
