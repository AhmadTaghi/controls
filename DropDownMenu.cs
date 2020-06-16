using System;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IndividualCredit.UI.Controls
{
    public partial class DropDownMenu : Label
    {
        private Label lblBtn;
        private TextBox txtSearch;
        private Label lblSearchBtn;
        private Label lblLine;
        private ListBox listBox;
        private Panel pnlSearch;
        private Panel pnlList;
        private Panel pnlMain;
        private ToolStripControlHost host;
        private ToolStripDropDown popup;
        bool filtered = false;

        public DropDownMenu()
        {
            dataTable = new DataTable();
            dataTable.Columns.Add("Key");
            dataTable.Columns.Add("Value");
            dataTable.TableNewRow += DataTable_TableNewRow;
            dataTable.RowDeleted += DataTable_RowDeleted;
            dataTable.TableCleared += DataTable_TableCleared;
            items = new DropDownItemCollection(this);

            lblBtn = new Label();
            lblBtn.BackColor = Color.Transparent;
            lblBtn.Dock = DockStyle.Right;
            lblBtn.Width = 20;
            lblBtn.ImageAlign = ContentAlignment.MiddleCenter;
            lblBtn.Image = GetDropDownIcon();
            lblBtn.MouseDown += LblBtn_MouseDown;
            lblBtn.MouseEnter += LblBtn_MouseEnter;
            lblBtn.MouseLeave += LblBtn_MouseLeave;

            txtSearch = new TextBox();
            txtSearch.BorderStyle = BorderStyle.None;
            txtSearch.Dock = DockStyle.Fill;
            txtSearch.TextChanged += TxtSearch_TextChanged;
            txtSearch.KeyDown += TxtSearch_KeyDown;

            lblSearchBtn = new Label();
            lblSearchBtn.Dock = DockStyle.Right;
            lblSearchBtn.ImageAlign = ContentAlignment.MiddleCenter;
            lblSearchBtn.Image = GetSearchIcon();
            lblSearchBtn.Width = 14;
            lblSearchBtn.MouseDown += LblSearchBtn_MouseDown;
            lblSearchBtn.MouseEnter += LblSearchBtn_MouseEnter;
            lblSearchBtn.MouseLeave += LblSearchBtn_MouseLeave;

            lblLine = new Label();
            lblLine.Dock = DockStyle.Top;
            lblLine.BackColor = Color.Gray;
            lblLine.Height = 1;

            listBox = new ListBox();
            listBox.Dock = DockStyle.Fill;
            listBox.BorderStyle = BorderStyle.None;
            listBox.BindingContext = new BindingContext();
            listBox.DataSource = dataTable;
            listBox.DisplayMember = "Value";
            listBox.ValueMember = "Key";
            listBox.MouseDown += ListBox_MouseDown;
            listBox.MouseMove += ListBox_MouseMove;
            listBox.KeyDown += ListBox_KeyDown;

            pnlSearch = new Panel();
            pnlSearch.Dock = DockStyle.Top;
            pnlSearch.Padding = new Padding(3);
            pnlSearch.MinimumSize = new Size(0, 20);
            pnlSearch.Height = 20;
            pnlSearch.Controls.Add(txtSearch);
            pnlSearch.Controls.Add(lblSearchBtn);

            pnlList = new Panel();
            pnlList.Dock = DockStyle.Fill;
            pnlList.Controls.Add(listBox);
            pnlList.Controls.Add(lblLine);

            pnlMain = new Panel();
            pnlMain.BorderStyle = BorderStyle.FixedSingle;
            pnlMain.BackColor = Color.White;
            pnlMain.Controls.Add(pnlList);
            pnlMain.Controls.Add(pnlSearch);

            host = new ToolStripControlHost(pnlMain);
            host.AutoSize = false;
            host.Padding = Padding.Empty;
            host.Margin = Padding.Empty;
            host.Width = this.Width;
            host.Height = 10;

            popup = new ToolStripDropDown();
            popup.Padding = Padding.Empty;
            popup.Margin = Padding.Empty;
            popup.Items.Add(host);
            popup.Opened += Popup_Opened;
            popup.Closed += Popup_Closed;

            this.Width = 120;
            this.Height = 20;
            this.BorderStyle = BorderStyle.FixedSingle;
            this.TextAlign = ContentAlignment.MiddleLeft;
            this.BackColor = SystemColors.Window;
            this.Padding = new Padding(0, 0, lblBtn.Width, 0);
            this.Controls.Add(lblBtn);
        }



        private void DataTable_TableNewRow(object sender, DataTableNewRowEventArgs e)
        {
            this.selectedIndex = this.Items.IndexOf(this.selectedItem);
        }

        private void DataTable_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            this.selectedIndex = this.Items.IndexOf(this.selectedItem);
            if (this.selectedIndex == -1) SetSelectedItem(null);
        }

        private void DataTable_TableCleared(object sender, DataTableClearEventArgs e)
        {
            this.selectedIndex = -1;
            SetSelectedItem(null);
        }


        private void LblBtn_MouseDown(object sender, MouseEventArgs e)
        {
            this.Focus();
            int showCount = this.Items.Count;
            if (showCount > 1)
            {
                lblLine.Visible = pnlSearch.Visible = true;
                popup.Show(this, -1, this.Height - 1);
                if (showCount > this.viewItemCount) showCount = this.viewItemCount;
                host.Height = pnlSearch.Height + listBox.ItemHeight * showCount + 3;
            }
            else
            {
                lblLine.Visible = pnlSearch.Visible = false;
                popup.Show(this, -1, this.Height - 1);
                host.Height = listBox.ItemHeight + 2;
            }
        }

        private void LblBtn_MouseLeave(object sender, EventArgs e)
        {
            lblBtn.BackColor = Color.Transparent;
        }

        private void LblBtn_MouseEnter(object sender, EventArgs e)
        {
            lblBtn.BackColor = SystemColors.GradientInactiveCaption;
        }


        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            dataTable.DefaultView.RowFilter = $@"Value LIKE '%{ txtSearch.Text.Replace("'", "''") }%'";
            filtered = txtSearch.Text != "";
            CheckSearch();
        }

        private void TxtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                SelectItem();
                popup.Hide();
            }
            else if (e.KeyCode == Keys.Down)
            {
                if (listBox.SelectedIndex < listBox.Items.Count - 1)
                {
                    listBox.Focus();
                    listBox.SelectedIndex++;
                }
            }
            else if (e.KeyCode == Keys.Up)
            {
                if (listBox.SelectedIndex > 0)
                {
                    listBox.Focus();
                    listBox.SelectedIndex--;
                }
            }
        }


        private void LblSearchBtn_MouseDown(object sender, MouseEventArgs e)
        {
            if (filtered) txtSearch.Text = "";
        }

        private void LblSearchBtn_MouseEnter(object sender, EventArgs e)
        {
            if (filtered) lblSearchBtn.BackColor = SystemColors.GradientInactiveCaption;
        }

        private void LblSearchBtn_MouseLeave(object sender, EventArgs e)
        {
            if (filtered) lblSearchBtn.BackColor = Color.Transparent;
        }


        private void ListBox_MouseMove(object sender, MouseEventArgs e)
        {
            listBox.SelectedIndex = listBox.IndexFromPoint(e.Location);
        }

        private void ListBox_MouseDown(object sender, MouseEventArgs e)
        {
            SelectItem();
            popup.Hide();
        }

        private void ListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SelectItem();
                popup.Hide();
            }
            else if (e.KeyCode == Keys.Up)
            {
                if (listBox.SelectedIndex < 1) txtSearch.Focus();
            }
        }


        private void Popup_Opened(object sender, EventArgs e)
        {
            txtSearch.Focus();
            this.Invalidate();
            this.OnGotFocus(e);
            OnOpened();
        }

        private void Popup_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            this.Invalidate();
            this.OnLostFocus(e);
        }

        private DataTable dataTable;
        internal DataTable DataSource
        {
            get { return dataTable; }
        }


        private DropDownItemCollection items;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [MergableProperty(false)]
        public DropDownItemCollection Items
        {
            get { return items; }
            set
            {
                this.dataTable.Rows.Clear();
                if (value == null) this.items.Clear();
                else
                {
                    this.items = value;
                    foreach (var item in this.items)
                    {
                        this.dataTable.Rows.Add(item.Key, item.Value);
                    }
                }
            }
        }

        private int selectedIndex = -1;
        [Browsable(false)]
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                if (value < this.Items.Count)
                {
                    if (value > -1)
                    {
                        this.selectedIndex = value;
                        SetSelectedItem(this.Items[value]);
                    }
                    else if (value == -1)
                    {
                        this.selectedIndex = -1;
                        SetSelectedItem(null);
                    }
                    else throw new IndexOutOfRangeException();
                }
                else throw new IndexOutOfRangeException();
            }
        }

        private DropDownItem selectedItem;
        [Browsable(false)]
        public DropDownItem SelectedItem
        {
            get { return selectedItem; }
            set
            {
                this.selectedIndex = this.Items.IndexOf(value);
                if (this.selectedIndex == -1) SetSelectedItem(null);
                else SetSelectedItem(value);
            }
        }


        private int viewItemCount = 12;
        [DefaultValue(12)]
        public int ViewItemCount
        {
            get { return viewItemCount; }
            set { viewItemCount = value; }
        }


        private void SelectItem()
        {
            if (listBox.SelectedItem is DataRowView) this.SelectedIndex = this.dataTable.Rows.IndexOf(((DataRowView)listBox.SelectedItem).Row);
            else
            {
                this.selectedIndex = -1;
                SetSelectedItem(null);
            }
        }

        private void SetSelectedItem(DropDownItem item)
        {
            if (this.selectedItem != item)
            {
                if (item == null)
                {
                    this.selectedItem = null;
                    base.Text = "";
                }
                else
                {
                    this.selectedItem = item;
                    base.Text = item.Value;
                }
                OnSelectedChanged();
            }
        }

        private void CheckSearch()
        {
            if (filtered != (txtSearch.Text == ""))
            {
                if (filtered) lblSearchBtn.Image = GetCloseIcon();
                else
                {
                    lblSearchBtn.Image = GetSearchIcon();
                    lblSearchBtn.BackColor = Color.Transparent;
                }
            }
        }


        public void SelectFirstItemByKey(string key)
        {
            this.SelectedItem = this.Items.FirstOrDefault(x => x.Key == key);
        }

        public void Fill(List<DropDownItem> items)
        {
            this.Items.Clear();
            foreach (var item in items)
            {
                this.Items.Add(item);
            }
        }

        public void Fill(DataTable dt)
        {
            this.Items.Clear();
            foreach (DataRow dr in dt.Rows)
            {
                this.Items.Add(new DropDownItem(dr[0].ToString(), dr[1].ToString()));
            }
        }

        public void Fill(DataTable dt, int keyColumnIndex, int valueColumnIndex)
        {
            this.Items.Clear();
            foreach (DataRow dr in dt.Rows)
            {
                this.Items.Add(new DropDownItem(dr[keyColumnIndex].ToString(), dr[valueColumnIndex].ToString()));
            }
        }

        public void Fill(string[] array)
        {
            this.Items.Clear();
            for (int i = 0; i < array.Length; i++)
            {
                this.Items.Add(new DropDownItem(i.ToString(), array[i]));
            }
        }

        public void Fill(string[][] array)
        {
            this.Items.Clear();
            foreach (var arr in array)
            {
                this.Items.Add(new DropDownItem(arr[0], arr[1]));
            }
        }

        public void Fill(string[][] array, int keyIndex, int valueIndex)
        {
            this.Items.Clear();
            foreach (var arr in array)
            {
                this.Items.Add(new DropDownItem(arr[keyIndex], arr[valueIndex]));
            }
        }


        [DefaultValue(typeof(Padding), "0, 0, 20, 0")]
        public new Padding Padding
        {
            get { return base.Padding; }
            set
            {
                if (value.Right < lblBtn.Width) value.Right = lblBtn.Width;
                base.Padding = value;
            }
        }


        [DefaultValue(typeof(ContentAlignment), "MiddleLeft")]
        public override ContentAlignment TextAlign
        {
            get { return base.TextAlign; }
            set { base.TextAlign = value; }
        }


        [DefaultValue(typeof(BorderStyle), "FixedSingle")]
        public override BorderStyle BorderStyle
        {
            get { return base.BorderStyle; }
            set { base.BorderStyle = value; }
        }

        [DefaultValue(typeof(SystemColors), "Window")]
        public override Color BackColor
        {
            get { return base.BackColor; }
            set { base.BackColor = value; }
        }


        [Browsable(false)]
        public override string Text
        {
            get { return base.Text; }
        }


        [Browsable(false)]
        public override bool AutoSize
        {
            get { return false; }
        }

        protected override void OnAutoSizeChanged(EventArgs e)
        {
            base.AutoSize = false;
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            host.Width = this.Width;
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            txtSearch.Font = listBox.Font = this.Font;
            lblSearchBtn.Width = txtSearch.Height;
            pnlSearch.Height = txtSearch.Height + 6;
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            if (this.Enabled) base.BackColor = SystemColors.Window;
            else base.BackColor = SystemColors.Control;
        }


        public event EventHandler SelectedItemChanged;
        private void OnSelectedChanged()
        {
            if (SelectedItemChanged != null) SelectedItemChanged(this, EventArgs.Empty);
        }

        public event EventHandler Opened;
        private void OnOpened()
        {
            if (Opened != null) Opened(this, EventArgs.Empty);
        }

        private Bitmap searchIcon;
        private Bitmap GetSearchIcon()
        {
            if (searchIcon == null) searchIcon = GetBitmap(searchIconStr);
            return searchIcon;
        }

        private Bitmap dropDownIcon;
        private Bitmap GetDropDownIcon()
        {
            if (dropDownIcon == null) dropDownIcon = GetBitmap(dropDownIconStr);
            return dropDownIcon;
        }

        private Bitmap closeIcon;
        private Bitmap GetCloseIcon()
        {
            if (closeIcon == null) closeIcon = GetBitmap(closeIconStr);
            return closeIcon;
        }

        private Bitmap GetBitmap(string base64)
        {
            MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(base64));
            memoryStream.Position = 0;
            return new Bitmap(memoryStream);
        }


        string searchIconStr = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAABGdBTUEAALGPC/xhBQAAACBjSFJNAAB6JgAAgIQAAPoAAACA6AAAdTAAAOpgAAA6mAAAF3CculE8AAAACXBIWXMAAA7DAAAOwwHHb6hkAAAAB3RJTUUH4woKDAM4czwpcAAAABl0RVh0U29mdHdhcmUAd3d3Lmlua3NjYXBlLm9yZ5vuPBoAAAEtSURBVDhPtZI7SgNhFIWnErXx2aRJoYWFugRFFzH9wLyZarZhr61FVmAhrsBX7ITswiJFQKKIfgfOGMOESBAPHJj/3HMf//0n+BeEYbiUpul+URQHdV2vWP4dWZatwjM4hJ/mKM/z8yiK1m2bDXXCeO+kMXzy+dXaoCzLDdvbwKDOMj5j3LMsvQtvFaPgpeVp6M4YNPb4Z3ID9I7jbzOn0LIIqnvfUgss9dqeU0sTIB4qqDtbaoH4lTxJkhxZmkALJDiCWljX8jfiON5Ef4HvcNvyNAhcQI14BzuWm+Qbx3qW29A7YxjYOPSdNbY6SxP7c/8HbVhPhVGjNkn67sG+z/OLCCrEy5xQ7Liqqi1pSmKqx6bIQr94AxLX4AP8gLuWFwOTLDPhjo9/RRB8Aa2PtyT73kPqAAAAAElFTkSuQmCC";

        string dropDownIconStr = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAABGdBTUEAALGPC/xhBQAAACBjSFJNAAB6JgAAgIQAAPoAAACA6AAAdTAAAOpgAAA6mAAAF3CculE8AAAACXBIWXMAAA7DAAAOwwHHb6hkAAAAB3RJTUUH4woKDDkytkAeFwAAABl0RVh0U29mdHdhcmUAd3d3Lmlua3NjYXBlLm9yZ5vuPBoAAACKSURBVDhPYxgFVAS5ubl8aWlpDUDMChXCACA5kBqQWqgQAgAltgDxfyBeA1IIFYYDkBhUDqRmC1QYAVJTUw2AEm+gCtYCMdyQ0NBQZqD8MqjceyDbFCqFCrAZQrRmGAAqMgLit1ANICfDnA0SM4Iqww/QXEKczegAyRDSNcMAUDPIO8Q5e6gCBgYA821dya99irsAAAAASUVORK5CYII=";

        string closeIconStr = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAAAUoAAAFKAemF+aIAAAAZdEVYdFNvZnR3YXJlAHd3dy5pbmtzY2FwZS5vcmeb7jwaAAAAxUlEQVQ4T92SawrCMBCEexLxdQntVvwrScQziN5PBLGH0dpaSVIvodk6LdJG6u9+EAizM5tn0DO0jPZG0bkQNIZUw5qRFFtBW0htjIxOzvRyI7NqMYJchTPUjpDbFKv5xBlyGFOzWQ65UTn/aHf2wO7nuZ5NraRHGRDh7Sucd4YrtKKBCyQI8khYQ7kbNLjWDRRd/m7A58bWm0dIuAabn8Zt+y4x9T1xjTPEMP5+RvdPILfhj6RFePCtwhrXrIp2kPpBELwB6YCcYXK5GkoAAAAASUVORK5CYII=";
    }


    public class DropDownItem
    {
        public DropDownItem()
        {
        }
        public DropDownItem(string key, string value)
        {
            Key = key;
            Value = value;
        }
        public string Key { get; set; }
        public string Value { get; set; }
    }


    public class DropDownItemCollection : Collection<DropDownItem>
    {
        DataTable dt;
        public DropDownItemCollection(DropDownMenu owner)
        {
            owner.Items = this;
            dt = owner.DataSource;
        }

        public new DropDownItem this[int index]
        {
            get { return this.Items[index]; }
        }

        public new void Add(DropDownItem item)
        {
            dt.Rows.Add(item.Key, item.Value);
            this.Items.Add(item);
        }

        public virtual void AddRange(params DropDownItem[] items)
        {
            foreach (var item in items)
            {
                dt.Rows.Add(item.Key, item.Value);
                this.Items.Add(item);
            }
        }

        public new void Insert(int index, DropDownItem item)
        {
            var arr = new[] { item.Key, item.Value };
            this.Items.Insert(index, item);
            DataRow row = dt.NewRow();
            row.ItemArray = arr;
            dt.Rows.InsertAt(row, index);
        }

        public new void Remove(DropDownItem item)
        {
            int index = this.Items.IndexOf(item);
            if (index > -1) RemoveAt(index);
        }

        public new void RemoveAt(int index)
        {
            this.Items.RemoveAt(index);
            this.dt.Rows.RemoveAt(index);
        }

        public new void Clear()
        {
            this.Items.Clear();
            this.dt.Rows.Clear();
        }

    }
}
