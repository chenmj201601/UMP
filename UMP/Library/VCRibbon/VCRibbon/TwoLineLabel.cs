//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    3dcf6c2f-8ddc-43d1-ad98-14621589c6aa
//        CLR Version:              4.0.30319.18444
//        Name:                     TwoLineLabel
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Ribbon
//        File Name:                TwoLineLabel
//
//        created by Charley at 2014/5/28 11:28:58
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace VoiceCyber.Ribbon
{
    /// <summary>
    /// Represents specific label to use in particular ribbon controls
    /// </summary>
    [TemplatePart(Name = "PART_TextRun", Type = typeof(TextBlock))]
    [TemplatePart(Name = "PART_TextRun2", Type = typeof(TextBlock))]
    [TemplatePart(Name = "PART_Glyph", Type = typeof(InlineUIContainer))]
    public class TwoLineLabel : Control
    {
        #region Fields

        /// <summary>
        /// Run with text
        /// </summary>
        private AccessText textRun;

        private AccessText textRun2;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets whether label must have two lines
        /// </summary>
        public bool HasTwoLines
        {
            get { return (bool)GetValue(HasTwoLinesProperty); }
            set { SetValue(HasTwoLinesProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for HasTwoLines.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HasTwoLinesProperty =
            DependencyProperty.Register("HasTwoLines", typeof(bool), typeof(TwoLineLabel), new UIPropertyMetadata(true, OnHasTwoLinesChanged));

        /// <summary>
        /// Handles HasTwoLines property changes
        /// </summary>
        /// <param name="d">Object</param>
        /// <param name="e">The event data</param>
        private static void OnHasTwoLinesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as TwoLineLabel).UpdateTextRun();
        }

        /// <summary>
        /// Gets or sets whether label has glyph
        /// </summary>
        public bool HasGlyph
        {
            get { return (bool)GetValue(HasGlyphProperty); }
            set { SetValue(HasGlyphProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for HasGlyph.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HasGlyphProperty =
            DependencyProperty.Register("HasGlyph", typeof(bool), typeof(TwoLineLabel), new UIPropertyMetadata(false, OnHasGlyphChanged));

        /// <summary>
        /// Handles HasGlyph property changes
        /// </summary>
        /// <param name="d">Object</param>
        /// <param name="e">The event data</param>
        private static void OnHasGlyphChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as TwoLineLabel).UpdateTextRun();
        }

        /// <summary>
        /// Gets or sets labels text
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(TwoLineLabel), new UIPropertyMetadata("TwoLineLabel", OnTextChanged));

        #endregion

        #region Initialize

        /// <summary>
        /// Static constructor
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static TwoLineLabel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TwoLineLabel), new FrameworkPropertyMetadata(typeof(TwoLineLabel)));
            StyleProperty.OverrideMetadata(typeof(TwoLineLabel), new FrameworkPropertyMetadata(null, new CoerceValueCallback(OnCoerceStyle)));
        }

        // Coerce object style
        static object OnCoerceStyle(DependencyObject d, object basevalue)
        {
            if (basevalue == null)
            {
                basevalue = (d as FrameworkElement).TryFindResource(typeof(TwoLineLabel));
            }

            return basevalue;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public TwoLineLabel()
        {
            this.Focusable = false;
        }

        #endregion

        #region Overrides

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal 
        /// processes call System.Windows.FrameworkElement.ApplyTemplate().
        /// </summary>
        public override void OnApplyTemplate()
        {
            textRun = GetTemplateChild("PART_TextRun") as AccessText;
            textRun2 = GetTemplateChild("PART_TextRun2") as AccessText;
            UpdateTextRun();
        }

        #endregion

        #region Event handling

        /// <summary>
        /// Handles text property changes
        /// </summary>
        /// <param name="d">Object</param>
        /// <param name="e">The event data</param>
        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TwoLineLabel label = d as TwoLineLabel;
            label.UpdateTextRun();
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Updates text run adds newline if HasTwoLines == true
        /// </summary>
        void UpdateTextRun()
        {
            if ((textRun != null) && (textRun2 != null) && (Text != null))
            {
                textRun.Text = Text;
                textRun2.Text = "";
                string text = Text.Trim();
                if (HasTwoLines)
                {
                    int centerIndex = Text.Length / 2;
                    // Find spaces nearest to center from left and right
                    int leftSpaceIndex = text.LastIndexOf(" ", centerIndex, centerIndex);
                    int rightSpaceIndex = text.IndexOf(" ", centerIndex, StringComparison.CurrentCulture);
                    if ((leftSpaceIndex == -1) && (rightSpaceIndex == -1))
                    {
                        // The text can`t be separated. Add new line for glyph
                        //textRun.Text += '\u0085';
                    }
                    else if (leftSpaceIndex == -1)
                    {
                        // Finds only space from right. New line adds on it
                        textRun.Text = text.Substring(0, rightSpaceIndex);
                        textRun2.Text = text.Substring(rightSpaceIndex) + " ";
                    }
                    else if (rightSpaceIndex == -1)
                    {
                        // Finds only space from left. New line adds on it
                        textRun.Text = text.Substring(0, leftSpaceIndex);
                        textRun2.Text = text.Substring(leftSpaceIndex) + " ";
                    }
                    else
                    {
                        // Find nearest to center space and add new line on it
                        if (Math.Abs(centerIndex - leftSpaceIndex) < Math.Abs(centerIndex - rightSpaceIndex))
                        {
                            textRun.Text = text.Substring(0, leftSpaceIndex);
                            textRun2.Text = text.Substring(leftSpaceIndex) + " ";
                        }
                        else
                        {
                            textRun.Text = text.Substring(0, rightSpaceIndex);
                            textRun2.Text = text.Substring(rightSpaceIndex) + " ";
                        }
                    }
                }
            }
        }

        #endregion
    }
}
