using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace IndividualCredit.UI.Controls
{
    public class DateControl : MaskedTextBox
    {
        private Label btn;
        private ToolStripDropDown popup;
        private MonthCalendar calendar;
        private CultureInfo engCulture;
        private string[] formats = new string[]
        {
            "dd.MM.yyyy",
            "dd/MM/yyyy",
            "dd-MM-yyyy",
            "yyyy.MM.dd",
            "yyyy/MM/dd",
            "yyyy-MM-dd",
            "yyyyMMdd",
            "ddMMyyyy"
        };

        public DateControl()
        {
            engCulture = CultureInfo.GetCultureInfo("en");
            btn = new Label();
            calendar = new MonthCalendar();
            popup = new ToolStripDropDown();

            btn.Padding = new Padding(1, 0, 0, 0);
            btn.Width = 19;
            btn.Top = 1;
            btn.Image = GetIcon();
            btn.MouseDown += Btn_MouseDown;
            btn.MouseEnter += Btn_MouseEnter;
            btn.MouseLeave += Btn_MouseLeave;
            btn.Cursor = Cursors.Default;

            calendar.MaxSelectionCount = 1;
            calendar.DateSelected += Calendar_DateSelected;

            popup.Margin = Padding.Empty;
            popup.Padding = Padding.Empty;
            var host = new ToolStripControlHost(calendar);
            host.Margin = Padding.Empty;
            host.Padding = Padding.Empty;
            popup.Items.Add(host);

            this.BorderStyle = BorderStyle.FixedSingle;
            this.Width = 120;
            this.Height = 20;
            this.Format = "dd.MM.yyyy";
            this.Controls.Add(btn);

            ControlsStyleCheck();
        }

        private void Btn_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.Date == null) this.calendar.SetDate(DateTime.Now);
            else this.calendar.SetDate(this.Date.Value);
            popup.Show(this, -1, this.Height);
        }

        private void Btn_MouseEnter(object sender, EventArgs e)
        {
            btn.BackColor = SystemColors.GradientInactiveCaption;
        }

        private void Btn_MouseLeave(object sender, EventArgs e)
        {
            btn.BackColor = Color.Transparent;
        }


        private void Calendar_DateSelected(object sender, DateRangeEventArgs e)
        {
            this.Date = e.Start;
            popup.Hide();
        }

        private DateTime? date;
        [Browsable(false)]
        public DateTime? Date
        {
            get { return date; }
            set
            {
                if (date != value)
                {
                    date = value;
                    OnDateChanged();
                }
                SetMaskText();
            }
        }

        private string format;
        [DefaultValue("dd.MM.yyyy")]
        public string Format
        {
            get { return format; }
            set
            {
                if (formats.Contains(value))
                {
                    format = value;
                    base.Mask = format.Replace('d', '0').Replace('M', '0').Replace('y', '0');
                    SetMaskText();
                }
            }
        }


        public override string Text
        {
            get { return base.Text; }
            set
            {
                SetValue(value);
                SetMaskText();
            }
        }


        [DefaultValue(typeof(BorderStyle), "FixedSingle")]
        public new BorderStyle BorderStyle
        {
            get { return base.BorderStyle; }
            set
            {
                if (value == BorderStyle.None) base.BorderStyle = BorderStyle.None;
                else base.BorderStyle = BorderStyle.FixedSingle;
            }
        }


        [DefaultValue("00.00.0000")]
        [Browsable(false)]
        public new string Mask
        {
            get { return base.Mask; }
        }

        private void SetValue(string txt)
        {
            try
            {
                this.Date = new DateTime(
                    int.Parse(txt.Substring(format.IndexOf('y'), 4)),
                    int.Parse(txt.Substring(format.IndexOf('M'), 2)),
                    int.Parse(txt.Substring(format.IndexOf('d'), 2))
                    );
            }
            catch { this.Date = null; }
        }

        private void SetMaskText()
        {
            if (date == null) base.Text = "";
            else base.Text = date.Value.ToString(this.format, engCulture);
        }

        private void ControlsStyleCheck()
        {
            btn.Height = this.Height - 2;
            btn.Left = this.Width - 20;
        }

        protected override void OnTextChanged(EventArgs e)
        {
            if (this.MaskCompleted) SetValue(this.Text);
            base.OnTextChanged(e);
        }

        protected override void OnValidating(CancelEventArgs e)
        {
            if (!this.MaskCompleted) SetValue(this.Text);
            base.OnValidating(e);
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

        private Bitmap GetIcon()
        {
            string base64 = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAZdEVYdFNvZnR3YXJlAEFkb2JlIEltYWdlUmVhZHlxyWU8AAABh0lEQVQ4T6WRS0tCURSFPcNmjoKCqOj5DwL/gRNnThX/gdSkgZOImjeOIAoh4iaIIImJIMolEbFEEUSIHghBE1EEk7tb60Dmid5d+Nz3rP2xPHBdIvIvPgx/g/4JK+VaB3jWQJDTwvkBcI7n9OgbBTcIbpXavlAqU61WhfNOqbNXxnN69I2COgJwfu3zSTQYlCuvV5xIZATPzLmnR98osHE3UHgKBOTS45G23y/dUGgEz8y5p0ffKFhFAMJ4F+cL+NCjj9e3ghUEYOPZceQ0nZaTVEqiyaQcJRJyGI/LQSwm+5Yl3NOjbxQsIwCb/eFQF5RaLV1QqNd1QaZc1gXc06NvFCwhAJHOYCDpUklSxaIkbVsS+bzEczmJZbNiZTLCPT36RsEiArD11O/rf+9g2o2Gnjl8Pk7egnt69I2CBQRgt93r6YJis6kL8rWaLshWKrqAe3r0jYIJfNd5pfYeccX7bvdTuKdH3yjAMzup1M6MUsffQQ/+3PsCN5gC0z+Antso+DviegFltkyP/mR3/AAAAABJRU5ErkJggg==";
            MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(base64));
            memoryStream.Position = 0;
            return new Bitmap(memoryStream);
        }


        public event EventHandler DateChanged;

        private void OnDateChanged()
        {
            if (DateChanged != null)
            {
                DateChanged(this, EventArgs.Empty);
            }
        }

    }
}
