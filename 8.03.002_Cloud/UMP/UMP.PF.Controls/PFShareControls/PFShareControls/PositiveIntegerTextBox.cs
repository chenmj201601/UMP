using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;

namespace PFShareControls
{
    class PositiveIntegerTextBox : TextBox
    {
        int IntMinValue = int.MinValue;
        int IntMaxValue = int.MaxValue;
        int IntDefaultValue = 0;

        public void SetMinMaxDefaultValue(int AIntMinValue, int AIntMaxValue, int AIntDefaultValue)
        {
            IntMinValue = AIntMinValue; IntMaxValue = AIntMaxValue; IntDefaultValue = AIntDefaultValue;
        }

        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            base.OnPreviewTextInput(e);

            char LCharInput = char.Parse(e.Text);

            if (LCharInput < '0' || LCharInput > '9')
            {
                if (Text.Length == 1 && LCharInput == '0')
                {
                    e.Handled = true; return;
                }
                e.Handled = true;
                return;
            }
        }

        protected override void OnTextInput(TextCompositionEventArgs e)
        {
            base.OnTextInput(e);

            int LIntValue;

            if (!int.TryParse(Text, out LIntValue))
            {
                Text = IntDefaultValue.ToString();
                return;
            }
            if (LIntValue > IntMaxValue) { Text = IntDefaultValue.ToString(); }
            if (LIntValue <= 0)
            {
                if (LIntValue < IntMinValue) { Text = IntDefaultValue.ToString(); }
            }
        }

        protected override void OnLostFocus(System.Windows.RoutedEventArgs e)
        {
 	        base.OnLostFocus(e);

            int LIntValue;

            if (!int.TryParse(Text, out LIntValue))
            {
                Text = IntDefaultValue.ToString();
                return;
            }
            if (LIntValue < IntMinValue) { Text = IntDefaultValue.ToString(); return; }
            Text = LIntValue.ToString();
        }
    
    }
}
