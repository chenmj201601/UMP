//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    15d44490-b5ea-4563-a134-5d3dbd522947
//        CLR Version:              4.0.30319.18408
//        Name:                     EditableTextBlock
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.CustomControls.MaskedTextBox.Implementation
//        File Name:                EditableTextBlock
//
//        created by Charley at 2016/7/14 09:50:49
//        http://www.voicecyber.com 
//
//======================================================================

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace VoiceCyber.Wpf.CustomControls
{
    /// <summary>
    /// 
    /// </summary>
    public class EditableTextBlock : Control
    {
        private bool mCanEditable;

        static EditableTextBlock()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (EditableTextBlock),
                new FrameworkPropertyMetadata(typeof (EditableTextBlock)));
        }

        /// <summary>
        /// 
        /// </summary>
        public EditableTextBlock()
        {
            mCanEditable = false;
        }


        #region IsInEditMode

        public static readonly DependencyProperty IsInEditModeProperty =
            DependencyProperty.Register("IsInEditMode", typeof (bool), typeof (EditableTextBlock), new PropertyMetadata(default(bool)));

        public bool IsInEditMode
        {
            get { return (bool) GetValue(IsInEditModeProperty); }
            set { SetValue(IsInEditModeProperty, value); }
        }

        #endregion


        #region IsReadOnly

        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register("IsReadOnly", typeof (bool), typeof (EditableTextBlock), new PropertyMetadata(default(bool)));

        public bool IsReadOnly
        {
            get { return (bool) GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        #endregion


        #region Text

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof (string), typeof (EditableTextBlock), new PropertyMetadata(default(string)));

        public string Text
        {
            get { return (string) GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        #endregion


        #region Template

        private const string PART_TextBlock = "PART_TextBlock";
        private const string PART_EditBox = "PART_EditBox";

        private TextBlock mTextBlock;
        private TextBox mEditBox;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            mTextBlock = GetTemplateChild(PART_TextBlock) as TextBlock;
            if (mTextBlock != null)
            {
                mTextBlock.MouseUp += TextBlock_MouseUp;
                mTextBlock.MouseLeave += TextBlock_MouseLeave;
            }

            mEditBox = GetTemplateChild(PART_EditBox) as TextBox;
            if (mEditBox != null)
            {
                mEditBox.LostFocus += EditBox_LostFocus;
                mEditBox.KeyDown += EditBox_KeyDown;
            }
        }

        #endregion


        #region Event Handlers

        void EditBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                mCanEditable = false;
                IsInEditMode = false;
            }
        }

        void EditBox_LostFocus(object sender, RoutedEventArgs e)
        {
            mCanEditable = false;
            IsInEditMode = false;
        }

        void TextBlock_MouseLeave(object sender, MouseEventArgs e)
        {
            mCanEditable = false;
        }

        void TextBlock_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (mCanEditable)
            {
                IsInEditMode = true;

                if (mEditBox != null)
                {
                    mEditBox.Focus();
                }
            }
            else
            {
                mCanEditable = true;
            }
        }

        #endregion

    }
}
