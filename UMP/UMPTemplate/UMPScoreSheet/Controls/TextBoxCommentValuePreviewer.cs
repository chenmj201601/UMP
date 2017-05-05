//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    a2e34f37-f7eb-4fb5-a6c3-5ad1ebf241d3
//        CLR Version:              4.0.30319.18444
//        Name:                     TextBoxCommentValuePreviewer
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets.Controls
//        File Name:                TextBoxCommentValuePreviewer
//
//        created by Charley at 2014/8/6 14:59:38
//        http://www.voicecyber.com 
//
//======================================================================

using System.Windows;
using System.Windows.Controls;

namespace VoiceCyber.UMP.ScoreSheets.Controls
{
    [TemplatePart(Name = PART_Panel, Type = typeof(Border))]
    public class TextBoxCommentValuePreviewer : ScoreObjectPreViewer
    {
        public static readonly DependencyProperty TextCommentProperty =
            DependencyProperty.Register("TextComment", typeof(TextComment), typeof(TextBoxCommentValuePreviewer), new PropertyMetadata(default(TextComment)));

        public TextComment TextComment
        {
            get { return (TextComment)GetValue(TextCommentProperty); }
            set { SetValue(TextCommentProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(TextBoxCommentValuePreviewer), new PropertyMetadata(default(string)));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        static TextBoxCommentValuePreviewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TextBoxCommentValuePreviewer),
                new FrameworkPropertyMetadata(typeof(TextBoxCommentValuePreviewer)));
        }

        public override void Init()
        {
            base.Init();

            if (TextComment != null)
            {
                Text = TextComment.Text;
            }
        }

        private const string PART_Panel = "PART_Panel";
        private Border mBorderPanel;
    }
}
