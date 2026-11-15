using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Viewparty.Data;
using Viewparty.Models;
using Viewparty.Services;

namespace Viewparty.Forms
{
    internal sealed class CaseOfficerWorkspaceControl : RoleWorkspaceControl
    {
        private readonly AuthenticatedUser user;
        private readonly InteragencyWorkflowService interagencyService = new InteragencyWorkflowService();
        private readonly NgoaiLeWorkflowService exceptionService = new NgoaiLeWorkflowService();
        private readonly CaseOfficerWorkflowService caseOfficerService = new CaseOfficerWorkflowService();

        public CaseOfficerWorkspaceControl(AuthenticatedUser user)
        {
            this.user = user ?? throw new ArgumentNullException("user");
        }

        public override void OpenModule(string module)
        {
            NotifyModuleChanged(module);
            switch (module)
            {
                case "Tổng quan": Dashboard(); break;
                case "Tiếp nhận hồ sơ": Intake(); break;
                case "Kiểm tra hợp lệ": Validity(); break;
                case "Thẩm định hồ sơ": Review(); break;
                case "Ý kiến liên ngành": InteragencyRequests(); break;
                case "Tổng hợp / ngoại lệ": Summary(); break;
                default: Dashboard(); break;
            }
        }

        private void Dashboard()
        {
            var inbox = LoadInbox();
            var checking = LoadAssigned("DANG_KIEM_TRA");
            var reviewing = LoadAssigned("DANG_THAM_DINH");
            var interagency = LoadInteragencyCases()
                .Where(c => c.Status != "DANG_THAM_DINH")
                .ToList();

            BuildPage("Tổng quan Cán bộ xử lý", null, stack =>
            {
                AddStats(stack,
                    new StatCardSpec("Chờ tiếp nhận", inbox.Count.ToString("00"), "Hồ sơ mới nộp", false),
                    new StatCardSpec("Đang kiểm tra", checking.Count.ToString("00"), "Rà soát hồ sơ", false),
                    new StatCardSpec("Đang thẩm định", reviewing.Count.ToString("00"), "Thẩm định thực tế", false),
                    new StatCardSpec("Liên ngành & Tổng hợp", interagency.Count.ToString("00"), "Theo dõi tiến độ", interagency.Count > 0));

                var section = TableSection(stack, "Danh sách hồ sơ cần xử lý hôm nay", null,
                    "Mã hồ sơ", "Tên sự kiện", "Đơn vị / Loại sự kiện", "Trạng thái hiện tại", "Công việc tiếp theo");
                var grid = BodyOf(section).Controls.OfType<DataGridView>().First();
                foreach (var item in inbox)
                {
                    int r = grid.Rows.Add("HS-" + item.CaseId.ToString("0000"), item.EventName, item.OrganizerName, FormatStatus(item.Status), "Tiếp nhận hồ sơ");
                    grid.Rows[r].Tag = "Tiếp nhận hồ sơ";
                }
                foreach (var item in checking)
                {
                    int r = grid.Rows.Add("HS-" + item.CaseId.ToString("0000"), item.EventName, item.OrganizerName, FormatStatus(item.Status), "Kiểm tra tính hợp lệ");
                    grid.Rows[r].Tag = "Kiểm tra hợp lệ";
                }
                foreach (var item in reviewing)
                {
                    int r = grid.Rows.Add("HS-" + item.CaseId.ToString("0000"), item.EventName, item.OrganizerName, FormatStatus(item.Status), "Thẩm định điều kiện");
                    grid.Rows[r].Tag = "Thẩm định hồ sơ";
                }
                foreach (var item in interagency)
                {
                    string nextMod = item.Status == "CHO_Y_KIEN_LIEN_NGANH" && item.RespondedRequiredUnits < item.RequiredUnits
                        ? "Ý kiến liên ngành"
                        : "Tổng hợp / ngoại lệ";
                    int r = grid.Rows.Add("HS-" + item.CaseId.ToString("0000"), item.EventName, item.EventType, FormatStatus(item.Status),
                        nextMod + " (" + item.RespondedRequiredUnits + "/" + item.RequiredUnits + " đơn vị)");
                    grid.Rows[r].Tag = nextMod;
                }
                if (grid.Rows.Count == 0)
                {
                    grid.Rows.Add("—", "Hiện không có hồ sơ tồn đọng cần xử lý.", "—", "—", "—");
                }
                grid.CellDoubleClick += (s, e) =>
                {
                    if (e.RowIndex >= 0 && grid.Rows[e.RowIndex].Tag is string targetModule)
                    {
                        OpenModule(targetModule);
                    }
                };
            });
        }

        private void Intake()
        {
            IList<CaseOfficerCaseItem> cases = LoadInbox();
            BuildPage("Tiếp nhận hồ sơ mới nộp", null, stack =>
            {
                var section = TableSection(stack, "Hồ sơ đang chờ tiếp nhận", null,
                    "Mã hồ sơ", "Tên sự kiện", "Đơn vị / Người tổ chức", "Trạng thái", "Thời gian nộp");
                var grid = BodyOf(section).Controls.OfType<DataGridView>().First();
                PopulateCases(grid, cases);

                var actions = AddSection(stack, "Thao tác tiếp nhận", null, 115);
                AddButtons(actions,
                    ActionButton("Tiếp nhận hồ sơ đã chọn", () =>
                    {
                        var selected = SelectedCaseOfficerCase(grid);
                        if (selected == null) { ShowWorkflowMessage("Vui lòng chọn một hồ sơ trước khi tiếp nhận.", "Chưa chọn hồ sơ"); return; }
                        try
                        {
                            caseOfficerService.Accept(user, selected.CaseId);
                            ShowWorkflowMessage("Đã tiếp nhận hồ sơ thành công và chuyển sang bước Kiểm tra tính hợp lệ.", "Đã tiếp nhận");
                            Intake();
                        }
                        catch (Exception ex) { ShowWorkflowError(ex, "tiếp nhận hồ sơ"); }
                    }, true),
                    ActionButton("Xem danh mục tài liệu", () => ShowCaseDocuments(grid)),
                    ActionButton("Xuất phiếu tiếp nhận (VB09)", () =>
                    {
                        var selected = SelectedCaseOfficerCase(grid);
                        var hs = selected == null
                            ? null
                            : new HoSoCapPhep
                            {
                                ma_ho_so = selected.CaseId,
                                ten_su_kien = selected.EventName,
                                ten_don_vi_to_chuc = selected.OrganizerName,
                                trang_thai = selected.Status,
                                ngay_nop = selected.SubmittedAt
                            };
                        WordExportService.ShowPreviewDialog(this, "VB09", hs);
                    }),
                    ActionButton("Làm mới danh sách", () => Intake()));
            });
        }

        private void Validity()
        {
            IList<CaseOfficerCaseItem> cases = LoadAssigned("DANG_KIEM_TRA");
            BuildPage("Kiểm tra tính hợp lệ của hồ sơ", null, stack =>
            {
                var list = TableSection(stack, "Hồ sơ đang kiểm tra hợp lệ", null,
                    "Mã hồ sơ", "Tên sự kiện", "Đơn vị / Người tổ chức", "Trạng thái", "Thời gian nộp");
                var grid = BodyOf(list).Controls.OfType<DataGridView>().First();
                PopulateCases(grid, cases);

                var documents = TableSection(stack, "Tài liệu đính kèm trong hồ sơ", null,
                    "Loại tài liệu", "Tên tệp đính kèm", "Đường dẫn lưu trữ");
                var documentGrid = BodyOf(documents).Controls.OfType<DataGridView>().First();

                var decision = AddSection(stack, "Kết luận kiểm tra hợp lệ", null, 220);
                var reason = AddMultilineInput(decision, "Nội dung hướng dẫn / tài liệu cần yêu cầu bổ sung (nếu hồ sơ chưa đầy đủ)");

                AddButtons(decision,
                    ActionButton("Xác nhận hồ sơ hợp lệ", () =>
                    {
                        var selected = SelectedCaseOfficerCase(grid);
                        if (selected == null) { ShowWorkflowMessage("Vui lòng chọn hồ sơ cần kiểm tra.", "Chưa chọn hồ sơ"); return; }
                        try
                        {
                            caseOfficerService.CheckValidity(user, selected.CaseId, true, null);
                            ShowWorkflowMessage("Đã xác nhận hồ sơ hợp lệ và chuyển sang bước Thẩm định chuyên môn.", "Đã xác nhận hợp lệ");
                            Validity();
                        }
                        catch (Exception ex) { ShowWorkflowError(ex, "xác nhận hồ sơ hợp lệ"); }
                    }, true),
                    ActionButton("Yêu cầu bổ sung hồ sơ", () =>
                    {
                        var selected = SelectedCaseOfficerCase(grid);
                        if (selected == null) { ShowWorkflowMessage("Vui lòng chọn hồ sơ cần kiểm tra.", "Chưa chọn hồ sơ"); return; }
                        try
                        {
                            caseOfficerService.CheckValidity(user, selected.CaseId, false, reason.Text);
                            ShowWorkflowMessage("Đã gửi thông báo yêu cầu bổ sung hồ sơ cho đơn vị tổ chức.", "Đã yêu cầu bổ sung");
                            Validity();
                        }
                        catch (Exception ex) { ShowWorkflowError(ex, "yêu cầu bổ sung"); }
                    }),
                    ActionButton("Xuất thông báo bổ sung (VB02)", () =>
                    {
                        var selected = SelectedCaseOfficerCase(grid);
                        var hs = selected == null
                            ? null
                            : new HoSoCapPhep
                            {
                                ma_ho_so = selected.CaseId,
                                ten_su_kien = selected.EventName,
                                ten_don_vi_to_chuc = selected.OrganizerName,
                                trang_thai = selected.Status
                            };
                        WordExportService.ShowPreviewDialog(this, "VB02", hs, reason.Text);
                    }),
                    ActionButton("Tải lại tài liệu", () => LoadDocuments(grid, documentGrid)));

                grid.SelectionChanged += (s, e) =>
                {
                    if (SelectedCaseOfficerCase(grid) != null)
                        LoadDocuments(grid, documentGrid);
                };
                if (cases.Count > 0)
                    LoadDocuments(grid, documentGrid);
            });
        }

        private void Review()
        {
            IList<CaseOfficerCaseItem> cases = LoadAssigned("DANG_THAM_DINH");
            BuildPage("Thẩm định điều kiện tổ chức sự kiện", null, stack =>
            {
                var list = TableSection(stack, "Hồ sơ đang thẩm định chuyên môn", null,
                    "Mã hồ sơ", "Tên sự kiện", "Đơn vị / Người tổ chức", "Trạng thái", "Thời gian nộp");
                var caseGrid = BodyOf(list).Controls.OfType<DataGridView>().First();
                PopulateCases(caseGrid, cases);

                var rulesSection = TableSection(stack, "Danh mục tiêu chí thẩm định áp dụng cho sự kiện", null,
                    "Mã tiêu chí", "Phiên bản", "Nhóm đánh giá", "Tính chất", "Kết quả đánh giá", "Nhận xét thẩm định");
                var rulesGrid = BodyOf(rulesSection).Controls.OfType<DataGridView>().First();

                var result = AddSection(stack, "Ghi nhận kết quả thẩm định tiêu chí", null, 280);
                var resultBox = new ComboBox
                {
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Height = 32,
                    Font = new Font("Arial", 9.5F)
                };
                resultBox.Items.AddRange(new object[]
                {
                    "Đạt yêu cầu",
                    "Chưa đạt yêu cầu",
                    "Cần lưu ý kiểm tra"
                });
                resultBox.SelectedIndex = 0;

                var actual = new TextBox
                {
                    Text = "Đã đối chiếu thực tế hồ sơ, sức chứa địa điểm và phương án đảm bảo an toàn."
                };
                var note = new TextBox
                {
                    Text = "Đáp ứng tiêu chí thẩm định theo quy định hiện hành."
                };

                AddFieldRows(result,
                    new FieldSpec("Kết quả đánh giá tiêu chí", false, null, resultBox),
                    new FieldSpec("Số liệu / Kết quả đối chiếu thực tế", false, null, actual),
                    new FieldSpec("Nhận xét chi tiết của cán bộ thẩm định", true, null, note));

                rulesGrid.SelectionChanged += (s, e) =>
                {
                    var selectedRule = rulesGrid.CurrentRow == null ? null : rulesGrid.CurrentRow.Tag as CaseOfficerRuleItem;
                    if (selectedRule != null)
                    {
                        if (selectedRule.Result == "KHONG_DAT") resultBox.SelectedIndex = 1;
                        else if (selectedRule.Result == "CANH_BAO") resultBox.SelectedIndex = 2;
                        else resultBox.SelectedIndex = 0;

                        if (!string.IsNullOrWhiteSpace(selectedRule.Note))
                            note.Text = selectedRule.Note;
                    }
                };

                AddButtons(result,
                    ActionButton("Lưu kết quả tiêu chí đang chọn", () =>
                    {
                        var selectedCase = SelectedCaseOfficerCase(caseGrid);
                        var selectedRule = rulesGrid.CurrentRow == null ? null : rulesGrid.CurrentRow.Tag as CaseOfficerRuleItem;
                        if (selectedCase == null || selectedRule == null)
                        {
                            ShowWorkflowMessage("Vui lòng chọn hồ sơ và tiêu chí cần đánh giá trước khi lưu.", "Chưa chọn dữ liệu");
                            return;
                        }
                        string codeResult = resultBox.SelectedIndex == 1
                            ? "KHONG_DAT"
                            : (resultBox.SelectedIndex == 2 ? "CANH_BAO" : "DAT");
                        try
                        {
                            string wrappedJson = WrapPlainTextAsJson(actual.Text, "ket_qua_thuc_te");
                            caseOfficerService.SaveRuleResult(user, selectedCase.CaseId, selectedRule.RuleId, codeResult, wrappedJson, note.Text);
                            ShowWorkflowMessage("Đã lưu kết quả đánh giá cho tiêu chí " + selectedRule.RuleCode + ".", "Đã lưu kết quả");
                            LoadRules(caseGrid, rulesGrid);
                        }
                        catch (Exception ex) { ShowWorkflowError(ex, "lưu kết quả thẩm định"); }
                    }, true),
                    ActionButton("Hoàn tất thẩm định hồ sơ", () =>
                    {
                        var selected = SelectedCaseOfficerCase(caseGrid);
                        if (selected == null) { ShowWorkflowMessage("Vui lòng chọn hồ sơ cần hoàn tất thẩm định.", "Chưa chọn hồ sơ"); return; }
                        try
                        {
                            caseOfficerService.FinishAppraisal(user, selected.CaseId);
                            ShowWorkflowMessage("Đã hoàn tất bước thẩm định tiêu chí. Bạn có thể chuyển sang gửi yêu cầu lấy ý kiến liên ngành.", "Đã kết thúc thẩm định");
                            Review();
                        }
                        catch (Exception ex) { ShowWorkflowError(ex, "kết thúc thẩm định"); }
                    }),
                    ActionButton("Xuất biên bản kiểm tra thực tế (VB10)", () =>
                    {
                        var selected = SelectedCaseOfficerCase(caseGrid);
                        var hs = selected == null
                            ? null
                            : new HoSoCapPhep
                            {
                                ma_ho_so = selected.CaseId,
                                ten_su_kien = selected.EventName,
                                ten_don_vi_to_chuc = selected.OrganizerName,
                                trang_thai = selected.Status
                            };
                        WordExportService.ShowPreviewDialog(this, "VB10", hs, note.Text);
                    }),
                    ActionButton("Tải lại tiêu chí", () => LoadRules(caseGrid, rulesGrid)));

                caseGrid.SelectionChanged += (s, e) =>
                {
                    if (SelectedCaseOfficerCase(caseGrid) != null)
                        LoadRules(caseGrid, rulesGrid);
                };
                if (cases.Count > 0)
                    LoadRules(caseGrid, rulesGrid);
            });
        }

        private IList<CaseOfficerCaseItem> LoadInbox()
        {
            try { return caseOfficerService.GetInbox(user); }
            catch (Exception ex) { ShowWorkflowError(ex, "tải hồ sơ chờ tiếp nhận"); return new List<CaseOfficerCaseItem>(); }
        }

        private IList<CaseOfficerCaseItem> LoadAssigned(string status)
        {
            try { return caseOfficerService.GetAssigned(user, status); }
            catch (Exception ex) { ShowWorkflowError(ex, "tải hồ sơ được phân công"); return new List<CaseOfficerCaseItem>(); }
        }

        private static void PopulateCases(DataGridView grid, IList<CaseOfficerCaseItem> cases)
        {
            grid.Rows.Clear();
            foreach (var item in cases)
            {
                int row = grid.Rows.Add(
                    "HS-" + item.CaseId.ToString("0000"),
                    item.EventName,
                    item.OrganizerName,
                    FormatStatus(item.Status),
                    item.SubmittedAt.HasValue ? item.SubmittedAt.Value.ToString("dd/MM/yyyy HH:mm") : "—");
                grid.Rows[row].Tag = item;
            }
        }

        private static CaseOfficerCaseItem SelectedCaseOfficerCase(DataGridView grid)
        {
            return grid == null || grid.CurrentRow == null ? null : grid.CurrentRow.Tag as CaseOfficerCaseItem;
        }

        private void LoadDocuments(DataGridView caseGrid, DataGridView documentGrid)
        {
            var selected = SelectedCaseOfficerCase(caseGrid);
            if (selected == null) return;
            try
            {
                documentGrid.Rows.Clear();
                foreach (var item in caseOfficerService.GetDocuments(user, selected.CaseId))
                {
                    documentGrid.Rows.Add(item.Type, item.Name, item.Path);
                }
            }
            catch (Exception ex) { ShowWorkflowError(ex, "tải tài liệu hồ sơ"); }
        }

        private void LoadRules(DataGridView caseGrid, DataGridView rulesGrid)
        {
            var selected = SelectedCaseOfficerCase(caseGrid);
            if (selected == null) return;
            try
            {
                rulesGrid.Rows.Clear();
                foreach (var item in caseOfficerService.GetRules(user, selected.CaseId))
                {
                    int row = rulesGrid.Rows.Add(
                        item.RuleCode,
                        "Phiên bản " + item.Version,
                        item.Group,
                        FormatStatus(item.Severity),
                        string.IsNullOrWhiteSpace(item.Result) ? "Chưa đánh giá" : FormatStatus(item.Result),
                        item.Note ?? "");
                    rulesGrid.Rows[row].Tag = item;
                }
            }
            catch (Exception ex) { ShowWorkflowError(ex, "tải tiêu chí thẩm định"); }
        }

        private void ShowCaseDocuments(DataGridView caseGrid)
        {
            var selected = SelectedCaseOfficerCase(caseGrid);
            if (selected == null) { ShowWorkflowMessage("Vui lòng chọn một hồ sơ trước.", "Chưa chọn hồ sơ"); return; }
            try
            {
                var docs = caseOfficerService.GetDocuments(user, selected.CaseId);
                ShowWorkflowMessage(
                    docs.Count == 0
                        ? "Hồ sơ hiện chưa có tài liệu đính kèm."
                        : string.Join("\n", docs.Select(item => "• " + item.Type + ": " + item.Name)),
                    "Danh mục tài liệu đính kèm");
            }
            catch (Exception ex) { ShowWorkflowError(ex, "tải tài liệu hồ sơ"); }
        }

        private void InteragencyRequests()
        {
            IList<InteragencyCaseItem> cases = LoadInteragencyCases();
            BuildPage("Lấy ý kiến phối hợp liên ngành", null, stack =>
            {
                var section = TableSection(stack, "Danh sách hồ sơ phối hợp lấy ý kiến chuyên môn", null,
                    "Mã hồ sơ", "Tên sự kiện", "Loại hình sự kiện", "Trạng thái hiện tại", "Tiến độ phản hồi liên ngành");
                var grid = BodyOf(section).Controls.OfType<DataGridView>().First();
                foreach (var item in cases)
                {
                    int row = grid.Rows.Add(
                        "HS-" + item.CaseId.ToString("0000"),
                        item.EventName,
                        item.EventType,
                        FormatStatus(item.Status),
                        item.RespondedRequiredUnits + " / " + item.RequiredUnits + " đơn vị bắt buộc");
                    grid.Rows[row].Tag = item;
                }

                var actions = AddSection(stack, "Thao tác gửi văn bản lấy ý kiến", null, 115);
                AddButtons(actions,
                    ActionButton("Gửi yêu cầu lấy ý kiến các đơn vị liên ngành", () =>
                    {
                        InteragencyCaseItem selected = SelectedCase(grid);
                        if (selected == null)
                        {
                            ShowWorkflowMessage("Vui lòng chọn một hồ sơ trước khi gửi yêu cầu.", "Chưa chọn hồ sơ");
                            return;
                        }
                        try
                        {
                            int created = interagencyService.CreateRequiredRequests(user, selected.CaseId);
                            ShowWorkflowMessage(
                                "Đã gửi " + created + " yêu cầu phối hợp thẩm định liên ngành mới.",
                                "Đã xử lý yêu cầu");
                            InteragencyRequests();
                        }
                        catch (Exception ex) { ShowWorkflowError(ex, "gửi yêu cầu liên ngành"); }
                    }, true),
                    ActionButton("Xuất công văn xin ý kiến liên ngành (VB03)", () =>
                    {
                        InteragencyCaseItem selected = SelectedCase(grid);
                        var hs = selected == null
                            ? null
                            : new HoSoCapPhep
                            {
                                ma_ho_so = selected.CaseId,
                                ten_su_kien = selected.EventName,
                                ten_loai = selected.EventType,
                                trang_thai = selected.Status
                            };
                        WordExportService.ShowPreviewDialog(this, "VB03", hs);
                    }),
                    ActionButton("Làm mới danh sách", () => InteragencyRequests()));
            });
        }

        private void Summary()
        {
            IList<InteragencyCaseItem> cases = LoadInteragencyCases()
                .Where(item => item.Status != "DANG_THAM_DINH").ToList();
            BuildPage("Tổng hợp kết quả thẩm định & Trình xét ngoại lệ", null, stack =>
            {
                var section = TableSection(stack, "Danh sách hồ sơ chờ tổng hợp hoặc trình xét ngoại lệ", null,
                    "Mã hồ sơ", "Tên sự kiện", "Loại hình sự kiện", "Trạng thái hiện tại", "Tiến độ phản hồi liên ngành");
                var grid = BodyOf(section).Controls.OfType<DataGridView>().First();
                foreach (var item in cases)
                {
                    int row = grid.Rows.Add(
                        "HS-" + item.CaseId.ToString("0000"),
                        item.EventName,
                        item.EventType,
                        FormatStatus(item.Status),
                        item.RespondedRequiredUnits + " / " + item.RequiredUnits + " đơn vị bắt buộc");
                    grid.Rows[row].Tag = item;
                }

                var actions = AddSection(stack, "Thao tác tổng hợp và đề xuất xử lý", null, 130);
                AddButtons(actions,
                    ActionButton("Tổng hợp kết quả thẩm định hồ sơ", () =>
                    {
                        InteragencyCaseItem selected = SelectedCase(grid);
                        if (selected == null)
                        {
                            ShowWorkflowMessage("Vui lòng chọn hồ sơ cần tổng hợp.", "Chưa chọn hồ sơ");
                            return;
                        }
                        if (selected.Status != "CHO_Y_KIEN_LIEN_NGANH" &&
                            selected.Status != "TONG_HOP_LIEN_NGANH")
                        {
                            ShowWorkflowMessage(
                                selected.Status == "XU_LY_NGOAI_LE"
                                    ? "Hồ sơ đã được tổng hợp và đang cần lập Phiếu đề xuất ngoại lệ. Vui lòng nhấn nút 'Lập phiếu đề xuất ngoại lệ'."
                                    : "Hồ sơ hiện đang ở bước '" + FormatStatus(selected.Status) + "', chưa cần tổng hợp lại.",
                                "Thông báo");
                            return;
                        }
                        try
                        {
                            InteragencySummaryResult result = interagencyService.Summarize(user, selected.CaseId);
                            ShowWorkflowMessage(
                                "Tổng số tiêu chí đã đánh giá: " + result.RuleCount +
                                "\nTiêu chí bắt buộc chưa đạt: " + result.MandatoryFailures +
                                "\nTiêu chí cần lưu ý: " + result.Warnings +
                                "\nKết luận đề xuất: " + FormatStatus(result.ProposedConclusion) +
                                "\nTrạng thái chuyển tiếp: " + FormatStatus(result.NextStatus),
                                "Tổng hợp hoàn tất");
                            Summary();
                        }
                        catch (Exception ex) { ShowWorkflowError(ex, "tổng hợp thẩm định"); }
                    }, true),
                    ActionButton("Lập phiếu đề xuất ngoại lệ", () =>
                    {
                        InteragencyCaseItem selected = SelectedCase(grid);
                        if (selected == null)
                        {
                            ShowWorkflowMessage("Vui lòng chọn hồ sơ cần đề xuất ngoại lệ.", "Chưa chọn hồ sơ");
                            return;
                        }
                        if (selected.Status != "XU_LY_NGOAI_LE")
                        {
                            ShowWorkflowMessage(
                                "Chức năng lập phiếu đề xuất ngoại lệ áp dụng cho hồ sơ đang ở trạng thái 'Đang trình xét ngoại lệ' sau khi tổng hợp.",
                                "Chưa thể đề xuất");
                            return;
                        }
                        ExceptionRequest(selected.CaseId, selected.EventName);
                    }),
                    ActionButton("Xuất phiếu đề xuất ngoại lệ (VB05)", () =>
                    {
                        InteragencyCaseItem selected = SelectedCase(grid);
                        var hs = selected == null
                            ? null
                            : new HoSoCapPhep
                            {
                                ma_ho_so = selected.CaseId,
                                ten_su_kien = selected.EventName,
                                ten_loai = selected.EventType,
                                trang_thai = selected.Status
                            };
                        WordExportService.ShowPreviewDialog(this, "VB05", hs);
                    }),
                    ActionButton("Làm mới danh sách", () => Summary()));
            });
        }

        private IList<InteragencyCaseItem> LoadInteragencyCases()
        {
            try { return interagencyService.GetAssignedCases(user); }
            catch (Exception ex)
            {
                ShowWorkflowError(ex, "tải hồ sơ liên ngành");
                return new List<InteragencyCaseItem>();
            }
        }

        private static InteragencyCaseItem SelectedCase(DataGridView grid)
        {
            return grid.CurrentRow == null ? null : grid.CurrentRow.Tag as InteragencyCaseItem;
        }

        private void ShowWorkflowError(Exception exception, string operation)
        {
            LocalExceptionLogger.Write(exception);
            string detail = exception != null && !string.IsNullOrWhiteSpace(exception.Message)
                ? exception.Message
                : ("Không thể " + operation + " lúc này.");
            ShowWorkflowMessage(detail, "Thông báo xử lý");
        }

        private void ShowWorkflowMessage(string message, string title)
        {
            MessageBox.Show(this, message, title, MessageBoxButtons.OK,
                title == "Tổng hợp hoàn tất" || title == "Đã gửi đề xuất" || title == "Đã tiếp nhận" || title == "Đã xác nhận hợp lệ" || title == "Đã lưu kết quả"
                    ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
        }

        private void ExceptionRequest(long caseId, string eventName = null)
        {
            IList<NgoaiLeEligibleResult> results;
            try { results = exceptionService.GetEligibleResults(user, caseId); }
            catch (Exception ex)
            {
                ShowWorkflowError(ex, "tải danh mục tiêu chí cần xét ngoại lệ");
                return;
            }

            BuildPage("Lập phiếu đề xuất ngoại lệ — Hồ sơ HS-" + caseId.ToString("0000"), null, stack =>
            {
                var rules = TableSection(stack, "Tiêu chí cần trình lãnh đạo xem xét ngoại lệ", null,
                    "Mã kết quả", "Mã tiêu chí", "Phiên bản", "Nhóm đánh giá", "Cấp phê duyệt", "Ghi chú thẩm định");
                var grid = BodyOf(rules).Controls.OfType<DataGridView>().First();
                foreach (NgoaiLeEligibleResult item in results)
                {
                    int row = grid.Rows.Add(
                        "KQ-" + item.ResultId.ToString("000"),
                        item.RuleCode,
                        "Phiên bản " + item.Version,
                        item.Group,
                        "Cấp " + item.ApprovalLevel,
                        item.Note);
                    grid.Rows[row].Tag = item;
                }

                var proposal = AddSection(stack, "Nội dung Phiếu đề xuất ngoại lệ", null, 380);
                var reason = AddMultilineInput(proposal, "Lý do đề xuất xem xét ngoại lệ");
                var evidence = AddMultilineInput(proposal, "Tài liệu / Minh chứng giải trình kèm theo");
                var mitigation = AddMultilineInput(proposal, "Biện pháp tăng cường đảm bảo an toàn bù đắp");

                var buttons = AddSection(stack, "Trình cấp thẩm quyền phê duyệt", null, 105);
                AddButtons(buttons,
                    ActionButton("Gửi phiếu đề xuất ngoại lệ", () =>
                    {
                        NgoaiLeEligibleResult selected = grid.CurrentRow == null
                            ? null : grid.CurrentRow.Tag as NgoaiLeEligibleResult;
                        if (selected == null)
                        {
                            ShowWorkflowMessage("Vui lòng chọn một tiêu chí trong bảng trước khi gửi đề xuất.", "Chưa chọn tiêu chí");
                            return;
                        }
                        try
                        {
                            long exceptionId = exceptionService.Propose(user, caseId, selected.ResultId,
                                reason.Text, evidence.Text, mitigation.Text);
                            ShowWorkflowMessage("Đã trình Phiếu đề xuất ngoại lệ (Mã đề xuất #" + exceptionId + ") lên lãnh đạo phê duyệt.", "Đã gửi đề xuất");
                            Summary();
                        }
                        catch (Exception ex) { ShowWorkflowError(ex, "gửi đề xuất ngoại lệ"); }
                    }, true),
                    ActionButton("Xem & Xuất Phiếu đề xuất ngoại lệ (VB05)", () =>
                    {
                        var hs = new HoSoCapPhep
                        {
                            ma_ho_so = caseId,
                            ten_su_kien = eventName ?? ("Sự kiện thuộc hồ sơ HS-" + caseId.ToString("0000")),
                            trang_thai = "XU_LY_NGOAI_LE"
                        };
                        WordExportService.ShowPreviewDialog(this, "VB05", hs, reason.Text);
                    }),
                    ActionButton("← Quay lại màn hình tổng hợp", () => Summary()));
            });
        }

        private TextBox AddMultilineInput(Panel section, string label)
        {
            var body = BodyOf(section);
            body.Controls.Add(new Label
            {
                Text = label,
                AutoSize = true,
                ForeColor = Ink,
                Font = new Font("Arial", 9.5f, FontStyle.Bold),
                Margin = new Padding(2, 6, 2, 4)
            });
            var input = new TextBox
            {
                Multiline = true,
                Height = 82,
                Width = Math.Max(420, body.ClientSize.Width - 12),
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Arial", 9.5f),
                BorderStyle = BorderStyle.FixedSingle,
                MaxLength = 4000,
                Margin = new Padding(2, 0, 4, 8)
            };
            body.Controls.Add(input);
            body.Resize += (s, e) =>
            {
                if (!input.IsDisposed)
                    input.Width = Math.Max(420, body.ClientSize.Width - 12);
            };
            ResizeSectionBody(section);
            return input;
        }
    }
}
