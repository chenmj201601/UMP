//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    2db468d8-b68e-4d15-a645-8c08c8025866
//        CLR Version:              4.0.30319.18444
//        Name:                     DropDownItemCommentValueViewer
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets.Controls.Design
//        File Name:                DropDownItemCommentValueViewer
//
//        created by Charley at 2014/6/25 16:28:13
//        http://www.voicecyber.com
//
//======================================================================

using System;
using System.Collections.Generic;
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

namespace VoiceCyber.UMP.ScoreSheets.Controls.Design
{
    /// <summary>
    /// DropDownItemCommentValueViewer.xaml 的交互逻辑
    /// </summary>
    public partial class DropDownItemCommentValueViewer : IScoreObjectViewer
    {
        /// <summary>
        /// ViewerLoaded
        /// </summary>
        public event Action ViewerLoaded;
        /// <summary>
        /// ItemComment
        /// </summary>
        public ItemComment ItemComment;
        /// <summary>
        /// DropDownItemCommentValueViewer
        /// </summary>
        public DropDownItemCommentValueViewer()
        {
            InitializeComponent();
        }

        private void DropDownItemCommentValueViewer_OnLoaded(object sender, RoutedEventArgs e)
        {
            DataContext = ItemComment;

            Init();
        }
        /// <summary>
        /// 
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
        /// 
        /// </summary>
        public void Init()
        {
            if (ItemComment == null) { return; }

            ComboCommentValue.ItemsSource = ItemComment.ValueItems;
            if (ItemComment.SelectValue != null)
            {
                CommentItem temp = ItemComment.ValueItems.FirstOrDefault(i => i.ID == ItemComment.SelectValue.ID);
                if (temp != null)
                {
                    ComboCommentValue.SelectedItem = temp;
                }
            }
        }

        private void ComboCommentValue_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectItem = ComboCommentValue.SelectedItem as CommentItem;
            if (selectItem != null)
            {
                ItemComment.SelectValue = selectItem;
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
