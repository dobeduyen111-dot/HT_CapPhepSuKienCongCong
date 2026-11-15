using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Viewparty.Data;
using Viewparty.Models;

namespace Viewparty.Forms
{
    /// <summary>Shared responsive design-system base for all role workspace screens.</summary>
    internal abstract class RoleWorkspaceControl : UserControl
    {
        protected static readonly Color Navy = Color.FromArgb(20, 44, 65);            // #142C41
        protected static readonly Color HeadingInk = Color.FromArgb(29, 51, 72);      // #1D3348
        protected static readonly Color Ink = Color.FromArgb(38, 57, 75);             // #26394B
        protected static readonly Color Teal = Color.FromArgb(19, 122, 120);          // #137A78
        protected static readonly Color DarkTeal = Color.FromArgb(12, 97, 95);        // #0C615F
        protected static readonly Color Canvas = Color.FromArgb(245, 248, 252);       // #F5F8FC
        protected static readonly Color Muted = Color.FromArgb(107, 124, 141);        // #6B7C8D
        protected static readonly Color Border = Color.FromArgb(219, 228, 237);       // #DBE4ED
        protected static readonly Color BorderLine = Border;
        protected static readonly Color MintBg = Color.FromArgb(233, 247, 243);       // #E9F7F3
        protected static readonly Color SoftBlueBg = Color.FromArgb(236, 244, 250);   // #ECF4FA
        protected static readonly Color TableHeaderBg = Color.FromArgb(239, 244, 249);// #EFF4F9
        protected static readonly Color TableAltRowBg = Color.FromArgb(251, 252, 254);// #FBFCFE
        protected static readonly Color StatusGreen = Color.FromArgb(38, 124, 87);    // #267C57
        protected static readonly Color StatusAmber = Color.FromArgb(162, 103, 33);   // #A26721

        private static readonly Regex TechnicalTagRegex = new Regex(
            @"\s*\((?:UC\d+[^)]*|BR\d+[^)]*|fn_[^)]*|sp_[^)]*|LICH_SU_XU_LY|THONG_BAO|QUY_TAC|CAU_HINH_LIEN_NGANH|NHAT_KY_CAU_HINH|Trang-\d+[^)]*|VB\d+\s*-\s*VB\d+)\)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public event Action<string> ModuleChanged;

        private Panel stack;
        private bool isResizing;
        private const int HorizontalMargin = 28;
        private const int MinContentWidth = 580;

        protected RoleWorkspaceControl()
        {
            Dock = DockStyle.Fill;
            AutoScroll = true;
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Canvas;
            Font = new Font("Arial", 9.5F);
            DoubleBuffered = true;
            Resize += (s, e) => ResizeContent();
        }

        protected static string CleanText(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;
            string cleaned = TechnicalTagRegex.Replace(text, string.Empty);
            cleaned = Regex.Replace(cleaned, @"\bUC\d+(?:/\d+)?\s*·\s*", string.Empty, RegexOptions.IgnoreCase);
            return cleaned.Trim();
        }

        public static string FormatStatus(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return "—";
            switch (code.Trim().ToUpperInvariant())
            {
                case "NHAP": return "Bản nháp";
                case "DA_NOP": return "Đã nộp (Chờ tiếp nhận)";
                case "DANG_KIEM_TRA": return "Đang kiểm tra hợp lệ";
                case "YEU_CAU_BO_SUNG": return "Yêu cầu bổ sung hồ sơ";
                case "DANG_THAM_DINH": return "Đang thẩm định chuyên môn";
                case "CHO_Y_KIEN_LIEN_NGANH": return "Chờ ý kiến liên ngành";
                case "XU_LY_NGOAI_LE": return "Đang trình xét ngoại lệ";
                case "CHO_PHE_DUYET": return "Chờ lãnh đạo phê duyệt";
                case "DA_CAP_PHEP": return "Đã cấp giấy phép";
                case "TU_CHOI": return "Từ chối cấp phép";
                case "THU_HOI": return "Đã thu hồi giấy phép";
                case "CON_HIEU_LUC": return "Đang có hiệu lực";
                case "HET_HIEU_LUC": return "Đã hết hiệu lực";
                case "HOAT_DONG": return "Đang hoạt động";
                case "KHOA": return "Đang tạm khóa";
                case "NGUNG_HOAT_DONG": return "Ngừng hoạt động";
                case "CHO_PHAN_HOI": return "Chờ đơn vị phản hồi";
                case "DA_PHAN_HOI": return "Đã có văn bản phản hồi";
                case "QUA_HAN": return "Quá hạn phản hồi";
                case "CHO_DUYET": return "Chờ lãnh đạo xét duyệt";
                case "CHAP_NHAN": return "Đã chấp thuận ngoại lệ";
                case "DAT": return "Đạt yêu cầu";
                case "KHONG_DAT": return "Chưa đạt yêu cầu";
                case "CANH_BAO": return "Cần lưu ý kiểm tra";
                case "PHU_HOP": return "Phù hợp";
                case "PHU_HOP_CO_DIEU_KIEN": return "Phù hợp có điều kiện";
                case "KHONG_PHU_HOP": return "Không phù hợp";
                case "BAT_BUOC": return "Quy định bắt buộc";
                case "KHUYEN_NGHI": return "Khuyến nghị áp dụng";
                case "THUONG_MAI": return "Có yếu tố thương mại";
                case "PHI_THUONG_MAI": return "Phi thương mại (Cộng đồng)";
                default: return code;
            }
        }

        public static string FormatRole(string roleCode)
        {
            if (string.IsNullOrWhiteSpace(roleCode)) return "Người dùng";
            switch (roleCode.Trim().ToUpperInvariant())
            {
                case "NGUOI_TO_CHUC": return "Người tổ chức sự kiện";
                case "NGUOI_XU_LY": return "Cán bộ xử lý & thẩm định";
                case "LIEN_NGANH": return "Đơn vị phối hợp liên ngành";
                case "NGUOI_PHE_DUYET": return "Lãnh đạo phê duyệt cấp phép";
                case "QUAN_TRI_VIEN": return "Quản trị viên hệ thống";
                default: return roleCode;
            }
        }

        public static string WrapPlainTextAsJson(string textOrJson, string defaultKey = "noi_dung")
        {
            if (string.IsNullOrWhiteSpace(textOrJson))
                return "{\"" + defaultKey + "\":\"Đã ghi nhận kiểm tra thực tế\"}";

            string trimmed = textOrJson.Trim();
            if ((trimmed.StartsWith("{", StringComparison.Ordinal) && trimmed.EndsWith("}", StringComparison.Ordinal)) ||
                (trimmed.StartsWith("[", StringComparison.Ordinal) && trimmed.EndsWith("]", StringComparison.Ordinal)))
            {
                return trimmed;
            }

            string escaped = trimmed
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\r", " ")
                .Replace("\n", " ");
            return "{\"" + defaultKey + "\":\"" + escaped + "\"}";
        }

        public static string ExtractPlainTextFromJson(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return string.Empty;
            string trimmed = raw.Trim();
            if (!trimmed.StartsWith("{", StringComparison.Ordinal))
                return trimmed;

            var match = Regex.Match(trimmed, "\"(?:noi_dung|mo_ta|ghi_chu|chi_tiet)\"\\s*:\\s*\"([^\"]*)\"", RegexOptions.IgnoreCase);
            if (match.Success)
                return match.Groups[1].Value;

            string cleaned = trimmed.Trim('{', '}', ' ');
            cleaned = cleaned.Replace("\"", string.Empty).Replace(":", ": ").Replace(",", "; ");
            return cleaned;
        }

        protected void NotifyModuleChanged(string module)
        {
            if (ModuleChanged != null) ModuleChanged(module);
        }

        public static RoleWorkspaceControl CreateFor(AuthenticatedUser user)
        {
            switch (user.RoleCode)
            {
                case "NGUOI_TO_CHUC": return new OrganizerWorkspaceControl(user);
                case "NGUOI_XU_LY": return new CaseOfficerWorkspaceControl(user);
                case "LIEN_NGANH": return new InteragencyWorkspaceControl(user);
                case "NGUOI_PHE_DUYET": return new ApproverWorkspaceControl(user);
                case "QUAN_TRI_VIEN": return new AdminWorkspaceControl(user);
                default: throw new UnauthorizedAccessException("Vai trò không có giao diện trên hệ thống ViewParty.");
            }
        }

        public abstract void OpenModule(string module);

        public void ShowAccountProfilePage(AuthenticatedUser currentUser, Action<string> onFullNameUpdated = null)
        {
            if (currentUser == null) return;
            NotifyModuleChanged("Tài khoản cá nhân");

            var repo = new AccountRepository();
            TaiKhoan profile = null;
            try
            {
                profile = repo.GetProfile(currentUser.AccountId);
            }
            catch
            {
                profile = null;
            }

            string currentFullName = profile != null && !string.IsNullOrWhiteSpace(profile.ho_ten)
                ? profile.ho_ten
                : currentUser.FullName;
            string currentEmail = profile != null && !string.IsNullOrWhiteSpace(profile.email)
                ? profile.email
                : currentUser.Username + "@viewparty.vn";
            string currentPhone = profile != null && !string.IsNullOrWhiteSpace(profile.so_dien_thoai)
                ? profile.so_dien_thoai
                : "0909 123 456";
            string currentStatus = profile != null ? FormatStatus(profile.trang_thai) : "Đang hoạt động";
            string joinedDate = profile != null
                ? profile.ngay_tao.ToString("dd/MM/yyyy")
                : DateTime.Now.ToString("dd/MM/yyyy");
            string capThamQuyenText = currentUser.ApprovalLevel.HasValue
                ? "Thẩm quyền cấp " + currentUser.ApprovalLevel.Value
                : (currentUser.UnitId.HasValue ? "Đơn vị #" + currentUser.UnitId.Value : "Tài khoản chuẩn");

            BuildPage("Tài khoản cá nhân & Bảo mật", null, root =>
            {
                AddStats(root,
                    new StatCardSpec("Tài khoản đăng nhập", "@" + currentUser.Username, "Đã xác thực", false),
                    new StatCardSpec("Vai trò đảm nhiệm", FormatRole(currentUser.RoleCode), capThamQuyenText, false),
                    new StatCardSpec("Trạng thái hoạt động", currentStatus, "Trực tuyến", false),
                    new StatCardSpec("Ngày kích hoạt", joinedDate, "Hồ sơ hợp lệ", false));

                var split = new TableLayoutPanel
                {
                    Width = root.Width,
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    ColumnCount = 2,
                    RowCount = 1,
                    BackColor = Canvas,
                    Margin = new Padding(0, 0, 0, 14)
                };
                split.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 52F));
                split.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 48F));
                split.RowStyles.Add(new RowStyle(SizeType.AutoSize));

                var cardProfile = CreateSection("Thông tin hồ sơ cá nhân & Liên hệ", null, 560, 340);
                cardProfile.Margin = new Padding(0, 0, 8, 0);

                var txtFullName = new TextBox { Text = currentFullName };
                var txtUsername = new TextBox { Text = currentUser.Username, ReadOnly = true, BackColor = SoftBlueBg };
                var txtRole = new TextBox { Text = FormatRole(currentUser.RoleCode), ReadOnly = true, BackColor = SoftBlueBg };
                var txtEmail = new TextBox { Text = currentEmail };
                var txtPhone = new TextBox { Text = currentPhone };
                var txtStatus = new TextBox { Text = currentStatus + " (" + capThamQuyenText + ")", ReadOnly = true, BackColor = SoftBlueBg };

                AddFieldRows(cardProfile,
                    new FieldSpec("Họ và tên / Người đại diện", false, null, txtFullName),
                    new FieldSpec("Tên đăng nhập", false, null, txtUsername),
                    new FieldSpec("Địa chỉ Email liên hệ", false, null, txtEmail),
                    new FieldSpec("Số điện thoại liên hệ", false, null, txtPhone),
                    new FieldSpec("Vai trò trên hệ thống", false, null, txtRole),
                    new FieldSpec("Trạng thái & Phân quyền", false, null, txtStatus));

                AddButtons(cardProfile,
                    ActionButton("Lưu thông tin cá nhân", () =>
                    {
                        try
                        {
                            repo.UpdateProfile(currentUser.AccountId, txtFullName.Text, txtEmail.Text, txtPhone.Text);
                            currentUser.UpdateFullName(txtFullName.Text);
                            if (onFullNameUpdated != null)
                                onFullNameUpdated(currentUser.FullName);

                            MessageBox.Show(this,
                                "Đã cập nhật thông tin hồ sơ tài khoản thành công.",
                                "Cập nhật tài khoản",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(this, ex.Message, "Không thể lưu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }, true),
                    ActionButton("Làm mới thông tin", () => ShowAccountProfilePage(currentUser, onFullNameUpdated)));

                var cardSecurity = CreateSection("Đổi mật khẩu & Bảo mật đăng nhập", null, 520, 340);
                cardSecurity.Margin = new Padding(8, 0, 0, 0);

                var txtCurrentPwd = new TextBox { UseSystemPasswordChar = true };
                var txtNewPwd = new TextBox { UseSystemPasswordChar = true };
                var txtConfirmPwd = new TextBox { UseSystemPasswordChar = true };
                var txtSecurityNote = new TextBox
                {
                    Text = "Mật khẩu được mã hóa bảo mật một chiều. Khuyến nghị sử dụng tối thiểu 6 ký tự kết hợp chữ và số.",
                    ReadOnly = true,
                    BackColor = SoftBlueBg
                };

                AddFieldRows(cardSecurity,
                    new FieldSpec("Mật khẩu hiện tại", false, null, txtCurrentPwd),
                    new FieldSpec("Mật khẩu mới (tối thiểu 6 ký tự)", false, null, txtNewPwd),
                    new FieldSpec("Xác nhận lại mật khẩu mới", false, null, txtConfirmPwd),
                    new FieldSpec("Ghi chú an toàn bảo mật", false, null, txtSecurityNote));

                AddButtons(cardSecurity,
                    ActionButton("Cập nhật mật khẩu", () =>
                    {
                        if (!string.Equals(txtNewPwd.Text, txtConfirmPwd.Text, StringComparison.Ordinal))
                        {
                            MessageBox.Show(this, "Mật khẩu xác nhận không khớp với mật khẩu mới.", "Đổi mật khẩu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        try
                        {
                            repo.ChangePassword(currentUser.AccountId, txtCurrentPwd.Text, txtNewPwd.Text);
                            txtCurrentPwd.Clear();
                            txtNewPwd.Clear();
                            txtConfirmPwd.Clear();
                            MessageBox.Show(this, "Đã đổi mật khẩu đăng nhập thành công.", "Bảo mật tài khoản", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(this, ex.Message, "Không thể đổi mật khẩu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }, true),
                    ActionButton("Hiện / Ẩn mật khẩu", () =>
                    {
                        bool hide = !txtCurrentPwd.UseSystemPasswordChar;
                        txtCurrentPwd.UseSystemPasswordChar = hide;
                        txtNewPwd.UseSystemPasswordChar = hide;
                        txtConfirmPwd.UseSystemPasswordChar = hide;
                    }));

                split.Controls.Add(cardProfile, 0, 0);
                split.Controls.Add(cardSecurity, 1, 0);
                root.Controls.Add(split);

                var activitySection = AddSection(root, "Nhật ký hoạt động & Thông báo gần đây của tài khoản", null, 310);
                var grid = AddTable(activitySection, "Thời gian", "Phân loại", "Nội dung hoạt động", "Chi tiết ghi nhận");
                var activities = repo.GetRecentActivity(currentUser.AccountId);
                if (activities.Count == 0)
                {
                    grid.Rows.Add(DateTime.Now.ToString("dd/MM/yyyy HH:mm"), "Phiên đăng nhập", "Đăng nhập hệ thống ViewParty Portal", "Tài khoản @" + currentUser.Username + " đang hoạt động bình thường.");
                }
                else
                {
                    foreach (var item in activities)
                    {
                        grid.Rows.Add(
                            item.Timestamp.ToString("dd/MM/yyyy HH:mm"),
                            item.Category,
                            FormatStatus(item.Title),
                            item.Detail);
                    }
                }
            });
        }

        protected void HostEmbeddedView(Control view)
        {
            if (view == null) return;
            SuspendLayout();
            Controls.Clear();
            stack = null;
            AutoScrollMinSize = Size.Empty;
            var form = view as Form;
            if (form != null)
            {
                form.TopLevel = false;
                form.FormBorderStyle = FormBorderStyle.None;
                form.WindowState = FormWindowState.Normal;
            }
            view.Dock = DockStyle.Fill;
            Controls.Add(view);
            view.Show();
            ResumeLayout(true);
        }

        protected void BuildPage(string title, string subtitle, Action<FlowLayoutPanel> build, Button headerAction = null)
        {
            SuspendLayout();
            Controls.Clear();

            int initialWidth = Math.Max(MinContentWidth, ClientSize.Width - HorizontalMargin * 2);

            var flow = new FlowLayoutPanel
            {
                Location = new Point(HorizontalMargin, 18),
                Width = initialWidth,
                AutoSize = false,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                BackColor = Canvas,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            stack = flow;
            Controls.Add(flow);

            var titleRow = new Panel
            {
                Width = initialWidth,
                Height = 46,
                BackColor = Canvas,
                Margin = new Padding(0, 0, 0, 14)
            };

            var lblTitle = new Label
            {
                Text = CleanText(title),
                Location = new Point(0, 4),
                AutoSize = true,
                Font = new Font("Arial", 19.5F, FontStyle.Bold),
                ForeColor = HeadingInk
            };
            titleRow.Controls.Add(lblTitle);

            if (headerAction != null)
            {
                headerAction.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                headerAction.Location = new Point(titleRow.Width - headerAction.Width - 4, 2);
                titleRow.Controls.Add(headerAction);
                titleRow.Resize += (s, e) =>
                {
                    headerAction.Location = new Point(titleRow.Width - headerAction.Width - 4, 2);
                };
            }

            flow.Controls.Add(titleRow);
            build(flow);
            ResumeLayout(false);
            ResizeContent();
        }

        protected void ResizeContent()
        {
            if (isResizing || stack == null || stack.IsDisposed) return;
            isResizing = true;
            try
            {
                int availWidth = ClientSize.Width - HorizontalMargin * 2;
                if (VerticalScroll.Visible) availWidth -= SystemInformation.VerticalScrollBarWidth;
                int contentWidth = Math.Max(MinContentWidth, availWidth);

                stack.Location = new Point(HorizontalMargin, 18);
                stack.Width = contentWidth;

                Panel lastTableSection = null;

                foreach (Control child in stack.Controls)
                {
                    child.Width = contentWidth - child.Margin.Horizontal;

                    var splitTable = child as TableLayoutPanel;
                    if (splitTable != null)
                    {
                        foreach (Control cell in splitTable.Controls)
                        {
                            var subSection = cell as Panel;
                            if (subSection != null && subSection.Tag as string == "SectionCard")
                            {
                                subSection.Dock = DockStyle.Fill;
                                ResizeSectionBody(subSection);
                            }
                        }
                    }

                    var section = child as Panel;
                    if (section != null && section.Tag as string == "SectionCard")
                    {
                        ResizeSectionBody(section);
                        var body = BodyOf(section);
                        if (body != null && body.Controls.OfType<DataGridView>().Any())
                        {
                            lastTableSection = section;
                        }
                    }
                }

                // Calculate total height and expand last table section if window has extra vertical space
                int usedHeight = 0;
                foreach (Control child in stack.Controls)
                {
                    usedHeight += child.Height + child.Margin.Vertical;
                }

                int availableHeight = ClientSize.Height - 36;
                if (lastTableSection != null && usedHeight < availableHeight)
                {
                    int extra = availableHeight - usedHeight;
                    lastTableSection.Height += extra;
                    ResizeSectionBody(lastTableSection);
                    usedHeight += extra;
                }

                stack.Height = usedHeight + 16;
                AutoScrollMinSize = new Size(0, stack.Bottom + 18);
            }
            finally
            {
                isResizing = false;
            }
        }

        protected Panel AddSection(FlowLayoutPanel target, string title, string subtitle = null, int height = 280)
        {
            int width = Math.Max(MinContentWidth, (stack != null ? stack.Width : ClientSize.Width - HorizontalMargin * 2));
            var section = CreateSection(title, null, width, height);
            target.Controls.Add(section);
            return section;
        }

        protected Panel CreateSection(string title, string subtitle, int width, int height)
        {
            var section = new Panel
            {
                Tag = "SectionCard",
                Width = Math.Max(260, width),
                Height = height,
                BackColor = Color.White,
                Margin = new Padding(0, 0, 0, 14),
                Padding = new Padding(20, 14, 20, 14)
            };
            section.Paint += (s, e) => DrawBorder(e.Graphics, section.ClientRectangle, Border);

            bool hasTitle = !string.IsNullOrWhiteSpace(title);
            if (hasTitle)
            {
                section.Controls.Add(new Label
                {
                    Text = CleanText(title),
                    Location = new Point(20, 14),
                    AutoSize = true,
                    Font = new Font("Arial", 13F, FontStyle.Bold),
                    ForeColor = Ink
                });
            }

            int bodyTop = hasTitle ? 46 : 14;
            var body = new FlowLayoutPanel
            {
                Name = "SectionBody",
                Location = new Point(20, bodyTop),
                Size = new Size(Math.Max(220, section.Width - 40), Math.Max(44, height - bodyTop - 14)),
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = false,
                BackColor = Color.White,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            section.Controls.Add(body);
            section.Resize += (s, e) => ResizeSectionBody(section);
            return section;
        }

        protected void ResizeSectionBody(Panel section)
        {
            if (section == null || section.IsDisposed) return;
            var body = BodyOf(section);
            if (body == null || body.Name != "SectionBody") return;

            int bodyWidth = Math.Max(220, section.ClientSize.Width - 40);
            body.Width = bodyWidth;

            var grid = body.Controls.OfType<DataGridView>().FirstOrDefault();
            var buttonRow = body.Controls.OfType<FlowLayoutPanel>().FirstOrDefault(p => p.Tag as string == "ButtonRow");

            foreach (Control child in body.Controls)
            {
                // IMPORTANT: In a TopDown FlowLayoutPanel, AnchorStyles.Right causes WinForms to shrink
                // the control to the narrowest label in the column. Always strip AnchorStyles.Right on direct children!
                if ((child.Anchor & AnchorStyles.Right) == AnchorStyles.Right)
                {
                    child.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                }

                if (child is DataGridView) continue;

                int targetChildWidth = Math.Max(220, bodyWidth - child.Margin.Horizontal - 4);
                if (child is TableLayoutPanel)
                {
                    child.Width = targetChildWidth;
                    var tbl = (TableLayoutPanel)child;
                    foreach (Control cell in tbl.Controls)
                    {
                        var fieldPanel = cell as Panel;
                        if (fieldPanel != null)
                        {
                            foreach (Control fc in fieldPanel.Controls)
                            {
                                if (!(fc is Label))
                                {
                                    fc.Width = Math.Max(140, fieldPanel.ClientSize.Width - 18);
                                }
                            }
                        }
                    }
                }
                else if (child is Panel && child.Tag as string != "ButtonRow")
                {
                    child.Width = targetChildWidth;
                }
                else if (child is TextBox || child is ComboBox || child is DateTimePicker || child is NumericUpDown)
                {
                    child.Width = targetChildWidth;
                }
                else if (child is Label && !child.AutoSize)
                {
                    child.Width = targetChildWidth;
                }
            }

            if (grid != null)
            {
                int buttonRowHeight = buttonRow != null ? buttonRow.Height + buttonRow.Margin.Vertical + 6 : 0;
                int bodyHeight = Math.Max(140, section.ClientSize.Height - body.Top - 14);
                body.Height = bodyHeight;
                grid.Width = Math.Max(220, bodyWidth - grid.Margin.Horizontal);
                grid.Height = Math.Max(120, bodyHeight - buttonRowHeight - 8);
            }
            else
            {
                // Auto-fit form/action card height so controls & buttons are never clipped
                int contentBottom = 0;
                foreach (Control child in body.Controls)
                {
                    if (!child.Visible) continue;
                    contentBottom = Math.Max(contentBottom, child.Bottom + child.Margin.Bottom);
                }
                if (contentBottom > 0)
                {
                    body.Height = contentBottom + 6;
                    int neededSectionHeight = body.Top + body.Height + 14;
                    if (section.Dock != DockStyle.Fill && section.Height != neededSectionHeight)
                    {
                        section.Height = neededSectionHeight;
                    }
                }
            }
        }

        protected Panel BodyOf(Panel section)
        {
            return section == null || section.Controls.Count == 0
                ? null
                : section.Controls[section.Controls.Count - 1] as Panel;
        }

        protected sealed class StatCardSpec
        {
            public string Label { get; private set; }
            public string Value { get; private set; }
            public string Badge { get; private set; }
            public bool AmberBadge { get; private set; }

            public StatCardSpec(string label, int value, string badge = "Cập nhật", bool amberBadge = false)
                : this(label, value.ToString("00"), badge, amberBadge)
            {
            }

            public StatCardSpec(string label, string value, string badge = "Cập nhật", bool amberBadge = false)
            {
                Label = CleanText(label);
                Value = value ?? "00";
                Badge = CleanText(badge ?? "Cập nhật");
                AmberBadge = amberBadge;
            }

            public StatCardSpec(string label, string value, string badge, string subtitle)
                : this(label, value, badge, false)
            {
            }
        }

        protected void AddStats(FlowLayoutPanel target, string[] labels)
        {
            AddStats(target, labels.Select(l => new StatCardSpec(l, "00", "Trực tuyến")).ToArray());
        }

        protected void AddStats(FlowLayoutPanel target, params StatCardSpec[] specs)
        {
            int count = specs.Length;
            int width = Math.Max(MinContentWidth, stack != null ? stack.Width : target.Width);
            var row = new TableLayoutPanel
            {
                Width = width,
                Height = 114,
                ColumnCount = count,
                RowCount = 1,
                BackColor = Canvas,
                Margin = new Padding(0, 0, 0, 14),
                Padding = Padding.Empty
            };
            for (int i = 0; i < count; i++)
                row.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / count));
            row.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            for (int i = 0; i < count; i++)
            {
                var spec = specs[i];
                var card = new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = Color.White,
                    Margin = new Padding(i == 0 ? 0 : 7, 0, i == count - 1 ? 0 : 7, 0),
                    Padding = new Padding(18, 14, 18, 14)
                };
                card.Paint += (s, e) => DrawBorder(e.Graphics, card.ClientRectangle, Border);

                var lblName = new Label
                {
                    Text = spec.Label,
                    Location = new Point(18, 14),
                    AutoSize = true,
                    ForeColor = Muted,
                    Font = new Font("Arial", 9.5F, FontStyle.Bold)
                };
                bool isLongValue = (spec.Value ?? string.Empty).Length > 10;
                var lblVal = new Label
                {
                    Text = spec.Value,
                    Location = new Point(18, isLongValue ? 46 : 42),
                    AutoSize = true,
                    ForeColor = HeadingInk,
                    Font = new Font("Arial", isLongValue ? 13.5F : 22F, FontStyle.Bold)
                };

                var badgePanel = new Panel
                {
                    Size = new Size(104, 26),
                    Location = new Point(Math.Max(110, card.Width - 118), isLongValue ? 12 : 54),
                    Anchor = AnchorStyles.Top | AnchorStyles.Right,
                    BackColor = MintBg
                };
                ApplyRoundedRegion(badgePanel, 13);
                var lblBadge = new Label
                {
                    Text = string.IsNullOrWhiteSpace(spec.Badge) ? "Trực tuyến" : spec.Badge,
                    Dock = DockStyle.Fill,
                    Font = new Font("Arial", 8F, FontStyle.Bold),
                    ForeColor = spec.AmberBadge ? StatusAmber : StatusGreen,
                    TextAlign = ContentAlignment.MiddleCenter
                };
                badgePanel.Controls.Add(lblBadge);

                card.Controls.Add(lblName);
                card.Controls.Add(lblVal);
                card.Controls.Add(badgePanel);
                row.Controls.Add(card, i, 0);
            }
            target.Controls.Add(row);
        }

        protected DataGridView AddTable(Panel section, params string[] columns)
        {
            var body = BodyOf(section);
            int width = Math.Max(MinContentWidth - 40, body.ClientSize.Width);
            var grid = new DataGridView
            {
                Width = width,
                Height = Math.Max(150, body.ClientSize.Height - 10),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                GridColor = Border,
                EnableHeadersVisualStyles = false,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single,
                ColumnHeadersHeight = 40,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                Margin = new Padding(0, 0, 0, 4)
            };
            grid.RowTemplate.Height = 40;
            grid.ColumnHeadersDefaultCellStyle.BackColor = TableHeaderBg;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Muted;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 9F, FontStyle.Bold);
            grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = TableHeaderBg;
            grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = Muted;

            grid.DefaultCellStyle.Font = new Font("Arial", 9.5F);
            grid.DefaultCellStyle.ForeColor = Ink;
            grid.DefaultCellStyle.BackColor = Color.White;
            grid.DefaultCellStyle.SelectionBackColor = MintBg;
            grid.DefaultCellStyle.SelectionForeColor = HeadingInk;
            grid.DefaultCellStyle.Padding = new Padding(6, 2, 6, 2);

            grid.AlternatingRowsDefaultCellStyle.BackColor = TableAltRowBg;

            foreach (var column in columns)
                grid.Columns.Add(column.Replace(" ", ""), column);

            body.Controls.Add(grid);
            ResizeSectionBody(section);
            return grid;
        }

        protected void AddFields(Panel section, params string[] fieldLabels)
        {
            AddFieldRows(section, fieldLabels.Select(label => new FieldSpec(label)).ToArray());
        }

        protected void AddFieldRows(Panel section, params FieldSpec[] fields)
        {
            var body = BodyOf(section);
            int columns = fields.Any(field => field.Multiline) ? 1 : (fields.Length == 3 ? 3 : 2);
            int rowCount = (int)Math.Ceiling(fields.Length / (double)columns);
            var table = new TableLayoutPanel
            {
                Width = Math.Max(MinContentWidth - 40, body.ClientSize.Width),
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = columns,
                RowCount = rowCount,
                BackColor = Color.White,
                Margin = new Padding(0, 0, 0, 4)
            };
            for (int col = 0; col < columns; col++) table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / columns));
            for (int row = 0; row < rowCount; row++) table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            for (int i = 0; i < fields.Length; i++)
            {
                var spec = fields[i];
                var field = new Panel
                {
                    Dock = DockStyle.Fill,
                    Height = spec.Multiline ? 116 : 68,
                    Padding = new Padding(4, 2, 12, 4),
                    BackColor = Color.White
                };
                field.Controls.Add(new Label
                {
                    Text = CleanText(spec.Label),
                    Location = new Point(4, 2),
                    AutoSize = true,
                    Font = new Font("Arial", 9F, FontStyle.Bold),
                    ForeColor = Muted
                });
                Control input;
                if (spec.Control is DateTimePicker)
                {
                    var picker = (DateTimePicker)spec.Control;
                    picker.Location = new Point(4, 24);
                    picker.Width = Math.Max(160, field.Width - 18);
                    picker.Height = 32;
                    picker.Font = new Font("Arial", 9.5F);
                    picker.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                    input = picker;
                }
                else if (spec.Control is NumericUpDown)
                {
                    var num = (NumericUpDown)spec.Control;
                    num.Location = new Point(4, 24);
                    num.Width = Math.Max(160, field.Width - 18);
                    num.Height = 32;
                    num.Font = new Font("Arial", 9.5F);
                    num.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                    input = num;
                }
                else if (spec.Choices != null || spec.Control is ComboBox)
                {
                    var combo = spec.Control as ComboBox ?? new ComboBox();
                    if (combo.DropDownStyle == ComboBoxStyle.Simple)
                        combo.DropDownStyle = ComboBoxStyle.DropDownList;
                    combo.Location = new Point(4, 24);
                    combo.Width = Math.Max(160, field.Width - 18);
                    combo.Height = 32;
                    combo.Font = new Font("Arial", 9.5F);
                    combo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                    combo.DropDownWidth = Math.Max(450, combo.Width);
                    if (spec.Choices != null)
                    {
                        combo.Items.Clear();
                        foreach (string choice in spec.Choices) combo.Items.Add(choice);
                        if (combo.Items.Count > 0 && combo.SelectedIndex < 0) combo.SelectedIndex = 0;
                    }
                    input = combo;
                }
                else
                {
                    var textBox = spec.Control as TextBox ?? new TextBox();
                    textBox.Location = new Point(4, 24);
                    textBox.Width = Math.Max(160, field.Width - 18);
                    textBox.Height = spec.Multiline ? 80 : 30;
                    textBox.Multiline = spec.Multiline;
                    if (spec.Multiline) textBox.ScrollBars = ScrollBars.Vertical;
                    textBox.Font = new Font("Arial", 9.5F);
                    textBox.BorderStyle = BorderStyle.FixedSingle;
                    textBox.AccessibleDescription = spec.Label;
                    textBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                    input = textBox;
                }
                field.Resize += (s, e) =>
                {
                    input.Width = Math.Max(140, field.ClientSize.Width - 18);
                    if (input is ComboBox cb)
                        cb.DropDownWidth = Math.Max(450, cb.Width);
                };
                field.Controls.Add(input);
                table.Controls.Add(field, i % columns, i / columns);
            }
            body.Controls.Add(table);
            ResizeSectionBody(section);
        }

        protected sealed class FieldSpec
        {
            public string Label { get; private set; }
            public bool Multiline { get; private set; }
            public string[] Choices { get; private set; }
            public Control Control { get; private set; }
            public FieldSpec(string label, bool multiline = false, string[] choices = null, Control control = null)
            {
                Label = label;
                Multiline = multiline;
                Choices = choices;
                Control = control;
            }
        }

        protected void AddButtons(Panel section, params Button[] buttons)
        {
            var row = new FlowLayoutPanel
            {
                Tag = "ButtonRow",
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                BackColor = Color.White,
                Margin = new Padding(0, 6, 0, 2)
            };
            row.Controls.AddRange(buttons);
            BodyOf(section).Controls.Add(row);
            ResizeSectionBody(section);
        }

        protected Button ActionButton(string text, Action action, bool primary = false)
        {
            var button = new Button
            {
                Text = CleanText(text),
                AutoSize = true,
                Height = 38,
                MinimumSize = new Size(120, 38),
                Padding = new Padding(14, 0, 14, 0),
                Margin = new Padding(0, 0, 10, 4),
                FlatStyle = FlatStyle.Flat,
                BackColor = primary ? Teal : Color.White,
                ForeColor = primary ? Color.White : DarkTeal,
                Font = new Font("Arial", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            button.FlatAppearance.BorderColor = primary ? Teal : Border;
            button.Click += (s, e) => action();
            return button;
        }

        protected static void DrawBorder(Graphics g, Rectangle rect, Color borderColor)
        {
            if (rect.Width <= 1 || rect.Height <= 1) return;
            using (var pen = new Pen(borderColor, 1))
            {
                g.DrawRectangle(pen, 0, 0, rect.Width - 1, rect.Height - 1);
            }
        }

        protected static void ApplyRoundedRegion(Control control, int radius)
        {
            if (control == null || control.Width <= 0 || control.Height <= 0) return;
            var path = new GraphicsPath();
            int d = radius * 2;
            path.AddArc(0, 0, d, d, 180, 90);
            path.AddArc(control.Width - d, 0, d, d, 270, 90);
            path.AddArc(control.Width - d, control.Height - d, d, d, 0, 90);
            path.AddArc(0, control.Height - d, d, d, 90, 90);
            path.CloseFigure();
            control.Region = new Region(path);
        }

        protected void NotConnected(string feature)
        {
            MessageBox.Show(this, feature + " hiện chưa khả dụng.",
                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        protected void ChooseFiles(string title)
        {
            using (var dialog = new OpenFileDialog
            {
                Title = title,
                Filter = "Tệp hồ sơ|*.pdf;*.doc;*.docx;*.jpg;*.jpeg;*.png|Tất cả tệp|*.*",
                Multiselect = true
            })
            {
                if (dialog.ShowDialog(this) == DialogResult.OK)
                    MessageBox.Show(this, dialog.FileNames.Length + " tệp đã được chọn.",
                        "Tệp đã chọn", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        protected Panel TableSection(FlowLayoutPanel stack, string title, string subtitle, params string[] columns)
        {
            var section = AddSection(stack, title, null, 330);
            AddTable(section, columns);
            return section;
        }
    }
}
