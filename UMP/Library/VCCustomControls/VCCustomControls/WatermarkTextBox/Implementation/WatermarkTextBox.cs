//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    ea71892f-f6e7-49c8-9ab4-d10d9a3a96a5
//        CLR Version:              4.0.30319.18444
//        Name:                     WatermarkTextBox
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.CustomControls.WatermarkTextBox.Implementation
//        File Name:                WatermarkTextBox
//
//        created by Charley at 2014/7/17 15:04:08
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace VoiceCyber.Wpf.CustomControls
{
    public class WatermarkTextBox : AutoSelectTextBox
    {
        #region Properties

        #region SelectAllOnGotFocus (Obsolete)

        [Obsolete("This property is obsolete and should no longer be used. Use AutoSelectTextBox.AutoSelectBehavior instead.")]
        public static readonly DependencyProperty SelectAllOnGotFocusProperty = DependencyProperty.Register("SelectAllOnGotFocus", typeof(bool), typeof(WatermarkTextBox), new PropertyMetadata(false));
        [Obsolete("This property is obsolete and should no longer be used. Use AutoSelectTextBox.AutoSelectBehavior instead.")]
        public bool SelectAllOnGotFocus
        {
            get
            {
                return (bool)GetValue(SelectAllOnGotFocusProperty);
            }
            set
            {
                SetValue(SelectAllOnGotFocusProperty, value);
            }
        }

        #endregion //SelectAllOnGotFocus

        #region Watermark

        public static readonly DependencyProperty WatermarkProperty = DependencyProperty.Register("Watermark", typeof(object), typeof(WatermarkTextBox), new UIPropertyMetadata(null));
        public object Watermark
        {
            get
            {
                return (object)GetValue(WatermarkProperty);
            }
            set
            {
                SetValue(WatermarkProperty, value);
            }
        }

        #endregion //Watermark

        #region WatermarkTemplate

        public static readonly DependencyProperty WatermarkTemplateProperty = DependencyProperty.Register("WatermarkTemplate", typeof(DataTemplate), typeof(WatermarkTextBox), new UIPropertyMetadata(null));
        public DataTemplate WatermarkTemplate
        {
            get
            {
                return (DataTemplate)GetValue(WatermarkTemplateProperty);
            }
            set
            {
                SetValue(WatermarkTemplateProperty, value);
            }
        }

        #endregion //WatermarkTemplate

        #endregion //Properties

        #region Constructors

        static WatermarkTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WatermarkTextBox), new FrameworkPropertyMetadata(typeof(WatermarkTextBox)));
        }

        #endregion //Constructors

        #region Base Class Overrides

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);

            if (SelectAllOnGotFocus)
                SelectAll();
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (!IsKeyboardFocused && SelectAllOnGotFocus)
            {
                e.Handled = true;
                Focus();
            }

            base.OnPreviewMouseLeftButtonDown(e);
        }

        #endregion //Base Class Overrides
    }
}
