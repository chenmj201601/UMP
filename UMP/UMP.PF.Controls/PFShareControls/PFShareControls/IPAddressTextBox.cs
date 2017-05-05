using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;

namespace PFShareControls
{
    class IPAddressTextBox : TextBox
    {
        IPAddressTextBox LeftIPAddressTextBox = null;
        IPAddressTextBox RightIPAddressTextBox = null;

        public void SetNeighbour(IPAddressTextBox ALeft, IPAddressTextBox ARight)
        {
            LeftIPAddressTextBox = ALeft;
            RightIPAddressTextBox = ARight;
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            if (e.Key == Key.Back)
            {
                if ((CaretIndex == 0) && (LeftIPAddressTextBox != null) && SelectionLength == 0)
                {
                    LeftIPAddressTextBox.Focus();
                    LeftIPAddressTextBox.CaretIndex = LeftIPAddressTextBox.Text.Length;
                    e.Handled = true;
                }
            }

            if (e.Key == Key.Left)
            {
                if ((CaretIndex == 0) && (LeftIPAddressTextBox != null))
                {
                    LeftIPAddressTextBox.Focus();
                    LeftIPAddressTextBox.CaretIndex = LeftIPAddressTextBox.Text.Length;
                    e.Handled = true;
                }
            }

            if (e.Key == Key.Right)
            {
                if ((CaretIndex == Text.Length) && (RightIPAddressTextBox != null))
                {
                    RightIPAddressTextBox.Focus();
                    RightIPAddressTextBox.CaretIndex = 0;
                    e.Handled = true;
                }
            }
        }

        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            base.OnPreviewTextInput(e);

            char LCharInput = char.Parse(e.Text);

            if (LCharInput == '.')
            {
                if ((CaretIndex == Text.Length) && (RightIPAddressTextBox != null))
                {
                    RightIPAddressTextBox.Focus();
                    RightIPAddressTextBox.SelectAll();
                    e.Handled = true;
                    return;
                }
            }

            if (LCharInput < '0' || LCharInput > '9')
            {
                e.Handled = true;
                return;
            }

            if ((Text.Length >= 3) && SelectionLength == 0)
            {
                e.Handled = true;
                return;
            }
        }

        protected override void OnTextInput(TextCompositionEventArgs e)
        {
            base.OnTextInput(e);

            int LIntIP = Int16.Parse(Text);

            if (LIntIP > 255) { Text = "255"; }

            if (Text.Length == 3)
            {
                if ((CaretIndex == Text.Length) && (RightIPAddressTextBox != null))
                {
                    RightIPAddressTextBox.Focus();
                    RightIPAddressTextBox.SelectAll();
                }
            }
        }
    }
}
