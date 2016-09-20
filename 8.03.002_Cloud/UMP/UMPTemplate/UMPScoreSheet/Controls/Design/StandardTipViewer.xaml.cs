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
    public partial class StandardTipViewer : IScoreObjectViewer
    {
        /// <summary>
        /// ViewerLoaded
        /// </summary>
        public event Action ViewerLoaded;
        /// <summary>
        /// Standard
        /// </summary>
        public Standard Standard;
        /// <summary>
        /// StandardTipViewer
        /// </summary>
        public StandardTipViewer()
        {
            InitializeComponent();
        }
        /// <summary>
        /// ViewClassic
        /// </summary>
        public ScoreItemClassic ViewClassic { get; set; }
        /// <summary>
        /// Init
        /// </summary>
        public void Init()
        {
            SetTipValue();
        }

        private void StandardTipViewer_OnLoaded(object sender, RoutedEventArgs e)
        {
            DataContext = Standard;
            if (Standard != null)
            {
                Standard.OnPropertyChanged += Standard_OnPropertyChanged;
            }
            Init();
        }

        void Standard_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SetTipValue();
        }

        private void SetTipValue()
        {
            if (Standard == null) { return; }
            double score = Standard.Score * Standard.PointSystem;
            double total = Standard.TotalScore;
            double point = Standard.PointSystem;
            if (point == 1)
            {
                LbTip.Content = string.Format("{0}/{1}", score, total);
            }
            else
            {
                LbTip.Content = string.Format("{0}/{1}/{2}", score, total, point);
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
