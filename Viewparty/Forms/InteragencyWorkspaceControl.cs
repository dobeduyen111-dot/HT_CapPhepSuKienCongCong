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
    internal sealed class InteragencyWorkspaceControl : RoleWorkspaceControl
    {
        private readonly AuthenticatedUser user;
        private readonly InteragencyWorkflowService workflow = new InteragencyWorkflowService();

        public InteragencyWorkspaceControl(AuthenticatedUser user)
        {
            this.user = user ?? throw new ArgumentNullException("user");
        }

        public override void OpenModule(string module)
        {
            NotifyModuleChanged(module);
            switch (module)
            {
                case "Tổng quan": Dashboard(); break;
                case "Yêu cầu cần phản hồi": Requests(); break;
                case "Chi tiết hồ sơ": CaseDetails(); break;
                case "Gửi ý kiến": Opinion(); break;
                default: Dashboard(); break;
            }
        }

        private void Dashboard()
        {
            IList<InteragencyRequestItem> requests = LoadRequests();
            int pendingCount = requests.Count(item => item.Status != "DA_PHAN_HOI");
            int respondedCount = requests.Count(item => item.Status == "DA_PHAN_HOI");
            int dueSoonCount = requests.Count(item => item.Status != "DA_PHAN_HOI" && item.Deadline >= DateTimeOffset.Now && item.Deadline <= DateTimeOffset.Now.AddDays(3));
            int overdueCount = requests.Count(item => item.Status == "QUA_HAN" || (item.Status != "DA_PHAN_HOI" && item.Deadline < DateTimeOffset.Now));

            BuildPage("Tổng quan Đơn vị phối hợp liên ngành", null, stack =>
            {
                AddStats(stack,
                    new StatCardSpec("Chờ phản hồi", pendingCount.ToString("00"), "Đang mở", false),
                    new StatCardSpec("Sắp đến hạn", dueSoonCount.ToString("00"), "Ưu tiên xử lý", dueSoonCount > 0),
                    new StatCardSpec("Đã phản hồi", respondedCount.ToString("00"), "Đã hoàn tất", false),
                    new StatCardSpec("Quá hạn phản hồi", overdueCount.ToString("00"), "Cần xử lý ngay", overdueCount > 0));

                var grid = AddRequestTable(stack, requests, "Danh sách yêu cầu lấy ý kiến chuyên môn gần đây");
                grid.CellDoubleClick += (s, e) =>
                {
                    var selected = SelectedRequest(grid);
                    if (selected != null) Opinion(selected.RequestId);
                };
                var action = AddSection(stack, "Thao tác nhanh", null, 105);
                AddButtons(
                    action,
                    ActionButton("Mở danh sách yêu cầu", () => Requests(), true),
                    ActionButton("Gửi ý kiến đánh giá chuyên môn", () =>
                    {
                        var selected = SelectedRequest(grid);
                        if (selected != null) Opinion(selected.RequestId);
                        else Opinion();
                    }),
                    ActionButton("Xuất Văn bản ý kiến liên ngành (VB04)", () =>
                    {
                        var selected = SelectedRequest(grid);
                        PreviewOrExportVB04(selected, false);
                    }));
            });
        }

        private void Requests()
        {
            IList<InteragencyRequestItem> requests = LoadRequests();
            BuildPage("Yêu cầu cần phản hồi ý kiến chuyên môn", null, stack =>
            {
                DataGridView grid = AddRequestTable(stack, requests, "Danh sách yêu cầu phối hợp thẩm định");
                var actions = AddSection(stack, "Thao tác xử lý", null, 105);
                AddButtons(actions,
                    ActionButton("Gửi / Xem nội dung phản hồi", () =>
                    {
                        InteragencyRequestItem selected = SelectedRequest(grid);
                        if (selected == null)
                        {
                            ShowMessage("Vui lòng chọn một yêu cầu trong danh sách trước.", "Chưa chọn yêu cầu");
                            return;
                        }
                        Opinion(selected.RequestId);
                    }, true),
                    ActionButton("Xem chi tiết hồ sơ", () => CaseDetails()),
                    ActionButton("Làm mới danh sách", () => Requests()));
            });
        }

        private void CaseDetails()
        {
            BuildPage("Chi tiết hồ sơ xin ý kiến liên ngành", null, stack =>
            {
                IList<InteragencyRequestItem> requests = LoadRequests();
                DataGridView grid = AddRequestTable(stack, requests, "Chọn hồ sơ cần xem chi tiết");
                var details = AddSection(stack, "Thông tin hồ sơ và nội dung đánh giá chuyên môn", null, 250);
                AddRequestDetails(details, null);
                var actions = AddSection(stack, "Thao tác", null, 105);
                AddButtons(actions,
                    ActionButton("Chuyển đến màn hình gửi ý kiến", () =>
                    {
                        InteragencyRequestItem selected = SelectedRequest(grid);
                        if (selected == null)
                        {
                            ShowMessage("Vui lòng chọn một yêu cầu trước.", "Chưa chọn yêu cầu");
                            return;
                        }
                        Opinion(selected.RequestId);
                    }, true),
                    ActionButton("Xem trước Văn bản ý kiến liên ngành (VB04)", () =>
                    {
                        var selected = SelectedRequest(grid);
                        PreviewOrExportVB04(selected, false);
                    }));
                grid.SelectionChanged += (s, e) => AddRequestDetails(details, SelectedRequest(grid));
                AddRequestDetails(details, SelectedRequest(grid));
            });
        }

        private void Opinion(long? requestId = null)
        {
            IList<InteragencyRequestItem> requests = LoadRequests();
            BuildPage("Gửi ý kiến đánh giá chuyên môn liên ngành", null, stack =>
            {
                DataGridView grid = AddRequestTable(stack, requests, "Chọn yêu cầu cần gửi ý kiến chuyên môn");
                var form = AddSection(stack, "Nội dung đánh giá và kết luận chuyên môn", null, 370);
                var body = BodyOf(form);
                var conclusion = new ComboBox
                {
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Width = 520,
                    Height = 32,
                    Font = new Font("Arial", 9.5f)
                };
                conclusion.Items.Add(new ConclusionOption("PHU_HOP", "Phù hợp (Đạt yêu cầu chuyên môn)"));
                conclusion.Items.Add(new ConclusionOption("PHU_HOP_CO_DIEU_KIEN", "Phù hợp có điều kiện (Cần thực hiện kiến nghị kèm theo)"));
                conclusion.Items.Add(new ConclusionOption("KHONG_PHU_HOP", "Không phù hợp (Chưa đảm bảo yêu cầu an toàn/chuyên môn)"));
                conclusion.SelectedIndex = 0;

                var response = new TextBox
                {
                    Multiline = true,
                    ScrollBars = ScrollBars.Vertical,
                    Width = 700,
                    Height = 88,
                    MaxLength = 4000,
                    Font = new Font("Arial", 9.5f),
                    BorderStyle = BorderStyle.FixedSingle
                };
                var recommendation = new TextBox
                {
                    Multiline = true,
                    ScrollBars = ScrollBars.Vertical,
                    Width = 700,
                    Height = 72,
                    MaxLength = 4000,
                    Font = new Font("Arial", 9.5f),
                    BorderStyle = BorderStyle.FixedSingle
                };
                AddInput(form, body, "Kết luận đánh giá chuyên môn", conclusion, 56);
                AddInput(form, body, "Nội dung nhận xét, đánh giá chi tiết *", response, 114);
                AddInput(form, body, "Điều kiện / Kiến nghị yêu cầu đơn vị tổ chức thực hiện", recommendation, 98);

                var sendButton = ActionButton("Gửi văn bản phản hồi", () =>
                {
                    InteragencyRequestItem selected = SelectedRequest(grid);
                    if (selected == null)
                    {
                        ShowMessage("Vui lòng chọn yêu cầu cần phản hồi.", "Chưa chọn yêu cầu");
                        return;
                    }
                    if (selected.Status == "DA_PHAN_HOI")
                    {
                        ShowMessage("Yêu cầu này đã được gửi phản hồi hoàn tất.", "Yêu cầu đã hoàn tất");
                        return;
                    }

                    var option = conclusion.SelectedItem as ConclusionOption;
                    if (option == null)
                    {
                        ShowMessage("Vui lòng chọn kết luận đánh giá.", "Thiếu kết luận");
                        return;
                    }
                    try
                    {
                        workflow.SubmitOpinion(user, selected.RequestId, option.Value,
                            response.Text, recommendation.Text);
                        ShowMessage("Đã gửi ý kiến phản hồi chuyên môn thành công.", "Đã gửi phản hồi");
                        Opinion(selected.RequestId);
                    }
                    catch (Exception ex) { ShowWorkflowError(ex, "gửi phản hồi"); }
                }, true);

                AddButtons(
                    form,
                    sendButton,
                    ActionButton("Xem & Xuất Văn bản ý kiến liên ngành (VB04)", () =>
                    {
                        var selected = SelectedRequest(grid);
                        PreviewOrExportVB04(selected, false, response.Text);
                    }),
                    ActionButton("Làm mới danh sách", () => Opinion()));

                grid.SelectionChanged += (s, e) => UpdateReplyState(grid, sendButton, response,
                    recommendation, conclusion);
                if (requestId.HasValue)
                {
                    foreach (DataGridViewRow row in grid.Rows)
                    {
                        var item = row.Tag as InteragencyRequestItem;
                        if (item != null && item.RequestId == requestId.Value)
                        {
                            row.Selected = true;
                            grid.CurrentCell = row.Cells[0];
                            break;
                        }
                    }
                }
                UpdateReplyState(grid, sendButton, response, recommendation, conclusion);
            });
        }

        private void ResponseDocuments()
        {
            IList<InteragencyRequestItem> requests = LoadRequests();
            BuildPage("Văn bản ý kiến liên ngành (Biểu mẫu VB04)", null, stack =>
            {
                var grid = AddRequestTable(stack, requests, "Danh sách văn bản phản hồi chuyên môn");
                var actions = AddSection(stack, "Xem trước và Xuất văn bản hành chính (.docx)", null, 110);
                AddButtons(
                    actions,
                    ActionButton("Xem trước Văn bản ý kiến liên ngành (VB04)", () =>
                    {
                        var selected = SelectedRequest(grid);
                        PreviewOrExportVB04(selected, false);
                    }),
                    ActionButton("Xuất file Word .docx (VB04)", () =>
                    {
                        var selected = SelectedRequest(grid);
                        PreviewOrExportVB04(selected, true);
                    }, true),
                    ActionButton("Làm mới danh sách", () => ResponseDocuments()));
            });
        }

        private void PreviewOrExportVB04(InteragencyRequestItem selected, bool directExport, string overrideNotes = null)
        {
            var hs = selected == null
                ? null
                : new HoSoCapPhep
                {
                    ma_ho_so = selected.CaseId,
                    ten_su_kien = selected.EventName,
                    trang_thai = selected.Status
                };
            string notes = !string.IsNullOrWhiteSpace(overrideNotes)
                ? overrideNotes
                : (selected == null ? null : selected.Response);

            if (directExport)
                WordExportService.ExportToDocxWithDialog(this, "VB04", hs, notes);
            else
                WordExportService.ShowPreviewDialog(this, "VB04", hs, notes);
        }

        private IList<InteragencyRequestItem> LoadRequests()
        {
            try { return workflow.GetUnitRequests(user); }
            catch (Exception ex)
            {
                ShowWorkflowError(ex, "tải yêu cầu liên ngành");
                return new List<InteragencyRequestItem>();
            }
        }

        private DataGridView AddRequestTable(
            FlowLayoutPanel stack, IList<InteragencyRequestItem> requests, string title)
        {
            var section = TableSection(stack, title, null,
                "Mã yêu cầu", "Mã hồ sơ", "Tên sự kiện", "Đơn vị / Lĩnh vực phụ trách", "Hạn phản hồi", "Trạng thái", "Kết luận chuyên môn");
            var grid = BodyOf(section).Controls.OfType<DataGridView>().First();
            foreach (var item in requests)
            {
                int row = grid.Rows.Add(
                    "YC-" + item.RequestId.ToString("0000"),
                    "HS-" + item.CaseId.ToString("0000"),
                    item.EventName,
                    item.UnitName + " (" + item.Field + ")",
                    item.Deadline.ToString("dd/MM/yyyy HH:mm"),
                    FormatStatus(item.Status),
                    string.IsNullOrWhiteSpace(item.Conclusion) ? "Chưa kết luận" : FormatStatus(item.Conclusion));
                grid.Rows[row].Tag = item;
            }
            return grid;
        }

        private void AddRequestDetails(Panel section, InteragencyRequestItem item)
        {
            Panel body = BodyOf(section);
            if (body == null) return;
            body.Controls.Clear();
            string text = item == null
                ? "Vui lòng chọn một yêu cầu trong danh sách phía trên để xem thông tin chi tiết."
                : "Mã yêu cầu: YC-" + item.RequestId.ToString("0000") + "   |   Mã hồ sơ: HS-" + item.CaseId.ToString("0000") + Environment.NewLine +
                  "Tên sự kiện: " + (item.EventName ?? string.Empty) + Environment.NewLine +
                  "Đơn vị phụ trách: " + (item.UnitName ?? string.Empty) + " — Lĩnh vực: " + (item.Field ?? string.Empty) + Environment.NewLine +
                  "Thời hạn phản hồi: " + item.Deadline.ToString("dd/MM/yyyy HH:mm") + Environment.NewLine +
                  "Trạng thái xử lý: " + FormatStatus(item.Status) + Environment.NewLine +
                  "Kết luận chuyên môn: " + (string.IsNullOrWhiteSpace(item.Conclusion) ? "Chưa có kết luận" : FormatStatus(item.Conclusion)) + Environment.NewLine +
                  "Nội dung đánh giá: " + (string.IsNullOrWhiteSpace(item.Response) ? "Chưa cập nhật" : item.Response) + Environment.NewLine +
                  "Kiến nghị / Điều kiện kèm theo: " + (string.IsNullOrWhiteSpace(item.Recommendation) ? "Không có" : item.Recommendation);

            var detailsBox = new TextBox
            {
                Text = text,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                BorderStyle = BorderStyle.FixedSingle,
                Width = Math.Max(360, body.ClientSize.Width - 8),
                Height = 165,
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                BackColor = Color.White,
                ForeColor = Ink,
                Font = new Font("Arial", 9.5f),
                Margin = new Padding(0, 0, 0, 4)
            };
            body.Controls.Add(detailsBox);
            ResizeSectionBody(section);
        }

        private void AddInput(Panel section, Panel body, string label, Control input, int height)
        {
            var panel = new Panel
            {
                Width = Math.Max(420, body.ClientSize.Width - 8),
                Height = height,
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                Margin = new Padding(0, 2, 0, 6)
            };
            panel.Controls.Add(new Label
            {
                Text = label,
                Location = new Point(2, 0),
                AutoSize = true,
                ForeColor = Ink,
                Font = new Font("Arial", 9.5f, FontStyle.Bold)
            });
            input.Location = new Point(2, 22);
            input.Width = Math.Max(360, panel.ClientSize.Width - 6);
            input.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panel.Controls.Add(input);
            panel.Resize += (s, e) =>
            {
                input.Width = Math.Max(260, panel.ClientSize.Width - 6);
            };
            body.Controls.Add(panel);
            ResizeSectionBody(section);
        }

        private static InteragencyRequestItem SelectedRequest(DataGridView grid)
        {
            return grid.CurrentRow == null ? null : grid.CurrentRow.Tag as InteragencyRequestItem;
        }

        private static void UpdateReplyState(
            DataGridView grid, Button sendButton, TextBox response, TextBox recommendation, ComboBox conclusion)
        {
            InteragencyRequestItem selected = SelectedRequest(grid);
            bool canReply = selected != null && selected.Status != "DA_PHAN_HOI";
            sendButton.Enabled = canReply;
            response.ReadOnly = !canReply;
            recommendation.ReadOnly = !canReply;
            conclusion.Enabled = canReply;
            if (selected != null)
            {
                response.Text = selected.Response ?? string.Empty;
                recommendation.Text = selected.Recommendation ?? string.Empty;
                var match = conclusion.Items.Cast<ConclusionOption>()
                    .FirstOrDefault(option => option.Value == selected.Conclusion);
                if (match != null)
                    conclusion.SelectedItem = match;
            }
        }

        private void ShowWorkflowError(Exception exception, string operation)
        {
            LocalExceptionLogger.Write(exception);
            string msg = exception != null && !string.IsNullOrWhiteSpace(exception.Message)
                ? exception.Message
                : ("Không thể " + operation + " lúc này.");
            ShowMessage(msg, "Thông báo");
        }

        private void ShowMessage(string message, string title)
        {
            MessageBox.Show(this, message, title, MessageBoxButtons.OK,
                title == "Đã gửi phản hồi" ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
        }

        private sealed class ConclusionOption
        {
            public string Value { get; private set; }
            public string Text { get; private set; }
            public ConclusionOption(string value, string text) { Value = value; Text = text; }
            public override string ToString() { return Text; }
        }
    }
}
