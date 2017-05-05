//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    a7eaccdd-f540-4fbf-b9d4-37c7eeaa6712
//        CLR Version:              4.0.30319.18444
//        Name:                     TextBoxCommentValueViewer
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets.Controls
//        File Name:                TextBoxCommentValueViewer
//
//        created by Charley at 2014/8/12 11:46:34
//        http://www.voicecyber.com 
//
//======================================================================

using System.Windows;
using System.Windows.Controls;

namespace VoiceCyber.UMP.ScoreSheets.Controls
{
    [TemplatePart(Name = PART_Panel, Type = typeof(Border))]
    [TemplatePart(Name = PART_Value, Type = typeof(TextBox))]
    public class TextBoxCommentValueViewer : ScoreObjectViewer
    {
        private const string PART_Panel = "PART_Panel";
        private const string PART_Value = "PART_Value";

        private Border mBorderPanel;
        private TextBox mValue;

        public static readonly DependencyProperty TextCommentProperty =
            DependencyProperty.Register("TextComment", typeof(TextComment), typeof(TextBoxCommentValueViewer), new PropertyMetadata(default(TextComment)));

        public TextComment TextComment
        {
            get { return (TextComment)GetValue(TextCommentProperty); }
            set { SetValue(TextCommentProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(TextBoxCommentValueViewer), new PropertyMetadata(default(string)));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        static TextBoxCommentValueViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TextBoxCommentValueViewer),
                new FrameworkPropertyMetadata(typeof(TextBoxCommentValueViewer)));
        }

        public override void Init()
        {
            base.Init();

            if (TextComment != null)
            {
                if (ViewMode == 0)
                {
                    Text = string.Empty;
                }
                else
                {
                    Text = TextComment.Text;
                    if (ViewMode == 2)
                    {
                        IsEnabled = false;
                    }
                }
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            mBorderPanel = GetTemplateChild(PART_Panel) as Border;
            if (mBorderPanel != null)
            {
                
            }
            mValue = GetTemplateChild(PART_Value) as TextBox;
            if (mValue != null)
            {
                mValue.TextChanged += mValue_TextChanged;
            }
        }

        void mValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (mValue != null)
            {
                string strValue = mValue.Text;
                TextComment.Text = strValue;
            }
        }
    }
}
