using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UMPS2400.Classes
{
    public class MaskedTextBox : TextBox
    {
        public static readonly DependencyProperty InputMaskProperty;
        public static readonly DependencyProperty InputMaskType;

        private List<InputMaskChar> _maskChars;
        private int _caretIndex;

        static MaskedTextBox()
        {
            TextProperty.OverrideMetadata(typeof(MaskedTextBox),
                new FrameworkPropertyMetadata(null, new CoerceValueCallback(Text_CoerceValue)));
            InputMaskProperty = DependencyProperty.Register("InputMask", typeof(string), typeof(MaskedTextBox),
                new PropertyMetadata(string.Empty, new PropertyChangedCallback(InputMask_Changed)));
            InputMaskType = DependencyProperty.Register("InputMaskType", typeof(string), typeof(MaskedTextBox),
                            new PropertyMetadata(string.Empty, new PropertyChangedCallback(MaskType_Changed)));
        }

        public MaskedTextBox()
        {
            this._maskChars = new List<InputMaskChar>();
            DataObject.AddPastingHandler(this, new DataObjectPastingEventHandler(MaskedTextBox_Paste));
        }

        public string InputMask
        {
            get { return this.GetValue(InputMaskProperty) as string; }
            set { this.SetValue(InputMaskProperty, value); }
        }

        public string MaskType
        {
            get { return this.GetValue(InputMaskType) as string; }
            set { this.SetValue(InputMaskType, value); }
        }

        [Flags]
        protected enum InputMaskValidationFlags
        {
            None = 0,
            AllowInteger = 1,
            AllowDecimal = 2,
            AllowAlphabet = 4,
            AllowAlphanumeric = 8
        }

        public bool IsTextValid()
        {
            string value;
            bool lb_return = false;

            lb_return = this.ValidateTextInternal(this.Text, out value);

            if (MaskType == "D" && lb_return)
            {
                lb_return = CheckInputIsDate(this.Text);
            }

            if (MaskType == "T" && lb_return)
            {
                lb_return = CheckInputIsTime(this.Text);
            }
            if (MaskType == "DT" && lb_return)
            {
                lb_return = CheckInputIsDateTime(this.Text);
            }
            return lb_return;
        }

        private bool CheckInputIsDate(string as_text)
        {
            bool lb_return = true;
            DateTime ld_Datetime;

            lb_return = DateTime.TryParse(as_text + " 00:00:00", out ld_Datetime);

            return lb_return;
        }

        private bool CheckInputIsTime(string as_text)
        {
            bool lb_return = true;

            int li_hour = 0, li_min = 0, li_sec = 0;

            int.TryParse(as_text.Substring(0, 2), out li_hour);
            int.TryParse(as_text.Substring(3, 2), out li_min);
            int.TryParse(as_text.Substring(6, 2), out li_sec);

            if (li_hour > 23 || li_hour < 0 || li_min > 59 || li_min < 0 || li_sec > 59 || li_sec < 0) { lb_return = false; }

            return lb_return;
        }

        private bool CheckInputIsDateTime(string as_text)
        {
            bool lb_return = true;
            DateTime ld_datetime;

            lb_return = DateTime.TryParse(as_text, out ld_datetime);

            return lb_return;
        }

        private class InputMaskChar
        {

            private InputMaskValidationFlags _validationFlags;
            private char _literal;

            public InputMaskChar(InputMaskValidationFlags validationFlags)
            {
                this._validationFlags = validationFlags;
                this._literal = (char)0;
            }

            public InputMaskChar(char literal)
            {
                this._literal = literal;
            }

            public InputMaskValidationFlags ValidationFlags
            {
                get { return this._validationFlags; }
                set { this._validationFlags = value; }
            }

            public char Literal
            {
                get { return this._literal; }
                set { this._literal = value; }
            }

            public bool IsLiteral()
            {
                return (this._literal != (char)0);
            }

            public char GetDefaultChar()
            {
                return (this.IsLiteral()) ? this.Literal : '0';
            }

        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            this._caretIndex = this.CaretIndex;
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (this._maskChars.Count == 0)
                return;

            if (e.Key == Key.Delete)
            {
                this.Text = this.GetDefaultText();
                this._caretIndex = this.CaretIndex = 0;
                e.Handled = true;
            }
            else
            {
                if (e.Key == Key.Back)
                {
                    if (this._caretIndex > 0 || this.SelectionLength > 0)
                    {
                        if (this.SelectionLength > 0)
                        {
                            this.DeleteSelectedText();
                        }
                        else
                        {
                            this.MoveBack();

                            char[] characters = this.Text.ToCharArray();
                            characters[this._caretIndex] = this._maskChars[this._caretIndex].GetDefaultChar();
                            this.Text = new string(characters);
                        }

                        this.CaretIndex = this._caretIndex;
                        e.Handled = true;
                    }
                }
                else if (e.Key == Key.Left)
                {
                    this.MoveBack();
                    e.Handled = true;
                }
                else if (e.Key == Key.Right || e.Key == Key.Space)
                {
                    this.MoveForward();
                    e.Handled = true;
                }
            }
        }

        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {

            base.OnPreviewTextInput(e);

            if (this._maskChars.Count == 0)
                return;

            this._caretIndex = this.CaretIndex = this.SelectionStart;

            if (this._caretIndex == this._maskChars.Count)
            {
                e.Handled = true;
            }
            else
            {
                bool isValid = this.ValidateInputChar(char.Parse(e.Text),
                    this._maskChars[this._caretIndex].ValidationFlags);

                if (isValid)
                {
                    if (this.SelectionLength > 0)
                    {
                        this.DeleteSelectedText();
                    }

                    char[] characters = this.Text.ToCharArray();
                    characters[this._caretIndex] = char.Parse(e.Text);
                    this.Text = new string(characters);

                    this.MoveForward();
                }

                e.Handled = true;
            }
        }

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
                        if (this.ValidateCharInternal(input, instance))
                        {
                            valid = true;
                            break;
                        }
                    }
                }
            }

            return valid;
        }

        protected virtual bool ValidateTextInternal(string text, out string displayText)
        {
            if (this._maskChars.Count == 0)
            {
                displayText = text;
                return true;
            }

            StringBuilder displayTextBuilder = new StringBuilder(this.GetDefaultText());

            bool valid = (!string.IsNullOrEmpty(text) &&
                text.Length <= this._maskChars.Count);

            if (valid)
            {
                for (int i = 0; i < text.Length; i++)
                {
                    if (!this._maskChars[i].IsLiteral())
                    {
                        if (this.ValidateInputChar(text[i], this._maskChars[i].ValidationFlags))
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

        protected virtual void DeleteSelectedText()
        {
            StringBuilder text = new StringBuilder(this.Text);
            string defaultText = this.GetDefaultText();
            int selectionStart = this.SelectionStart;
            int selectionLength = this.SelectionLength;

            text.Remove(selectionStart, selectionLength);
            text.Insert(selectionStart, defaultText.Substring(selectionStart, selectionLength));
            this.Text = text.ToString();

            this.CaretIndex = this._caretIndex = selectionStart;
        }

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
            (obj as MaskedTextBox).UpdateInputMask();
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

                if (this.ValidateTextInternal(value, out displayText))
                {
                    this.Text = displayText;
                }
            }

            e.CancelCommand();
        }

        private void UpdateInputMask()
        {

            string text = this.Text;
            this._maskChars.Clear();

            this.Text = string.Empty;

            string mask = this.InputMask;

            if (string.IsNullOrEmpty(mask))
                return;

            InputMaskValidationFlags validationFlags = InputMaskValidationFlags.None;

            for (int i = 0; i < mask.Length; i++)
            {
                bool isPlaceholder = this.IsPlaceholderChar(mask[i], out validationFlags);

                if (isPlaceholder)
                {
                    this._maskChars.Add(new InputMaskChar(validationFlags));
                }
                else
                {
                    this._maskChars.Add(new InputMaskChar(mask[i]));
                }
            }

            string displayText;
            if (text.Length > 0 && this.ValidateTextInternal(text, out displayText))
            {
                this.Text = displayText;
            }
            else
            {
                this.Text = this.GetDefaultText();
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
                        input == '.' && !this.Text.Contains('.'))
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
            foreach (InputMaskChar maskChar in this._maskChars)
            {
                text.Append(maskChar.GetDefaultChar());
            }
            return text.ToString();
        }

        private void MoveForward()
        {
            int pos = this._caretIndex;
            while (pos < this._maskChars.Count)
            {
                if (++pos == this._maskChars.Count || !this._maskChars[pos].IsLiteral())
                {
                    this._caretIndex = this.CaretIndex = pos;
                    break;
                }
            }
        }

        private void MoveBack()
        {
            int pos = this._caretIndex;
            while (pos > 0)
            {
                if (--pos == 0 || !this._maskChars[pos].IsLiteral())
                {
                    this._caretIndex = this.CaretIndex = pos;
                    break;
                }
            }
        }
    }
}
