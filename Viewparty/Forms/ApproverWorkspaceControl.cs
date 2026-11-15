using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Viewparty.Data;
using Viewparty.Models;
using Viewparty.Services;

namespace Viewparty.Forms
{
    internal sealed class ApproverWorkspaceControl : RoleWorkspaceControl
    {
        private readonly AuthenticatedUser user;
        private readonly NgoaiLeWorkflowService exceptionService = new NgoaiLeWorkflowService();
        private readonly LicenseDecisionService licenseService = new LicenseDecisionService();

        public ApproverWorkspaceControl(AuthenticatedUser user)
        {
            this.user = user ?? throw new ArgumentNullException("user");
        }

        public override void OpenModule(string module)
        {
            NotifyModuleChanged(module);
            switch (module)
            {
                case "Tổng quan": Dashboard(); break;
                case "Hồ sơ chờ duyệt": Pending(); break;
                case "Xét ngoại lệ": Exceptions(); break;
                case "Quyết định cấp phép": Decision(); break;
                case "Giấy phép đã cấp": Licenses(); break;
                default: Dashboard(); break;
            }
        }

        private void Dashboard()
        {
            IList<LicenseDecisionCase> decisionCases = LoadDecisionCases();
            IList<NgoaiLeDecisionItem> pendingExceptions;
            try { pendingExceptions = exceptionService.GetPendingDecisions(user); }
            catch { pendingExceptions = new List<NgoaiLeDecisionItem>(); }
            IList<IssuedLicenseItem> issuedLicenses;
            try { issuedLicenses = licenseService.GetIssuedLicenses(user); }
            catch { issuedLicenses = new List<IssuedLicenseItem>(); }

            int waitingApproval = decisionCases.Count(c => c.Status == "CHO_PHE_DUYET");
            int eligibleCount = decisionCases.Count(c => c.CanApprove);

            BuildPage("Tổng quan Lãnh đạo phê duyệt", null, stack =>
            {
                AddStats(stack,
                    new StatCardSpec("Chờ phê duyệt", waitingApproval.ToString("00"), "Chờ quyết định", waitingApproval > 0),
                    new StatCardSpec("Chờ xét ngoại lệ", pendingExceptions.Count.ToString("00"), "Thẩm quyền cấp " + user.ApprovalLevel, pendingExceptions.Count > 0),
                    new StatCardSpec("Đủ điều kiện cấp phép", eligibleCount.ToString("00"), "Hồ sơ đạt chuẩn", false),
                    new StatCardSpec("Giấy phép đã ban hành", issuedLicenses.Count.ToString("00"), "Đang hiệu lực", false));

                var section = TableSection(stack, "Danh sách hồ sơ cần xem xét quyết định", null,
                    "Mã hồ sơ", "Tên sự kiện", "Địa điểm tổ chức", "Trạng thái hiện tại", "Đánh giá điều kiện", "Bước thực hiện");
                var grid = BodyOf(section).Controls.OfType<DataGridView>().First();
                ConfigureApproverGrid(grid);
                foreach (var item in decisionCases)
                {
                    int r = grid.Rows.Add(
                        "HS-" + item.CaseId.ToString("0000"),
                        item.EventName,
                        item.VenueName ?? "—",
                        FormatStatus(item.Status),
                        item.CanApprove ? "Đủ điều kiện cấp phép" : "Cần xem xét / Từ chối",
                        "Ra quyết định cấp phép");
                    grid.Rows[r].Tag = "Quyết định cấp phép";
                }
                foreach (var exItem in pendingExceptions)
                {
                    int r = grid.Rows.Add(
                        "HS-" + exItem.CaseId.ToString("0000"),
                        exItem.EventName,
                        "Tiêu chí: " + exItem.RuleCode + " (Phiên bản " + exItem.Version + ")",
                        "Chờ lãnh đạo xét ngoại lệ",
                        "Yêu cầu thẩm quyền cấp " + exItem.RequiredApprovalLevel,
                        "Xét duyệt ngoại lệ");
                    grid.Rows[r].Tag = "Xét ngoại lệ";
                }
                if (grid.Rows.Count == 0)
                {
                    grid.Rows.Add("—", "Hiện không có hồ sơ chờ phê duyệt hoặc xét ngoại lệ.", "—", "—", "—", "—");
                }
                grid.CellDoubleClick += (s, e) =>
                {
                    if (e.RowIndex >= 0 && grid.Rows[e.RowIndex].Tag is string targetModule)
                        OpenModule(targetModule);
                };
            });
        }

        private void Pending()
        {
            IList<LicenseDecisionCase> cases = LoadDecisionCases();
            BuildPage("Danh sách hồ sơ chờ phê duyệt", null, stack =>
            {
                var section = TableSection(stack, "Hồ sơ đang trong danh sách chờ quyết định", null,
                    "Mã hồ sơ", "Tên sự kiện", "Trạng thái hiện tại", "Quy mô khách", "Địa điểm tổ chức", "Đủ điều kiện cấp phép");
                var grid = BodyOf(section).Controls.OfType<DataGridView>().First();
                ConfigureApproverGrid(grid);
                foreach (LicenseDecisionCase item in cases)
                {
                    int row = grid.Rows.Add(
                        "HS-" + item.CaseId.ToString("0000"),
                        item.EventName,
                        FormatStatus(item.Status),
                        item.Attendance + " người",
                        item.VenueName ?? "—",
                        item.CanApprove ? "Đủ điều kiện" : "Chưa đạt điều kiện");
                    grid.Rows[row].Tag = item;
                }
                var actions = AddSection(stack, "Thao tác", null, 100);
                AddButtons(actions,
                    ActionButton("Mở màn hình Quyết định cấp phép", () => Decision(), true),
                    ActionButton("Làm mới danh sách", () => Pending()));
            });
        }

        private void Exceptions()
        {
            IList<NgoaiLeDecisionItem> items;
            try { items = exceptionService.GetPendingDecisions(user); }
            catch (Exception ex)
            {
                ShowExceptionError(ex, "tải đề xuất chờ duyệt");
                items = new List<NgoaiLeDecisionItem>();
            }

            BuildPage("Xét duyệt đề xuất ngoại lệ", null, stack =>
            {
                var proposals = TableSection(stack, "Danh sách Phiếu đề xuất ngoại lệ thuộc thẩm quyền phê duyệt", null,
                    "Mã đề xuất", "Mã hồ sơ", "Tên sự kiện", "Cán bộ đề xuất", "Tiêu chí áp dụng", "Cấp phê duyệt", "Thời gian trình");
                var grid = BodyOf(proposals).Controls.OfType<DataGridView>().First();
                ConfigureApproverGrid(grid);
                foreach (NgoaiLeDecisionItem item in items)
                {
                    int row = grid.Rows.Add(
                        "NL-" + item.ExceptionId.ToString("000"),
                        "HS-" + item.CaseId.ToString("0000"),
                        item.EventName,
                        item.ProposerName,
                        item.RuleCode + " (Phiên bản " + item.Version + ")",
                        "Cấp " + item.RequiredApprovalLevel,
                        item.ProposedAt.ToString("dd/MM/yyyy HH:mm"));
                    grid.Rows[row].Tag = item;
                }

                var detailSection = AddSection(stack, "Chi tiết Phiếu đề xuất ngoại lệ và phương án bù đắp", null, 250);
                var details = new TextBox
                {
                    Multiline = true,
                    ReadOnly = true,
                    ScrollBars = ScrollBars.Vertical,
                    Width = 680,
                    Height = 175,
                    MaxLength = 20000,
                    Font = new Font("Arial", 9.5f),
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = Color.White
                };
                BodyOf(detailSection).Controls.Add(details);
                ConfigureResponsiveTextBox(detailSection, details);

                Action updateDetails = () =>
                {
                    NgoaiLeDecisionItem selected = grid.CurrentRow == null
                        ? null : grid.CurrentRow.Tag as NgoaiLeDecisionItem;
                    details.Text = selected == null
                        ? "Vui lòng chọn một phiếu đề xuất ngoại lệ trong bảng phía trên để xem nội dung giải trình và phương án bù đắp."
                        : "Mã hồ sơ: HS-" + selected.CaseId.ToString("0000") + " — Sự kiện: " + selected.EventName +
                          "\r\nCán bộ trình đề xuất: " + selected.ProposerName +
                          "\r\nTiêu chí xin xét ngoại lệ: " + selected.RuleCode + " (Phiên bản " + selected.Version + "), Nhóm: " + selected.Group +
                          "\r\nCấp thẩm quyền phê duyệt: Cấp " + selected.RequiredApprovalLevel +
                          "\r\nNhận xét lúc thẩm định: " + (selected.AssessmentNote ?? "Không có") +
                          "\r\n\r\n1. Lý do đề xuất ngoại lệ:\r\n" + selected.Reason +
                          "\r\n\r\n2. Minh chứng kèm theo:\r\n" + selected.Evidence +
                          "\r\n\r\n3. Biện pháp tăng cường an toàn bù đắp:\r\n" + selected.Mitigation;
                };
                grid.SelectionChanged += (s, e) => updateDetails();
                updateDetails();

                var decision = AddSection(stack, "Ý kiến chỉ đạo và căn cứ quyết định của lãnh đạo", null, 155);
                var note = AddDecisionInput(decision, 75);

                var actions = AddSection(stack, "Phê duyệt đề xuất ngoại lệ", null, 100);
                AddButtons(actions,
                    ActionButton("Chấp thuận phương án ngoại lệ", () => Decide(grid, note, true), true),
                    ActionButton("Không chấp thuận ngoại lệ", () => Decide(grid, note, false)),
                    ActionButton("Xem Phiếu đề xuất ngoại lệ (VB05)", () =>
                    {
                        NgoaiLeDecisionItem selected = grid.CurrentRow == null ? null : grid.CurrentRow.Tag as NgoaiLeDecisionItem;
                        var hs = selected == null
                            ? null
                            : new HoSoCapPhep
                            {
                                ma_ho_so = selected.CaseId,
                                ten_su_kien = selected.EventName,
                                trang_thai = "XU_LY_NGOAI_LE"
                            };
                        WordExportService.ShowPreviewDialog(this, "VB05", hs, selected == null ? null : selected.Reason);
                    }),
                    ActionButton("Làm mới danh sách", () => Exceptions()));
            });
        }

        private void Decide(DataGridView grid, TextBox note, bool approve)
        {
            NgoaiLeDecisionItem selected = grid.CurrentRow == null
                ? null : grid.CurrentRow.Tag as NgoaiLeDecisionItem;
            if (selected == null)
            {
                ShowExceptionMessage("Vui lòng chọn một đề xuất ngoại lệ trước khi ra quyết định.", "Chưa chọn đề xuất");
                return;
            }

            try
            {
                exceptionService.Decide(user, selected.ExceptionId, approve, note.Text);
                ShowExceptionMessage(
                    approve
                        ? "Đã chấp thuận phương án ngoại lệ và chuyển hồ sơ sang bước chờ phê duyệt cấp phép."
                        : "Đã từ chối đề xuất ngoại lệ.",
                    "Đã lưu quyết định");
                Exceptions();
            }
            catch (Exception ex) { ShowExceptionError(ex, "lưu quyết định ngoại lệ"); }
        }

        private void ShowExceptionError(Exception exception, string operation)
        {
            LocalExceptionLogger.Write(exception);
            string msg = exception != null && !string.IsNullOrWhiteSpace(exception.Message)
                ? exception.Message
                : ("Không thể " + operation + " lúc này.");
            ShowExceptionMessage(msg, "Thông báo");
        }

        private void ShowExceptionMessage(string message, string title)
        {
            MessageBox.Show(this, message, title, MessageBoxButtons.OK,
                title == "Đã lưu quyết định" ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
        }

        private void Decision()
        {
            IList<LicenseDecisionCase> cases = LoadDecisionCases();
            BuildPage("Quyết định phê duyệt và cấp phép tổ chức sự kiện", null, stack =>
            {
                var caseSection = TableSection(stack, "Chọn hồ sơ cần ra quyết định", null,
                    "Mã hồ sơ", "Tên sự kiện", "Trạng thái hiện tại", "Quy mô khách", "Địa điểm tổ chức", "Đánh giá tổng hợp");
                var grid = BodyOf(caseSection).Controls.OfType<DataGridView>().First();
                ConfigureApproverGrid(grid);
                foreach (LicenseDecisionCase item in cases)
                {
                    int row = grid.Rows.Add(
                        "HS-" + item.CaseId.ToString("0000"),
                        item.EventName,
                        FormatStatus(item.Status),
                        item.Attendance + " người",
                        item.VenueName ?? "—",
                        item.CanApprove ? "Đủ điều kiện cấp phép" : "Chưa đạt điều kiện");
                    grid.Rows[row].Tag = item;
                }

                var facts = AddSection(stack, "Tóm tắt kết quả thẩm định, ý kiến liên ngành và phương án an toàn", null, 225);
                var details = new TextBox
                {
                    Multiline = true,
                    ReadOnly = true,
                    ScrollBars = ScrollBars.Vertical,
                    Width = 680,
                    Height = 155,
                    Font = new Font("Arial", 9.5f),
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = Color.White
                };
                BodyOf(facts).Controls.Add(details);
                ConfigureResponsiveTextBox(facts, details);

                var review = AddSection(stack, "Chi tiết ý kiến các đơn vị liên ngành, tài liệu và lịch sử xử lý", null, 260);
                var reviewText = new TextBox
                {
                    Multiline = true,
                    ReadOnly = true,
                    ScrollBars = ScrollBars.Vertical,
                    Width = 680,
                    Height = 190,
                    Font = new Font("Arial", 9.5f),
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = Color.White
                };
                BodyOf(review).Controls.Add(reviewText);
                ConfigureResponsiveTextBox(review, reviewText);

                Action refreshDetails = () =>
                {
                    LicenseDecisionCase selected = SelectedDecisionCase(grid);
                    details.Text = selected == null
                        ? "Vui lòng chọn một hồ sơ trong danh sách phía trên để xem toàn bộ kết quả thẩm định."
                        : "Mã hồ sơ: HS-" + selected.CaseId.ToString("0000") + " — Tên sự kiện: " + selected.EventName +
                          "\r\nTrạng thái hiện tại: " + FormatStatus(selected.Status) +
                          "\r\nThời gian nộp hồ sơ: " + FormatDate(selected.SubmittedAt) +
                          "\r\nKhung thời gian tổ chức: " + FormatDate(selected.StartsAt) + " đến " + FormatDate(selected.EndsAt) +
                          "\r\nĐịa điểm tổ chức: " + (selected.VenueName ?? "Chưa cập nhật") + " (" + (selected.VenueAddress ?? string.Empty) + ")" +
                          "\r\nQuy mô đăng ký / Sức chứa tối đa: " + selected.Attendance + " / " + selected.VenueCapacity + " người" +
                          "\r\nTổng số tiêu chí đã thẩm định: " + selected.AssessedRuleCount +
                          " | Tiêu chí chưa đạt: " + selected.MandatoryFailureCount +
                          " | Ngoại lệ đã chấp thuận: " + selected.ApprovedExceptionCount +
                          "\r\nSố đơn vị liên ngành đã gửi ý kiến: " + selected.RespondedOpinionCount +
                          "\r\nĐánh giá điều kiện cấp giấy phép: " + (selected.CanApprove ? "Đủ điều kiện cấp giấy phép" : "Chưa đủ điều kiện cấp giấy phép") +
                          (string.IsNullOrWhiteSpace(selected.RejectionReason) ? string.Empty :
                              "\r\nLý do từ chối đã ghi nhận: " + selected.RejectionReason);
                    LoadReviewDetails(selected, reviewText);
                };

                var decision = AddSection(stack, "Điều kiện và yêu cầu kèm theo giấy phép (nếu phê duyệt cấp phép)", null, 150);
                var conditions = AddDecisionInput(decision, 72);
                conditions.Text = "Thực hiện đúng phương án đảm bảo an ninh trật tự, an toàn PCCC, y tế và kiểm soát số lượng người tham dự không vượt quá quy mô được duyệt.";

                var rejection = AddSection(stack, "Lý do không chấp thuận cấp phép (bắt buộc nhập nếu từ chối hồ sơ)", null, 150);
                var reason = AddDecisionInput(rejection, 72);

                var actions = AddSection(stack, "Ban hành quyết định phê duyệt và văn bản hành chính", null, 110);
                var approveButton = ActionButton("Phê duyệt & Cấp giấy phép chính thức",
                    () => DecideCase(grid, conditions, reason, true), true);
                var rejectButton = ActionButton("Từ chối cấp phép hồ sơ",
                    () => DecideCase(grid, conditions, reason, false));

                AddButtons(
                    actions,
                    approveButton,
                    rejectButton,
                    ActionButton("Xem Văn bản chấp thuận (VB06)", () =>
                    {
                        var selected = SelectedDecisionCase(grid);
                        WordExportService.ShowPreviewDialog(this, "VB06", ToHoSo(selected), conditions.Text);
                    }),
                    ActionButton("Xem Giấy phép (VB08)", () =>
                    {
                        var selected = SelectedDecisionCase(grid);
                        WordExportService.ShowPreviewDialog(this, "VB08", ToHoSo(selected), conditions.Text);
                    }),
                    ActionButton("Xem Thông báo từ chối (VB07)", () =>
                    {
                        var selected = SelectedDecisionCase(grid);
                        WordExportService.ShowPreviewDialog(this, "VB07", ToHoSo(selected), reason.Text);
                    }),
                    ActionButton("Làm mới", () => Decision()));

                Action updateSelection = () =>
                {
                    LicenseDecisionCase selected = SelectedDecisionCase(grid);
                    refreshDetails();
                    approveButton.Enabled = selected != null && selected.CanApprove;
                    rejectButton.Enabled = selected != null &&
                        (selected.Status == "CHO_PHE_DUYET" ||
                         selected.Status == "TONG_HOP_LIEN_NGANH");
                };
                grid.SelectionChanged += (s, e) => updateSelection();
                updateSelection();
            });
        }

        private static HoSoCapPhep ToHoSo(LicenseDecisionCase selected)
        {
            if (selected == null) return null;
            return new HoSoCapPhep
            {
                ma_ho_so = selected.CaseId,
                ten_su_kien = selected.EventName,
                ten_dia_diem = selected.VenueName,
                dia_chi_dia_diem = selected.VenueAddress,
                quy_mo = selected.Attendance,
                suc_chua_dia_diem = selected.VenueCapacity,
                bat_dau = selected.StartsAt,
                ket_thuc = selected.EndsAt,
                ngay_nop = selected.SubmittedAt,
                trang_thai = selected.Status,
                ly_do_tu_choi = selected.RejectionReason
            };
        }

        private void Licenses()
        {
            IList<IssuedLicenseItem> licenses;
            try { licenses = licenseService.GetIssuedLicenses(user); }
            catch (Exception ex)
            {
                ShowDecisionError(ex, "tải giấy phép đã cấp");
                licenses = new List<IssuedLicenseItem>();
            }
            BuildPage("Quản lý Giấy phép và Văn bản chấp thuận đã ban hành", null, stack =>
            {
                var section = TableSection(stack, "Danh sách Giấy phép tổ chức sự kiện đã cấp", null,
                    "Số giấy phép", "Mã hồ sơ", "Tên sự kiện", "Địa điểm tổ chức", "Ngày cấp phép", "Thời hạn hiệu lực");
                var grid = BodyOf(section).Controls.OfType<DataGridView>().First();
                ConfigureApproverGrid(grid);
                foreach (IssuedLicenseItem item in licenses)
                {
                    int row = grid.Rows.Add(
                        item.LicenseNumber,
                        "HS-" + item.CaseId.ToString("0000"),
                        item.EventName,
                        item.VenueName,
                        item.IssuedAt.ToString("dd/MM/yyyy"),
                        item.ValidFrom.ToString("dd/MM/yyyy HH:mm") + " — " + item.ValidUntil.ToString("dd/MM/yyyy HH:mm"));
                    grid.Rows[row].Tag = item;
                }
                var detailSection = AddSection(stack, "Điều kiện và yêu cầu kèm theo giấy phép", null, 150);
                var details = new TextBox
                {
                    Multiline = true,
                    ReadOnly = true,
                    ScrollBars = ScrollBars.Vertical,
                    Width = 680,
                    Height = 76,
                    Font = new Font("Arial", 9.5f),
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = Color.White
                };
                BodyOf(detailSection).Controls.Add(details);
                ConfigureResponsiveTextBox(detailSection, details);
                grid.SelectionChanged += (s, e) =>
                {
                    IssuedLicenseItem selected = grid.CurrentRow == null
                        ? null : grid.CurrentRow.Tag as IssuedLicenseItem;
                    details.Text = selected == null ? string.Empty :
                        (selected.Conditions ?? "Thực hiện đúng nội dung được cấp phép và quy định pháp luật hiện hành.");
                };
                var refresh = AddSection(stack, "Xem trước và Xuất văn bản hành chính (.docx)", null, 105);
                AddButtons(
                    refresh,
                    ActionButton("Xuất Giấy phép chính thức .docx (VB08)", () =>
                    {
                        IssuedLicenseItem selected = grid.CurrentRow == null ? null : grid.CurrentRow.Tag as IssuedLicenseItem;
                        var hs = selected == null
                            ? null
                            : new HoSoCapPhep
                            {
                                ma_ho_so = selected.CaseId,
                                ten_su_kien = selected.EventName,
                                ten_dia_diem = selected.VenueName,
                                so_giay_phep = selected.LicenseNumber,
                                bat_dau = selected.ValidFrom,
                                ket_thuc = selected.ValidUntil,
                                trang_thai = "DA_CAP_PHEP"
                            };
                        WordExportService.ExportToDocxWithDialog(this, "VB08", hs, selected == null ? null : selected.Conditions);
                    }, true),
                    ActionButton("Xem trước Giấy phép (VB08)", () =>
                    {
                        IssuedLicenseItem selected = grid.CurrentRow == null ? null : grid.CurrentRow.Tag as IssuedLicenseItem;
                        var hs = selected == null
                            ? null
                            : new HoSoCapPhep
                            {
                                ma_ho_so = selected.CaseId,
                                ten_su_kien = selected.EventName,
                                ten_dia_diem = selected.VenueName,
                                so_giay_phep = selected.LicenseNumber,
                                bat_dau = selected.ValidFrom,
                                ket_thuc = selected.ValidUntil,
                                trang_thai = "DA_CAP_PHEP"
                            };
                        WordExportService.ShowPreviewDialog(this, "VB08", hs, selected == null ? null : selected.Conditions);
                    }),
                    ActionButton("Xem & Xuất Văn bản chấp thuận (VB06)", () =>
                    {
                        IssuedLicenseItem selected = grid.CurrentRow == null ? null : grid.CurrentRow.Tag as IssuedLicenseItem;
                        var hs = selected == null
                            ? null
                            : new HoSoCapPhep
                            {
                                ma_ho_so = selected.CaseId,
                                ten_su_kien = selected.EventName,
                                ten_dia_diem = selected.VenueName,
                                so_giay_phep = selected.LicenseNumber,
                                bat_dau = selected.ValidFrom,
                                ket_thuc = selected.ValidUntil,
                                trang_thai = "DA_CAP_PHEP"
                            };
                        WordExportService.ShowPreviewDialog(this, "VB06", hs, selected == null ? null : selected.Conditions);
                    }),
                    ActionButton("Làm mới danh sách", () => Licenses()));
            });
        }

        private IList<LicenseDecisionCase> LoadDecisionCases()
        {
            try { return licenseService.GetDecisionCases(user); }
            catch (Exception ex)
            {
                ShowDecisionError(ex, "tải hồ sơ chờ quyết định");
                return new List<LicenseDecisionCase>();
            }
        }

        private static LicenseDecisionCase SelectedDecisionCase(DataGridView grid)
        {
            return grid.CurrentRow == null ? null : grid.CurrentRow.Tag as LicenseDecisionCase;
        }

        private void LoadReviewDetails(LicenseDecisionCase selected, TextBox target)
        {
            if (selected == null)
            {
                target.Text = "Chọn hồ sơ để xem ý kiến các đơn vị liên ngành, danh mục tài liệu và nhật ký xử lý.";
                return;
            }
            try
            {
                ApproverReviewData data = licenseService.GetReviewData(user, selected.CaseId);
                var text = new StringBuilder();
                text.AppendLine("I. Ý KIẾN CÁC ĐƠN VỊ LIÊN NGÀNH:");
                if (data.Opinions.Count == 0) text.AppendLine("- Chưa có ý kiến phản hồi.");
                foreach (ApproverOpinionItem opinion in data.Opinions)
                    text.AppendLine("- " + (opinion.UnitName ?? "Đơn vị phối hợp") + " | " +
                        FormatStatus(opinion.Status) + " | Kết luận: " + FormatStatus(opinion.Conclusion) +
                        ": " + (string.IsNullOrWhiteSpace(opinion.Response) ? "Chưa có nội dung" : opinion.Response));
                text.AppendLine();
                text.AppendLine("II. TÀI LIỆU ĐÍNH KÈM TRONG HỒ SƠ:");
                if (data.Documents.Count == 0) text.AppendLine("- Chưa có tài liệu đính kèm.");
                foreach (ApproverDocumentItem document in data.Documents)
                    text.AppendLine("- " + (document.Type ?? "Tài liệu") + ": " + (document.Name ?? "Tệp đính kèm"));
                text.AppendLine();
                text.AppendLine("III. NHẬT KÝ TIẾN ĐỘ XỬ LÝ HỒ SƠ:");
                if (data.History.Count == 0) text.AppendLine("- Chưa có lịch sử xử lý.");
                foreach (ApproverHistoryItem history in data.History)
                    text.AppendLine("- " + history.At.ToString("dd/MM/yyyy HH:mm") + " | " +
                        FormatStatus(history.Action) + " | " + FormatStatus(history.Result ?? string.Empty) +
                        (string.IsNullOrWhiteSpace(history.Note) ? string.Empty : " — " + history.Note));
                target.Text = text.ToString();
            }
            catch (Exception ex)
            {
                target.Text = "Không thể tải chi tiết hồ sơ lúc này. Vui lòng thử lại.";
                LocalExceptionLogger.Write(ex);
            }
        }

        private TextBox AddDecisionInput(Panel section, int height)
        {
            var input = new TextBox
            {
                Multiline = true,
                Height = height,
                Width = 680,
                MaxLength = 4000,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Arial", 9.5f),
                BorderStyle = BorderStyle.FixedSingle
            };
            BodyOf(section).Controls.Add(input);
            ConfigureResponsiveTextBox(section, input);
            return input;
        }

        private void ConfigureResponsiveTextBox(Panel section, TextBox input)
        {
            var body = BodyOf(section);
            input.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            input.Width = Math.Max(360, body.ClientSize.Width - input.Margin.Horizontal - 4);
            ResizeSectionBody(section);
        }

        private static void ConfigureApproverGrid(DataGridView grid)
        {
            grid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            grid.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            grid.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            grid.HorizontalScrollingOffset = 0;
            foreach (DataGridViewColumn column in grid.Columns)
            {
                column.MinimumWidth = 96;
            }
        }

        private void DecideCase(DataGridView grid, TextBox conditions, TextBox reason, bool approve)
        {
            LicenseDecisionCase selected = SelectedDecisionCase(grid);
            if (selected == null)
            {
                ShowDecisionMessage("Vui lòng chọn hồ sơ trước khi ra quyết định.", "Chưa chọn hồ sơ", false);
                return;
            }
            try
            {
                if (approve)
                {
                    if (!selected.CanApprove)
                    {
                        ShowDecisionMessage(
                            "Hồ sơ hiện chưa hoàn tất thẩm định/ngoại lệ hoặc còn thiếu ý kiến liên ngành bắt buộc.",
                            "Chưa đủ điều kiện cấp phép", false);
                        return;
                    }
                    LicenseIssueResult issued = licenseService.Approve(user, selected.CaseId, conditions.Text);
                    ShowDecisionMessage("Đã phê duyệt và cấp giấy phép số " + issued.LicenseNumber +
                        " cho hồ sơ HS-" + issued.CaseId.ToString("0000") + ".", "Đã cấp giấy phép", true);
                }
                else
                {
                    licenseService.Reject(user, selected.CaseId, reason.Text);
                    ShowDecisionMessage("Đã ban hành thông báo không chấp thuận cấp phép cho hồ sơ HS-" + selected.CaseId.ToString("0000") + ".", "Đã từ chối hồ sơ", true);
                }
                Decision();
            }
            catch (Exception ex) { ShowDecisionError(ex, approve ? "phê duyệt hồ sơ" : "từ chối hồ sơ"); }
        }

        private void ShowDecisionError(Exception exception, string operation)
        {
            LocalExceptionLogger.Write(exception);
            string msg = exception != null && !string.IsNullOrWhiteSpace(exception.Message)
                ? exception.Message
                : ("Không thể " + operation + " lúc này.");
            ShowDecisionMessage(msg, "Thông báo", false);
        }

        private void ShowDecisionMessage(string message, string title, bool success)
        {
            MessageBox.Show(this, message, title, MessageBoxButtons.OK,
                success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
        }

        private static string FormatDate(DateTimeOffset? value)
        {
            return value.HasValue ? value.Value.ToString("dd/MM/yyyy HH:mm") : "Chưa cập nhật";
        }
    }
}
