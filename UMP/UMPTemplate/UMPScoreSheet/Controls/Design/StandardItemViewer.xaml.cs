//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    1e53c84c-5612-412d-bed8-16b4b7bd1a00
//        CLR Version:              4.0.30319.18444
//        Name:                     StandardItemViewer
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets.Controls.Design
//        File Name:                StandardItemViewer
//
//        created by Charley at 2014/6/20 9:57:16
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
    /// StandardItemViewer.xaml 的交互逻辑
    /// </summary>
    public partial class StandardItemViewer : IScoreObjectViewer
    {
        /// <summary>
        /// ViewerLoaded
        /// </summary>
        public event Action ViewerLoaded;
        /// <summary>
        /// StandardItem
        /// </summary>
        public StandardItem StandardItem;
        /// <summary>
        /// StandardItemViewer
        /// </summary>
        public StandardItemViewer()
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
            
        }

        private void StandardItemViewer_OnLoaded(object sender, RoutedEventArgs e)
        {
            DataContext = StandardItem;

            Init();
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
