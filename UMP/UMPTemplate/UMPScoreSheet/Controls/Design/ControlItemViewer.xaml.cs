//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    e2d4edf2-0510-4cb8-915a-57d14d90ac01
//        CLR Version:              4.0.30319.18444
//        Name:                     ControlItemViewer
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets.Controls.Design
//        File Name:                ControlItemViewer
//
//        created by Charley at 2014/7/1 9:43:08
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
    /// ControlItemViewer.xaml 的交互逻辑
    /// </summary>
    public partial class ControlItemViewer : IScoreObjectViewer
    {
        /// <summary>
        /// ControlItem
        /// </summary>
        public ControlItem ControlItem;
        /// <summary>
        /// ControlItemViewer
        /// </summary>
        public ControlItemViewer()
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
            if (ControlItem == null) { return; }
            if (Languages == null) { return; }
            ScoreLangauge lang = Languages.FirstOrDefault(l => l.LangID == LangID && l.Category == "PropertyViewer" && l.Code == string.Format("P_Enum_JugeType_{0}", ControlItem.JugeType));
            if (lang == null)
            {
                LbContent.Content = ControlItem.JugeType;
            }
            else
            {
                LbContent.Content = lang.Display;
            }
        }
        /// <summary>
        /// ViewerLoaded
        /// </summary>
        public event Action ViewerLoaded;

        private void ControlItemViewer_OnLoaded(object sender, RoutedEventArgs e)
        {
            DataContext = ControlItem;
            Init();
        }
    }
}
