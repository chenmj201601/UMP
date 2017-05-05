//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    ba61a4c6-bafe-49b7-8f41-4d3bfa9caa58
//        CLR Version:              4.0.30319.18444
//        Name:                     ScoreResultViewer
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets.Controls
//        File Name:                ScoreResultViewer
//
//        created by Charley at 2014/8/8 15:07:45
//        http://www.voicecyber.com 
//
//======================================================================

using System.Windows;
using System.Windows.Controls;

namespace VoiceCyber.UMP.ScoreSheets.Controls
{
    public class ScoreResultViewer : Control
    {
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof (string), typeof (ScoreResultViewer), new PropertyMetadata(default(string)));

        public string Title
        {
            get { return (string) GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        static ScoreResultViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (ScoreResultViewer),
                new FrameworkPropertyMetadata(typeof (ScoreResultViewer)));
        }
    }
}
