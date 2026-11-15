using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Viewparty.Data;
using Viewparty.Models;
using Viewparty.Services;

namespace Viewparty.Forms.Organizer
{
    public partial class VenueSearchForm : Form
    {
        private readonly DiaDiemService diaDiemService;
        private readonly ComboBox cmbVenueQuickPick = new ComboBox();
        private IList<DiaDiem> allVenues = new List<DiaDiem>();
        private bool hasDraftContext;

        public event Action CancelRequested;

        internal AuthenticatedUser CurrentUser { get; private set; }
        public long DraftId { get; private set; }
        public DiaDiem SelectedVenue { get; private set; }

        public VenueSearchForm() : this(new DiaDiemService())
        {
        }

        public VenueSearchForm(DiaDiemService diaDiemService)
        {
            this.diaDiemService = diaDiemService ?? throw new ArgumentNullException("diaDiemService");
            InitializeComponent();
            btnCancel.Text = "← Quay lại";

            ReplaceKeywordWithCombo();

            btnSearch.Click += Search_Click;
            btnSelect.Click += Select_Click;
            btnCancel.Click += (s, e) =>
            {
                if (CancelRequested != null)
                {
                    CancelRequested();
                    return;
                }
                DialogResult = DialogResult.Cancel;
                Close();
            };
            gridVenues.SelectionChanged += (s, e) => UpdateSelectButton();
            gridVenues.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex >= 0 && btnSelect.Enabled) SelectVenue();
            };
            dtpStart.Value = DateTime.Now.AddDays(45);
            dtpEnd.Value = dtpStart.Value.AddHours(4);

            LoadVenueList();
            RunSearchInternal();
        }

        private void ReplaceKeywordWithCombo()
        {
            var parentTable = txtKeyword.Parent as TableLayoutPanel;
            if (parentTable != null)
            {
                cmbVenueQuickPick.DropDownStyle = ComboBoxStyle.DropDown;
                cmbVenueQuickPick.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                cmbVenueQuickPick.AutoCompleteSource = AutoCompleteSource.ListItems;
                cmbVenueQuickPick.Dock = DockStyle.Fill;
                cmbVenueQuickPick.Margin = txtKeyword.Margin;
                cmbVenueQuickPick.SelectedIndexChanged += (s, e) =>
                {
                    if (cmbVenueQuickPick.SelectedIndex <= 0)
                        txtKeyword.Text = string.Empty;
                    else if (cmbVenueQuickPick.SelectedIndex - 1 < allVenues.Count)
                        txtKeyword.Text = allVenues[cmbVenueQuickPick.SelectedIndex - 1].ten_dia_diem;
                    RunSearchInternal();
                };
                cmbVenueQuickPick.TextChanged += (s, e) =>
                {
                    if (!cmbVenueQuickPick.Text.StartsWith("--"))
                        txtKeyword.Text = cmbVenueQuickPick.Text.Trim();
                };
                txtKeyword.Visible = false;
                parentTable.Controls.Remove(txtKeyword);
                parentTable.Controls.Add(cmbVenueQuickPick, 0, 1);
            }
        }

        private void LoadVenueList()
        {
            try
            {
                allVenues = diaDiemService.GetAllActive();
                cmbVenueQuickPick.Items.Clear();
                cmbVenueQuickPick.Items.Add("-- Tất cả địa điểm --");
                foreach (var v in allVenues)
                {
                    cmbVenueQuickPick.Items.Add(v.ten_dia_diem + " — " + v.dia_chi + " (" + v.suc_chua.ToString("N0") + " chỗ)");
                }
                cmbVenueQuickPick.SelectedIndex = 0;
            }
            catch
            {
                allVenues = new List<DiaDiem>();
            }
        }

        internal VenueSearchForm(AuthenticatedUser user) : this()
        {
            CurrentUser = user;
        }

        public VenueSearchForm(
            AuthenticatedUser user, long draftId,
            DateTimeOffset start, DateTimeOffset end, int audienceSize) : this()
        {
            CurrentUser = user;
            DraftId = draftId;
            hasDraftContext = user != null &&
                string.Equals(user.RoleCode, "NGUOI_TO_CHUC", StringComparison.Ordinal) &&
                user.AccountId > 0 && draftId > 0;

            dtpStart.Value = start.LocalDateTime;
            dtpEnd.Value = end.LocalDateTime;
            if (audienceSize >= (int)numAudienceSize.Minimum &&
                audienceSize <= (int)numAudienceSize.Maximum)
                numAudienceSize.Value = audienceSize;

            RunSearchInternal();
        }

        private void Search_Click(object sender, EventArgs e)
        {
            RunSearchInternal();
        }

        private void RunSearchInternal()
        {
            btnSearch.Enabled = false;
            try
            {
                DateTimeOffset start = new DateTimeOffset(dtpStart.Value);
                DateTimeOffset end = new DateTimeOffset(dtpEnd.Value);
                int audienceSize = Decimal.ToInt32(numAudienceSize.Value);
                string keyword = txtKeyword.Text;
                if (cmbVenueQuickPick.SelectedIndex > 0 && cmbVenueQuickPick.SelectedIndex - 1 < allVenues.Count)
                    keyword = allVenues[cmbVenueQuickPick.SelectedIndex - 1].ten_dia_diem;
                else if (!string.IsNullOrWhiteSpace(cmbVenueQuickPick.Text) && !cmbVenueQuickPick.Text.StartsWith("--"))
                    keyword = cmbVenueQuickPick.Text.Trim();

                IList<DiaDiem> results = diaDiemService.Search(
                    keyword, start, end, audienceSize);

                gridVenues.DataSource = null;
                gridVenues.DataSource = results;
                UpdateSelectButton();

                lblResults.Text = results.Count == 0
                    ? "Không tìm thấy địa điểm đang hoạt động đủ sức chứa."
                    : "Danh sách địa điểm phù hợp (" + results.Count + " địa điểm)";
            }
            catch (DiaDiemValidationException ex)
            {
                MessageBox.Show(this, ex.Message, "Bộ lọc chưa hợp lệ",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                LocalExceptionLogger.Write(ex);
            }
            finally
            {
                if (!IsDisposed) btnSearch.Enabled = true;
            }
        }

        private void Select_Click(object sender, EventArgs e)
        {
            SelectVenue();
        }

        private void SelectVenue()
        {
            var venue = gridVenues.CurrentRow == null
                ? null
                : gridVenues.CurrentRow.DataBoundItem as DiaDiem;
            if (venue == null)
            {
                MessageBox.Show(this, "Chọn một địa điểm trong danh sách.",
                    "Chưa chọn địa điểm", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int audienceSize = Decimal.ToInt32(numAudienceSize.Value);
            if (!venue.trang_thai || venue.suc_chua < audienceSize)
            {
                MessageBox.Show(this, "Địa điểm đã ngừng hoạt động hoặc không đủ sức chứa theo quy mô đã chọn.",
                    "Không thể chọn địa điểm", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!hasDraftContext)
            {
                MessageBox.Show(this,
                    "Hãy đăng nhập tài khoản Người tổ chức và tạo hồ sơ Nháp để gán địa điểm.",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            btnSelect.Enabled = false;
            try
            {
                diaDiemService.SelectForDraft(CurrentUser, DraftId, venue.ma_dia_diem);
                SelectedVenue = venue;
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                LocalExceptionLogger.Write(ex);
                MessageBox.Show(this, ex.Message,
                    "Không thể gán địa điểm", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                if (!IsDisposed) UpdateSelectButton();
            }
        }

        private void UpdateSelectButton()
        {
            var venue = gridVenues.CurrentRow == null
                ? null
                : gridVenues.CurrentRow.DataBoundItem as DiaDiem;
            btnSelect.Visible = hasDraftContext;
            btnSelect.Enabled = hasDraftContext && venue != null &&
                venue.trang_thai && venue.suc_chua >= Decimal.ToInt32(numAudienceSize.Value);
        }
    }
}
