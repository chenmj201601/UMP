//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    7e31515b-04d3-432e-b724-27f2410a2bcf
//        CLR Version:              4.0.30319.18444
//        Name:                     MaskedTextBox
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.CustomControls
//        File Name:                MaskedTextBox
//
//        created by Charley at 2014/7/4 15:40:02
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace VoiceCyber.CustomControls
{
    /// <summary>
    /// 自定义的TextBox，允许输入指定格式的文本
    /// </summary>
    public class MaskedTextBox : TextBox
    {
        /// <summary>
        /// 格式类型
        /// </summary>
        public static readonly DependencyProperty InputMaskProperty;
        /// <summary>
        /// 格式类型
        /// </summary>
        public static readonly DependencyProperty InputMaskType;

        private List<InputMaskChar> mListMaskChars;
        private int mCaretIndex;

        static MaskedTextBox()
        {
            TextProperty.OverrideMetadata(typeof(MaskedTextBox),
                new FrameworkPropertyMetadata(null, Text_CoerceValue));
            InputMaskProperty = DependencyProperty.Register("InputMask", typeof(string), typeof(MaskedTextBox),
                new PropertyMetadata(string.Empty, InputMask_Changed));
            InputMaskType = DependencyProperty.Register("InputMaskType", typeof(string), typeof(MaskedTextBox),
                            new PropertyMetadata(string.Empty, MaskType_Changed));
        }
        /// <summary>
        /// MaskedTextBox
        /// </summary>
        public MaskedTextBox()
        {
            mListMaskChars = new List<InputMaskChar>();
            DataObject.AddPastingHandler(this, MaskedTextBox_Paste);
        }
        /// <summary>
        /// 格式类型
        /// </summary>
        public string InputMask
        {
            get { return GetValue(InputMaskProperty) as string; }
            set { SetValue(InputMaskProperty, value); }
        }
        /// <summary>
        /// 格式类型
        /// </summary>
        public string MaskType
        {
            get { return GetValue(InputMaskType) as string; }
            set { SetValue(InputMaskType, value); }
        }
        /// <summary>
        /// 校验标志
        /// </summary>
        [Flags]
        protected enum InputMaskValidationFlags
        {
            /// <summary>
            /// None
            /// </summary>
            None = 0,
            /// <summary>
            /// AllowInteger
            /// </summary>
            AllowInteger = 1,
            /// <summary>
            /// AllowDecimal
            /// </summary>
            AllowDecimal = 2,
            /// <summary>
            /// AllowAlphabet
            /// </summary>
            AllowAlphabet = 4,
            /// <summary>
            /// AllowAlphanumeric
            /// </summary>
            AllowAlphanumeric = 8
        }
        /// <summary>
        /// 检验有效性
        /// </summary>
        /// <returns></returns>
        public bool IsTextValid()
        {
            string value;
            bool bReturn;

            bReturn = ValidateTextInternal(Text, out value);

            if (MaskType == "D" && bReturn)
            {
                bReturn = CheckInputIsDate(Text);
            }

            if (MaskType == "T" && bReturn)
            {
                bReturn = CheckInputIsTime(Text);
            }
            if (MaskType == "DT" && bReturn)
            {
                bReturn = CheckInputIsDateTime(Text);
            }
            return bReturn;
        }

        private bool CheckInputIsDate(string strText)
        {
            bool bReturn;
            DateTime dtDateTime;

            bReturn = DateTime.TryParse(strText + " 00:00:00", out dtDateTime);

            return bReturn;
        }

        private bool CheckInputIsTime(string strText)
        {
            bool bReturn = true;

            int intHoure, intMinute, intSecond;

            int.TryParse(strText.Substring(0, 2), out intHoure);
            int.TryParse(strText.Substring(3, 2), out intMinute);
            int.TryParse(strText.Substring(6, 2), out intSecond);

            if (intHoure > 23 || intHoure < 0 || intMinute > 59 || intMinute < 0 || intSecond > 59 || intSecond < 0) { bReturn = false; }

            return bReturn;
        }

        private bool CheckInputIsDateTime(string strText)
        {
            bool bReturn;
            DateTime dtDateTime;

            bReturn = DateTime.TryParse(strText, out dtDateTime);

            return bReturn;
        }

        private class InputMaskChar
        {
            private InputMaskValidationFlags validationFlags;
            private char literal;

            public InputMaskChar(InputMaskValidationFlags validationFlags)
            {
                this.validationFlags = validationFlags;
                literal = (char)0;
            }

            public InputMaskChar(char literal)
            {
                this.literal = literal;
            }


            public InputMaskValidationFlags ValidationFlags
            {
                get { return validationFlags; }
            }

            public char Literal
            {
                get { return literal; }
            }

            public bool IsLiteral()
            {
                return (literal != (char)0);
            }

            public char GetDefaultChar()
            {
                return (IsLiteral()) ? Literal : '0';
            }

        }

        /// <summary>
        /// OnMouseUp
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            mCaretIndex = CaretIndex;
        }

        /// <summary>
        /// OnPreviewKeyDown
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (mListMaskChars.Count == 0)
                return;

            if (e.Key == Key.Delete)
            {
                Text = GetDefaultText();
                mCaretIndex = CaretIndex = 0;
                e.Handled = true;
            }
            else
            {
                if (e.Key == Key.Back)
                {
                    if (mCaretIndex > 0 || SelectionLength > 0)
                    {
                        if (SelectionLength > 0)
                        {
                            DeleteSelectedText();
                        }
                        else
                        {
                            MoveBack();

                            char[] characters = Text.ToCharArray();
                            characters[mCaretIndex] = mListMaskChars[mCaretIndex].GetDefaultChar();
                            Text = new string(characters);
                        }

                        CaretIndex = mCaretIndex;
                        e.Handled = true;
                    }
                }
                else if (e.Key == Key.Left)
                {
                    MoveBack();
                    e.Handled = true;
                }
                else if (e.Key == Key.Right || e.Key == Key.Space)
                {
                    MoveForward();
                    e.Handled = true;
                }
            }
        }

        /// <summary>
        /// OnPreviewTextInput
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {

            base.OnPreviewTextInput(e);

            if (mListMaskChars.Count == 0)
                return;

            mCaretIndex = CaretIndex = SelectionStart;

            if (mCaretIndex == mListMaskChars.Count)
            {
                e.Handled = true;
            }
            else
            {
                bool isValid = ValidateInputChar(char.Parse(e.Text),
                    mListMaskChars[mCaretIndex].ValidationFlags);

                if (isValid)
                {
                    if (SelectionLength > 0)
                    {
                        DeleteSelectedText();
                    }

                    char[] characters = Text.ToCharArray();
                    characters[mCaretIndex] = char.Parse(e.Text);
                    Text = new string(characters);

                    MoveForward();
                }

                e.Handled = true;
            }
        }

        /// <summary>
        /// ValidateInputChar
        /// </summary>
        /// <param name="input"></param>
        /// <param name="validationFlags"></param>
        /// <returns></returns>
        protected virtual bool ValidateInputChar(char input, InputMaskValidationFlags validationFlags)
        {
            bool valid = (validationFlags == InputMaskValidationFlags.None);

            if (!valid)
            {
                Array values = Enum.GetValues(typeof(InputMaskValidationFlags));

                foreach (object o in values)
                {
                    InputMaskValidationFlags instance = (InputMaskValidationFlags)(int)o;
                    if ((instance & validationFlags) != 0)
                    {
                        if (ValidateCharInternal(input, instance))
                        {
                            valid = true;
                            break;
                        }
                    }
                }
            }

            return valid;
        }

        /// <summary>
        /// ValidateTextInternal
        /// </summary>
        /// <param name="text"></param>
        /// <param name="displayText"></param>
        /// <returns></returns>
        protected virtual bool ValidateTextInternal(string text, out string displayText)
        {
            if (mListMaskChars.Count == 0)
            {
                displayText = text;
                return true;
            }

            StringBuilder displayTextBuilder = new StringBuilder(GetDefaultText());

            bool valid = (!string.IsNullOrEmpty(text) &&
                text.Length <= mListMaskChars.Count);

            if (valid)
            {
                for (int i = 0; i < text.Length; i++)
                {
                    if (!mListMaskChars[i].IsLiteral())
                    {
                        if (ValidateInputChar(text[i], mListMaskChars[i].ValidationFlags))
                        {
                            displayTextBuilder[i] = text[i];
                        }
                        else
                        {
                            valid = false;
                        }
                    }
                }
            }

            displayText = displayTextBuilder.ToString();

            return valid;
        }

        /// <summary>
        /// DeleteSelectedText
        /// </summary>
        protected virtual void DeleteSelectedText()
        {
            StringBuilder text = new StringBuilder(Text);
            string defaultText = GetDefaultText();
            int selectionStart = SelectionStart;
            int selectionLength = SelectionLength;

            text.Remove(selectionStart, selectionLength);
            text.Insert(selectionStart, defaultText.Substring(selectionStart, selectionLength));
            Text = text.ToString();

            CaretIndex = mCaretIndex = selectionStart;
        }

        /// <summary>
        /// IsPlaceholderChar
        /// </summary>
        /// <param name="character"></param>
        /// <param name="validationFlags"></param>
        /// <returns></returns>
        protected virtual bool IsPlaceholderChar(char character, out InputMaskValidationFlags validationFlags)
        {
            validationFlags = InputMaskValidationFlags.None;

            switch (character.ToString().ToUpper())
            {
                case "I":
                    validationFlags = InputMaskValidationFlags.AllowInteger;
                    break;
                case "D":
                    validationFlags = InputMaskValidationFlags.AllowDecimal;
                    break;
                case "A":
                    validationFlags = InputMaskValidationFlags.AllowAlphabet;
                    break;
                case "W":
                    validationFlags = (InputMaskValidationFlags.AllowAlphanumeric);
                    break;
            }

            return (validationFlags != InputMaskValidationFlags.None);
        }

        private static object Text_CoerceValue(DependencyObject obj, object value)
        {
            MaskedTextBox mtb = (MaskedTextBox)obj;

            if (value == null || value.Equals(string.Empty))
                value = mtb.GetDefaultText();
            else if (value.ToString().Length > 0)
            {
                string displayText;
                mtb.ValidateTextInternal(value.ToString(), out displayText);
                value = displayText;
            }

            return value;
        }

        private static void InputMask_Changed(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var maskedTextBox = obj as MaskedTextBox;
            if (maskedTextBox != null) maskedTextBox.UpdateInputMask();
        }

        private static void MaskType_Changed(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
        }

        private void MaskedTextBox_Paste(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string value = e.DataObject.GetData(typeof(string)).ToString();
                string displayText;

                if (ValidateTextInternal(value, out displayText))
                {
                    Text = displayText;
                }
            }

            e.CancelCommand();
        }

        private void UpdateInputMask()
        {

            string text = Text;
            mListMaskChars.Clear();

            Text = string.Empty;

            string mask = InputMask;

            if (string.IsNullOrEmpty(mask))
                return;

            InputMaskValidationFlags validationFlags;

            for (int i = 0; i < mask.Length; i++)
            {
                bool isPlaceholder = IsPlaceholderChar(mask[i], out validationFlags);

                if (isPlaceholder)
                {
                    mListMaskChars.Add(new InputMaskChar(validationFlags));
                }
                else
                {
                    mListMaskChars.Add(new InputMaskChar(mask[i]));
                }
            }

            string displayText;
            if (text.Length > 0 && ValidateTextInternal(text, out displayText))
            {
                Text = displayText;
            }
            else
            {
                Text = GetDefaultText();
            }
        }

        private bool ValidateCharInternal(char input, InputMaskValidationFlags validationType)
        {
            bool valid = false;

            switch (validationType)
            {
                case InputMaskValidationFlags.AllowInteger:
                case InputMaskValidationFlags.AllowDecimal:
                    int i;
                    if (validationType == InputMaskValidationFlags.AllowDecimal &&
                        input == '.' && !Text.Contains('.'))
                    {
                        valid = true;
                    }
                    else
                    {
                        valid = int.TryParse(input.ToString(), out i);
                    }
                    break;
                case InputMaskValidationFlags.AllowAlphabet:
                    valid = char.IsLetter(input);
                    break;
                case InputMaskValidationFlags.AllowAlphanumeric:
                    valid = (char.IsLetter(input) || char.IsNumber(input));
                    break;
            }

            return valid;
        }

        private string GetDefaultText()
        {
            StringBuilder text = new StringBuilder();
            foreach (InputMaskChar maskChar in mListMaskChars)
            {
                text.Append(maskChar.GetDefaultChar());
            }
            return text.ToString();
        }

        private void MoveForward()
        {
            int pos = mCaretIndex;
            while (pos < mListMaskChars.Count)
            {
                if (++pos == mListMaskChars.Count || !mListMaskChars[pos].IsLiteral())
                {
                    mCaretIndex = CaretIndex = pos;
                    break;
                }
            }
        }

        private void MoveBack()
        {
            int pos = mCaretIndex;
            while (pos > 0)
            {
                if (--pos == 0 || !mListMaskChars[pos].IsLiteral())
                {
                    mCaretIndex = CaretIndex = pos;
                    break;
                }
            }
        }
    }
}
