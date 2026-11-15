using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Viewparty.Data;
using Viewparty.Forms.Organizer;
using Viewparty.Models;
using Viewparty.Services;

namespace Viewparty.Forms
{
    internal sealed class OrganizerWorkspaceControl : RoleWorkspaceControl
    {
        private readonly AuthenticatedUser currentUser;
        private readonly HoSoService hoSoService = new HoSoService();
        private readonly DiaDiemService diaDiemService = new DiaDiemService();

        internal OrganizerWorkspaceControl(AuthenticatedUser user)
        {
            currentUser = user ?? throw new ArgumentNullException("user");
        }

        public override void OpenModule(string module)
        {
            NotifyModuleChanged(module);
            switch (module)
            {
                case "Tổng quan":
                    Dashboard();
                    break;
                case "Hồ sơ của tôi":
                    MyApplications();
                    break;
                case "Tạo hồ sơ":
                    CreateApplication();
                    break;
                case "Tra cứu địa điểm":
                    VenueSearch();
                    break;
                case "Thông báo":
                    Notifications();
                    break;
                default:
                    Dashboard();
                    break;
            }
        }

        // ================================================================
        // SCREEN #4: Tổng quan Người tổ chức
        // ================================================================
        private void Dashboard()
        {
            IList<HoSoCapPhep> allCases;
            try
            {
                allCases = hoSoService.GetAllByOrganizer(currentUser);
            }
            catch (Exception ex)
            {
                LocalExceptionLogger.Write(ex);
                allCases = new List<HoSoCapPhep>();
            }

            int totalCount = allCases.Count;
            int inProgressCount = allCases.Count(x =>
                x.trang_thai == "DA_NOP" ||
                x.trang_thai == "DANG_KIEM_TRA" ||
                x.trang_thai == "DANG_THAM_DINH" ||
                x.trang_thai == "CHO_Y_KIEN_LIEN_NGANH" ||
                x.trang_thai == "TONG_HOP_LIEN_NGANH" ||
                x.trang_thai == "XU_LY_NGOAI_LE" ||
                x.trang_thai == "CHO_PHE_DUYET");
            int supplementCount = allCases.Count(x => x.trang_thai == "YEU_CAU_BO_SUNG");
            int approvedCount = allCases.Count(x => x.trang_thai == "DA_CAP_PHEP");

            BuildPage(
                "Tổng quan Người tổ chức",
                null,
                stack =>
                {
                    AddStats(stack, new[]
                    {
                        new StatCardSpec("Tổng hồ sơ", totalCount.ToString(CultureInfo.InvariantCulture), "Cá nhân"),
                        new StatCardSpec("Đang xử lý", inProgressCount.ToString(CultureInfo.InvariantCulture), "Theo dõi"),
                        new StatCardSpec("Cần bổ sung", supplementCount.ToString(CultureInfo.InvariantCulture), "Ưu tiên", supplementCount > 0),
                        new StatCardSpec("Đã cấp phép", approvedCount.ToString(CultureInfo.InvariantCulture), "Hoàn tất")
                    });

                    int width = Math.Max(620, stack.ClientSize.Width);
                    var row = new TableLayoutPanel
                    {
                        Width = width,
                        Height = 460,
                        ColumnCount = 2,
                        RowCount = 1,
                        Margin = new Padding(0, 0, 0, 14),
                        BackColor = Canvas
                    };
                    row.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 68F));
                    row.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 32F));
                    row.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

                    var recentCard = CreateSection("Hồ sơ gần đây", null, (int)(width * 0.68) - 8, 460);
                    recentCard.Dock = DockStyle.Fill;
                    recentCard.Margin = new Padding(0, 0, 8, 0);

                    var recentGrid = AddTable(
                        recentCard,
                        "Mã hồ sơ",
                        "Tên sự kiện",
                        "Địa điểm",
                        "Thời gian",
                        "Trạng thái",
                        "Thao tác");
                    foreach (var item in allCases.Take(8))
                    {
                        int r = recentGrid.Rows.Add(
                            FormatCaseCode(item.ma_ho_so),
                            item.ten_su_kien ?? "(Chưa đặt tên)",
                            item.ten_dia_diem ?? "Chưa chọn địa điểm",
                            FormatEventWindow(item.bat_dau, item.ket_thuc),
                            FormatStatusBadge(item.trang_thai),
                            GetActionLabel(item.trang_thai));
                        recentGrid.Rows[r].Tag = item;
                    }
                    if (recentGrid.Rows.Count == 0)
                    {
                        recentGrid.Rows.Add("—", "Chưa có hồ sơ. Nhấn 'Tạo hồ sơ mới' để khởi tạo.", "—", "—", "—", "—");
                    }
                    recentGrid.CellDoubleClick += (s, e) =>
                    {
                        if (e.RowIndex >= 0 && recentGrid.Rows[e.RowIndex].Tag is HoSoCapPhep selected)
                        {
                            RouteCaseAction(selected);
                        }
                    };

                    var quickCard = CreateSection("Thao tác nhanh", null, (int)(width * 0.32) - 8, 460);
                    quickCard.Dock = DockStyle.Fill;
                    quickCard.Margin = new Padding(8, 0, 0, 0);
                    var quickBody = BodyOf(quickCard);
                    AddQuickActionTile(
                        quickBody,
                        "TẠO HỒ SƠ MỚI",
                        "Khởi tạo hồ sơ sự kiện và đính kèm tài liệu.",
                        Color.FromArgb(236, 244, 250),
                        Color.FromArgb(12, 97, 95),
                        () => OpenModule("Tạo hồ sơ"));
                    AddQuickActionTile(
                        quickBody,
                        "CHỌN ĐỊA ĐIỂM",
                        "Tra cứu danh sách địa điểm, lọc theo sức chứa và gán vào hồ sơ.",
                        Color.FromArgb(236, 244, 250),
                        Color.FromArgb(12, 97, 95),
                        () => OpenModule("Tra cứu địa điểm"));
                    AddQuickActionTile(
                        quickBody,
                        "THEO DÕI & THÔNG BÁO",
                        "Xem thông báo kết quả thẩm định, yêu cầu bổ sung và quyết định cấp phép.",
                        Color.FromArgb(233, 247, 243),
                        Color.FromArgb(12, 97, 95),
                        () => OpenModule("Thông báo"));

                    row.Controls.Add(recentCard, 0, 0);
                    row.Controls.Add(quickCard, 1, 0);
                    stack.Controls.Add(row);
                });
        }

        private void AddQuickActionTile(
            Panel container,
            string title,
            string subtitle,
            Color backColor,
            Color titleColor,
            Action onClick)
        {
            var card = new Panel
            {
                Width = Math.Max(220, container.ClientSize.Width - 6),
                Height = 104,
                BackColor = backColor,
                Margin = new Padding(0, 0, 0, 12),
                Padding = new Padding(16, 14, 16, 12),
                Cursor = Cursors.Hand
            };
            card.Paint += (s, e) =>
            {
                using (var pen = new Pen(BorderLine))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, card.Width - 1, card.Height - 1);
                }
            };
            container.Resize += (s, e) =>
            {
                if (!card.IsDisposed)
                    card.Width = Math.Max(200, container.ClientSize.Width - 6);
            };
            var lblTitle = new Label
            {
                Text = title,
                Dock = DockStyle.Top,
                Height = 26,
                Font = new Font("Arial", 10.5F, FontStyle.Bold),
                ForeColor = titleColor,
                Cursor = Cursors.Hand
            };
            var lblSub = new Label
            {
                Text = subtitle,
                Dock = DockStyle.Fill,
                Font = new Font("Arial", 9.25F, FontStyle.Regular),
                ForeColor = Ink,
                Cursor = Cursors.Hand
            };
            EventHandler clickHandler = (s, e) =>
            {
                if (onClick != null) onClick();
            };
            card.Click += clickHandler;
            lblTitle.Click += clickHandler;
            lblSub.Click += clickHandler;
            card.Controls.Add(lblSub);
            card.Controls.Add(lblTitle);
            container.Controls.Add(card);
        }

        // ================================================================
        // SCREEN #5: Hồ sơ của tôi
        // ================================================================
        private void MyApplications()
        {
            IList<HoSoCapPhep> applications = new List<HoSoCapPhep>();
            IList<DiaDiem> allVenues;
            try { allVenues = diaDiemService.GetAllActive(); }
            catch { allVenues = new List<DiaDiem>(); }

            BuildPage(
                "Hồ sơ của tôi",
                null,
                stack =>
                {
                    var filters = AddSection(stack, "Bộ lọc hồ sơ", null, 180);

                    var txtKeyword = new TextBox();
                    var cboVenue = new ComboBox();
                    var cboStatus = new ComboBox();

                    var venueOptions = new List<string> { "Tất cả địa điểm", "Chưa chọn địa điểm" };
                    venueOptions.AddRange(allVenues.Select(v => v.ten_dia_diem));

                    var statusOptions = new[]
                    {
                        "Tất cả trạng thái",
                        FormatStatus("NHAP"),
                        FormatStatus("DA_NOP"),
                        FormatStatus("DANG_KIEM_TRA"),
                        FormatStatus("YEU_CAU_BO_SUNG"),
                        FormatStatus("DANG_THAM_DINH"),
                        FormatStatus("CHO_Y_KIEN_LIEN_NGANH"),
                        FormatStatus("XU_LY_NGOAI_LE"),
                        FormatStatus("CHO_PHE_DUYET"),
                        FormatStatus("DA_CAP_PHEP"),
                        FormatStatus("TU_CHOI")
                    };

                    AddFieldRows(
                        filters,
                        new FieldSpec("Từ khóa (mã hồ sơ / tên sự kiện)", control: txtKeyword),
                        new FieldSpec("Lọc theo địa điểm", choices: venueOptions.ToArray(), control: cboVenue),
                        new FieldSpec("Lọc theo trạng thái", choices: statusOptions, control: cboStatus));

                    var listSection = AddSection(stack, "Danh sách hồ sơ sự kiện", null, 390);
                    var grid = AddTable(
                        listSection,
                        "Mã hồ sơ",
                        "Tên sự kiện",
                        "Loại sự kiện",
                        "Địa điểm",
                        "Thời gian",
                        "Quy mô",
                        "Trạng thái",
                        "Thao tác");
                    grid.Columns[0].FillWeight = 11;
                    grid.Columns[1].FillWeight = 22;
                    grid.Columns[2].FillWeight = 14;
                    grid.Columns[3].FillWeight = 16;
                    grid.Columns[4].FillWeight = 14;
                    grid.Columns[5].FillWeight = 9;
                    grid.Columns[6].FillWeight = 12;
                    grid.Columns[7].FillWeight = 12;

                    Action applyFilter = () =>
                        ApplyApplicationFilter(grid, applications, txtKeyword.Text, cboVenue.SelectedItem as string, cboStatus.SelectedItem as string);

                    Action reload = () =>
                        RefreshApplications(grid, applications, txtKeyword.Text, cboVenue.SelectedItem as string, cboStatus.SelectedItem as string);

                    grid.CellDoubleClick += (s, e) =>
                    {
                        if (e.RowIndex >= 0 && grid.Rows[e.RowIndex].Tag is HoSoCapPhep selected)
                        {
                            RouteCaseAction(selected);
                        }
                    };

                    AddButtons(
                        filters,
                        ActionButton("Tìm kiếm", applyFilter, true),
                        ActionButton("Làm mới", reload),
                        ActionButton("Xem chi tiết & tiến độ", () =>
                        {
                            var selected = GetSelectedApplication(grid);
                            if (selected != null) ShowApplicationDetailView(selected.ma_ho_so);
                        }),
                        ActionButton("Mở / sửa Nháp", () => OpenSelectedDraft(grid)),
                        ActionButton("Chọn địa điểm", () =>
                        {
                            var selected = grid.CurrentRow != null ? grid.CurrentRow.Tag as HoSoCapPhep : null;
                            if (selected != null && selected.trang_thai == "NHAP")
                                VenueSearch(selected.ma_ho_so);
                            else
                                VenueSearch();
                        }),
                        ActionButton("Bổ sung hồ sơ", () =>
                        {
                            var selected = GetSelectedApplication(grid);
                            if (selected == null) return;
                            if (selected.trang_thai != "YEU_CAU_BO_SUNG")
                            {
                                MessageBox.Show(
                                    this,
                                    "Chức năng bổ sung áp dụng cho hồ sơ đang ở trạng thái 'Yêu cầu bổ sung hồ sơ'.",
                                    "Trạng thái không phù hợp",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                                return;
                            }
                            ShowSupplementView(selected.ma_ho_so);
                        }),
                        ActionButton("Xuất Đơn đề nghị (VB01)", () =>
                        {
                            var selected = GetSelectedApplication(grid);
                            if (selected == null) return;
                            WordExportService.ShowPreviewDialog(this, "VB01", selected);
                        }),
                        ActionButton("Tải kết quả / Giấy phép", () =>
                        {
                            var selected = GetSelectedApplication(grid);
                            if (selected == null) return;
                            if (selected.trang_thai == "DA_CAP_PHEP")
                            {
                                WordExportService.ShowPreviewDialog(this, "VB08", selected);
                            }
                            else if (selected.trang_thai == "TU_CHOI")
                            {
                                WordExportService.ShowPreviewDialog(this, "VB07", selected);
                            }
                            else if (selected.trang_thai == "YEU_CAU_BO_SUNG")
                            {
                                WordExportService.ShowPreviewDialog(this, "VB02", selected);
                            }
                            else
                            {
                                WordExportService.ShowPreviewDialog(this, "VB01", selected);
                            }
                        }),
                        ActionButton("+ Tạo hồ sơ mới", () => CreateApplication(), true));

                    txtKeyword.TextChanged += (s, e) => applyFilter();
                    cboVenue.SelectedIndexChanged += (s, e) => applyFilter();
                    cboStatus.SelectedIndexChanged += (s, e) => applyFilter();

                    reload();
                });
        }

        private void RefreshApplications(
            DataGridView grid,
            IList<HoSoCapPhep> applications,
            string keyword,
            string venueFilter,
            string status)
        {
            try
            {
                var latest = hoSoService.GetAllByOrganizer(currentUser);
                applications.Clear();
                foreach (var item in latest)
                    applications.Add(item);
                ApplyApplicationFilter(grid, applications, keyword, venueFilter, status);
            }
            catch (Exception ex)
            {
                HandleApplicationLoadError(ex);
            }
        }

        private void ApplyApplicationFilter(
            DataGridView grid,
            IList<HoSoCapPhep> applications,
            string keyword,
            string venueFilter,
            string status)
        {
            if (grid == null) return;
            string normalized = (keyword ?? string.Empty).Trim();
            bool allStatuses = string.IsNullOrWhiteSpace(status) || status == "Tất cả trạng thái";
            bool allVenues = string.IsNullOrWhiteSpace(venueFilter) || venueFilter == "Tất cả địa điểm";
            grid.Rows.Clear();

            foreach (var item in applications)
            {
                if (item == null) continue;
                string itemStatus = string.IsNullOrWhiteSpace(item.trang_thai) ? "NHAP" : item.trang_thai;
                if (!allStatuses &&
                    !string.Equals(itemStatus, status, StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(FormatStatus(itemStatus), status, StringComparison.OrdinalIgnoreCase))
                    continue;

                if (!allVenues)
                {
                    if (venueFilter == "Chưa chọn địa điểm")
                    {
                        if (!string.IsNullOrWhiteSpace(item.ten_dia_diem)) continue;
                    }
                    else if (!string.Equals(item.ten_dia_diem ?? string.Empty, venueFilter, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                }

                string codeText = FormatCaseCode(item.ma_ho_so);
                if (!string.IsNullOrWhiteSpace(normalized) &&
                    item.ma_ho_so.ToString(CultureInfo.InvariantCulture).IndexOf(normalized, StringComparison.OrdinalIgnoreCase) < 0 &&
                    codeText.IndexOf(normalized, StringComparison.OrdinalIgnoreCase) < 0 &&
                    (item.ten_su_kien ?? string.Empty).IndexOf(normalized, StringComparison.OrdinalIgnoreCase) < 0 &&
                    (item.ten_dia_diem ?? string.Empty).IndexOf(normalized, StringComparison.OrdinalIgnoreCase) < 0)
                {
                    continue;
                }

                int rowIndex = grid.Rows.Add(
                    codeText,
                    item.ten_su_kien ?? "(Chưa đặt tên)",
                    item.ten_loai ?? "—",
                    item.ten_dia_diem ?? "Chưa chọn",
                    FormatEventWindow(item.bat_dau, item.ket_thuc),
                    item.quy_mo.HasValue ? item.quy_mo.Value.ToString("N0", CultureInfo.GetCultureInfo("vi-VN")) : "—",
                    FormatStatusBadge(itemStatus),
                    GetActionLabel(itemStatus));
                grid.Rows[rowIndex].Tag = item;
            }

            if (grid.Rows.Count == 0)
            {
                grid.Rows.Add("—", "Chưa có hồ sơ phù hợp bộ lọc.", "—", "—", "—", "—", "—", "—");
            }
        }

        private HoSoCapPhep GetSelectedApplication(DataGridView grid)
        {
            if (grid == null || grid.CurrentRow == null || !(grid.CurrentRow.Tag is HoSoCapPhep))
            {
                MessageBox.Show(
                    this,
                    "Hãy chọn một hồ sơ trong danh sách trước khi thực hiện thao tác.",
                    "Chưa chọn hồ sơ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return null;
            }
            return (HoSoCapPhep)grid.CurrentRow.Tag;
        }

        private void RouteCaseAction(HoSoCapPhep selected)
        {
            if (selected == null) return;
            if (selected.trang_thai == "NHAP")
            {
                CreateApplication(selected.ma_ho_so);
                return;
            }
            if (selected.trang_thai == "YEU_CAU_BO_SUNG")
            {
                ShowSupplementView(selected.ma_ho_so);
                return;
            }
            ShowApplicationDetailView(selected.ma_ho_so);
        }

        private void OpenSelectedDraft(DataGridView grid)
        {
            var selected = GetSelectedApplication(grid);
            if (selected == null) return;
            if (!string.IsNullOrWhiteSpace(selected.trang_thai) && selected.trang_thai != "NHAP")
            {
                MessageBox.Show(
                    this,
                    "Hồ sơ này đang ở trạng thái '" + FormatStatus(selected.trang_thai) + "'. Chỉ hồ sơ ở trạng thái Bản nháp mới chỉnh sửa trực tiếp; hãy chọn 'Xem chi tiết & tiến độ' hoặc 'Bổ sung hồ sơ'.",
                    "Không thể sửa hồ sơ đã nộp",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }
            CreateApplication(selected.ma_ho_so);
        }

        // ================================================================
        // SCREEN #11: Chi tiết hồ sơ và tiến độ
        // ================================================================
        private void ShowApplicationDetailView(long caseId)
        {
            HoSoTheoDoiDetail detail;
            try
            {
                detail = hoSoService.GetTrackingDetail(currentUser, caseId);
            }
            catch (Exception ex)
            {
                HandleApplicationLoadError(ex);
                return;
            }

            var hoSo = detail.HoSo ?? new HoSoCapPhep { ma_ho_so = caseId };
            string code = FormatCaseCode(hoSo.ma_ho_so);

            BuildPage(
                "Chi tiết hồ sơ " + code,
                null,
                stack =>
                {
                    var topActions = AddSection(
                        stack,
                        "Trạng thái hiện tại: " + FormatStatusBadge(hoSo.trang_thai),
                        null,
                        110);

                    var buttons = new List<Button>
                    {
                        ActionButton("← Quay lại danh sách", MyApplications)
                    };
                    if (hoSo.trang_thai == "NHAP")
                    {
                        buttons.Add(ActionButton("Chỉnh sửa & nộp Nháp", () => CreateApplication(hoSo.ma_ho_so), true));
                    }
                    if (hoSo.trang_thai == "YEU_CAU_BO_SUNG")
                    {
                        buttons.Add(ActionButton("Bổ sung hồ sơ", () => ShowSupplementView(hoSo.ma_ho_so), true));
                        buttons.Add(ActionButton("Xem Thông báo yêu cầu bổ sung (VB02)", () =>
                            WordExportService.ShowPreviewDialog(this, "VB02", hoSo)));
                    }
                    if (hoSo.trang_thai == "DA_CAP_PHEP")
                    {
                        buttons.Add(ActionButton("Tải Giấy phép chính thức (VB08)", () =>
                            WordExportService.ShowPreviewDialog(this, "VB08", hoSo), true));
                        buttons.Add(ActionButton("Tải Văn bản chấp thuận (VB06)", () =>
                            WordExportService.ShowPreviewDialog(this, "VB06", hoSo)));
                    }
                    if (hoSo.trang_thai == "TU_CHOI")
                    {
                        buttons.Add(ActionButton("Xem Thông báo không chấp thuận (VB07)", () =>
                            WordExportService.ShowPreviewDialog(this, "VB07", hoSo), true));
                    }
                    buttons.Add(ActionButton("Xuất Đơn đề nghị cấp phép (VB01)", () =>
                        WordExportService.ShowPreviewDialog(this, "VB01", hoSo)));
                    AddButtons(topActions, buttons.ToArray());

                    int width = Math.Max(620, stack.ClientSize.Width);
                    var split = new TableLayoutPanel
                    {
                        Width = width,
                        Height = 490,
                        ColumnCount = 2,
                        RowCount = 1,
                        Margin = new Padding(0, 0, 0, 14),
                        BackColor = Canvas
                    };
                    split.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 57F));
                    split.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 43F));
                    split.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

                    var infoCard = CreateSection("Thông tin hồ sơ", null, (int)(width * 0.57) - 8, 490);
                    infoCard.Dock = DockStyle.Fill;
                    infoCard.Margin = new Padding(0, 0, 8, 0);
                    var infoGrid = AddTable(infoCard, "Hạng mục", "Nội dung khai báo");
                    infoGrid.Columns[0].FillWeight = 34;
                    infoGrid.Columns[1].FillWeight = 66;
                    infoGrid.Rows.Add("Mã hồ sơ", code);
                    infoGrid.Rows.Add("Tên sự kiện", hoSo.ten_su_kien ?? "—");
                    infoGrid.Rows.Add("Đơn vị tổ chức", hoSo.ten_don_vi_to_chuc ?? "—");
                    infoGrid.Rows.Add("Loại sự kiện", hoSo.ten_loai ?? "—");
                    infoGrid.Rows.Add("Địa điểm tổ chức",
                        (hoSo.ten_dia_diem ?? "Chưa gán") +
                        (hoSo.suc_chua_dia_diem.HasValue ? " (Sức chứa: " + hoSo.suc_chua_dia_diem.Value + ")" : string.Empty));
                    infoGrid.Rows.Add("Thời gian sự kiện", FormatEventWindow(hoSo.bat_dau, hoSo.ket_thuc));
                    infoGrid.Rows.Add("Quy mô dự kiến", hoSo.quy_mo.HasValue ? hoSo.quy_mo.Value + " người" : "—");
                    infoGrid.Rows.Add("Hình thức / Loại vé",
                        FormatStatus(hoSo.hinh_thuc_thuong_mai ?? "PHI_THUONG_MAI") + " / " + (hoSo.loai_ve ?? "Miễn phí") +
                        (hoSo.so_luong_ve.HasValue ? " (" + hoSo.so_luong_ve.Value + " vé)" : string.Empty));
                    infoGrid.Rows.Add("Nội dung chương trình", hoSo.noi_dung ?? "—");
                    infoGrid.Rows.Add("Phương án y tế / an ninh",
                        "Y tế: " + (hoSo.nhan_su_y_te ?? "—") + " | An ninh: " + (hoSo.nhan_su_an_ninh ?? "—"));
                    infoGrid.Rows.Add("PCCC & xử lý sự cố",
                        "PCCC: " + (hoSo.phuong_an_thoat_hiem ?? "—") + " | Sự cố: " + (hoSo.phuong_an_xu_ly_su_co ?? "—"));
                    if (detail.QuyenTrinhChieu != null)
                    {
                        var q = detail.QuyenTrinhChieu;
                        infoGrid.Rows.Add(
                            "Quyền trình chiếu",
                            (q.don_vi_cap ?? "—") + " | " + (q.loai_quyen ?? "—") +
                            " (" + q.hieu_luc_tu.ToString("dd/MM/yyyy") + " - " + q.hieu_luc_den.ToString("dd/MM/yyyy") + ")");
                    }
                    infoGrid.Rows.Add("Tài liệu đính kèm", detail.TaiLieu.Count + " tệp tin");

                    var historyCard = CreateSection("Lịch sử xử lý", null, (int)(width * 0.43) - 8, 490);
                    historyCard.Dock = DockStyle.Fill;
                    historyCard.Margin = new Padding(8, 0, 0, 0);
                    var historyGrid = AddTable(
                        historyCard,
                        "Thời điểm",
                        "Bước thực hiện",
                        "Chuyển trạng thái",
                        "Ghi chú");
                    foreach (var step in detail.LichSu)
                    {
                        historyGrid.Rows.Add(
                            step.thoi_diem.ToString("dd/MM/yyyy HH:mm"),
                            FormatStatus(step.hanh_dong ?? "—"),
                            (string.IsNullOrWhiteSpace(step.tu_trang_thai) ? "Khởi tạo" : FormatStatus(step.tu_trang_thai)) +
                            " → " + FormatStatus(step.den_trang_thai ?? "—"),
                            step.ghi_chu ?? string.Empty);
                    }

                    split.Controls.Add(infoCard, 0, 0);
                    split.Controls.Add(historyCard, 1, 0);
                    stack.Controls.Add(split);
                });
        }

        // ================================================================
        // SCREEN #12: Bổ sung hồ sơ
        // ================================================================
        private void ShowSupplementView(long caseId)
        {
            HoSoTheoDoiDetail detail;
            try
            {
                detail = hoSoService.GetTrackingDetail(currentUser, caseId);
            }
            catch (Exception ex)
            {
                HandleApplicationLoadError(ex);
                return;
            }

            var hoSo = detail.HoSo;
            if (hoSo == null)
            {
                MessageBox.Show(this, "Không tìm thấy hồ sơ cần bổ sung.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string code = FormatCaseCode(hoSo.ma_ho_so);
            var pendingUploads = new List<HoSoFileUpload>();

            BuildPage(
                "Bổ sung hồ sơ " + code,
                null,
                stack =>
                {
                    var noticeCard = new Panel
                    {
                        Width = Math.Max(620, stack.ClientSize.Width),
                        Height = 84,
                        BackColor = Color.FromArgb(255, 249, 235),
                        Margin = new Padding(0, 0, 0, 14),
                        Padding = new Padding(18, 12, 18, 12)
                    };
                    noticeCard.Paint += (s, e) =>
                    {
                        using (var pen = new Pen(Color.FromArgb(241, 194, 125)))
                        {
                            e.Graphics.DrawRectangle(pen, 0, 0, noticeCard.Width - 1, noticeCard.Height - 1);
                        }
                    };
                    var lblNoticeTitle = new Label
                    {
                        Text = "Nội dung yêu cầu bổ sung từ Cán bộ xử lý",
                        Dock = DockStyle.Top,
                        Height = 24,
                        Font = new Font("Arial", 10.5F, FontStyle.Bold),
                        ForeColor = Color.FromArgb(138, 86, 19)
                    };
                    var lblNoticeBody = new Label
                    {
                        Text = !string.IsNullOrWhiteSpace(hoSo.ghi_chu_bo_sung)
                            ? hoSo.ghi_chu_bo_sung
                            : "Vui lòng rà soát và bổ sung chi tiết nội dung chương trình, phương án y tế/an ninh/PCCC và tài liệu minh chứng.",
                        Dock = DockStyle.Fill,
                        Font = new Font("Arial", 9.5F, FontStyle.Regular),
                        ForeColor = Color.FromArgb(138, 86, 19)
                    };
                    noticeCard.Controls.Add(lblNoticeBody);
                    noticeCard.Controls.Add(lblNoticeTitle);
                    stack.Controls.Add(noticeCard);

                    var formSection = AddSection(stack, "Thông tin cập nhật và giải trình bổ sung", null, 460);

                    var txtGiaiTrinh = new TextBox();
                    var txtNoiDung = new TextBox { Text = hoSo.noi_dung ?? string.Empty };
                    var txtYTe = new TextBox { Text = hoSo.nhan_su_y_te ?? string.Empty };
                    var txtAnNinh = new TextBox { Text = hoSo.nhan_su_an_ninh ?? string.Empty };
                    var txtThoatHiem = new TextBox { Text = hoSo.phuong_an_thoat_hiem ?? string.Empty };
                    var txtSuCo = new TextBox { Text = hoSo.phuong_an_xu_ly_su_co ?? string.Empty };
                    var cboLoaiTaiLieu = new ComboBox();
                    var txtSelectedFiles = new TextBox { ReadOnly = true };

                    AddFieldRows(
                        formSection,
                        new FieldSpec("Nội dung giải trình bổ sung", true, control: txtGiaiTrinh),
                        new FieldSpec("Nội dung chương trình sau chỉnh sửa", true, control: txtNoiDung),
                        new FieldSpec("Phương án nhân sự y tế", control: txtYTe),
                        new FieldSpec("Phương án nhân sự an ninh", control: txtAnNinh),
                        new FieldSpec("Phương án thoát hiểm / PCCC", control: txtThoatHiem),
                        new FieldSpec("Phương án xử lý sự cố", control: txtSuCo),
                        new FieldSpec("Loại tài liệu bổ sung", choices: new[] { "Phương án đảm bảo an toàn", "Giấy tờ pháp lý / Bản quyền liên quan", "Tài liệu bổ sung khác" }, control: cboLoaiTaiLieu),
                        new FieldSpec("Tệp đính kèm mới", control: txtSelectedFiles));

                    AddButtons(
                        formSection,
                        ActionButton("← Quay lại", () => ShowApplicationDetailView(hoSo.ma_ho_so)),
                        ActionButton("Xem Thông báo bổ sung (VB02)", () => WordExportService.ShowPreviewDialog(this, "VB02", hoSo)),
                        ActionButton("Xuất Đơn đề nghị (VB01)", () => WordExportService.ShowPreviewDialog(this, "VB01", hoSo)),
                        ActionButton("+ Chọn tệp bổ sung...", () =>
                        {
                            using (var ofd = new OpenFileDialog())
                            {
                                ofd.Title = "Chọn tài liệu bổ sung hồ sơ";
                                ofd.Multiselect = true;
                                ofd.Filter = "Tài liệu (*.pdf;*.docx;*.doc;*.png;*.jpg)|*.pdf;*.docx;*.doc;*.png;*.jpg|Tất cả tệp (*.*)|*.*";
                                if (ofd.ShowDialog(this) == DialogResult.OK)
                                {
                                    string docType = cboLoaiTaiLieu.SelectedIndex == 0
                                        ? "PHUONG_AN_AN_TOAN"
                                        : (cboLoaiTaiLieu.SelectedIndex == 1 ? "GIAY_TO_LIEN_QUAN" : "TAI_LIEU_KHAC");
                                    foreach (string file in ofd.FileNames)
                                    {
                                        pendingUploads.Add(new HoSoFileUpload(docType, file));
                                    }
                                    txtSelectedFiles.Text = string.Join("; ", pendingUploads.Select(u => Path.GetFileName(u.SourcePath)));
                                }
                            }
                        }),
                        ActionButton("Gửi lại hồ sơ đã bổ sung", () =>
                        {
                            try
                            {
                                string nextStatus = hoSoService.SupplementApplication(
                                    currentUser,
                                    hoSo.ma_ho_so,
                                    txtNoiDung.Text,
                                    txtYTe.Text,
                                    txtAnNinh.Text,
                                    txtThoatHiem.Text,
                                    txtSuCo.Text,
                                    txtGiaiTrinh.Text,
                                    pendingUploads);

                                MessageBox.Show(
                                    this,
                                    "Đã cập nhật bổ sung và gửi lại hồ sơ " + code + " thành công.\nTrạng thái mới: " + nextStatus,
                                    "Bổ sung hồ sơ thành công",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                                ShowApplicationDetailView(hoSo.ma_ho_so);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(
                                    this,
                                    ex.Message,
                                    "Không thể gửi bổ sung hồ sơ",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                            }
                        }, true));
                });
        }

        private void HandleApplicationLoadError(Exception ex)
        {
            LocalExceptionLogger.Write(ex);
            MessageBox.Show(
                this,
                ex.Message,
                "Thông báo hồ sơ",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }

        // ================================================================
        // SCREEN #6-#10: Tạo / Sửa hồ sơ trực tiếp trên cùng cửa sổ
        // ================================================================
        private void CreateApplication(long? draftId = null)
        {
            NotifyModuleChanged("Tạo hồ sơ");
            var form = draftId.HasValue && draftId.Value > 0
                ? new HoSoCreateForm(currentUser, draftId.Value)
                : new HoSoCreateForm(currentUser);

            form.RequestClose += () =>
            {
                OpenModule("Hồ sơ của tôi");
            };
            form.RequestOpenVenueSearch += savedDraftId =>
            {
                VenueSearch(savedDraftId);
            };

            HostEmbeddedView(form);
        }

        // ================================================================
        // SCREEN #9: Tra cứu địa điểm và kiểm tra lịch trống
        // ================================================================
        private void VenueSearch(long? preselectedDraftId = null)
        {
            NotifyModuleChanged("Tra cứu địa điểm");

            IList<DiaDiem> allVenues;
            try { allVenues = diaDiemService.GetAllActive(); }
            catch { allVenues = new List<DiaDiem>(); }

            IList<HoSoCapPhep> drafts;
            try { drafts = diaDiemService.GetOrganizerDrafts(currentUser); }
            catch { drafts = new List<HoSoCapPhep>(); }

            BuildPage(
                "Tra cứu địa điểm và kiểm tra lịch trống",
                null,
                stack =>
                {
                    var filter = AddSection(stack, "Bộ lọc tra cứu và chọn địa điểm", null, 240);

                    var cboVenueList = new ComboBox
                    {
                        DropDownStyle = ComboBoxStyle.DropDown,
                        AutoCompleteMode = AutoCompleteMode.SuggestAppend,
                        AutoCompleteSource = AutoCompleteSource.ListItems
                    };
                    cboVenueList.Items.Add("-- Tất cả địa điểm --");
                    foreach (var v in allVenues)
                    {
                        cboVenueList.Items.Add(v.ten_dia_diem + " — " + v.dia_chi + " (" + v.suc_chua.ToString("N0") + " chỗ)");
                    }
                    cboVenueList.SelectedIndex = 0;

                    var cboArea = new ComboBox
                    {
                        DropDownStyle = ComboBoxStyle.DropDown,
                        AutoCompleteMode = AutoCompleteMode.SuggestAppend,
                        AutoCompleteSource = AutoCompleteSource.ListItems
                    };
                    cboArea.Items.Add("-- Tất cả khu vực --");
                    foreach (string area in new[] { "Quận 1", "Thủ Đức", "Bình Thạnh", "Quận 7", "TP.HCM" })
                    {
                        cboArea.Items.Add(area);
                    }
                    cboArea.SelectedIndex = 0;

                    var numGuests = new NumericUpDown
                    {
                        Minimum = 100,
                        Maximum = 10000,
                        Increment = 100,
                        ThousandsSeparator = true,
                        Value = 1500
                    };

                    var cboDraft = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
                    if (drafts.Count == 0)
                    {
                        cboDraft.Items.Add("-- Chưa có hồ sơ Nháp (Hãy tạo hồ sơ trước khi gán) --");
                        cboDraft.SelectedIndex = 0;
                    }
                    else
                    {
                        int selectedIdx = 0;
                        for (int i = 0; i < drafts.Count; i++)
                        {
                            var d = drafts[i];
                            cboDraft.Items.Add(
                                FormatCaseCode(d.ma_ho_so) + " — " + (d.ten_su_kien ?? "(Chưa đặt tên)") +
                                (d.quy_mo.HasValue ? " (" + d.quy_mo.Value.ToString("N0") + " người)" : ""));
                            if (preselectedDraftId.HasValue && d.ma_ho_so == preselectedDraftId.Value)
                                selectedIdx = i;
                        }
                        cboDraft.SelectedIndex = selectedIdx;
                    }

                    var dtpStart = new DateTimePicker
                    {
                        Format = DateTimePickerFormat.Custom,
                        CustomFormat = "dd/MM/yyyy HH:mm",
                        ShowUpDown = true,
                        Value = DateTime.Now.Date.AddDays(40).AddHours(18)
                    };
                    var dtpEnd = new DateTimePicker
                    {
                        Format = DateTimePickerFormat.Custom,
                        CustomFormat = "dd/MM/yyyy HH:mm",
                        ShowUpDown = true,
                        Value = DateTime.Now.Date.AddDays(40).AddHours(22)
                    };

                    Action syncFromSelectedDraft = () =>
                    {
                        int idx = cboDraft.SelectedIndex;
                        if (idx >= 0 && idx < drafts.Count)
                        {
                            var d = drafts[idx];
                            if (d.quy_mo.HasValue && d.quy_mo.Value >= (int)numGuests.Minimum && d.quy_mo.Value <= (int)numGuests.Maximum)
                                numGuests.Value = d.quy_mo.Value;
                            if (d.bat_dau.HasValue)
                                dtpStart.Value = d.bat_dau.Value.LocalDateTime;
                            if (d.ket_thuc.HasValue)
                                dtpEnd.Value = d.ket_thuc.Value.LocalDateTime;
                        }
                    };
                    syncFromSelectedDraft();

                    AddFieldRows(
                        filter,
                        new FieldSpec("Chọn địa điểm từ danh sách", control: cboVenueList),
                        new FieldSpec("Lọc theo khu vực / địa chỉ", control: cboArea),
                        new FieldSpec("Số người tham gia dự kiến", control: numGuests),
                        new FieldSpec("Hồ sơ Nháp cần gán địa điểm", control: cboDraft),
                        new FieldSpec("Thời gian bắt đầu", control: dtpStart),
                        new FieldSpec("Thời gian kết thúc", control: dtpEnd));

                    var resultSection = AddSection(stack, "Danh sách địa điểm", null, 360);
                    var grid = AddTable(
                        resultSection,
                        "Mã",
                        "Tên địa điểm",
                        "Khu vực / Địa chỉ",
                        "Sức chứa",
                        "Đáp ứng quy mô",
                        "Trạng thái lịch",
                        "Thao tác");
                    grid.Columns[0].FillWeight = 9;
                    grid.Columns[1].FillWeight = 24;
                    grid.Columns[2].FillWeight = 24;
                    grid.Columns[3].FillWeight = 12;
                    grid.Columns[4].FillWeight = 12;
                    grid.Columns[5].FillWeight = 13;
                    grid.Columns[6].FillWeight = 12;

                    Action runSearch = () =>
                    {
                        try
                        {
                            DateTimeOffset start = new DateTimeOffset(dtpStart.Value);
                            DateTimeOffset end = new DateTimeOffset(dtpEnd.Value);
                            int guests = Decimal.ToInt32(numGuests.Value);

                            string keyword = string.Empty;
                            if (cboVenueList.SelectedIndex > 0 && cboVenueList.SelectedIndex - 1 < allVenues.Count)
                            {
                                keyword = allVenues[cboVenueList.SelectedIndex - 1].ten_dia_diem;
                            }
                            else if (!string.IsNullOrWhiteSpace(cboVenueList.Text) && !cboVenueList.Text.StartsWith("--"))
                            {
                                keyword = cboVenueList.Text.Trim();
                            }
                            else if (cboArea.SelectedIndex > 0)
                            {
                                keyword = cboArea.Text.Trim();
                            }
                            else if (!string.IsNullOrWhiteSpace(cboArea.Text) && !cboArea.Text.StartsWith("--"))
                            {
                                keyword = cboArea.Text.Trim();
                            }

                            var results = diaDiemService.Search(keyword, start, end, guests);
                            grid.Rows.Clear();
                            foreach (var v in results)
                            {
                                bool available = v.trang_thai && v.suc_chua >= guests && !v.co_canh_bao_trung_lich;
                                int r = grid.Rows.Add(
                                    "DD-" + v.ma_dia_diem.ToString("00"),
                                    v.ten_dia_diem,
                                    v.dia_chi,
                                    v.suc_chua.ToString("N0", CultureInfo.GetCultureInfo("vi-VN")) + " người",
                                    v.suc_chua >= guests ? "Đạt (" + v.suc_chua.ToString("N0") + ")" : "Không đủ",
                                    v.co_canh_bao_trung_lich ? "Cảnh báo trùng lịch" : "Trống lịch",
                                    available ? "Sẵn sàng gán" : "Có cảnh báo");
                                grid.Rows[r].Tag = v;
                            }
                            if (grid.Rows.Count == 0)
                            {
                                grid.Rows.Add("—", "Không có địa điểm khớp điều kiện lọc và sức chứa.", "—", "—", "—", "—", "—");
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(this, ex.Message, "Tra cứu địa điểm", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    };

                    Action assignSelectedVenue = () =>
                    {
                        if (drafts.Count == 0)
                        {
                            var goCreate = MessageBox.Show(
                                this,
                                "Tài khoản hiện chưa có hồ sơ ở trạng thái Nháp. Bạn có muốn chuyển sang màn hình Tạo hồ sơ ngay bây giờ?",
                                "Chưa có hồ sơ Nháp",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Information);
                            if (goCreate == DialogResult.Yes)
                                CreateApplication();
                            return;
                        }

                        DiaDiem venue = null;
                        if (grid.CurrentRow != null && grid.CurrentRow.Tag is DiaDiem rowVenue)
                        {
                            venue = rowVenue;
                        }
                        else if (cboVenueList.SelectedIndex > 0 && cboVenueList.SelectedIndex - 1 < allVenues.Count)
                        {
                            venue = allVenues[cboVenueList.SelectedIndex - 1];
                        }

                        if (venue == null)
                        {
                            MessageBox.Show(this, "Hãy chọn một địa điểm trong danh sách hoặc bảng kết quả.", "Chưa chọn địa điểm", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        int draftIdx = Math.Max(0, cboDraft.SelectedIndex);
                        var targetDraft = drafts[draftIdx];

                        try
                        {
                            diaDiemService.SelectForDraft(currentUser, targetDraft.ma_ho_so, venue.ma_dia_diem);
                            MessageBox.Show(
                                this,
                                "Đã gán địa điểm '" + venue.ten_dia_diem + "' cho hồ sơ " + FormatCaseCode(targetDraft.ma_ho_so) + " thành công!",
                                "Gán địa điểm thành công",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);

                            if (preselectedDraftId.HasValue)
                            {
                                CreateApplication(targetDraft.ma_ho_so);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(this, ex.Message, "Không thể gán địa điểm", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    };

                    cboVenueList.SelectedIndexChanged += (s, e) => runSearch();
                    cboArea.SelectedIndexChanged += (s, e) => runSearch();
                    cboDraft.SelectedIndexChanged += (s, e) =>
                    {
                        syncFromSelectedDraft();
                        runSearch();
                    };

                    grid.CellDoubleClick += (s, e) =>
                    {
                        if (e.RowIndex >= 0) assignSelectedVenue();
                    };

                    var buttons = new List<Button>
                    {
                        ActionButton("Kiểm tra khả dụng", runSearch, true),
                        ActionButton("Gán địa điểm vào hồ sơ Nháp", assignSelectedVenue),
                        ActionButton("Đặt lại bộ lọc", () =>
                        {
                            cboVenueList.SelectedIndex = 0;
                            cboArea.SelectedIndex = 0;
                            runSearch();
                        })
                    };
                    if (preselectedDraftId.HasValue)
                    {
                        buttons.Insert(0, ActionButton("← Quay lại hồ sơ Nháp", () => CreateApplication(preselectedDraftId.Value)));
                    }
                    AddButtons(filter, buttons.ToArray());

                    runSearch();
                });
        }

        // ================================================================
        // SCREEN #13: Thông báo của tôi
        // ================================================================
        private void Notifications()
        {
            BuildPage(
                "Thông báo của tôi",
                null,
                stack =>
                {
                    var section = AddSection(stack, "Danh sách thông báo từ hệ thống", null, 460);
                    var grid = AddTable(
                        section,
                        "Mã TB",
                        "Thời điểm",
                        "Loại thông báo",
                        "Mã hồ sơ",
                        "Nội dung thông báo",
                        "Trạng thái");
                    grid.Columns[0].FillWeight = 10;
                    grid.Columns[1].FillWeight = 16;
                    grid.Columns[2].FillWeight = 18;
                    grid.Columns[3].FillWeight = 12;
                    grid.Columns[4].FillWeight = 32;
                    grid.Columns[5].FillWeight = 12;

                    Action loadNotifications = () =>
                    {
                        try
                        {
                            var items = hoSoService.GetNotifications(currentUser);
                            grid.Rows.Clear();
                            foreach (var tb in items)
                            {
                                int r = grid.Rows.Add(
                                    "TB-" + tb.ma_thong_bao.ToString("000"),
                                    tb.thoi_diem_tao.ToString("dd/MM/yyyy HH:mm"),
                                    FormatStatus(tb.loai_thong_bao ?? "Thông báo hệ thống"),
                                    tb.ma_ho_so.HasValue ? FormatCaseCode(tb.ma_ho_so.Value) : "—",
                                    tb.noi_dung ?? string.Empty,
                                    tb.trang_thai == "DA_DOC" ? "Đã xem" : "Mới nhận");
                                grid.Rows[r].Tag = tb;
                            }
                            if (grid.Rows.Count == 0)
                            {
                                grid.Rows.Add("—", "—", "—", "—", "Hiện chưa có thông báo mới cho tài khoản của bạn.", "—");
                            }
                        }
                        catch (Exception ex)
                        {
                            LocalExceptionLogger.Write(ex);
                            MessageBox.Show(this, ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    };

                    Action openRelatedCase = () =>
                    {
                        if (grid.CurrentRow == null || !(grid.CurrentRow.Tag is ThongBao tb) || !tb.ma_ho_so.HasValue)
                        {
                            MessageBox.Show(this, "Hãy chọn một thông báo có gắn mã hồ sơ.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        if (tb.loai_thong_bao == "YEU_CAU_BO_SUNG")
                            ShowSupplementView(tb.ma_ho_so.Value);
                        else
                            ShowApplicationDetailView(tb.ma_ho_so.Value);
                    };

                    grid.CellDoubleClick += (s, e) =>
                    {
                        if (e.RowIndex >= 0) openRelatedCase();
                    };

                    AddButtons(
                        section,
                        ActionButton("Làm mới thông báo", loadNotifications),
                        ActionButton("Mở hồ sơ liên quan", openRelatedCase, true));

                    loadNotifications();
                });
        }

        // ================================================================
        // SCREEN #14: Biểu mẫu Word dùng chung
        // ================================================================
        private void WordForms()
        {
            IList<HoSoCapPhep> myCases;
            try
            {
                myCases = hoSoService.GetAllByOrganizer(currentUser);
            }
            catch
            {
                myCases = new List<HoSoCapPhep>();
            }

            BuildPage(
                "Biểu mẫu Word dùng chung",
                null,
                stack =>
                {
                    var choose = AddSection(stack, "Chọn hồ sơ và loại biểu mẫu cần trích xuất", null, 180);

                    var cboCase = new ComboBox { DropDownWidth = 560 };
                    var cboTemplate = new ComboBox { DropDownWidth = 480 };

                    var caseChoices = myCases.Count > 0
                        ? myCases.Select(c => FormatCaseCode(c.ma_ho_so) + " — " + (c.ten_su_kien ?? "(Chưa đặt tên)") + " [" + FormatStatus(c.trang_thai) + "]").ToArray()
                        : new[] { "HS-0001 — Hồ sơ mẫu ViewParty Portal" };

                    var templates = WordExportService.GetTemplates();
                    var templateChoices = templates.Select(t => t.Code + " — " + t.Name).ToArray();

                    AddFieldRows(
                        choose,
                        new FieldSpec("Hồ sơ áp dụng", choices: caseChoices, control: cboCase),
                        new FieldSpec("Biểu mẫu hành chính", choices: templateChoices, control: cboTemplate));

                    Func<HoSoCapPhep> resolveSelectedCase = () =>
                    {
                        int idx = cboCase.SelectedIndex;
                        if (idx >= 0 && idx < myCases.Count)
                            return myCases[idx];
                        return myCases.FirstOrDefault();
                    };

                    Func<string> resolveSelectedTemplateCode = () =>
                    {
                        int idx = cboTemplate.SelectedIndex;
                        if (idx >= 0 && idx < templates.Count)
                            return templates[idx].Code;
                        return "VB01";
                    };

                    AddButtons(
                        choose,
                        ActionButton("Xem trước biểu mẫu", () =>
                        {
                            WordExportService.ShowPreviewDialog(this, resolveSelectedTemplateCode(), resolveSelectedCase());
                        }),
                        ActionButton("Xuất .docx", () =>
                        {
                            WordExportService.ExportToDocxWithDialog(this, resolveSelectedTemplateCode(), resolveSelectedCase());
                        }, true));

                    var libSection = AddSection(stack, "Kho biểu mẫu dùng chung", null, 380);
                    var grid = AddTable(
                        libSection,
                        "Mã biểu mẫu",
                        "Tên biểu mẫu hành chính",
                        "Vai trò sử dụng",
                        "Định dạng",
                        "Thao tác");
                    grid.Columns[0].FillWeight = 14;
                    grid.Columns[1].FillWeight = 44;
                    grid.Columns[2].FillWeight = 20;
                    grid.Columns[3].FillWeight = 10;
                    grid.Columns[4].FillWeight = 12;

                    foreach (var tpl in templates)
                    {
                        int r = grid.Rows.Add(tpl.Code, tpl.Name, tpl.TargetRole, tpl.Format, "Xem / Xuất .docx");
                        grid.Rows[r].Tag = tpl;
                    }

                    grid.SelectionChanged += (s, e) =>
                    {
                        if (grid.CurrentRow != null && grid.CurrentRow.Index >= 0 && grid.CurrentRow.Index < cboTemplate.Items.Count)
                        {
                            cboTemplate.SelectedIndex = grid.CurrentRow.Index;
                        }
                    };

                    grid.CellDoubleClick += (s, e) =>
                    {
                        if (e.RowIndex >= 0 && grid.Rows[e.RowIndex].Tag is WordTemplateSpec spec)
                        {
                            WordExportService.ShowPreviewDialog(this, spec.Code, resolveSelectedCase());
                        }
                    };
                });
        }

        private static string FormatCaseCode(long id)
        {
            return string.Format(CultureInfo.InvariantCulture, "HS-{0:0000}", id);
        }

        private static string FormatEventWindow(DateTimeOffset? start, DateTimeOffset? end)
        {
            if (!start.HasValue) return "Chưa đặt lịch";
            if (!end.HasValue) return start.Value.ToString("dd/MM/yyyy HH:mm");
            return start.Value.ToString("dd/MM/yyyy HH:mm") + " - " + end.Value.ToString("HH:mm");
        }

        private static string FormatStatusBadge(string status)
        {
            return FormatStatus(string.IsNullOrWhiteSpace(status) ? "NHAP" : status);
        }

        private static string GetActionLabel(string status)
        {
            if (string.IsNullOrWhiteSpace(status) || status == "NHAP")
                return "Mở / sửa Nháp";
            if (status == "YEU_CAU_BO_SUNG")
                return "Bổ sung hồ sơ";
            if (status == "DA_CAP_PHEP")
                return "Xem & tải GP";
            return "Xem tiến độ";
        }
    }
}
