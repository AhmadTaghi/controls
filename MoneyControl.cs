using System;
using System.ComponentModel;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace IndividualCredit.UI.Controls
{
    public class MoneyControl : Control
    {
        private TextBox txtFull;
        private TextBox txtPenny;
        private Label lblSep;
        private Pen pGray;
        private Pen pBlue;
        private Regex regex;
        public MoneyControl()
        {
            regex = new Regex("^[0-9]+$");
            pGray = Pens.Gray;
            pBlue = SystemPens.HotTrack;

            txtFull = new TextBox();
            txtFull.TextAlign = HorizontalAlignment.Right;
            txtFull.BorderStyle = BorderStyle.None;
            txtFull.Text = "0";
            txtFull.KeyDown += TxtFull_KeyDown;
            txtFull.KeyPress += TxtFull_KeyPress;
            txtFull.TextChanged += TxtFull_TextChanged;
            txtFull.Validated += TxtFull_Validated;
            txtFull.GotFocus += Txt_GotFocus;
            txtFull.LostFocus += Txt_LostFocus;

            lblSep = new Label();
            lblSep.BackColor = Color.Gray;

            txtPenny = new TextBox();
            txtPenny.BorderStyle = BorderStyle.None;
            txtPenny.Width = 20;
            txtPenny.MaxLength = 2;
            txtPenny.Text = "00";
            txtPenny.KeyDown += TxtPenny_KeyDown;
            txtPenny.KeyPress += TxtPenny_KeyPress;
            txtPenny.TextChanged += TxtPenny_TextChanged;
            txtPenny.Validated += TxtPenny_Validated;
            txtPenny.GotFocus += Txt_GotFocus;
            txtPenny.LostFocus += Txt_LostFocus;

            this.Controls.Add(txtFull);
            this.Controls.Add(lblSep);
            this.Controls.Add(txtPenny);
            this.Width = 120;
            this.Height = 20;
            this.BackColor = Color.White;

            ControlsStyleCheck();
        }

        private void TxtFull_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Right)
            {
                if (txtFull.SelectionStart == txtFull.TextLength)
                {
                    txtPenny.Focus();
                    txtPenny.SelectionStart = 2;
                    txtPenny.SelectionStart = 0;
                }
            }
        }

        private void TxtFull_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (txtFull.Text == "0") txtFull.Text = "";
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void TxtFull_TextChanged(object sender, EventArgs e)
        {
            if (txtFull.Text.TrimStart('0') == "") fullValue = 0;
            else if (long.TryParse(txtFull.Text.TrimStart('0'), out long a)) fullValue = a;
            else txtFull.Text = "0";
            this.OnTextChanged(e);
        }

        private void TxtFull_Validated(object sender, EventArgs e)
        {
            if (txtFull.Text == "") txtFull.Text = "0";
        }



        private void TxtPenny_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                if (txtPenny.SelectionStart == 0)
                {
                    txtFull.Focus();
                    txtFull.SelectionStart = txtFull.TextLength;
                }
            }
        }

        private void TxtPenny_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (txtPenny.Text == "00") txtPenny.Text = "";
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void TxtPenny_TextChanged(object sender, EventArgs e)
        {
            if (txtPenny.Text.TrimStart('0') == "") pennyValue = 0;
            else if (long.TryParse(txtPenny.Text.TrimStart('0'), out long a)) pennyValue = ((decimal)a) / 100;
            else txtPenny.Text = "00";
            this.OnTextChanged(e);
        }

        private void TxtPenny_Validated(object sender, EventArgs e)
        {
            if (txtPenny.Text == "") txtPenny.Text = "00";
        }



        private void Txt_GotFocus(object sender, EventArgs e)
        {
            this.Invalidate();
            this.OnGotFocus(e);
        }

        private void Txt_LostFocus(object sender, EventArgs e)
        {
            this.Invalidate();
            this.OnLostFocus(e);
        }



        [DefaultValue(typeof(Color), "White")]
        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
                txtFull.BackColor = txtPenny.BackColor = this.BackColor;
            }
        }


        private decimal fullValue = 0;
        private decimal pennyValue = 0;
        public decimal Value
        {
            get { return fullValue + pennyValue; }
            set
            {
                string s = value.ToString("0.00").Replace(",", ".");
                txtFull.Text = s.Substring(0, s.IndexOf('.'));
                txtPenny.Text = s.Substring(s.IndexOf('.') + 1, 2);
            }
        }

        public override string Text
        {
            get
            {
                return string.Format("{0}.{1}", txtFull.Text, txtPenny.Text);
            }
            set
            {
                int index = value.IndexOf('.');
                if (index == -1) index = value.IndexOf(',');
                if (index == -1)
                {
                    txtFull.Text = value;
                    txtPenny.Text = "00";
                }
                else
                {
                    txtFull.Text = value.Substring(0, index);
                    txtPenny.Text = value.Substring(index + 1);
                }
            }
        }


        private void ControlsStyleCheck()
        {
            if (this.Height < txtFull.Height + 2) this.Height = txtFull.Height + 2;
            txtPenny.Width = txtPenny.Height + 2;
            txtFull.Top = txtPenny.Top = (this.Height - txtFull.Height) / 2;
            txtFull.Left = 2;
            txtFull.Width = this.Width - txtPenny.Width - 10;
            txtPenny.Left = this.Width - txtPenny.Width - 1;
            lblSep.Top = 1;
            lblSep.Left = this.Width - txtPenny.Width - 5;
            lblSep.Width = 1;
            lblSep.Height = this.Height - 2;
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (txtFull.Focused || txtPenny.Focused) e.Graphics.DrawRectangle(pBlue, 0, 0, this.Width - 1, this.Height - 1);
            else e.Graphics.DrawRectangle(pGray, 0, 0, this.Width - 1, this.Height - 1);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            ControlsStyleCheck();
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            ControlsStyleCheck();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.X < lblSep.Left) txtFull.Focus();
            else txtPenny.Focus();
        }


        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            if (this.Enabled) this.BackColor = SystemColors.Window;
            else this.BackColor = SystemColors.Control;
        }

    }
}
