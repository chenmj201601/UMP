//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    b197e8f2-7a69-4ca5-9168-d3555d2b08b7
//        CLR Version:              4.0.30319.18444
//        Name:                     IPv4ParagraphTextBox
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.CustomControls.MaskedTextBox.Implementation
//        File Name:                IPv4ParagraphTextBox
//
//        created by Charley at 2015/1/16 18:24:26
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Windows.Input;

namespace VoiceCyber.Wpf.CustomControls
{
    /// <summary>
    /// IP地址的一段
    /// </summary>
    public class IPv4ParagraphTextBox : AutoSelectTextBox
    {
        private IPv4ParagraphTextBox mLeftParagraph;
        private IPv4ParagraphTextBox mRightParagraph;

        /// <summary>
        /// 设置相关联的前段和后段
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public void SetNeighbour(IPv4ParagraphTextBox left, IPv4ParagraphTextBox right)
        {
            mLeftParagraph = left;
            mRightParagraph = right;
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            if (e.Key == Key.Back)
            {
                if ((CaretIndex == 0) && (mLeftParagraph != null) && SelectionLength == 0)
                {
                    mLeftParagraph.Focus();
                    mLeftParagraph.CaretIndex = mLeftParagraph.Text.Length;
                    e.Handled = true;
                }
            }

            if (e.Key == Key.Left)
            {
                if ((CaretIndex == 0) && (mLeftParagraph != null))
                {
                    mLeftParagraph.Focus();
                    mLeftParagraph.CaretIndex = mLeftParagraph.Text.Length;
                    e.Handled = true;
                }
            }

            if (e.Key == Key.Right)
            {
                if ((CaretIndex == Text.Length) && (mRightParagraph != null))
                {
                    mRightParagraph.Focus();
                    mRightParagraph.CaretIndex = 0;
                    e.Handled = true;
                }
            }
        }

        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            base.OnPreviewTextInput(e);

            char input = char.Parse(e.Text);

            if (input == '.')
            {
                if ((CaretIndex == Text.Length) && (mRightParagraph != null))
                {
                    mRightParagraph.Focus();
                    mRightParagraph.SelectAll();
                    e.Handled = true;
                    return;
                }
            }

            if (input < '0' || input > '9')
            {
                e.Handled = true;
                return;
            }

            if ((Text.Length >= 3) && SelectionLength == 0)
            {
                e.Handled = true;
            }
        }

        protected override void OnTextInput(TextCompositionEventArgs e)
        {
            base.OnTextInput(e);

            int ip = Int16.Parse(Text);

            if (ip > 255) { Text = "255"; }

            if (Text.Length == 3)
            {
                if ((CaretIndex == Text.Length) && (mRightParagraph != null))
                {
                    mRightParagraph.Focus();
                    mRightParagraph.SelectAll();
                }
            }
        }
    }
}
