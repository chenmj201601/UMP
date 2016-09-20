//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    11c23f2d-972a-4cea-b4c6-b74b9826d90f
//        CLR Version:              4.0.30319.18444
//        Name:                     TextBoxStandardValueViewer
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets.Controls.Design
//        File Name:                TextBoxStandardValueViewer
//
//        created by Charley at 2014/7/23 17:27:58
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
    /// TextBoxStandardValueViewer.xaml 的交互逻辑
    /// </summary>
    public partial class TextBoxStandardValueViewer : IScoreObjectViewer
    {
        /// <summary>
        /// ViewerLoaded
        /// </summary>
        public event Action ViewerLoaded;
        /// <summary>
        /// NumericStandard
        /// </summary>
        public NumericStandard NumericStandard;
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
        /// New TextBoxStandardValueViewer
        /// </summary>
        public TextBoxStandardValueViewer()
        {
            InitializeComponent();
        }

        private void TextBoxStandardValueViewer_OnLoaded(object sender, RoutedEventArgs e)
        {
            DataContext = NumericStandard;
            Init();
        }
        /// <summary>
        /// Init
        /// </summary>
        public void Init()
        {
            if (NumericStandard != null)
            {
                TxtScore.Value = NumericStandard.Score;
            }
        }

        private void TxtScore_OnLostFocus(object sender, RoutedEventArgs e)
        {

            if (NumericStandard != null)
            {
                string invalidMsg = "Input Invalid";
                string title = "UMP ScoreSheet Designer";
                if (Languages != null)
                {
                    ScoreLangauge lang =
                        Languages.FirstOrDefault(l => l.LangID == LangID && l.Category == "ScoreViewer" && l.Code == "T_AppName");
                    if (lang != null)
                    {
                        title = lang.Display;
                    }
                    lang =
                        Languages.FirstOrDefault(l => l.LangID == LangID && l.Category == "ScoreViewer" && l.Code == "N_001");
                    if (lang != null)
                    {
                        invalidMsg = lang.Display;
                    }
                }
                double doubleValue;
                if (!double.TryParse(TxtScore.Text, out doubleValue))
                {
                    MessageBox.Show(invalidMsg, title, MessageBoxButton.OK, MessageBoxImage.Error);
                    TxtScore.Value = NumericStandard.Score;
                    return;
                }
                if (doubleValue > NumericStandard.MaxValue || doubleValue < NumericStandard.MinValue)
                {
                    MessageBox.Show(invalidMsg, title, MessageBoxButton.OK, MessageBoxImage.Error);
                    TxtScore.Value = NumericStandard.Score;
                    return;
                }
                NumericStandard.Score = doubleValue;
                PropertyChangedEventArgs args = new PropertyChangedEventArgs();
                args.ScoreObject = NumericStandard;
                args.PropertyName = "Score";
                NumericStandard.PropertyChanged(NumericStandard, args);
            }
        }

        private void SubViewerLoaded()
        {
            if (ViewerLoaded != null)
            {
                ViewerLoaded();
            }
        }

        private void TxtScore_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TxtScore_OnLostFocus(this, null);
        }
    }
}
