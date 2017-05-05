//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    01726c1d-10ef-44bb-804e-a97c92959cfc
//        CLR Version:              4.0.30319.18444
//        Name:                     ScoreObjectPreViewer
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets.Controls
//        File Name:                ScoreObjectPreViewer
//
//        created by Charley at 2014/8/5 9:24:22
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace VoiceCyber.UMP.ScoreSheets.Controls
{
    public class ScoreObjectPreViewer : ContentControl
    {
        public static readonly DependencyProperty SettingsProperty =
            DependencyProperty.Register("Settings", typeof (List<ScoreSetting>), typeof (ScoreObjectPreViewer), new PropertyMetadata(default(List<ScoreSetting>)));

        public List<ScoreSetting> Settings
        {
            get { return (List<ScoreSetting>) GetValue(SettingsProperty); }
            set { SetValue(SettingsProperty, value); }
        }

        public static readonly DependencyProperty LanguagesProperty =
            DependencyProperty.Register("Languages", typeof (List<ScoreLangauge>), typeof (ScoreObjectPreViewer), new PropertyMetadata(default(List<ScoreLangauge>)));

        public List<ScoreLangauge> Languages
        {
            get { return (List<ScoreLangauge>) GetValue(LanguagesProperty); }
            set { SetValue(LanguagesProperty, value); }
        }

        public static readonly DependencyProperty LangIDProperty =
            DependencyProperty.Register("LangID", typeof (int), typeof (ScoreObjectPreViewer), new PropertyMetadata(default(int)));

        public int LangID
        {
            get { return (int) GetValue(LangIDProperty); }
            set { SetValue(LangIDProperty, value); }
        }

        public static readonly DependencyProperty ViewClassicProperty =
            DependencyProperty.Register("ViewClassic", typeof (ScoreItemClassic), typeof (ScoreObjectPreViewer), new PropertyMetadata(default(ScoreItemClassic)));

        public ScoreItemClassic ViewClassic
        {
            get { return (ScoreItemClassic) GetValue(ViewClassicProperty); }
            set { SetValue(ViewClassicProperty, value); }
        }

        public static readonly DependencyProperty ViewModeProperty =
            DependencyProperty.Register("ViewMode", typeof (int), typeof (ScoreObjectPreViewer), new PropertyMetadata(default(int)));

        /// <summary>
        /// 0       评分模式
        /// 1       修改模式
        /// 2       查看模式
        /// </summary>
        public int ViewMode
        {
            get { return (int) GetValue(ViewModeProperty); }
            set { SetValue(ViewModeProperty, value); }
        }

        public ScoreObjectPreViewer()
        {
            this.Loaded += (s, e) => Init();
        }

        public virtual void Init()
        {
            
        }
    }
}
