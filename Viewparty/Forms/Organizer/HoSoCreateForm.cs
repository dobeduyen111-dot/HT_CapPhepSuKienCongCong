using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Viewparty.Data;
using Viewparty.Models;
using Viewparty.Services;

namespace Viewparty.Forms.Organizer
{
    public partial class HoSoCreateForm : Form
    {
        private readonly HoSoService hoSoService = new HoSoService();
        private readonly DiaDiemService diaDiemService = new DiaDiemService();
        private readonly List<HoSoFileUpload> newDocuments = new List<HoSoFileUpload>();
        private IList<DiaDiem> activeVenues = new List<DiaDiem>();
        private HoSoDraftData draft;
        private string newProjectionAttachmentPath;
        private bool removeProjectionAttachment;
        private readonly ComboBox cmbEventType = new ComboBox();
        private readonly ComboBox cmbVenue = new ComboBox();
        private readonly TextBox txtOrganizerUnit = new TextBox();
        private readonly TextBox txtEventName = new TextBox();
        private readonly DateTimePicker dtpStart = new DateTimePicker();
        private readonly DateTimePicker dtpEnd = new DateTimePicker();
        private readonly NumericUpDown numAudienceSize = new NumericUpDown();
        private readonly TextBox txtEventDescription = new TextBox();
        private readonly ComboBox cmbCommercialForm = new ComboBox();
        private readonly CheckBox chkSponsored = new CheckBox();
        private readonly TextBox txtSponsorInfo = new TextBox();
        private readonly ComboBox cmbTicketType = new ComboBox();
        private readonly NumericUpDown numTicketCount = new NumericUpDown();
        private readonly NumericUpDown numTicketPrice = new NumericUpDown();
        private readonly TextBox txtTicketChannel = new TextBox();
        private readonly TextBox txtTicketControl = new TextBox();
        private readonly TextBox txtRightsIssuer = new TextBox();
        private readonly TextBox txtRightsDocumentNumber = new TextBox();
        private readonly ComboBox txtRightsType = new ComboBox();
        private readonly TextBox txtSignalSource = new TextBox();
        private readonly TextBox txtRightsContent = new TextBox();
        private readonly ComboBox txtRightsScope = new ComboBox();
        private readonly DateTimePicker dtpRightsStart = new DateTimePicker();
        private readonly DateTimePicker dtpRightsEnd = new DateTimePicker();
        private readonly Label lblRightsAttachment = new Label();
        private readonly TextBox txtRightsAttachmentPath = new TextBox();
        private readonly TextBox txtMedicalPlan = new TextBox();
        private readonly TextBox txtSecurityPlan = new TextBox();
        private readonly TextBox txtEvacuationPlan = new TextBox();
        private readonly TextBox txtIncidentPlan = new TextBox();
        private readonly ComboBox cmbDocumentType = new ComboBox();
        private readonly DataGridView gridDocuments = new DataGridView();
        private readonly Label lblVenue = new Label();
        private readonly Label lblStatus = new Label();
        private readonly Button btnExportVB01 = new Button();
        private readonly Button btnChooseVenue = new Button();
        private readonly Button btnSaveDraft = new Button();
        private readonly Button btnSubmit = new Button();

        public event Action RequestClose;
        public event Action<long> RequestOpenVenueSearch;

        internal AuthenticatedUser CurrentUser { get; private set; }
        public long DraftId { get; private set; }
        public long? SelectedVenueId { get; private set; }

        public HoSoCreateForm()
        {
            InitializeComponent();
            ConfigureControls();
            BuildForm();
            ResetToNewDraft();
        }

        internal HoSoCreateForm(AuthenticatedUser user) : this()
        {
            CurrentUser = user;
            LoadEventTypes();
            LoadVenues();
        }

        internal HoSoCreateForm(AuthenticatedUser user, long draftId) : this(user)
        {
            if (draftId <= 0) throw new ArgumentOutOfRangeException("draftId");
            LoadDraft(draftId);
        }

        private void CloseOrExit()
        {
            if (RequestClose != null)
            {
                RequestClose();
                return;
            }
            Close();
        }

        private void ConfigureControls()
        {
            cmbEventType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbVenue.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCommercialForm.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTicketType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbDocumentType.DropDownStyle = ComboBoxStyle.DropDownList;

            txtRightsType.DropDownStyle = ComboBoxStyle.DropDown;
            txtRightsType.Items.AddRange(new object[]
            {
                "Quyền trình chiếu công cộng phi thương mại",
                "Quyền trình chiếu công cộng thương mại",
                "Quyền phát sóng & tiếp sóng trực tiếp"
            });

            txtRightsScope.DropDownStyle = ComboBoxStyle.DropDown;
            txtRightsScope.Items.AddRange(new object[]
            {
                "TOAN_QUOC",
                "TP.HCM",
                "Trung tâm Sự kiện Mẫu A",
                "Nhà thi đấu Mẫu B",
                "Sân vận động Mẫu C"
            });

            ConfigureDate(dtpStart);
            ConfigureDate(dtpEnd);
            ConfigureDate(dtpRightsStart);
            ConfigureDate(dtpRightsEnd);
            dtpRightsStart.ShowCheckBox = false;
            dtpRightsEnd.ShowCheckBox = false;
            numAudienceSize.Minimum = 0;
            numAudienceSize.Maximum = 10000;
            numAudienceSize.ThousandsSeparator = true;
            numTicketCount.Minimum = 0;
            numTicketCount.Maximum = 10000;
            numTicketCount.ThousandsSeparator = true;
            numTicketPrice.Minimum = 0;
            numTicketPrice.Maximum = 1000000000;
            numTicketPrice.DecimalPlaces = 2;
            numTicketPrice.ThousandsSeparator = true;

            AddChoices(cmbCommercialForm, new[]
            {
                new LookupOption("", "Chưa khai báo"),
                new LookupOption("PHI_THUONG_MAI", "Phi thương mại"),
                new LookupOption("THUONG_MAI", "Thương mại")
            });
            AddChoices(cmbTicketType, new[]
            {
                new LookupOption("", "Chưa khai báo"),
                new LookupOption("MIEN_PHI", "Miễn phí"),
                new LookupOption("THU_MOI", "Thư mời"),
                new LookupOption("BAN_VE", "Bán vé")
            });
            AddChoices(cmbDocumentType, new[]
            {
                new LookupOption("PHUONG_AN_AN_TOAN", "Phương án an toàn"),
                new LookupOption("GIAY_TO_LIEN_QUAN", "Giấy tờ liên quan"),
                new LookupOption("TAI_LIEU_KHAC", "Tài liệu khác")
            });

            cmbVenue.SelectedIndexChanged += (s, e) =>
            {
                var opt = cmbVenue.SelectedItem as VenueLookupItem;
                if (opt != null && opt.Venue != null)
                {
                    SelectedVenueId = opt.Venue.ma_dia_diem;
                    lblVenue.Text = "Địa điểm: " + opt.Venue.ten_dia_diem + " (Sức chứa: " + opt.Venue.suc_chua.ToString("N0") + ")";
                }
                else
                {
                    SelectedVenueId = null;
                    lblVenue.Text = "Địa điểm: chưa chọn";
                }
            };
        }

        private void BuildForm()
        {
            Text = "Tạo hồ sơ sự kiện — ViewParty Portal";
            StartPosition = FormStartPosition.CenterParent;
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoScaleDimensions = new SizeF(96F, 96F);
            MinimumSize = new Size(760, 560);
            ClientSize = new Size(1180, 820);
            BackColor = ColorTranslator.FromHtml("#F5F8FC");
            Font = new Font("Arial", 9.5f);

            var header = new Panel { Dock = DockStyle.Top, Height = 68, BackColor = Color.White, Padding = new Padding(24, 10, 24, 10) };
            header.Paint += (s, e) =>
            {
                using (var pen = new Pen(ColorTranslator.FromHtml("#DBE4ED")))
                    e.Graphics.DrawLine(pen, 0, header.Height - 1, header.Width, header.Height - 1);
            };
            header.Controls.Add(new Label
            {
                Text = "Lập và nộp hồ sơ sự kiện",
                Location = new Point(24, 10),
                AutoSize = true,
                Font = new Font("Arial", 17F, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#1D3348")
            });
            lblStatus.Text = "Điền thông tin theo các bước bên dưới.";
            lblStatus.Location = new Point(26, 40);
            lblStatus.AutoSize = true;
            lblStatus.ForeColor = ColorTranslator.FromHtml("#6B7C8D");
            header.Controls.Add(lblStatus);

            var footer = new Panel { Dock = DockStyle.Bottom, Height = 64, BackColor = Color.White };
            footer.Paint += (s, e) =>
            {
                using (var pen = new Pen(ColorTranslator.FromHtml("#DBE4ED")))
                    e.Graphics.DrawLine(pen, 0, 0, footer.Width, 0);
            };
            lblVenue.Text = "Địa điểm: chưa chọn";
            lblVenue.AutoSize = true;
            lblVenue.Location = new Point(24, 22);
            lblVenue.Font = new Font("Arial", 9.5f, FontStyle.Bold);
            lblVenue.ForeColor = ColorTranslator.FromHtml("#137A78");
            footer.Controls.Add(lblVenue);

            btnExportVB01.Text = "Xuất Đơn đề nghị (VB01)";
            StyleFooterButton(btnExportVB01, new Size(182, 38));
            btnExportVB01.Click += ExportVB01_Click;
            btnChooseVenue.Text = "Kiểm tra lịch địa điểm";
            StyleFooterButton(btnChooseVenue, new Size(168, 38));
            btnChooseVenue.Click += ChooseVenue_Click;
            btnSaveDraft.Text = "Lưu Nháp";
            StyleFooterButton(btnSaveDraft, new Size(106, 38), true);
            btnSaveDraft.Click += (s, e) => SaveCurrentDraft(true);
            btnSubmit.Text = "Nộp hồ sơ";
            StyleFooterButton(btnSubmit, new Size(118, 38), true);
            btnSubmit.Click += async (s, e) => await SubmitCurrentDraftAsync();
            var close = CreateButton("← Quay lại", false);
            close.Size = new Size(106, 38);
            close.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            close.Click += (s, e) => CloseOrExit();
            footer.Controls.AddRange(new Control[] { btnExportVB01, btnChooseVenue, btnSaveDraft, btnSubmit, close });
            footer.Resize += (s, e) => LayoutFooter(footer, close);
            LayoutFooter(footer, close);

            var bodyHost = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(24, 14, 24, 14),
                BackColor = ColorTranslator.FromHtml("#F5F8FC")
            };

            var tabs = new TabControl { Dock = DockStyle.Fill, Padding = new Point(14, 6), Font = new Font("Arial", 9.5f, FontStyle.Bold) };
            var eventTab = CreateTab("1. Thông tin sự kiện");
            var ticketTab = CreateTab("2. Thương mại và vé");
            var rightsTab = CreateTab("3. Quyền trình chiếu");
            var safetyTab = CreateTab("4. An toàn và tài liệu");
            var reviewTab = CreateTab("5. Xem lại và nộp hồ sơ");
            tabs.TabPages.AddRange(new[] { eventTab, ticketTab, rightsTab, safetyTab, reviewTab });

            var txtReviewSummary = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                BackColor = ColorTranslator.FromHtml("#FBFCFE"),
                ForeColor = ColorTranslator.FromHtml("#1D3348"),
                Font = new Font("Arial", 10F, FontStyle.Regular),
                BorderStyle = BorderStyle.FixedSingle
            };
            var reviewContainer = new Panel { Dock = DockStyle.Fill, Padding = new Padding(18), BackColor = Color.White };
            reviewContainer.Controls.Add(txtReviewSummary);
            reviewTab.Controls.Add(reviewContainer);
            tabs.SelectedIndexChanged += (s, e) =>
            {
                if (tabs.SelectedTab == reviewTab)
                {
                    txtReviewSummary.Text =
                        "TỔNG HỢP HỒ SƠ SỰ KIỆN TRƯỚC KHI NỘP\r\n" +
                        "============================================================\r\n" +
                        "1. Mã hồ sơ: " + (DraftId > 0 ? "HS-" + DraftId.ToString("0000") : "(Chưa lưu)") + "\r\n" +
                        "2. Đơn vị tổ chức: " + txtOrganizerUnit.Text + "\r\n" +
                        "3. Tên sự kiện: " + txtEventName.Text + "\r\n" +
                        "4. Loại sự kiện: " + cmbEventType.Text + "\r\n" +
                        "5. Địa điểm tổ chức: " + cmbVenue.Text + "\r\n" +
                        "6. Quy mô dự kiến: " + numAudienceSize.Value.ToString("N0") + " người\r\n" +
                        "7. Thời gian dự kiến: " + dtpStart.Value.ToString("dd/MM/yyyy HH:mm") + " -> " + dtpEnd.Value.ToString("dd/MM/yyyy HH:mm") + "\r\n" +
                        "8. Hình thức thương mại / Loại vé: " + cmbCommercialForm.Text + " / " + cmbTicketType.Text +
                        " (Số vé: " + numTicketCount.Value.ToString("N0") + ", Giá vé: " + numTicketPrice.Value.ToString("N0") + " VNĐ)\r\n" +
                        "9. Quyền trình chiếu: " + txtRightsIssuer.Text + " | Số VB: " + txtRightsDocumentNumber.Text +
                        " | Phạm vi: " + txtRightsScope.Text + "\r\n" +
                        "10. Phương án y tế: " + txtMedicalPlan.Text + "\r\n" +
                        "11. Phương án an ninh: " + txtSecurityPlan.Text + "\r\n" +
                        "12. Phương án PCCC & thoát hiểm: " + txtEvacuationPlan.Text + "\r\n" +
                        "13. Phương án xử lý sự cố: " + txtIncidentPlan.Text + "\r\n" +
                        "14. Số tệp đính kèm hiện tại: " + gridDocuments.Rows.Count + " tệp\r\n";
                }
            };

            var eventFields = CreateFieldsPanel(eventTab);
            AddField(eventFields, 0, "Đơn vị tổ chức *", txtOrganizerUnit);
            AddField(eventFields, 1, "Tên sự kiện *", txtEventName);
            AddField(eventFields, 2, "Loại sự kiện *", cmbEventType);
            AddField(eventFields, 3, "Địa điểm tổ chức *", cmbVenue);
            AddField(eventFields, 4, "Quy mô khách dự kiến (1.000 - 10.000)", numAudienceSize);
            AddField(eventFields, 5, "Hình thức thương mại", cmbCommercialForm);
            AddField(eventFields, 6, "Bắt đầu dự kiến", dtpStart);
            AddField(eventFields, 7, "Kết thúc dự kiến", dtpEnd);
            AddField(eventFields, 8, "Thông tin tài trợ", Multiline(txtSponsorInfo));
            AddField(eventFields, 9, "Nội dung chương trình", Multiline(txtEventDescription));

            var ticketFields = CreateFieldsPanel(ticketTab);
            AddField(ticketFields, 0, "Loại vé", cmbTicketType);
            AddField(ticketFields, 1, "Số lượng vé dự kiến", numTicketCount);
            AddField(ticketFields, 2, "Giá vé (VNĐ)", numTicketPrice);
            AddField(ticketFields, 3, "Có tài trợ thương mại", chkSponsored);
            AddField(ticketFields, 4, "Kênh phát hành vé", Multiline(txtTicketChannel));
            AddField(ticketFields, 5, "Cách kiểm soát vé / QR", Multiline(txtTicketControl));

            var rightsFields = CreateFieldsPanel(rightsTab);
            AddField(rightsFields, 0, "Đơn vị cấp quyền", txtRightsIssuer);
            AddField(rightsFields, 1, "Số văn bản", txtRightsDocumentNumber);
            AddField(rightsFields, 2, "Loại quyền", txtRightsType);
            AddField(rightsFields, 3, "Nguồn tín hiệu", txtSignalSource);
            AddField(rightsFields, 4, "Hiệu lực từ", dtpRightsStart);
            AddField(rightsFields, 5, "Hiệu lực đến", dtpRightsEnd);
            AddField(rightsFields, 6, "Nội dung được cấp quyền", Multiline(txtRightsContent));
            AddField(rightsFields, 7, "Phạm vi địa điểm", txtRightsScope);

            txtRightsAttachmentPath.ReadOnly = true;
            txtRightsAttachmentPath.BackColor = ColorTranslator.FromHtml("#FBFCFE");
            txtRightsAttachmentPath.ForeColor = ColorTranslator.FromHtml("#1D3348");
            txtRightsAttachmentPath.BorderStyle = BorderStyle.FixedSingle;
            txtRightsAttachmentPath.Text = "Chưa chọn tệp minh chứng bản quyền";

            var browseRights = CreateButton("Chọn tệp minh chứng...", true);
            browseRights.MinimumSize = new Size(176, 36);
            browseRights.Click += BrowseRightsAttachment_Click;
            var removeRights = CreateButton("Xóa tệp đã chọn", false);
            removeRights.MinimumSize = new Size(136, 36);
            removeRights.Click += (s, e) =>
            {
                newProjectionAttachmentPath = null;
                removeProjectionAttachment = true;
                lblRightsAttachment.Text = "Chưa chọn tệp";
                txtRightsAttachmentPath.Text = "Chưa chọn tệp minh chứng bản quyền";
            };
            var rightsAttachmentPanel = new Panel { Height = 44, Dock = DockStyle.Top };
            browseRights.Location = new Point(0, 3);
            removeRights.Location = new Point(browseRights.Width + 10, 3);
            txtRightsAttachmentPath.Location = new Point(removeRights.Right + 12, 7);
            txtRightsAttachmentPath.Width = 420;
            txtRightsAttachmentPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            rightsAttachmentPanel.Resize += (s, e) =>
            {
                txtRightsAttachmentPath.Location = new Point(removeRights.Right + 12, 7);
                txtRightsAttachmentPath.Width = Math.Max(200, rightsAttachmentPanel.ClientSize.Width - removeRights.Right - 16);
            };
            rightsAttachmentPanel.Controls.AddRange(new Control[] { browseRights, removeRights, txtRightsAttachmentPath });
            AddFullWidthField(rightsFields, 4, "Tệp minh chứng bản quyền trình chiếu (PDF, Word, Ảnh) *", rightsAttachmentPanel, 90);

            var safetyFields = CreateFieldsPanel(safetyTab);
            AddField(safetyFields, 0, "Phương án y tế / cứu hộ", Multiline(txtMedicalPlan));
            AddField(safetyFields, 1, "Phương án an ninh / điều phối", Multiline(txtSecurityPlan));
            AddField(safetyFields, 2, "Thoát hiểm và PCCC", Multiline(txtEvacuationPlan));
            AddField(safetyFields, 3, "Xử lý sự cố", Multiline(txtIncidentPlan));
            var attachmentTools = new Panel { Height = 44, Dock = DockStyle.Top };
            cmbDocumentType.Location = new Point(0, 6);
            cmbDocumentType.Width = 260;
            var addFiles = CreateButton("Thêm tài liệu đính kèm...", true);
            addFiles.MinimumSize = new Size(184, 36);
            addFiles.Location = new Point(272, 3);
            addFiles.Click += AddDocuments_Click;
            var removeFile = CreateButton("Gỡ tài liệu đã chọn", false);
            removeFile.MinimumSize = new Size(156, 36);
            removeFile.Location = new Point(466, 3);
            removeFile.Click += RemoveDocument_Click;
            attachmentTools.Controls.AddRange(new Control[] { cmbDocumentType, addFiles, removeFile });
            AddFullWidthField(safetyFields, 2, "Chọn loại tài liệu và tải lên tệp đính kèm", attachmentTools, 80);

            gridDocuments.Dock = DockStyle.Fill;
            gridDocuments.ReadOnly = true;
            gridDocuments.AllowUserToAddRows = false;
            gridDocuments.AllowUserToDeleteRows = false;
            gridDocuments.AllowUserToResizeRows = false;
            gridDocuments.RowHeadersVisible = false;
            gridDocuments.MultiSelect = false;
            gridDocuments.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridDocuments.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridDocuments.BackgroundColor = Color.White;
            gridDocuments.BorderStyle = BorderStyle.FixedSingle;
            gridDocuments.Columns.Add("DocumentType", "Loại tài liệu");
            gridDocuments.Columns.Add("DocumentName", "Tên tệp");
            var gridHost = new Panel { Height = 200, Dock = DockStyle.Top, Padding = new Padding(2, 4, 2, 6) };
            gridHost.Controls.Add(gridDocuments);
            AddFullWidthField(safetyFields, 3, "Danh sách tệp đính kèm của hồ sơ", gridHost, 236);

            bodyHost.Controls.Add(tabs);
            Controls.Add(bodyHost);
            Controls.Add(footer);
            Controls.Add(header);
        }

        private void LoadEventTypes()
        {
            try
            {
                var eventTypes = hoSoService.GetActiveEventTypes();
                cmbEventType.DataSource = eventTypes;
                cmbEventType.DisplayMember = "ten_loai";
                cmbEventType.ValueMember = "ma_loai_su_kien";
            }
            catch (Exception ex)
            {
                LocalExceptionLogger.Write(ex);
            }
        }

        private void LoadVenues()
        {
            try
            {
                activeVenues = diaDiemService.GetAllActive();
                cmbVenue.Items.Clear();
                cmbVenue.Items.Add(new VenueLookupItem(null, "-- Chọn địa điểm từ danh sách --"));
                foreach (var v in activeVenues)
                {
                    cmbVenue.Items.Add(new VenueLookupItem(
                        v,
                        v.ten_dia_diem + " — " + v.dia_chi + " (Sức chứa: " + v.suc_chua.ToString("N0") + ")"));
                }
                cmbVenue.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                LocalExceptionLogger.Write(ex);
            }
        }

        private void SetVenueSelection(long? venueId)
        {
            SelectedVenueId = venueId;
            if (!venueId.HasValue)
            {
                if (cmbVenue.Items.Count > 0) cmbVenue.SelectedIndex = 0;
                lblVenue.Text = "Địa điểm: chưa chọn";
                return;
            }
            for (int i = 0; i < cmbVenue.Items.Count; i++)
            {
                var item = cmbVenue.Items[i] as VenueLookupItem;
                if (item != null && item.Venue != null && item.Venue.ma_dia_diem == venueId.Value)
                {
                    cmbVenue.SelectedIndex = i;
                    return;
                }
            }
            lblVenue.Text = "Địa điểm đã chọn: mã " + venueId.Value;
        }

        private void LoadDraft(long draftId)
        {
            try
            {
                draft = hoSoService.GetDraft(CurrentUser, draftId);
                DraftId = draftId;
                SelectedVenueId = draft.ma_dia_diem;
                MapDraftToControls();
                lblStatus.Text = "Đang chỉnh sửa hồ sơ Nháp HS-" + draftId.ToString("0000") + ".";
            }
            catch (Exception ex)
            {
                LocalExceptionLogger.Write(ex);
                ShowError(ex.Message);
                CloseOrExit();
            }
        }

        private void ResetToNewDraft()
        {
            DateTime start = DateTime.Now.AddDays(45);
            dtpStart.Value = start;
            dtpEnd.Value = start.AddHours(4);
            dtpRightsStart.Value = DateTime.Today;
            dtpRightsEnd.Value = DateTime.Today.AddYears(1);
            if (cmbCommercialForm.Items.Count > 0) cmbCommercialForm.SelectedIndex = 0;
            if (cmbTicketType.Items.Count > 0) cmbTicketType.SelectedIndex = 0;
            if (cmbDocumentType.Items.Count > 0) cmbDocumentType.SelectedIndex = 0;
            draft = new HoSoDraftData
            {
                bat_dau = new DateTimeOffset(dtpStart.Value),
                ket_thuc = new DateTimeOffset(dtpEnd.Value),
                quy_mo = 1000,
                quyen_trinh_chieu = new QuyenTrinhChieu
                {
                    hieu_luc_tu = dtpRightsStart.Value.Date,
                    hieu_luc_den = dtpRightsEnd.Value.Date
                }
            };
            numAudienceSize.Value = 1000;
            lblRightsAttachment.Text = "Chưa chọn tệp";
            txtRightsAttachmentPath.Text = "Chưa chọn tệp minh chứng bản quyền";
            lblVenue.Text = "Địa điểm: chưa chọn";
            RefreshDocumentsGrid();
        }

        private void FillValidSampleData()
        {
            DateTime start = DateTime.Now.Date.AddDays(45).AddHours(18);
            txtOrganizerUnit.Text = "Công ty TNHH Sự kiện Thể thao Điện tử ViewParty";
            txtEventName.Text = "Viewing Party Chung kết Thế giới eSports " + DateTime.Now.ToString("HHmm");
            if (cmbEventType.Items.Count > 0) cmbEventType.SelectedIndex = 0;
            numAudienceSize.Value = 2000;
            dtpStart.Checked = true;
            dtpStart.Value = start;
            dtpEnd.Checked = true;
            dtpEnd.Value = start.AddHours(4);
            SetChoice(cmbCommercialForm, "PHI_THUONG_MAI");
            chkSponsored.Checked = true;
            txtSponsorInfo.Text = "Tài trợ nước uống và thiết bị màn hình LED cỡ lớn.";
            txtEventDescription.Text = "Chương trình xem chung trực tiếp trận Chung kết eSports, giao lưu khán giả và bốc thăm trúng thưởng.";

            if (cmbVenue.Items.Count > 1 && SelectedVenueId == null)
            {
                cmbVenue.SelectedIndex = 1;
            }

            SetChoice(cmbTicketType, "MIEN_PHI");
            numTicketCount.Value = 2000;
            numTicketPrice.Value = 0;
            txtTicketChannel.Text = "Đăng ký trực tuyến qua cổng vé điện tử ViewParty.";
            txtTicketControl.Text = "Quét mã QR định danh tại 4 cổng kiểm soát an ninh.";

            txtRightsIssuer.Text = "Ban Tổ chức Giải đấu Quốc tế Riot / VNGGames";
            txtRightsDocumentNumber.Text = "VB-QTC-2026-889";
            txtRightsType.Text = "Quyền trình chiếu công cộng phi thương mại";
            txtSignalSource.Text = "Clean Feed RTMP chính thức từ Ban tổ chức";
            txtRightsContent.Text = "Toàn bộ trận Chung kết và chương trình bình luận trực tiếp.";
            txtRightsScope.Text = "TOAN_QUOC";
            dtpRightsStart.Value = DateTime.Today.AddDays(-5);
            dtpRightsEnd.Value = DateTime.Today.AddYears(1);

            txtMedicalPlan.Text = "Bố trí 2 tổ y tế trực chiến và 1 xe cứu thương túc trực tại cổng chính.";
            txtSecurityPlan.Text = "Bố trí 30 nhân viên an ninh phân luồng khu vực soát vé và khán đài.";
            txtEvacuationPlan.Text = "Mở thông suốt 6 cửa thoát hiểm, trang bị bình chữa cháy và đèn chỉ dẫn PCCC.";
            txtIncidentPlan.Text = "Ngắt tín hiệu khẩn cấp, phát loa điều phối sơ tán theo 3 lối thoát hiểm độc lập.";

            string sampleFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "ViewParty", "SampleAttachments");
            Directory.CreateDirectory(sampleFolder);

            string sampleRightsFile = Path.Combine(sampleFolder, "MinhChung_QuyenTrinhChieu_ToanQuoc.txt");
            if (!File.Exists(sampleRightsFile))
                File.WriteAllText(sampleRightsFile, "Văn bản xác nhận quyền trình chiếu công cộng hợp lệ trên phạm vi TOAN_QUOC.");
            newProjectionAttachmentPath = sampleRightsFile;
            removeProjectionAttachment = false;
            lblRightsAttachment.Text = Path.GetFileName(sampleRightsFile);
            txtRightsAttachmentPath.Text = Path.GetFileName(sampleRightsFile) + " (Đã đính kèm)";

            string sampleSafetyFile = Path.Combine(sampleFolder, "PhuongAn_AnToan_PCCC_YTe.txt");
            if (!File.Exists(sampleSafetyFile))
                File.WriteAllText(sampleSafetyFile, "Phương án đảm bảo an ninh trật tự, y tế, PCCC và thoát hiểm chi tiết.");
            if (newDocuments.Count == 0 && (draft == null || draft.tai_lieu == null || draft.tai_lieu.Count == 0))
            {
                newDocuments.Add(new HoSoFileUpload("PHUONG_AN_AN_TOAN", sampleSafetyFile));
                RefreshDocumentsGrid();
            }

            lblStatus.Text = "Đã điền mẫu hợp lệ. Nhấn 'Lưu Nháp' hoặc 'Nộp hồ sơ'.";
        }

        private void MapDraftToControls()
        {
            txtOrganizerUnit.Text = draft.ten_don_vi_to_chuc ?? string.Empty;
            txtEventName.Text = draft.ten_su_kien ?? string.Empty;
            SetEventType(draft.ma_loai_su_kien);
            SetDate(dtpStart, draft.bat_dau);
            SetDate(dtpEnd, draft.ket_thuc);
            numAudienceSize.Value = draft.quy_mo.HasValue
                ? Math.Min(numAudienceSize.Maximum, Math.Max(numAudienceSize.Minimum, draft.quy_mo.Value))
                : 0;
            txtEventDescription.Text = draft.noi_dung ?? string.Empty;
            SetChoice(cmbCommercialForm, draft.hinh_thuc_thuong_mai);
            chkSponsored.Checked = draft.co_tai_tro;
            txtSponsorInfo.Text = draft.thong_tin_tai_tro ?? string.Empty;
            SetChoice(cmbTicketType, draft.loai_ve);
            numTicketCount.Value = draft.so_luong_ve.HasValue
                ? Math.Min(numTicketCount.Maximum, Math.Max(numTicketCount.Minimum, draft.so_luong_ve.Value))
                : 0;
            numTicketPrice.Value = draft.gia_ve.HasValue
                ? Math.Min(numTicketPrice.Maximum, Math.Max(numTicketPrice.Minimum, draft.gia_ve.Value))
                : 0;
            txtTicketChannel.Text = draft.kenh_phat_hanh_ve ?? string.Empty;
            txtTicketControl.Text = draft.cach_kiem_soat_ve ?? string.Empty;
            var right = draft.quyen_trinh_chieu;
            txtRightsIssuer.Text = right == null ? string.Empty : right.don_vi_cap ?? string.Empty;
            txtRightsDocumentNumber.Text = right == null ? string.Empty : right.so_van_ban ?? string.Empty;
            txtRightsType.Text = right == null ? string.Empty : right.loai_quyen ?? string.Empty;
            txtSignalSource.Text = right == null ? string.Empty : right.nguon_tin_hieu ?? string.Empty;
            txtRightsContent.Text = right == null ? string.Empty : right.noi_dung_duoc_cap ?? string.Empty;
            txtRightsScope.Text = right == null ? string.Empty : right.pham_vi_dia_diem ?? string.Empty;
            dtpRightsStart.Value = right == null ? DateTime.Today : right.hieu_luc_tu;
            dtpRightsEnd.Value = right == null ? DateTime.Today.AddYears(1) : right.hieu_luc_den;
            newProjectionAttachmentPath = null;
            removeProjectionAttachment = false;
            lblRightsAttachment.Text = right == null || string.IsNullOrWhiteSpace(right.tep_dinh_kem)
                ? "Chưa chọn tệp"
                : Path.GetFileName(right.tep_dinh_kem);
            txtRightsAttachmentPath.Text = right == null || string.IsNullOrWhiteSpace(right.tep_dinh_kem)
                ? "Chưa chọn tệp minh chứng bản quyền"
                : Path.GetFileName(right.tep_dinh_kem) + " (Đã lưu trong hồ sơ)";
            txtMedicalPlan.Text = draft.nhan_su_y_te ?? string.Empty;
            txtSecurityPlan.Text = draft.nhan_su_an_ninh ?? string.Empty;
            txtEvacuationPlan.Text = draft.phuong_an_thoat_hiem ?? string.Empty;
            txtIncidentPlan.Text = draft.phuong_an_xu_ly_su_co ?? string.Empty;
            newDocuments.Clear();
            RefreshDocumentsGrid();
            SetVenueSelection(draft.ma_dia_diem);
        }

        private bool SaveCurrentDraft(bool showSuccess)
        {
            if (CurrentUser == null)
            {
                ShowError("Chức năng cần tài khoản Người tổ chức đã đăng nhập.");
                return false;
            }

            try
            {
                long? chosenVenueId = SelectedVenueId;
                draft = ReadDraftFromControls();
                var result = hoSoService.SaveDraft(
                    CurrentUser, draft, newDocuments, newProjectionAttachmentPath);
                DraftId = result.DraftId;

                if (chosenVenueId.HasValue && chosenVenueId.Value > 0 && draft.quy_mo.HasValue && draft.quy_mo.Value >= 1)
                {
                    try
                    {
                        diaDiemService.SelectForDraft(CurrentUser, DraftId, chosenVenueId.Value);
                        draft.ma_dia_diem = chosenVenueId;
                    }
                    catch
                    {
                        // Keep draft saved even if venue capacity validation triggers a separate notice
                    }
                }

                newDocuments.Clear();
                newProjectionAttachmentPath = null;
                removeProjectionAttachment = false;
                MapDraftToControls();
                lblStatus.Text = "Đã lưu hồ sơ Nháp HS-" + DraftId.ToString("0000") + ".";
                if (result.CleanupFailures.Count > 0)
                    MessageBox.Show(this,
                        "Hồ sơ đã lưu. Không thể dọn một số tệp cũ: " +
                        string.Join(", ", result.CleanupFailures) + ".",
                        "Đã lưu với cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else if (showSuccess)
                    MessageBox.Show(this, "Đã lưu hồ sơ Nháp HS-" + DraftId.ToString("0000") + " thành công.",
                        "Lưu Nháp thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            catch (HoSoValidationException ex)
            {
                MessageBox.Show(this, ex.Message, "Thông tin chưa hợp lệ",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (HoSoRepositoryException ex)
            {
                LocalExceptionLogger.Write(ex);
                ShowError(ex.Message);
            }
            catch (Exception ex)
            {
                LocalExceptionLogger.Write(ex);
                ShowError("Không thể lưu hồ sơ Nháp lúc này. Vui lòng kiểm tra lại dữ liệu.");
            }
            return false;
        }

        private async Task SubmitCurrentDraftAsync()
        {
            if (!SaveCurrentDraft(false)) return;

            btnSubmit.Enabled = false;
            btnSaveDraft.Enabled = false;
            btnChooseVenue.Enabled = false;
            UseWaitCursor = true;
            try
            {
                var result = await hoSoService.NopHoSoAsync(CurrentUser, DraftId);
                if (!result.ThanhCong)
                {
                    lblStatus.Text = "Chưa nộp được · Hồ sơ đang ở trạng thái Bản nháp.";
                    MessageBox.Show(this, result.LyDo, "Hồ sơ chưa được nộp",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                lblStatus.Text = "Đã nộp hồ sơ HS-" + result.MaHoSo.ToString("0000") + ".";
                MessageBox.Show(this, result.LyDo, "Nộp hồ sơ thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
                CloseOrExit();
            }
            catch (HoSoValidationException ex)
            {
                MessageBox.Show(this, ex.Message, "Không thể nộp hồ sơ",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (HoSoRepositoryException ex)
            {
                LocalExceptionLogger.Write(ex);
                ShowError(ex.Message);
            }
            catch (Exception ex)
            {
                LocalExceptionLogger.Write(ex);
                ShowError("Không thể nộp hồ sơ lúc này. Vui lòng thử lại.");
            }
            finally
            {
                UseWaitCursor = false;
                if (!IsDisposed)
                {
                    btnSubmit.Enabled = true;
                    btnSaveDraft.Enabled = true;
                    btnChooseVenue.Enabled = true;
                }
            }
        }

        private HoSoDraftData ReadDraftFromControls()
        {
            long eventTypeId;
            if (cmbEventType.SelectedValue == null ||
                !long.TryParse(Convert.ToString(cmbEventType.SelectedValue), out eventTypeId))
                throw new HoSoValidationException("Hãy chọn loại sự kiện đang hoạt động.");

            draft.ma_ho_so = DraftId > 0 ? (long?)DraftId : null;
            draft.ma_dia_diem = SelectedVenueId;
            draft.ma_loai_su_kien = eventTypeId;
            draft.ten_don_vi_to_chuc = txtOrganizerUnit.Text;
            draft.ten_su_kien = txtEventName.Text;
            draft.bat_dau = dtpStart.Checked ? (DateTimeOffset?)new DateTimeOffset(dtpStart.Value) : null;
            draft.ket_thuc = dtpEnd.Checked ? (DateTimeOffset?)new DateTimeOffset(dtpEnd.Value) : null;
            draft.quy_mo = numAudienceSize.Value > 0 ? (int?)Decimal.ToInt32(numAudienceSize.Value) : null;
            draft.noi_dung = txtEventDescription.Text;
            draft.hinh_thuc_thuong_mai = Convert.ToString(cmbCommercialForm.SelectedValue);
            if (string.IsNullOrWhiteSpace(draft.hinh_thuc_thuong_mai)) draft.hinh_thuc_thuong_mai = null;
            draft.co_tai_tro = chkSponsored.Checked;
            draft.thong_tin_tai_tro = txtSponsorInfo.Text;
            draft.loai_ve = Convert.ToString(cmbTicketType.SelectedValue);
            if (string.IsNullOrWhiteSpace(draft.loai_ve)) draft.loai_ve = null;
            draft.so_luong_ve = numTicketCount.Value > 0 ? (int?)Decimal.ToInt32(numTicketCount.Value) : null;
            draft.gia_ve = numTicketPrice.Value > 0 ? (decimal?)numTicketPrice.Value : null;
            draft.kenh_phat_hanh_ve = txtTicketChannel.Text;
            draft.cach_kiem_soat_ve = txtTicketControl.Text;
            draft.nhan_su_y_te = txtMedicalPlan.Text;
            draft.nhan_su_an_ninh = txtSecurityPlan.Text;
            draft.phuong_an_thoat_hiem = txtEvacuationPlan.Text;
            draft.phuong_an_xu_ly_su_co = txtIncidentPlan.Text;

            var right = new QuyenTrinhChieu
            {
                don_vi_cap = txtRightsIssuer.Text,
                so_van_ban = txtRightsDocumentNumber.Text,
                loai_quyen = txtRightsType.Text,
                nguon_tin_hieu = txtSignalSource.Text,
                noi_dung_duoc_cap = txtRightsContent.Text,
                pham_vi_dia_diem = txtRightsScope.Text,
                hieu_luc_tu = dtpRightsStart.Value.Date,
                hieu_luc_den = dtpRightsEnd.Value.Date,
                tep_dinh_kem = removeProjectionAttachment
                    ? null
                    : draft.quyen_trinh_chieu == null ? null : draft.quyen_trinh_chieu.tep_dinh_kem
            };
            if (string.IsNullOrWhiteSpace(txtRightsIssuer.Text) &&
                string.IsNullOrWhiteSpace(txtRightsDocumentNumber.Text) &&
                string.IsNullOrWhiteSpace(txtRightsType.Text) &&
                string.IsNullOrWhiteSpace(txtSignalSource.Text) &&
                string.IsNullOrWhiteSpace(txtRightsContent.Text) &&
                string.IsNullOrWhiteSpace(txtRightsScope.Text) &&
                string.IsNullOrWhiteSpace(right.tep_dinh_kem) &&
                string.IsNullOrWhiteSpace(newProjectionAttachmentPath))
                draft.quyen_trinh_chieu = null;
            else
                draft.quyen_trinh_chieu = right;

            return draft;
        }

        private void ChooseVenue_Click(object sender, EventArgs e)
        {
            if (!SaveCurrentDraft(false)) return;
            if (RequestOpenVenueSearch != null)
            {
                RequestOpenVenueSearch(DraftId);
                return;
            }
        }

        private void ExportVB01_Click(object sender, EventArgs e)
        {
            var venueItem = cmbVenue.SelectedItem as VenueLookupItem;
            var previewCase = new HoSoCapPhep
            {
                ma_ho_so = DraftId > 0 ? DraftId : 1,
                ten_don_vi_to_chuc = txtOrganizerUnit.Text,
                ten_su_kien = txtEventName.Text,
                ten_loai = cmbEventType.Text,
                ten_dia_diem = venueItem != null && venueItem.Venue != null ? venueItem.Venue.ten_dia_diem : cmbVenue.Text,
                suc_chua_dia_diem = venueItem != null && venueItem.Venue != null ? (int?)venueItem.Venue.suc_chua : null,
                bat_dau = dtpStart.Value,
                ket_thuc = dtpEnd.Value,
                quy_mo = Decimal.ToInt32(numAudienceSize.Value),
                noi_dung = txtEventDescription.Text,
                hinh_thuc_thuong_mai = Convert.ToString(cmbCommercialForm.SelectedValue),
                co_tai_tro = chkSponsored.Checked,
                thong_tin_tai_tro = txtSponsorInfo.Text,
                loai_ve = Convert.ToString(cmbTicketType.SelectedValue),
                so_luong_ve = Decimal.ToInt32(numTicketCount.Value),
                gia_ve = numTicketPrice.Value,
                kenh_phat_hanh_ve = txtTicketChannel.Text,
                cach_kiem_soat_ve = txtTicketControl.Text,
                nhan_su_y_te = txtMedicalPlan.Text,
                nhan_su_an_ninh = txtSecurityPlan.Text,
                phuong_an_thoat_hiem = txtEvacuationPlan.Text,
                phuong_an_xu_ly_su_co = txtIncidentPlan.Text,
                trang_thai = "NHAP"
            };
            WordExportService.ShowPreviewDialog(this, "VB01", previewCase);
        }

        private void BrowseRightsAttachment_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog
            {
                Title = "Chọn tệp minh chứng quyền trình chiếu",
                Filter = "Tài liệu và hình ảnh|*.pdf;*.doc;*.docx;*.jpg;*.jpeg;*.png|Tất cả tệp|*.*"
            })
            {
                if (dialog.ShowDialog(this) != DialogResult.OK) return;
                newProjectionAttachmentPath = dialog.FileName;
                removeProjectionAttachment = false;
                lblRightsAttachment.Text = Path.GetFileName(dialog.FileName);
                txtRightsAttachmentPath.Text = Path.GetFileName(dialog.FileName) + " (Tệp mới chọn)";
            }
        }

        private void AddDocuments_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog
            {
                Title = "Chọn tài liệu hồ sơ",
                Filter = "Tài liệu và hình ảnh|*.pdf;*.doc;*.docx;*.jpg;*.jpeg;*.png|Tất cả tệp|*.*",
                Multiselect = true
            })
            {
                if (dialog.ShowDialog(this) != DialogResult.OK) return;
                string type = Convert.ToString(cmbDocumentType.SelectedValue);
                foreach (string path in dialog.FileNames)
                    newDocuments.Add(new HoSoFileUpload(type, path));
                RefreshDocumentsGrid();
            }
        }

        private void RemoveDocument_Click(object sender, EventArgs e)
        {
            if (gridDocuments.CurrentRow == null) return;
            var row = gridDocuments.CurrentRow.Tag as DocumentGridRow;
            if (row == null) return;
            if (row.Existing != null) draft.tai_lieu.Remove(row.Existing);
            if (row.Pending != null) newDocuments.Remove(row.Pending);
            RefreshDocumentsGrid();
        }

        private static string FormatDocumentTypeLabel(string code)
        {
            switch ((code ?? string.Empty).Trim().ToUpperInvariant())
            {
                case "PHUONG_AN_AN_TOAN": return "Phương án đảm bảo an toàn";
                case "GIAY_TO_LIEN_QUAN": return "Giấy tờ pháp lý liên quan";
                case "TAI_LIEU_KHAC": return "Tài liệu bổ sung khác";
                default: return code ?? "Tài liệu đính kèm";
            }
        }

        private void RefreshDocumentsGrid()
        {
            gridDocuments.Rows.Clear();
            if (draft != null && draft.tai_lieu != null)
            {
                foreach (var item in draft.tai_lieu)
                {
                    int row = gridDocuments.Rows.Add(FormatDocumentTypeLabel(item.loai_tai_lieu), item.ten_tai_lieu);
                    gridDocuments.Rows[row].Tag = new DocumentGridRow { Existing = item };
                }
            }
            foreach (var item in newDocuments)
            {
                int row = gridDocuments.Rows.Add(FormatDocumentTypeLabel(item.LoaiTaiLieu), Path.GetFileName(item.SourcePath) + " (tệp mới)");
                gridDocuments.Rows[row].Tag = new DocumentGridRow { Pending = item };
            }
        }

        private void AddField(TableLayoutPanel table, int row, string label, Control control)
        {
            int tableRow = row / 2;
            while (table.RowCount <= tableRow)
            {
                table.RowCount++;
                table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            }
            int fieldHeight = control is TextBox && ((TextBox)control).Multiline
                ? 118
                : control is FlowLayoutPanel
                    ? 76
                    : control is Panel ? control.Height + 36 : 68;
            var field = new Panel
            {
                Dock = DockStyle.Fill,
                Height = fieldHeight,
                MinimumSize = new Size(0, fieldHeight),
                Padding = new Padding(10, 4, 10, 4)
            };
            field.Controls.Add(new Label
            {
                Text = label,
                Location = new Point(4, 2),
                AutoSize = true,
                ForeColor = ColorTranslator.FromHtml("#203548"),
                Font = new Font("Arial", 9, FontStyle.Bold)
            });
            control.Location = new Point(4, 24);
            control.Width = Math.Max(180, table.ClientSize.Width / 2 - 34);
            control.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            if (control is FlowLayoutPanel)
                control.Height = 42;
            else if (!(control is Panel))
                control.Height = control is TextBox && ((TextBox)control).Multiline ? 80 : 30;
            field.Resize += (s, e) =>
            {
                control.Width = Math.Max(160, field.ClientSize.Width - field.Padding.Horizontal - 4);
            };
            field.Controls.Add(control);
            table.Controls.Add(field, row % 2, tableRow);
        }

        private void AddFullWidthField(TableLayoutPanel table, int tableRow, string label, Control control, int fieldHeight)
        {
            while (table.RowCount <= tableRow)
            {
                table.RowCount++;
                table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            }
            var field = new Panel
            {
                Dock = DockStyle.Fill,
                Height = fieldHeight,
                MinimumSize = new Size(0, fieldHeight),
                Padding = new Padding(10, 4, 10, 8)
            };
            field.Controls.Add(new Label
            {
                Text = label,
                Location = new Point(4, 2),
                AutoSize = true,
                ForeColor = ColorTranslator.FromHtml("#203548"),
                Font = new Font("Arial", 9.25f, FontStyle.Bold)
            });
            control.Location = new Point(4, 26);
            control.Width = Math.Max(320, table.ClientSize.Width - 36);
            control.Height = Math.Max(38, fieldHeight - 34);
            control.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            field.Resize += (s, e) =>
            {
                control.Width = Math.Max(260, field.ClientSize.Width - field.Padding.Horizontal - 4);
            };
            field.Controls.Add(control);
            table.Controls.Add(field, 0, tableRow);
            table.SetColumnSpan(field, 2);
        }

        private static TableLayoutPanel CreateFieldsPanel(TabPage page)
        {
            page.AutoScroll = true;
            var table = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 2,
                BackColor = Color.White,
                Padding = new Padding(14)
            };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            page.Controls.Add(table);
            return table;
        }

        private static Control Multiline(TextBox textBox)
        {
            textBox.Multiline = true;
            textBox.ScrollBars = ScrollBars.Vertical;
            return textBox;
        }

        private static TabPage CreateTab(string title)
        {
            return new TabPage(title) { BackColor = Color.White, Padding = new Padding(8) };
        }

        private static void ConfigureDate(DateTimePicker picker)
        {
            picker.Format = DateTimePickerFormat.Custom;
            picker.CustomFormat = "dd/MM/yyyy HH:mm";
            picker.ShowUpDown = true;
            picker.ShowCheckBox = true;
            picker.Checked = true;
            picker.Width = 220;
        }

        private static Button CreateButton(string text, bool primary)
        {
            var button = new Button
            {
                Text = text,
                AutoSize = true,
                MinimumSize = new Size(90, 34),
                FlatStyle = FlatStyle.Flat,
                BackColor = primary ? ColorTranslator.FromHtml("#137A78") : Color.White,
                ForeColor = primary ? Color.White : ColorTranslator.FromHtml("#203548"),
                Font = new Font("Arial", 9.5f, primary ? FontStyle.Bold : FontStyle.Regular),
                Margin = new Padding(4),
                Cursor = Cursors.Hand
            };
            button.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#DBE4ED");
            return button;
        }

        private static void StyleFooterButton(Button button, Size size, bool primary = false)
        {
            button.Size = size;
            button.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button.FlatStyle = FlatStyle.Flat;
            button.BackColor = primary ? ColorTranslator.FromHtml("#137A78") : Color.White;
            button.ForeColor = primary ? Color.White : ColorTranslator.FromHtml("#203548");
            button.Font = new Font("Arial", 9.5f, FontStyle.Bold);
            button.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#DBE4ED");
            button.Cursor = Cursors.Hand;
        }

        private void LayoutFooter(Panel footer, Button close)
        {
            bool compact = footer.ClientSize.Width < 920;
            footer.Height = compact ? 104 : 64;
            lblVenue.AutoEllipsis = true;
            lblVenue.AutoSize = false;
            lblVenue.Location = new Point(24, compact ? 10 : 21);
            int buttonTop = compact ? 52 : 13;
            close.Location = new Point(footer.ClientSize.Width - close.Width - 24, buttonTop);
            btnSubmit.Location = new Point(close.Left - btnSubmit.Width - 8, buttonTop);
            btnSaveDraft.Location = new Point(btnSubmit.Left - btnSaveDraft.Width - 8, buttonTop);
            btnChooseVenue.Location = new Point(btnSaveDraft.Left - btnChooseVenue.Width - 8, buttonTop);
            btnExportVB01.Location = new Point(btnChooseVenue.Left - btnExportVB01.Width - 8, buttonTop);
            lblVenue.Size = new Size(Math.Max(160, btnExportVB01.Left - 32), compact ? 28 : 32);
        }

        private static void AddChoices(ComboBox combo, IEnumerable<LookupOption> values)
        {
            combo.DataSource = values.ToList();
            combo.DisplayMember = "Text";
            combo.ValueMember = "Value";
        }

        private static void SetChoice(ComboBox combo, string value)
        {
            if (combo.DataSource == null) return;
            combo.SelectedValue = value ?? string.Empty;
        }

        private void SetEventType(long eventTypeId)
        {
            if (cmbEventType.DataSource != null)
                cmbEventType.SelectedValue = eventTypeId;
        }

        private static void SetDate(DateTimePicker picker, DateTimeOffset? value)
        {
            picker.Checked = value.HasValue;
            if (value.HasValue)
                picker.Value = value.Value.LocalDateTime;
        }

        private void ShowError(string message)
        {
            MessageBox.Show(this, message, "Không thể xử lý hồ sơ",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private sealed class LookupOption
        {
            public string Value { get; private set; }
            public string Text { get; private set; }
            public LookupOption(string value, string text) { Value = value; Text = text; }
        }

        private sealed class VenueLookupItem
        {
            public DiaDiem Venue { get; private set; }
            public string Text { get; private set; }
            public VenueLookupItem(DiaDiem venue, string text)
            {
                Venue = venue;
                Text = text;
            }
            public override string ToString()
            {
                return Text;
            }
        }

        private sealed class DocumentGridRow
        {
            public TaiLieu Existing { get; set; }
            public HoSoFileUpload Pending { get; set; }
        }
    }
}
