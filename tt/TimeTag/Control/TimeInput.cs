using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace TimeTag
{
    public class TimeInput: TextBox
    {
        public EventHandler ValueChanged
        {
            get;
            set;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.KeyDown += onKeyDown;
            this.KeyUp += onKeyUp;
            this.LostFocus += onLostFocus;
        }

        private void onKeyDown(object sender, KeyEventArgs e)
        {
            if (isNavigationKey(e.Key))
            {
                return;
            }

            if (!string.IsNullOrEmpty(SelectedText) && SelectedText.Length > 0)
            {
                return;
            }

            if (!isKeyValid(this.Text, e.Key))
            {
                e.Handled = true;
                return;
            }

            if (isPeriod(e.Key))
            {
                var caret = this.CaretIndex;
                if (!this.Text.Contains(":") && caret == 2)
                {
                    this.Text = this.Text.Insert(caret, ":");
                    this.CaretIndex = caret + 1;
                }
                e.Handled = true;
                return;
            }
            else if (this.Text.Replace(":", "").Length >= 4)
            {
                e.Handled = true;
                return;
            }
        }

        private void onKeyUp(object sender, KeyEventArgs e)
        {
            if (this.Text.IndexOf(":") > -1 && this.Text.IndexOf(":") != 2 && !isNavigationKey(e.Key))
            {
                var caret = this.CaretIndex;
                this.Text = this.Text.Replace(":", "");
                if (caret >= 2)
                {
                    caret--;
                }
                this.CaretIndex = caret;
            }

            if (this.Text.Length >= 3 && !this.Text.Contains(":"))
            {
                var caret = this.CaretIndex;
                this.Text = this.Text.Insert(2, ":");
                if (caret >= 2)
                {
                    caret++;
                }
                this.CaretIndex = caret;
            }

            if (this.Text.Length <= 3 && this.Text.Contains(":"))
            {
                var caret = this.CaretIndex;
                this.Text = this.Text.Replace(":", "");
                if (caret >= 2)
                {
                    caret--;
                }
                this.CaretIndex = caret;
            }
            ValidateInput();

            if (this.ValueChanged != null)
            {
                this.ValueChanged(this, EventArgs.Empty);
            }
        }

        private void onLostFocus(object sender, RoutedEventArgs e)
        {
            if (Text.Length >= 3 && !Text.Contains(":"))
            {
                Text = Text.Insert(Text.Length - 2, ":");
            }
            if (Text.Length == 2)
            {
                Text += ":00";
            }
            if (Text.Length == 4)
            {
                Text += "0";
            }
            if (Text.Length == 1)
            {
                Text = "0" + Text + ":00";
            }
            ValidateInput();
            if (this.ValueChanged != null)
            {
                this.ValueChanged(this, EventArgs.Empty);
            }
        }

        private bool isKeyValid(string time, Key key)
        {
            if (key >= Key.NumPad0 && key <= Key.NumPad9)
            {
                return true;
            }

            if (key >= Key.D0 && key <= Key.D9)
            {
                return true;
            }

            if (isPeriod(key))
            {
                return true;
            }

            if (key == Key.Tab || key == Key.Enter || key == Key.Back)
            {
                return true;
            }

            return false;
        }

        private bool isNavigationKey(Key key)
        {
            return key == Key.Tab || key == Key.Enter || key == Key.Back || key == Key.Delete || key == Key.Left || key == Key.Right;
        }

        private bool isPeriod(Key key)
        {
            return key == Key.OemComma || key == Key.OemSemicolon || key == Key.OemPeriod;
        }

        private bool ValidateInput()
        {
            var isValid = IsValidTime();
            if (!isValid)
            {
                Background = Brushes.Red;
            }
            else
            {
                Background = Brushes.White;
            }
            return isValid;
        }

        public bool IsValidTime()
        {
            if (string.IsNullOrEmpty(Text))
            {
                return true;
            }
            DateTime result;
            return DateTime.TryParseExact(Text, "HH:mm", CultureInfo.InvariantCulture,
                                          DateTimeStyles.None, out result);
        }

        private void MoveToNextUIElement(KeyEventArgs e)
        {
            // Creating a FocusNavigationDirection object and setting it to a
            // local field that contains the direction selected.
            FocusNavigationDirection focusDirection = FocusNavigationDirection.Next;

            // MoveFocus takes a TraveralReqest as its argument.
            TraversalRequest request = new TraversalRequest(focusDirection);

            // Gets the element with keyboard focus.
            UIElement elementWithFocus = Keyboard.FocusedElement as UIElement;

            // Change keyboard focus.
            if (elementWithFocus != null)
            {
                if (elementWithFocus.MoveFocus(request)) e.Handled = true;
            }
        }

    }
}
