//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    f0451c5f-df8f-41e6-a8fd-c10cde0d98f2
//        CLR Version:              4.0.30319.18444
//        Name:                     AutoCompletingMaskEventArgs
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.CustomControls.MaskedTextBox.Implementation
//        File Name:                AutoCompletingMaskEventArgs
//
//        created by Charley at 2014/7/21 10:19:48
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace VoiceCyber.Wpf.CustomControls
{
    public class AutoCompletingMaskEventArgs : CancelEventArgs
    {
        public AutoCompletingMaskEventArgs(MaskedTextProvider maskedTextProvider, int startPosition, int selectionLength, string input)
        {
            m_autoCompleteStartPosition = -1;

            m_maskedTextProvider = maskedTextProvider;
            m_startPosition = startPosition;
            m_selectionLength = selectionLength;
            m_input = input;
        }

        #region MaskedTextProvider PROPERTY

        private MaskedTextProvider m_maskedTextProvider;

        public MaskedTextProvider MaskedTextProvider
        {
            get { return m_maskedTextProvider; }
        }

        #endregion MaskedTextProvider PROPERTY

        #region StartPosition PROPERTY

        private int m_startPosition;

        public int StartPosition
        {
            get { return m_startPosition; }
        }

        #endregion StartPosition PROPERTY

        #region SelectionLength PROPERTY

        private int m_selectionLength;

        public int SelectionLength
        {
            get { return m_selectionLength; }
        }

        #endregion SelectionLength PROPERTY

        #region Input PROPERTY

        private string m_input;

        public string Input
        {
            get { return m_input; }
        }

        #endregion Input PROPERTY


        #region AutoCompleteStartPosition PROPERTY

        private int m_autoCompleteStartPosition;

        public int AutoCompleteStartPosition
        {
            get { return m_autoCompleteStartPosition; }
            set { m_autoCompleteStartPosition = value; }
        }

        #endregion AutoCompleteStartPosition PROPERTY

        #region AutoCompleteText PROPERTY

        private string m_autoCompleteText;

        public string AutoCompleteText
        {
            get { return m_autoCompleteText; }
            set { m_autoCompleteText = value; }
        }

        #endregion AutoCompleteText PROPERTY
    }
}
