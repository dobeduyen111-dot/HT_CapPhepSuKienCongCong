using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Viewparty.Models;
using Viewparty.Services;

namespace Viewparty.Forms
{
    internal sealed class AdminWorkspaceControl : RoleWorkspaceControl
    {
        private readonly AdminWorkflowService service = new AdminWorkflowService();
        private readonly AuthenticatedUser user;

        public AdminWorkspaceControl() { }

        internal AdminWorkspaceControl(AuthenticatedUser authenticatedUser)
        {
            user = authenticatedUser;
        }

        public override void OpenModule(string module)
        {
            NotifyModuleChanged(module);
            switch (module)
            {
                case "Tài khoản / vai trò":
                    Accounts();
                    break;
                case "Loại sự kiện":
                    EventTypesModule();
                    break;
                case "Địa điểm":
                    Venues();
                    break;
                case "Quy tắc / liên ngành":
                    Rules();
                    break;
                case "Nhật ký cấu hình":
                    Audit();
                    break;
                default:
                    Dashboard();
                    break;
            }
        }

        private static string FormatAuditTarget(string objectName)
        {
            if (string.IsNullOrWhiteSpace(objectName)) return "Hệ thống";
            switch (objectName.Trim().ToUpperInvariant())
            {
                case "TAI_KHOAN": return "Tài khoản người dùng";
                case "LOAI_SU_KIEN": return "Danh mục loại sự kiện";
                case "DIA_DIEM": return "Danh mục địa điểm";
                case "QUY_TAC": return "Tiêu chí thẩm định";
                case "CAU_HINH_LIEN_NGANH": return "Cấu hình phối hợp liên ngành";
                case "HO_SO_CAP_PHEP": return "Hồ sơ sự kiện";
                default: return objectName;
            }
        }

        private void Dashboard()
        {
            var accounts = LoadList(() => service.Accounts(user), "tải tài khoản");
            var venues = LoadList(() => service.Venues(user), "tải địa điểm");
            var rules = LoadList(() => service.Rules(user), "tải tiêu chí");
            var audit = LoadOne(() => service.Audit(user), "tải nhật ký") ?? new AdminAuditData();

            BuildPage(
                "Tổng quan Quản trị viên hệ thống",
                null,
                s =>
                {
                    AddStats(s,
                        new StatCardSpec("Tài khoản người dùng", accounts.Count.ToString("00"), "Đang quản lý"),
                        new StatCardSpec("Địa điểm tổ chức", venues.Count.ToString("00"), "Danh mục"),
                        new StatCardSpec("Tiêu chí thẩm định", rules.Count.ToString("00"), "Đang áp dụng"),
                        new StatCardSpec("Nhật ký thay đổi", audit.Configuration.Count.ToString("00"), "Đã ghi nhận"));

                    var actions = AddSection(s, "Truy cập nhanh các phân hệ quản trị", null, 100);
                    AddButtons(
                        actions,
                        ActionButton("Quản lý Tài khoản & Phân quyền", () => OpenModule("Tài khoản / vai trò"), true),
                        ActionButton("Quản lý Địa điểm tổ chức", () => OpenModule("Địa điểm")),
                        ActionButton("Danh mục Loại sự kiện", () => OpenModule("Loại sự kiện")),
                        ActionButton("Tiêu chí thẩm định & Liên ngành", () => OpenModule("Quy tắc / liên ngành")),
                        ActionButton("Nhật ký hoạt động hệ thống", () => OpenModule("Nhật ký cấu hình")));

                    var sec = TableSection(
                        s,
                        "Nhật ký cập nhật cấu hình gần đây",
                        null,
                        "Thời gian",
                        "Người thực hiện",
                        "Phân mục quản trị",
                        "Mã bản ghi",
                        "Nội dung thực hiện");
                    var g = Grid(sec);
                    foreach (var x in audit.Configuration.Take(12))
                    {
                        g.Rows.Add(
                            x.At.ToString("dd/MM/yyyy HH:mm"),
                            "Quản trị viên #" + x.AccountId,
                            FormatAuditTarget(x.ObjectName),
                            x.Key,
                            FormatStatus(x.Action));
                    }
                });
        }

        private void Accounts()
        {
            IList<AdminAccountItem> data = LoadList(() => service.Accounts(user), "tải tài khoản");
            BuildPage(
                "Quản lý Tài khoản và Phân quyền",
                null,
                s =>
                {
                    var f = AddSection(s, "Chọn tài khoản và cập nhật phân quyền", null, 230);
                    var cboAccount = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
                    var txtUserFullName = new TextBox { ReadOnly = true, BackColor = SoftBlueBg };
                    var txtUserAccountInfo = new TextBox { ReadOnly = true, BackColor = SoftBlueBg };
                    var cboRole = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
                    var cboStatus = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
                    var cboLevel = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };

                    var accountChoices = data.Select(a => a.FullName + " (@" + a.Username + " — TK-" + a.Id.ToString("00") + ")").ToArray();
                    var roleCodes = new[] { "NGUOI_TO_CHUC", "NGUOI_XU_LY", "LIEN_NGANH", "NGUOI_PHE_DUYET", "QUAN_TRI_VIEN" };
                    var roleLabels = roleCodes.Select(FormatRole).ToArray();
                    var statusLabels = new[] { "Đang hoạt động", "Đang tạm khóa" };

                    AddFieldRows(
                        f,
                        new FieldSpec("Chọn người dùng cần phân quyền", choices: accountChoices, control: cboAccount),
                        new FieldSpec("Họ và tên người dùng", control: txtUserFullName),
                        new FieldSpec("Tên đăng nhập & Địa chỉ Email", control: txtUserAccountInfo),
                        new FieldSpec("Vai trò đảm nhiệm", choices: roleLabels, control: cboRole),
                        new FieldSpec("Trạng thái hoạt động", choices: statusLabels, control: cboStatus),
                        new FieldSpec("Cấp thẩm quyền phê duyệt", choices: new[] { "Cấp 1", "Cấp 2", "Cấp 3" }, control: cboLevel));

                    var sec = TableSection(
                        s,
                        "Danh sách tài khoản trên hệ thống",
                        null,
                        "Mã TK",
                        "Tên đăng nhập",
                        "Họ và tên",
                        "Địa chỉ Email",
                        "Vai trò đảm nhiệm",
                        "Trạng thái",
                        "Thẩm quyền phê duyệt");
                    var g = Grid(sec);
                    foreach (var x in data)
                    {
                        int i = g.Rows.Add(
                            "TK-" + x.Id.ToString("00"),
                            "@" + x.Username,
                            x.FullName,
                            x.Email,
                            FormatRole(x.Role),
                            FormatStatus(x.Status),
                            x.ApprovalLevel.HasValue ? "Cấp " + x.ApprovalLevel.Value : "—");
                        g.Rows[i].Tag = x;
                    }

                    Action<AdminAccountItem> syncAccount = x =>
                    {
                        if (x == null) return;
                        txtUserFullName.Text = x.FullName ?? string.Empty;
                        txtUserAccountInfo.Text = "@" + x.Username + (string.IsNullOrWhiteSpace(x.Email) ? "" : " · " + x.Email);
                        int roleIdx = Array.IndexOf(roleCodes, x.Role);
                        if (roleIdx >= 0) cboRole.SelectedIndex = roleIdx;
                        cboStatus.SelectedIndex = string.Equals(x.Status, "HOAT_DONG", StringComparison.OrdinalIgnoreCase) ? 0 : 1;
                        if (x.ApprovalLevel.HasValue && x.ApprovalLevel.Value >= 1 && x.ApprovalLevel.Value <= 3)
                            cboLevel.SelectedIndex = x.ApprovalLevel.Value - 1;
                    };

                    cboAccount.SelectedIndexChanged += (a, b) =>
                    {
                        if (cboAccount.SelectedIndex >= 0 && cboAccount.SelectedIndex < data.Count)
                            syncAccount(data[cboAccount.SelectedIndex]);
                    };

                    g.SelectionChanged += (a, b) =>
                    {
                        var x = Selected<AdminAccountItem>(g);
                        if (x != null)
                        {
                            int idx = data.IndexOf(x);
                            if (idx >= 0 && idx < cboAccount.Items.Count)
                                cboAccount.SelectedIndex = idx;
                            syncAccount(x);
                        }
                    };
                    if (data.Count > 0)
                        syncAccount(data[0]);

                    AddButtons(
                        f,
                        ActionButton("Lưu vai trò & trạng thái", () =>
                        {
                            if (cboAccount.SelectedIndex < 0 || cboAccount.SelectedIndex >= data.Count)
                            {
                                Message("Vui lòng chọn một tài khoản hợp lệ.");
                                return;
                            }
                            long n = data[cboAccount.SelectedIndex].Id;
                            string selectedRoleCode = cboRole.SelectedIndex >= 0 && cboRole.SelectedIndex < roleCodes.Length
                                ? roleCodes[cboRole.SelectedIndex]
                                : "NGUOI_TO_CHUC";
                            string selectedStatusCode = cboStatus.SelectedIndex == 0 ? "HOAT_DONG" : "BI_KHOA";

                            if (n == user.AccountId && selectedRoleCode != "QUAN_TRI_VIEN")
                            {
                                var confirm = MessageBox.Show(this,
                                    "CẢNH BÁO: Bạn đang thay đổi vai trò của chính tài khoản đang đăng nhập (" + user.Username + ") từ Quản trị viên sang " + FormatRole(selectedRoleCode) + ".\r\n\r\nSau khi lưu, bạn sẽ không thể truy cập phân hệ Quản trị viên nữa. Bạn có chắc chắn muốn tiếp tục không?",
                                    "Cảnh báo tự hủy quyền Quản trị viên",
                                    MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Warning);
                                if (confirm != DialogResult.Yes) return;
                            }

                            if (n == user.AccountId && selectedStatusCode != "HOAT_DONG")
                            {
                                MessageBox.Show(this, "Không thể khóa tài khoản quản trị viên đang đăng nhập.", "Cảnh báo bảo vệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            try
                            {
                                service.SaveAccount(user, n, selectedRoleCode, selectedStatusCode);
                                Message("Đã cập nhật vai trò và trạng thái tài khoản thành công.", true);
                                Accounts();
                            }
                            catch (Exception e)
                            {
                                Error(e, "cập nhật tài khoản");
                            }
                        }, true),
                        ActionButton("Gán thẩm quyền phê duyệt", () =>
                        {
                            if (cboAccount.SelectedIndex < 0 || cboAccount.SelectedIndex >= data.Count)
                            {
                                Message("Vui lòng chọn tài khoản và cấp thẩm quyền phê duyệt.");
                                return;
                            }
                            short lv = (short)(Math.Max(0, cboLevel.SelectedIndex) + 1);
                            long n = data[cboAccount.SelectedIndex].Id;
                            try
                            {
                                service.SaveApprovalAuthority(user, n, lv);
                                Message("Đã gán thẩm quyền Lãnh đạo phê duyệt (Cấp " + lv + ") thành công.", true);
                                Accounts();
                            }
                            catch (Exception e)
                            {
                                Error(e, "gán thẩm quyền phê duyệt");
                            }
                        }),
                        ActionButton("Làm mới danh sách", () => Accounts()));
                });
        }

        private void EventTypesModule()
        {
            IList<AdminEventTypeItem> data = LoadList(() => service.EventTypes(user), "tải loại sự kiện");
            BuildPage(
                "Danh mục Loại hình sự kiện",
                null,
                s =>
                {
                    var f = AddSection(s, "Thêm mới / Cập nhật loại hình sự kiện", null, 175);
                    var cboName = new ComboBox { DropDownStyle = ComboBoxStyle.DropDown };
                    foreach (var item in data) cboName.Items.Add(item.Name);
                    var txtDesc = new TextBox();

                    AddFieldRows(
                        f,
                        new FieldSpec("Tên loại sự kiện (chọn có sẵn hoặc nhập mới)", control: cboName),
                        new FieldSpec("Mô tả đặc điểm sự kiện", control: txtDesc));

                    var sec = TableSection(
                        s,
                        "Danh mục loại sự kiện hiện có",
                        null,
                        "Tên loại sự kiện",
                        "Mô tả chi tiết",
                        "Trạng thái áp dụng");
                    var g = Grid(sec);
                    foreach (var x in data)
                    {
                        int i = g.Rows.Add(x.Name, x.Description, x.Active ? "Đang áp dụng" : "Ngừng áp dụng");
                        g.Rows[i].Tag = x;
                    }

                    cboName.SelectedIndexChanged += (a, b) =>
                    {
                        if (cboName.SelectedIndex >= 0 && cboName.SelectedIndex < data.Count)
                            txtDesc.Text = data[cboName.SelectedIndex].Description;
                    };

                    g.SelectionChanged += (a, b) =>
                    {
                        var x = Selected<AdminEventTypeItem>(g);
                        if (x != null)
                        {
                            cboName.Text = x.Name;
                            txtDesc.Text = x.Description;
                        }
                    };

                    AddButtons(
                        f,
                        ActionButton("Lưu loại sự kiện", () =>
                        {
                            try
                            {
                                service.SaveEventType(user, cboName.Text, txtDesc.Text);
                                Message("Đã lưu danh mục loại sự kiện thành công.", true);
                                EventTypesModule();
                            }
                            catch (Exception e)
                            {
                                Error(e, "cập nhật loại sự kiện");
                            }
                        }, true),
                        ActionButton("Nhập mới", () =>
                        {
                            cboName.Text = string.Empty;
                            txtDesc.Text = string.Empty;
                        }),
                        ActionButton("Làm mới", () => EventTypesModule()));
                });
        }

        private void Venues()
        {
            IList<AdminVenueItem> data = LoadList(() => service.Venues(user), "tải địa điểm");
            BuildPage(
                "Quản lý Địa điểm tổ chức sự kiện",
                null,
                s =>
                {
                    var f = AddSection(s, "Thêm mới / Cập nhật địa điểm tổ chức", null, 240);
                    var cboSelectVenue = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
                    var venueChoices = new List<string> { "-- Thêm mới địa điểm tổ chức --" };
                    venueChoices.AddRange(data.Select(v => "DD-" + v.Id.ToString("00") + " — " + v.Name + " (" + v.Capacity.ToString("N0") + " chỗ)"));

                    var txtName = new TextBox();
                    var txtAddress = new TextBox();
                    var txtCapacity = new TextBox { Text = "2500" };
                    var cboActive = new ComboBox();

                    AddFieldRows(
                        f,
                        new FieldSpec("Chọn địa điểm cần cập nhật (hoặc thêm mới)", choices: venueChoices.ToArray(), control: cboSelectVenue),
                        new FieldSpec("Tên địa điểm tổ chức", control: txtName),
                        new FieldSpec("Địa chỉ cụ thể / Khu vực", control: txtAddress),
                        new FieldSpec("Sức chứa tối đa (số người)", control: txtCapacity),
                        new FieldSpec("Trạng thái khai thác", choices: new[] { "Đang hoạt động", "Tạm ngừng khai thác" }, control: cboActive));

                    var sec = TableSection(
                        s,
                        "Danh mục địa điểm tổ chức sự kiện hiện có",
                        null,
                        "Mã ĐĐ",
                        "Tên địa điểm",
                        "Địa chỉ chi tiết",
                        "Sức chứa tối đa",
                        "Trạng thái");
                    var g = Grid(sec);
                    foreach (var x in data)
                    {
                        int i = g.Rows.Add("DD-" + x.Id.ToString("00"), x.Name, x.Address, x.Capacity.ToString("N0") + " người", x.Active ? "Đang hoạt động" : "Tạm ngừng");
                        g.Rows[i].Tag = x;
                    }

                    Action<AdminVenueItem> fillVenue = x =>
                    {
                        if (x == null)
                        {
                            txtName.Text = string.Empty;
                            txtAddress.Text = string.Empty;
                            txtCapacity.Text = "2500";
                            cboActive.SelectedIndex = 0;
                            return;
                        }
                        txtName.Text = x.Name;
                        txtAddress.Text = x.Address;
                        txtCapacity.Text = x.Capacity.ToString();
                        cboActive.SelectedIndex = x.Active ? 0 : 1;
                    };

                    cboSelectVenue.SelectedIndexChanged += (a, b) =>
                    {
                        int idx = cboSelectVenue.SelectedIndex - 1;
                        fillVenue(idx >= 0 && idx < data.Count ? data[idx] : null);
                    };

                    g.SelectionChanged += (a, b) =>
                    {
                        var x = Selected<AdminVenueItem>(g);
                        if (x != null)
                        {
                            int idx = data.IndexOf(x);
                            if (idx >= 0 && idx + 1 < cboSelectVenue.Items.Count)
                                cboSelectVenue.SelectedIndex = idx + 1;
                            fillVenue(x);
                        }
                    };

                    AddButtons(
                        f,
                        ActionButton("Lưu thông tin địa điểm", () =>
                        {
                            int idx = cboSelectVenue.SelectedIndex - 1;
                            long? venueId = idx >= 0 && idx < data.Count ? (long?)data[idx].Id : null;
                            int cap;
                            if (!int.TryParse(txtCapacity.Text, out cap) || cap < 100)
                            {
                                Message("Sức chứa tối đa phải là số nguyên từ 100 người trở lên.");
                                return;
                            }
                            bool isActive = cboActive.SelectedIndex != 1;
                            try
                            {
                                service.SaveVenue(user, venueId, txtName.Text, txtAddress.Text, cap, isActive);
                                Message("Đã lưu thông tin địa điểm tổ chức thành công.", true);
                                Venues();
                            }
                            catch (Exception e)
                            {
                                Error(e, "lưu địa điểm");
                            }
                        }, true),
                        ActionButton("Nhập mới", () =>
                        {
                            cboSelectVenue.SelectedIndex = 0;
                            fillVenue(null);
                        }),
                        ActionButton("Làm mới danh sách", () => Venues()));
                });
        }

        private void Rules()
        {
            IList<AdminRuleItem> data = LoadList(() => service.Rules(user), "tải tiêu chí");
            IList<AdminInteragencyConfigItem> configs = LoadList(() => service.Configs(user), "tải cấu hình");

            BuildPage(
                "Tiêu chí thẩm định & Cấu hình phối hợp liên ngành",
                null,
                s =>
                {
                    var sec = TableSection(
                        s,
                        "Danh sách phiên bản tiêu chí thẩm định",
                        null,
                        "Mã tiêu chí",
                        "Phiên bản",
                        "Loại sự kiện áp dụng",
                        "Nhóm tiêu chí",
                        "Tính chất",
                        "Cho phép xét ngoại lệ",
                        "Ngày bắt đầu hiệu lực");
                    var g = Grid(sec);
                    foreach (var x in data)
                    {
                        int r = g.Rows.Add(
                            x.Code,
                            "Phiên bản " + x.Version,
                            x.EventType,
                            x.Group,
                            FormatStatus(x.Severity),
                            x.ExceptionAllowed ? "Có cho phép" : "Không",
                            x.EffectiveFrom.ToString("dd/MM/yyyy"));
                        g.Rows[r].Tag = x;
                    }

                    var f = AddSection(s, "Ban hành phiên bản tiêu chí thẩm định mới", null, 340);
                    var cboCode = new ComboBox { DropDownStyle = ComboBoxStyle.DropDown };
                    cboCode.Items.AddRange(new object[] { "VP_DIA_DIEM", "VP_AN_TOAN", "VP_QUYEN_CHIEU", "VP_VE_QR" });
                    cboCode.SelectedIndex = 1;

                    var cboEventId = new ComboBox();
                    var cboGroup = new ComboBox { DropDownStyle = ComboBoxStyle.DropDown };
                    cboGroup.Items.AddRange(new object[] { "Địa điểm", "An toàn", "Quyền trình chiếu", "Vé và QR" });
                    cboGroup.SelectedIndex = 1;

                    var cboSeverity = new ComboBox();
                    var cboAllow = new ComboBox();
                    var cboLevel = new ComboBox();
                    var txtDate = new TextBox { Text = DateTime.Now.AddDays(30).ToString("yyyy-MM-dd") };
                    var txtApply = new TextBox { Text = "Áp dụng cho sự kiện có quy mô từ 1.000 người tham dự trở lên" };
                    var txtPass = new TextBox { Text = "Đảm bảo đầy đủ phương án an ninh, y tế, thoát hiểm và sức chứa phù hợp" };

                    AddFieldRows(
                        f,
                        new FieldSpec("Mã tiêu chí (chọn hoặc nhập mới)", control: cboCode),
                        new FieldSpec("Loại sự kiện áp dụng", choices: new[] { "Sự kiện Thể thao điện tử (Esports)", "Sự kiện Thể thao truyền thống" }, control: cboEventId),
                        new FieldSpec("Nhóm tiêu chí đánh giá", control: cboGroup),
                        new FieldSpec("Tính chất áp dụng", choices: new[] { "Quy định bắt buộc", "Khuyến nghị áp dụng" }, control: cboSeverity),
                        new FieldSpec("Cho phép trình xét ngoại lệ", choices: new[] { "Có cho phép", "Không cho phép" }, control: cboAllow),
                        new FieldSpec("Cấp thẩm quyền duyệt ngoại lệ", choices: new[] { "Cấp 2", "Cấp 1", "Cấp 3" }, control: cboLevel),
                        new FieldSpec("Ngày bắt đầu hiệu lực (yyyy-MM-dd)", control: txtDate),
                        new FieldSpec("Mô tả phạm vi áp dụng tiêu chí", control: txtApply),
                        new FieldSpec("Tiêu chuẩn đánh giá đạt yêu cầu", control: txtPass));

                    g.SelectionChanged += (a, b) =>
                    {
                        var x = Selected<AdminRuleItem>(g);
                        if (x != null)
                        {
                            cboCode.Text = x.Code;
                            cboGroup.Text = x.Group;
                            cboSeverity.SelectedIndex = string.Equals(x.Severity, "KHUYEN_NGHI", StringComparison.OrdinalIgnoreCase) ? 1 : 0;
                            cboAllow.SelectedIndex = x.ExceptionAllowed ? 0 : 1;
                        }
                    };

                    AddButtons(
                        f,
                        ActionButton("Ban hành phiên bản tiêu chí mới", () =>
                        {
                            long eid = cboEventId.SelectedIndex == 1 ? 2 : 1;
                            DateTime dt;
                            if (!DateTime.TryParse(txtDate.Text, out dt))
                            {
                                Message("Ngày bắt đầu hiệu lực không hợp lệ.");
                                return;
                            }
                            bool al = cboAllow.SelectedIndex == 0;
                            short? l = al ? (short?)(cboLevel.SelectedIndex == 1 ? 1 : (cboLevel.SelectedIndex == 2 ? 3 : 2)) : null;
                            string severityCode = cboSeverity.SelectedIndex == 1 ? "KHUYEN_NGHI" : "BAT_BUOC";
                            try
                            {
                                string applyJson = WrapPlainTextAsJson(txtApply.Text, "pham_vi_ap_dung");
                                string passJson = WrapPlainTextAsJson(txtPass.Text, "tieu_chuan_dat");
                                service.SaveRule(user, cboCode.Text, eid, cboGroup.Text, severityCode, applyJson, passJson, al, dt, l);
                                Message("Đã ban hành phiên bản tiêu chí thẩm định mới thành công.", true);
                                Rules();
                            }
                            catch (Exception e)
                            {
                                Error(e, "ban hành phiên bản tiêu chí");
                            }
                        }, true));

                    var c = AddSection(s, "Cấu hình đơn vị phối hợp lấy ý kiến liên ngành", null, 200);
                    var cboCfgEvent = new ComboBox();
                    var cboCfgUnit = new ComboBox();
                    var cboCfgReq = new ComboBox();
                    var txtDays = new TextBox { Text = "5" };
                    var txtFrom = new TextBox { Text = DateTime.Now.AddDays(30).ToString("yyyy-MM-dd") };

                    AddFieldRows(
                        c,
                        new FieldSpec("Loại sự kiện áp dụng", choices: new[] { "Sự kiện Thể thao điện tử (Esports)", "Sự kiện Thể thao truyền thống" }, control: cboCfgEvent),
                        new FieldSpec("Đơn vị chuyên môn liên ngành", choices: new[] { "Đơn vị An ninh trật tự", "Đơn vị Phòng cháy chữa cháy" }, control: cboCfgUnit),
                        new FieldSpec("Yêu cầu lấy ý kiến bắt buộc", choices: new[] { "Bắt buộc lấy ý kiến", "Tùy chọn tham khảo" }, control: cboCfgReq),
                        new FieldSpec("Thời hạn phản hồi tối đa (số ngày)", control: txtDays),
                        new FieldSpec("Ngày bắt đầu áp dụng (yyyy-MM-dd)", control: txtFrom));

                    AddButtons(
                        c,
                        ActionButton("Lưu cấu hình phối hợp liên ngành", () =>
                        {
                            long a = cboCfgEvent.SelectedIndex == 1 ? 2 : 1;
                            long b = cboCfgUnit.SelectedIndex == 1 ? 2 : 1;
                            int d;
                            DateTime dt;
                            if (!int.TryParse(txtDays.Text, out d) || !DateTime.TryParse(txtFrom.Text, out dt))
                            {
                                Message("Thông tin cấu hình liên ngành không hợp lệ.");
                                return;
                            }
                            bool r = cboCfgReq.SelectedIndex == 0;
                            try
                            {
                                service.SaveConfig(user, a, b, r, d, dt);
                                Message("Đã lưu cấu hình phối hợp liên ngành thành công.", true);
                                Rules();
                            }
                            catch (Exception e)
                            {
                                Error(e, "lưu cấu hình liên ngành");
                            }
                        }, true));

                    var cs = TableSection(
                        s,
                        "Danh sách cấu hình phối hợp liên ngành hiện hành",
                        null,
                        "Mã cấu hình",
                        "Loại sự kiện",
                        "Đơn vị chuyên môn",
                        "Tính chất",
                        "Thời hạn phản hồi",
                        "Ngày áp dụng");
                    var cg = Grid(cs);
                    foreach (var x in configs)
                    {
                        cg.Rows.Add("CH-" + x.Id.ToString("00"), x.EventType, x.Unit, x.Required ? "Bắt buộc" : "Tham khảo", x.Days + " ngày", x.EffectiveFrom.ToString("dd/MM/yyyy"));
                    }
                });
        }

        private void Audit()
        {
            AdminAuditData d = LoadOne(() => service.Audit(user), "tải nhật ký") ?? new AdminAuditData();
            BuildPage(
                "Nhật ký hoạt động và thay đổi cấu hình hệ thống",
                null,
                s =>
                {
                    // Section 1: Bộ lọc tìm kiếm và tra cứu nhật ký lịch sử
                    var filterSection = AddSection(s, "Bộ lọc tra cứu và kiểm tra lịch sử hệ thống", null, 240);

                    var txtKeyword = new TextBox();
                    var cboAction = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
                    cboAction.Items.AddRange(new object[]
                    {
                        "Tất cả thao tác / bước thực hiện",
                        "Nộp hồ sơ (Tạo mới, Lưu nháp, Nộp)",
                        "Tiếp nhận & Kiểm tra tính hợp lệ",
                        "Thẩm định hồ sơ chuyên môn",
                        "Lấy ý kiến cơ quan liên ngành",
                        "Tổng hợp & Đề xuất ngoại lệ",
                        "Phê duyệt quyết định & Cấp giấy phép",
                        "Cập nhật cấu hình quản trị hệ thống"
                    });
                    cboAction.SelectedIndex = 0;

                    var cboTime = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
                    cboTime.Items.AddRange(new object[]
                    {
                        "Tất cả mốc thời gian",
                        "Hôm nay",
                        "3 ngày gần nhất",
                        "7 ngày gần nhất",
                        "30 ngày gần nhất"
                    });
                    cboTime.SelectedIndex = 0;

                    var txtAccount = new TextBox();

                    AddFieldRows(filterSection,
                        new FieldSpec("Từ khóa tìm kiếm (Mã HS, phân mục, nội dung, ghi chú...)", false, null, txtKeyword),
                        new FieldSpec("Phân loại bước thực hiện / Thao tác nghiệp vụ", false, null, cboAction),
                        new FieldSpec("Khoảng thời gian ghi nhận", false, null, cboTime),
                        new FieldSpec("Mã định danh tài khoản thực hiện (ví dụ: 3, 6, 8...)", false, null, txtAccount));

                    var lblSummary = new Label
                    {
                        AutoSize = true,
                        Font = new Font("Arial", 9.5F, FontStyle.Bold),
                        ForeColor = Teal,
                        Margin = new Padding(12, 10, 8, 4)
                    };

                    // Section 2: Bảng nhật ký xử lý hồ sơ
                    var a = TableSection(
                        s,
                        "Nhật ký xử lý hồ sơ toàn hệ thống",
                        null,
                        "Thời gian",
                        "Tài khoản thực hiện",
                        "Phân mục",
                        "Mã hồ sơ",
                        "Bước thực hiện",
                        "Kết quả");
                    var g = Grid(a);

                    // Section 3: Bảng nhật ký cấu hình quản trị
                    var b = TableSection(
                        s,
                        "Nhật ký cập nhật cấu hình quản trị",
                        null,
                        "Thời gian",
                        "Quản trị viên",
                        "Phân mục cấu hình",
                        "Mã bản ghi",
                        "Nội dung thay đổi");
                    var h = Grid(b);

                    Action applyFilter = () =>
                    {
                        string kw = (txtKeyword.Text ?? string.Empty).Trim().ToLowerInvariant();
                        string accQ = (txtAccount.Text ?? string.Empty).Trim();
                        string actSel = cboAction.SelectedItem as string ?? "Tất cả thao tác / bước thực hiện";
                        string timeSel = cboTime.SelectedItem as string ?? "Tất cả mốc thời gian";

                        Func<DateTimeOffset, bool> timeMatch = at =>
                        {
                            DateTime localDt = at.LocalDateTime;
                            if (timeSel == "Hôm nay") return localDt.Date == DateTime.Today;
                            if (timeSel == "3 ngày gần nhất") return localDt >= DateTime.Today.AddDays(-3);
                            if (timeSel == "7 ngày gần nhất") return localDt >= DateTime.Today.AddDays(-7);
                            if (timeSel == "30 ngày gần nhất") return localDt >= DateTime.Today.AddDays(-30);
                            return true;
                        };

                        Func<long?, bool> accMatch = accId =>
                        {
                            if (string.IsNullOrEmpty(accQ)) return true;
                            if (!accId.HasValue) return false;
                            string sId = accId.Value.ToString();
                            return sId == accQ || sId.Contains(accQ);
                        };

                        Func<string, bool> actMatchHistory = act =>
                        {
                            if (actSel == "Tất cả thao tác / bước thực hiện") return true;
                            if (actSel.StartsWith("Cập nhật cấu hình")) return false;
                            string up = (act ?? string.Empty).ToUpperInvariant();
                            if (actSel.StartsWith("Nộp hồ sơ")) return up.Contains("NOP") || up.Contains("NHAP") || up.Contains("TAO");
                            if (actSel.StartsWith("Tiếp nhận")) return up.Contains("TIEP_NHAN") || up.Contains("KIEM_TRA") || up.Contains("HOP_LE");
                            if (actSel.StartsWith("Thẩm định")) return up.Contains("THAM_DINH");
                            if (actSel.StartsWith("Lấy ý kiến")) return up.Contains("LIEN_NGANH") || up.Contains("Y_KIEN");
                            if (actSel.StartsWith("Tổng hợp")) return up.Contains("TONG_HOP") || up.Contains("NGOAI_LE");
                            if (actSel.StartsWith("Phê duyệt")) return up.Contains("PHE_DUYET") || up.Contains("CAP_PHEP") || up.Contains("GIAY_PHEP");
                            return true;
                        };

                        Func<string, bool> actMatchConfig = act =>
                        {
                            if (actSel == "Tất cả thao tác / bước thực hiện" || actSel.StartsWith("Cập nhật cấu hình")) return true;
                            return false;
                        };

                        var filteredHist = d.History.Where(x =>
                        {
                            if (!timeMatch(x.At)) return false;
                            if (!accMatch(x.AccountId)) return false;
                            if (!actMatchHistory(x.Action)) return false;
                            if (!string.IsNullOrEmpty(kw))
                            {
                                string textSearch = string.Format("hs-{0} {0} tài khoản #{1} #{1} {2} {3} {4}",
                                    x.Key, x.AccountId, FormatAuditTarget(x.ObjectName), FormatStatus(x.Action), FormatStatus(x.Result ?? string.Empty)).ToLowerInvariant();
                                if (!textSearch.Contains(kw)) return false;
                            }
                            return true;
                        }).ToList();

                        var filteredCfg = d.Configuration.Where(x =>
                        {
                            if (!timeMatch(x.At)) return false;
                            if (!accMatch(x.AccountId)) return false;
                            if (!actMatchConfig(x.Action)) return false;
                            if (!string.IsNullOrEmpty(kw))
                            {
                                string textSearch = string.Format("quản trị viên #{0} #{0} {1} {2} {3}",
                                    x.AccountId, FormatAuditTarget(x.ObjectName), x.Key, FormatStatus(x.Action)).ToLowerInvariant();
                                if (!textSearch.Contains(kw)) return false;
                            }
                            return true;
                        }).ToList();

                        g.Rows.Clear();
                        if (filteredHist.Count == 0)
                        {
                            g.Rows.Add(DateTime.Now.ToString("dd/MM/yyyy HH:mm"), "—", "—", "—", "Không có bản ghi nhật ký hồ sơ phù hợp bộ lọc", "—");
                        }
                        else
                        {
                            foreach (var x in filteredHist)
                            {
                                g.Rows.Add(
                                    x.At.ToString("dd/MM/yyyy HH:mm"),
                                    "Tài khoản #" + x.AccountId,
                                    FormatAuditTarget(x.ObjectName),
                                    "HS-" + x.Key,
                                    FormatStatus(x.Action),
                                    FormatStatus(x.Result ?? "—"));
                            }
                        }

                        h.Rows.Clear();
                        if (filteredCfg.Count == 0)
                        {
                            h.Rows.Add(DateTime.Now.ToString("dd/MM/yyyy HH:mm"), "—", "—", "—", "Không có bản ghi nhật ký cấu hình phù hợp bộ lọc");
                        }
                        else
                        {
                            foreach (var x in filteredCfg)
                            {
                                h.Rows.Add(
                                    x.At.ToString("dd/MM/yyyy HH:mm"),
                                    "Quản trị viên #" + x.AccountId,
                                    FormatAuditTarget(x.ObjectName),
                                    x.Key,
                                    FormatStatus(x.Action));
                            }
                        }

                        lblSummary.Text = string.Format("Hiển thị {0}/{1} nhật ký hồ sơ · {2}/{3} nhật ký cấu hình",
                            filteredHist.Count, d.History.Count, filteredCfg.Count, d.Configuration.Count);
                    };

                    txtKeyword.KeyDown += (s1, e1) => { if (e1.KeyCode == Keys.Enter) { e1.SuppressKeyPress = true; applyFilter(); } };
                    txtAccount.KeyDown += (s1, e1) => { if (e1.KeyCode == Keys.Enter) { e1.SuppressKeyPress = true; applyFilter(); } };
                    cboAction.SelectedIndexChanged += (s1, e1) => applyFilter();
                    cboTime.SelectedIndexChanged += (s1, e1) => applyFilter();

                    AddButtons(filterSection,
                        ActionButton("Lọc dữ liệu", () => applyFilter(), true),
                        ActionButton("Đặt lại bộ lọc", () =>
                        {
                            txtKeyword.Text = string.Empty;
                            txtAccount.Text = string.Empty;
                            cboAction.SelectedIndex = 0;
                            cboTime.SelectedIndex = 0;
                            applyFilter();
                        }),
                        ActionButton("Làm mới từ hệ thống", () => Audit()));

                    BodyOf(filterSection).Controls.Add(lblSummary);

                    applyFilter();
                });
        }

        private DataGridView Grid(Panel section)
        {
            var g = BodyOf(section).Controls.OfType<DataGridView>().First();
            g.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            g.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            g.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;
            g.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            foreach (DataGridViewColumn c in g.Columns) c.MinimumWidth = 90;
            return g;
        }

        private static T Selected<T>(DataGridView g) where T : class
        {
            return g.CurrentRow == null ? null : g.CurrentRow.Tag as T;
        }

        private IList<T> LoadList<T>(Func<IList<T>> f, string op)
        {
            try { return f(); }
            catch (Exception e) { Error(e, op); return new List<T>(); }
        }

        private T LoadOne<T>(Func<T> f, string op)
        {
            try { return f(); }
            catch (Exception e) { Error(e, op); return default(T); }
        }

        private static void Error(Exception e, string op)
        {
            LocalExceptionLogger.Write(e);
            MessageBox.Show(
                e is AdminValidationException ? e.Message : "Không thể " + op + ". " + e.Message,
                "Thông báo quản trị",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }

        private void Message(string text, bool ok = false)
        {
            MessageBox.Show(
                this,
                text,
                ok ? "Đã lưu thành công" : "Thông báo",
                MessageBoxButtons.OK,
                ok ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
        }
    }
}
