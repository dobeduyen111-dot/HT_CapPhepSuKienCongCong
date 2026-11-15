using System.Drawing;
using System.Windows.Forms;

namespace Viewparty.Forms.Organizer
{
    partial class VenueSearchForm
    {
        private System.ComponentModel.IContainer components = null;
        private Label lblTitle;
        private Label lblSubtitle;
        private Label lblKeyword;
        private Label lblStart;
        private Label lblEnd;
        private Label lblAudienceSize;
        private Label lblResults;
        private TextBox txtKeyword;
        private DateTimePicker dtpStart;
        private DateTimePicker dtpEnd;
        private NumericUpDown numAudienceSize;
        private DataGridView gridVenues;
        private Button btnSearch;
        private Button btnSelect;
        private Button btnCancel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            Text = "Chọn địa điểm tổ chức";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = true;
            MinimizeBox = false;
            AutoScaleMode = AutoScaleMode.Font;
            AutoScaleDimensions = new SizeF(9F, 9F);
            ClientSize = new Size(1080, 690);
            MinimumSize = new Size(900, 600);
            BackColor = ColorTranslator.FromHtml("#F5F8FC");
            Font = new Font("Arial", 9.5F);

            lblTitle = new Label
            {
                Text = "Tra cứu địa điểm và kiểm tra lịch trống",
                Location = new Point(24, 18),
                AutoSize = true,
                Font = new Font("Arial", 19, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#1D3348")
            };
            lblSubtitle = new Label
            {
                Text = "Chọn địa điểm từ danh sách hoặc lọc theo khu vực, sức chứa và khung thời gian dự kiến.",
                Location = new Point(26, 56),
                AutoSize = true,
                MaximumSize = new Size(1020, 0),
                AutoEllipsis = true,
                ForeColor = ColorTranslator.FromHtml("#6B7C8D")
            };

            lblKeyword = FieldLabel("Chọn địa điểm / Từ khóa", 0, 0);
            txtKeyword = new TextBox
            {
                Size = new Size(300, 30),
                MaxLength = 100,
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };
            lblStart = FieldLabel("Ngày giờ bắt đầu", 0, 0);
            dtpStart = new DateTimePicker
            {
                Size = new Size(210, 30),
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy HH:mm",
                ShowUpDown = true
            };
            lblEnd = FieldLabel("Ngày giờ kết thúc", 0, 0);
            dtpEnd = new DateTimePicker
            {
                Size = new Size(210, 30),
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy HH:mm",
                ShowUpDown = true
            };
            lblAudienceSize = FieldLabel("Quy mô khách", 0, 0);
            numAudienceSize = new NumericUpDown
            {
                Size = new Size(120, 30),
                Minimum = 1,
                Maximum = 10000,
                ThousandsSeparator = true,
                Value = 1000
            };
            btnSearch = new Button
            {
                Text = "Kiểm tra khả dụng",
                Size = new Size(140, 36),
                Dock = DockStyle.Fill,
                FlatStyle = FlatStyle.Flat,
                BackColor = ColorTranslator.FromHtml("#137A78"),
                ForeColor = Color.White,
                Font = new Font("Arial", 9.5F, FontStyle.Bold)
            };
            btnSearch.FlatAppearance.BorderSize = 0;

            lblResults = new Label
            {
                Text = "Kết quả tra cứu",
                Location = new Point(26, 194),
                AutoSize = true,
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#1D3348")
            };
            gridVenues = new DataGridView
            {
                Location = new Point(26, 224),
                Size = new Size(1022, 360),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                MultiSelect = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoGenerateColumns = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                GridColor = ColorTranslator.FromHtml("#DBE4ED"),
                ColumnHeadersHeight = 42,
                EnableHeadersVisualStyles = false
            };
            gridVenues.RowTemplate.Height = 40;
            gridVenues.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#EFF4F9");
            gridVenues.ColumnHeadersDefaultCellStyle.ForeColor = ColorTranslator.FromHtml("#6B7C8D");
            gridVenues.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 9, FontStyle.Bold);
            gridVenues.DefaultCellStyle.Font = new Font("Arial", 9.5F);
            gridVenues.DefaultCellStyle.SelectionBackColor = ColorTranslator.FromHtml("#E9F7F3");
            gridVenues.DefaultCellStyle.SelectionForeColor = ColorTranslator.FromHtml("#1D3348");
            gridVenues.AlternatingRowsDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#FBFCFE");
            gridVenues.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "VenueName",
                HeaderText = "Tên địa điểm",
                DataPropertyName = "ten_dia_diem",
                FillWeight = 22
            });
            gridVenues.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "VenueAddress",
                HeaderText = "Địa chỉ",
                DataPropertyName = "dia_chi",
                FillWeight = 28
            });
            gridVenues.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "VenueCapacity",
                HeaderText = "Sức chứa",
                DataPropertyName = "suc_chua",
                FillWeight = 12
            });
            gridVenues.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ScheduleWarning",
                HeaderText = "Cảnh báo lịch (tham khảo)",
                DataPropertyName = "canh_bao_lich",
                FillWeight = 24
            });

            btnSelect = new Button
            {
                Text = "Chọn địa điểm",
                Location = new Point(716, 615),
                Size = new Size(150, 38),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                FlatStyle = FlatStyle.Flat,
                BackColor = ColorTranslator.FromHtml("#137A78"),
                ForeColor = Color.White,
                Font = new Font("Arial", 9.5F, FontStyle.Bold),
                Enabled = false
            };
            btnSelect.FlatAppearance.BorderSize = 0;
            btnCancel = new Button
            {
                Text = "Hủy",
                Location = new Point(884, 615),
                Size = new Size(164, 38),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                FlatStyle = FlatStyle.Flat,
                BackColor = ColorTranslator.FromHtml("#E9F2F4"),
                ForeColor = ColorTranslator.FromHtml("#1D3348"),
                Font = new Font("Arial", 9.5F, FontStyle.Bold)
            };
            btnCancel.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#DBE4ED");

            var filters = new TableLayoutPanel
            {
                Location = new Point(26, 100),
                Size = new Size(1022, 78),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                ColumnCount = 5,
                RowCount = 2,
                Padding = new Padding(0),
                BackColor = Color.Transparent
            };
            filters.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 24));
            filters.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 22));
            filters.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 22));
            filters.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16));
            filters.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16));
            filters.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));
            filters.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            filters.Controls.Add(lblKeyword, 0, 0);
            filters.Controls.Add(lblStart, 1, 0);
            filters.Controls.Add(lblEnd, 2, 0);
            filters.Controls.Add(lblAudienceSize, 3, 0);
            filters.Controls.Add(btnSearch, 4, 0);
            filters.Controls.Add(txtKeyword, 0, 1);
            filters.Controls.Add(dtpStart, 1, 1);
            filters.Controls.Add(dtpEnd, 2, 1);
            filters.Controls.Add(numAudienceSize, 3, 1);
            filters.SetRowSpan(btnSearch, 2);
            foreach (Control control in new Control[] { lblKeyword, lblStart, lblEnd, lblAudienceSize })
                control.Dock = DockStyle.Fill;
            foreach (Control control in new Control[] { txtKeyword, dtpStart, dtpEnd, numAudienceSize })
            {
                control.Dock = DockStyle.Fill;
                control.Margin = new Padding(0, 2, 8, 0);
            }

            Controls.AddRange(new Control[]
            {
                lblTitle, lblSubtitle, filters,
                lblResults, gridVenues, btnSelect, btnCancel
            });
            AcceptButton = btnSearch;
            CancelButton = btnCancel;
            ResumeLayout(false);
        }

        private static Label FieldLabel(string text, int x, int y)
        {
            return new Label
            {
                Text = text,
                Location = new Point(x, y),
                AutoSize = true,
                Font = new Font("Arial", 9, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#6B7C8D")
            };
        }
    }
}
