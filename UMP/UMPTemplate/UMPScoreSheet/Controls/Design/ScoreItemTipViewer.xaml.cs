//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    ac2d45ff-2050-4f37-b4cf-716c556a24bf
//        CLR Version:              4.0.30319.18444
//        Name:                     StandardTipViewer
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets.Controls.Design
//        File Name:                StandardTipViewer
//
//        created by Charley at 2014/6/30 11:46:00
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
    /// StandardTipViewer.xaml 的交互逻辑
    /// </summary>
    public partial class ScoreItemTipViewer : IScoreObjectViewer
    {
        /// <summary>
        /// ViewerLoaded
        /// </summary>
        public event Action ViewerLoaded;
        /// <summary>
        /// Standard
        /// </summary>
        public ScoreItem ScoreItem;
        /// <summary>
        /// StandardTipViewer
        /// </summary>
        public ScoreItemTipViewer()
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
            SetTipValue();
        }

        private void ScoreItemTipViewer_OnLoaded(object sender, RoutedEventArgs e)
        {
            DataContext = ScoreItem;
            if (ScoreItem != null)
            {
                ScoreItem.OnPropertyChanged += Standard_OnPropertyChanged;
            }
            Init();
        }

        void Standard_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SetTipValue();
        }

        private void SetTipValue()
        {
            Standard standard = ScoreItem as Standard;
            if (standard != null)
            {
                ScoreSheet scoreSheet = standard.ScoreSheet;
                if (scoreSheet != null)
                {
                    if (scoreSheet.UsePointSystem)
                    {
                        LbTip.Content = string.Format("{0}/{1}", standard.Score, standard.PointSystem);
                    }
                    else
                    {
                        LbTip.Content = string.Format("{0}/{1}", standard.Score, standard.TotalScore);
                    }
                }
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
