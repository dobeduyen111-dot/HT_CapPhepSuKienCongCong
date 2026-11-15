using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Viewparty.Models;

namespace Viewparty.Forms.Organizer
{
    internal sealed class DraftVenueSelectionForm : Form
    {
        private readonly DataGridView grid;
        public HoSoCapPhep SelectedDraft { get; private set; }

        public DraftVenueSelectionForm(IList<HoSoCapPhep> drafts)
        {
            if (drafts == null) throw new ArgumentNullException("drafts");

            Text = "Chọn hồ sơ Nháp";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.Sizable;
            MinimizeBox = false;
            MaximizeBox = false;
            AutoScaleMode = AutoScaleMode.Font;
            AutoScaleDimensions = new SizeF(9F, 9F);
            MinimumSize = new Size(720, 390);
            ClientSize = new Size(760, 420);
            BackColor = ColorTranslator.FromHtml("#F5F8FC");
            Font = new Font("Arial", 9.5f);

            Controls.Add(new Label
            {
                Text = "Chọn hồ sơ Nháp để gán địa điểm",
                Location = new Point(20, 18),
                AutoSize = true,
                Font = new Font("Arial", 17, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#1D3348")
            });
            Controls.Add(new Label
            {
                Text = "Chỉ liệt kê hồ sơ Nháp thuộc tài khoản đang đăng nhập.",
                Location = new Point(22, 53),
                AutoSize = true,
                ForeColor = ColorTranslator.FromHtml("#6B7C8D")
            });

            grid = new DataGridView
            {
                Location = new Point(20, 84),
                Size = new Size(720, 260),
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
                ColumnHeadersHeight = 40,
                EnableHeadersVisualStyles = false
            };
            grid.RowTemplate.Height = 38;
            grid.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#EFF4F9");
            grid.ColumnHeadersDefaultCellStyle.ForeColor = ColorTranslator.FromHtml("#6B7C8D");
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 9F, FontStyle.Bold);
            grid.DefaultCellStyle.SelectionBackColor = ColorTranslator.FromHtml("#E9F7F3");
            grid.DefaultCellStyle.SelectionForeColor = ColorTranslator.FromHtml("#1D3348");
            grid.AlternatingRowsDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#FBFCFE");
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Mã hồ sơ", DataPropertyName = "ma_ho_so", FillWeight = 15
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Tên sự kiện", DataPropertyName = "ten_su_kien", FillWeight = 35
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Quy mô", DataPropertyName = "quy_mo", FillWeight = 15
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Bắt đầu", DataPropertyName = "bat_dau", FillWeight = 20,
                DefaultCellStyle = { Format = "dd/MM/yyyy HH:mm" }
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Kết thúc", DataPropertyName = "ket_thuc", FillWeight = 20,
                DefaultCellStyle = { Format = "dd/MM/yyyy HH:mm" }
            });
            grid.DataSource = drafts;
            grid.CellDoubleClick += (s, e) => { if (e.RowIndex >= 0) SelectDraft(); };
            Controls.Add(grid);

            var select = new Button
            {
                Text = "Tiếp tục",
                Location = new Point(514, 360),
                Size = new Size(108, 36),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                FlatStyle = FlatStyle.Flat,
                BackColor = ColorTranslator.FromHtml("#137A78"),
                ForeColor = Color.White,
                Font = new Font("Arial", 9.5F, FontStyle.Bold),
                DialogResult = DialogResult.None
            };
            select.FlatAppearance.BorderSize = 0;
            select.Click += (s, e) => SelectDraft();
            var cancel = new Button
            {
                Text = "Hủy",
                Location = new Point(632, 360),
                Size = new Size(108, 36),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                FlatStyle = FlatStyle.Flat,
                BackColor = ColorTranslator.FromHtml("#E9F2F4"),
                ForeColor = ColorTranslator.FromHtml("#1D3348"),
                Font = new Font("Arial", 9.5F, FontStyle.Bold),
                DialogResult = DialogResult.Cancel
            };
            cancel.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#DBE4ED");
            Controls.Add(select);
            Controls.Add(cancel);
            AcceptButton = select;
            CancelButton = cancel;
        }

        private void SelectDraft()
        {
            SelectedDraft = grid.CurrentRow == null
                ? null
                : grid.CurrentRow.DataBoundItem as HoSoCapPhep;
            if (SelectedDraft == null)
            {
                MessageBox.Show(this, "Chọn một hồ sơ Nháp trong danh sách.",
                    "Chưa chọn hồ sơ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
