//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    a1973568-e0af-409a-8c7d-3853d1df0895
//        CLR Version:              4.0.30319.18444
//        Name:                     CachedTextInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.CustomControls.Core.Primitives
//        File Name:                CachedTextInfo
//
//        created by Charley at 2014/7/21 10:25:02
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace VoiceCyber.Wpf.CustomControls.Primitives
{
    internal class CachedTextInfo : ICloneable
    {
        private CachedTextInfo(string text, int caretIndex, int selectionStart, int selectionLength)
        {
            this.Text = text;
            this.CaretIndex = caretIndex;
            this.SelectionStart = selectionStart;
            this.SelectionLength = selectionLength;
        }

        public CachedTextInfo(TextBox textBox)
            : this(textBox.Text, textBox.CaretIndex, textBox.SelectionStart, textBox.SelectionLength)
        {
        }

        public string Text { get; private set; }
        public int CaretIndex { get; private set; }
        public int SelectionStart { get; private set; }
        public int SelectionLength { get; private set; }

        #region ICloneable Members

        public object Clone()
        {
            return new CachedTextInfo(this.Text, this.CaretIndex, this.SelectionStart, this.SelectionLength);
        }

        #endregion
    }
}
