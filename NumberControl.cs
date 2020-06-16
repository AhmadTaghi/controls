using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace IndividualCredit.UI.Controls
{
    public class NumberControl : TextBox
    {
        Regex regex;
        bool b = true;
        private long val = 0;

        public NumberControl()
        {
            regex = new Regex("[^0-9]");
            BorderStyle = BorderStyle.FixedSingle;
            this.MaxLength = 19;
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar)) e.Handled = true;
        }

        protected override void OnTextChanged(EventArgs e)
        {
            if (regex.IsMatch(this.Text))
            {
                b = false;
                this.Text = regex.Replace(this.Text, "");
                this.SelectionStart = this.TextLength;
                b = true;
                base.OnTextChanged(e);
            }
            else if (b) base.OnTextChanged(e);
        }

        [DefaultValue(0)]
        public long Value
        {
            get
            {
                if (!long.TryParse(this.Text.TrimStart('0'), out val)) val = 0;
                return val;
            }
            set { this.Text = value.ToString(); }
        }

        [DefaultValue(19)]
        public override int MaxLength
        {
            get { return base.MaxLength; }
            set
            {
                if (value > 19) base.MaxLength = 19;
                else base.MaxLength = value;
            }
        }

        [Browsable(false)]
        public override bool Multiline
        {
            get { return false; }
        }

    }
}
