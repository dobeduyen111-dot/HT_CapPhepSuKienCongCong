using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using Viewparty.Data;
using Viewparty.Models;

namespace Viewparty.Services
{
    public sealed class WordTemplateSpec
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string TargetRole { get; set; }
        public string UseCases { get; set; }
        public string Format { get; set; }
        public string TemplateFileName { get; set; }
    }

    public static class WordExportService
    {
        private static readonly IList<WordTemplateSpec> Templates = new List<WordTemplateSpec>
        {
            new WordTemplateSpec
            {
                Code = "VB01",
                Name = "Đơn đề nghị chấp thuận tổ chức sự kiện",
                TargetRole = "Người tổ chức",
                UseCases = "Nộp hồ sơ đề nghị cấp phép sự kiện",
                Format = ".docx",
                TemplateFileName = "VB01_ViewParty_VB01.docx"
            },
            new WordTemplateSpec
            {
                Code = "VB02",
                Name = "Thông báo yêu cầu bổ sung hồ sơ",
                TargetRole = "Người xử lý",
                UseCases = "Kiểm tra tính hợp lệ và hướng dẫn bổ sung",
                Format = ".docx",
                TemplateFileName = "VB02_ViewParty_VB02.docx"
            },
            new WordTemplateSpec
            {
                Code = "VB03",
                Name = "Công văn xin ý kiến liên ngành",
                TargetRole = "Người xử lý",
                UseCases = "Lấy ý kiến phối hợp từ đơn vị chuyên môn",
                Format = ".docx",
                TemplateFileName = "VB03_ViewParty_VB03.docx"
            },
            new WordTemplateSpec
            {
                Code = "VB04",
                Name = "Văn bản ý kiến liên ngành",
                TargetRole = "Đơn vị liên ngành",
                UseCases = "Phản hồi đánh giá chuyên môn liên ngành",
                Format = ".docx",
                TemplateFileName = "VB04_ViewParty_VB04.docx"
            },
            new WordTemplateSpec
            {
                Code = "VB05",
                Name = "Phiếu đề xuất ngoại lệ",
                TargetRole = "Người xử lý",
                UseCases = "Trình xem xét phương án bù đắp ngoại lệ",
                Format = ".docx",
                TemplateFileName = "VB05_ViewParty_VB05.docx"
            },
            new WordTemplateSpec
            {
                Code = "VB06",
                Name = "Văn bản chấp thuận tổ chức sự kiện",
                TargetRole = "Người phê duyệt",
                UseCases = "Chấp thuận chủ trương và phương án tổ chức",
                Format = ".docx",
                TemplateFileName = "VB06_ViewParty_VB06.docx"
            },
            new WordTemplateSpec
            {
                Code = "VB07",
                Name = "Thông báo không chấp thuận",
                TargetRole = "Người phê duyệt",
                UseCases = "Thông báo từ chối cấp phép và nêu rõ lý do",
                Format = ".docx",
                TemplateFileName = "VB07_ViewParty_VB07.docx"
            },
            new WordTemplateSpec
            {
                Code = "VB08",
                Name = "Giấy phép tổ chức sự kiện",
                TargetRole = "Người phê duyệt",
                UseCases = "Cấp giấy phép chính thức tổ chức sự kiện",
                Format = ".docx",
                TemplateFileName = "VB08_ViewParty_VB08.docx"
            },
            new WordTemplateSpec
            {
                Code = "VB09",
                Name = "Phiếu tiếp nhận hồ sơ",
                TargetRole = "Người xử lý",
                UseCases = "Xác nhận tiếp nhận hồ sơ hợp lệ",
                Format = ".docx",
                TemplateFileName = "VB09_ViewParty_VB09.docx"
            },
            new WordTemplateSpec
            {
                Code = "VB10",
                Name = "Biên bản kiểm tra địa điểm, an toàn và thẩm định thực tế",
                TargetRole = "Người xử lý",
                UseCases = "Ghi nhận kết quả kiểm tra thực địa và thẩm định",
                Format = ".docx",
                TemplateFileName = "VB10_ViewParty_VB10.docx"
            }
        };

        public static IList<WordTemplateSpec> GetTemplates()
        {
            return Templates;
        }

        public static WordTemplateSpec FindTemplate(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return Templates[0];
            foreach (var item in Templates)
            {
                if (string.Equals(item.Code, code.Trim(), StringComparison.OrdinalIgnoreCase))
                    return item;
            }
            return Templates[0];
        }

        private static string FormatCommercialType(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return "Phi thương mại (Phục vụ cộng đồng)";
            switch (code.Trim().ToUpperInvariant())
            {
                case "THUONG_MAI": return "Có yếu tố thương mại";
                case "PHI_THUONG_MAI": return "Phi thương mại";
                default: return code;
            }
        }

        private static string FormatTicketType(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return "Miễn phí (Đăng ký mã QR)";
            switch (code.Trim().ToUpperInvariant())
            {
                case "MIEN_PHI": return "Vé miễn phí";
                case "CO_THU_PHI": return "Vé có thu phí";
                case "VE_MOI": return "Vé mời";
                default: return code;
            }
        }

        private static Dictionary<string, string> BuildPlaceholderMap(string templateCode, HoSoCapPhep hoSo, string extraNotes)
        {
            var spec = FindTemplate(templateCode);
            DateTime now = DateTime.Now;

            string maHoSo = hoSo != null && hoSo.ma_ho_so > 0
                ? string.Format(CultureInfo.InvariantCulture, "HS-{0:0000}", hoSo.ma_ho_so)
                : "HS-0001";
            string tenSuKien = hoSo != null && !string.IsNullOrWhiteSpace(hoSo.ten_su_kien)
                ? hoSo.ten_su_kien
                : "Sự kiện Xem chung Chung kết Thể thao / Giải trí";
            string donViToChuc = hoSo != null && !string.IsNullOrWhiteSpace(hoSo.ten_don_vi_to_chuc)
                ? hoSo.ten_don_vi_to_chuc
                : "Công ty TNHH Sự kiện Thể thao & Giải trí Việt Nam";
            string nguoiDaiDien = hoSo != null && !string.IsNullOrWhiteSpace(hoSo.nguoi_to_chuc)
                ? hoSo.nguoi_to_chuc
                : "Nguyễn Văn An";
            string loaiSuKien = hoSo != null && !string.IsNullOrWhiteSpace(hoSo.ten_loai)
                ? hoSo.ten_loai
                : "Sự kiện Xem chung (Viewing Party)";
            string noiDung = hoSo != null && !string.IsNullOrWhiteSpace(hoSo.noi_dung)
                ? hoSo.noi_dung
                : "Trình chiếu trực tiếp trận chung kết kết hợp giao lưu khán giả và hoạt động cổ vũ văn minh.";
            string batDau = hoSo != null && hoSo.bat_dau.HasValue
                ? hoSo.bat_dau.Value.ToString("dd/MM/yyyy HH:mm")
                : now.AddDays(35).ToString("dd/MM/yyyy 18:00");
            string ketThuc = hoSo != null && hoSo.ket_thuc.HasValue
                ? hoSo.ket_thuc.Value.ToString("dd/MM/yyyy HH:mm")
                : now.AddDays(35).ToString("dd/MM/yyyy 22:30");
            string tenDiaDiem = hoSo != null && !string.IsNullOrWhiteSpace(hoSo.ten_dia_diem)
                ? hoSo.ten_dia_diem
                : "Nhà thi đấu Nguyễn Du";
            string diaChiDiaDiem = hoSo != null && !string.IsNullOrWhiteSpace(hoSo.dia_chi_dia_diem)
                ? hoSo.dia_chi_dia_diem
                : "116 Nguyễn Du, Phường Bến Thành, Quận 1, TP. Hồ Chí Minh";
            string quyMo = hoSo != null && hoSo.quy_mo.HasValue
                ? hoSo.quy_mo.Value.ToString("N0", CultureInfo.GetCultureInfo("vi-VN"))
                : "1.500";
            string sucChua = hoSo != null && hoSo.suc_chua_dia_diem.HasValue
                ? hoSo.suc_chua_dia_diem.Value.ToString("N0", CultureInfo.GetCultureInfo("vi-VN"))
                : "2.500";
            string hinhThucThuongMai = FormatCommercialType(hoSo != null ? hoSo.hinh_thuc_thuong_mai : null);
            string loaiVe = FormatTicketType(hoSo != null ? hoSo.loai_ve : null);
            string soLuongVe = hoSo != null && hoSo.so_luong_ve.HasValue
                ? hoSo.so_luong_ve.Value.ToString("N0", CultureInfo.GetCultureInfo("vi-VN")) + " vé"
                : quyMo + " vé";
            string giaVe = hoSo != null && hoSo.gia_ve.HasValue && hoSo.gia_ve.Value > 0
                ? hoSo.gia_ve.Value.ToString("N0", CultureInfo.GetCultureInfo("vi-VN")) + " VNĐ"
                : "0 VNĐ (Miễn phí)";
            string kenhPhatHanh = hoSo != null && !string.IsNullOrWhiteSpace(hoSo.kenh_phat_hanh_ve)
                ? hoSo.kenh_phat_hanh_ve
                : "Đăng ký trực tuyến và kiểm soát bằng mã QR tại cổng vào";
            string nhanSuAnNinh = hoSo != null && !string.IsNullOrWhiteSpace(hoSo.nhan_su_an_ninh)
                ? hoSo.nhan_su_an_ninh
                : "Bố trí 25 nhân sự bảo vệ chuyên nghiệp tại cổng kiểm soát, khu vực khán đài và lối thoát hiểm.";
            string nhanSuYTe = hoSo != null && !string.IsNullOrWhiteSpace(hoSo.nhan_su_y_te)
                ? hoSo.nhan_su_y_te
                : "01 tổ y tế thường trực (02 bác sĩ, 03 điều dưỡng) và 01 xe cứu thương túc trực ngoài sảnh.";
            string phuongAnThoatHiem = hoSo != null && !string.IsNullOrWhiteSpace(hoSo.phuong_an_thoat_hiem)
                ? hoSo.phuong_an_thoat_hiem
                : "Mở thông suốt 04 cửa thoát hiểm có đèn chỉ dẫn phát sáng và sơ đồ hướng dẫn tại từng khu vực.";
            string phuongAnSuCo = hoSo != null && !string.IsNullOrWhiteSpace(hoSo.phuong_an_xu_ly_su_co)
                ? hoSo.phuong_an_xu_ly_su_co
                : "Ngắt âm thanh/hình ảnh khẩn cấp, hướng dẫn sơ tán theo phân luồng và phối hợp lực lượng chức năng địa phương.";
            string soGiayPhep = hoSo != null && !string.IsNullOrWhiteSpace(hoSo.so_giay_phep)
                ? hoSo.so_giay_phep
                : string.Format(CultureInfo.InvariantCulture, "GP-VP/{0}/{1:0000}", now.Year, hoSo != null && hoSo.ma_ho_so > 0 ? hoSo.ma_ho_so : 1);
            string ngayNop = hoSo != null && hoSo.ngay_nop.HasValue
                ? hoSo.ngay_nop.Value.ToString("dd/MM/yyyy HH:mm")
                : now.AddDays(-2).ToString("dd/MM/yyyy 09:00");
            string ngayTiepNhan = now.ToString("dd/MM/yyyy HH:mm");

            // Default values for enriched fields
            string sdtToChuc = "0909 123 456";
            string emailToChuc = "lienhe@viewparty-event.vn";
            string diaChiToChuc = diaChiDiaDiem;
            string donViCapQuyen = "Đơn vị nắm giữ bản quyền phát sóng chính thức";
            string phamViQuyen = "Trình chiếu công cộng phi độc quyền tại địa điểm đăng ký";
            string hieuLucQuyen = batDau + " đến " + ketThuc;
            string danhSachTaiLieu = "Đơn đề nghị cấp phép; Văn bản chứng minh quyền trình chiếu; Phương án an ninh, y tế và thoát hiểm; Hợp đồng thuê địa điểm.";
            string noiDungCanBoSung = !string.IsNullOrWhiteSpace(extraNotes)
                ? extraNotes.Trim()
                : (hoSo != null && !string.IsNullOrWhiteSpace(hoSo.ghi_chu_bo_sung)
                    ? hoSo.ghi_chu_bo_sung
                    : "Bổ sung bản sao hợp lệ giấy xác nhận quyền trình chiếu công cộng và sơ đồ bố trí lối thoát hiểm chi tiết.");
            string taiLieuCanBoSung = "Văn bản xác nhận bản quyền trình chiếu công cộng; Sơ đồ phân luồng thoát hiểm và PCCC.";
            string hanBoSung = now.AddDays(5).ToString("dd/MM/yyyy");
            string tenDonViLienNganh = "Phòng Cảnh sát PCCC & CNCH / Công an địa phương";
            string linhVucChuyenMon = "An ninh trật tự, Phòng cháy chữa cháy và Cứu nạn cứu hộ";
            string noiDungXinYKien = !string.IsNullOrWhiteSpace(extraNotes)
                ? extraNotes.Trim()
                : "Đề nghị cho ý kiến đánh giá chuyên môn về phương án đảm bảo an ninh trật tự, an toàn PCCC và phân luồng giao thông cho sự kiện.";
            string hanPhanHoi = now.AddDays(3).ToString("dd/MM/yyyy HH:mm");
            string maYeuCau = "YC-" + (hoSo != null && hoSo.ma_ho_so > 0 ? hoSo.ma_ho_so.ToString("0000") : "0001");
            string ngayPhanHoi = now.ToString("dd/MM/yyyy HH:mm");
            string noiDungPhanHoi = !string.IsNullOrWhiteSpace(extraNotes)
                ? extraNotes.Trim()
                : "Qua rà soát hồ sơ và phương án thực tế, địa điểm đáp ứng yêu cầu về lối thoát hiểm, trang thiết bị PCCC tại chỗ và phương án bố trí lực lượng bảo vệ.";
            string ketLuanYKien = "Phù hợp (Đạt yêu cầu chuyên môn)";
            string kienNghi = "Đơn vị tổ chức cần bố trí nhân sự trực tại các cửa thoát hiểm trong suốt thời gian diễn ra sự kiện.";
            string tepDinhKem = "BienBan_ThamDinh_LienNganh_" + maHoSo + ".pdf";
            string nguoiDeXuat = "Cán bộ thẩm định hồ sơ";
            string maQuyTac = "QT_SUC_CHUA / QT_THOI_HAN";
            string phienBan = "Phiên bản 1.0";
            string dieuKienKhongDat = "Cần áp dụng phương án tăng cường kiểm soát do đặc thù quy mô và thời gian sự kiện.";
            string tongHopYKienLienNganh = "Các đơn vị liên ngành (An ninh, PCCC, Y tế) đã có văn bản thống nhất thông qua phương án đảm bảo an toàn.";
            string lyDoNgoaiLe = !string.IsNullOrWhiteSpace(extraNotes)
                ? extraNotes.Trim()
                : "Sự kiện phục vụ cộng đồng theo lịch thi đấu quốc tế cố định, đơn vị tổ chức đã chuẩn bị đầy đủ phương án tăng cường an toàn.";
            string minhChung = "Văn bản xác nhận lịch phát sóng quốc tế và hợp đồng tăng cường lực lượng bảo vệ, y tế.";
            string bienPhapBuDap = "Tăng thêm 30% nhân sự bảo vệ, mở thêm 02 cổng kiểm soát phụ và bố trí xe cứu thương trực 100% thời gian.";
            string capThamQuyen = "Cấp thẩm quyền phê duyệt";
            string quyMoDuocDuyet = quyMo;
            string quyenTrinhChieu = donViCapQuyen + " (" + phamViQuyen + ")";
            string dieuKienKemTheo = !string.IsNullOrWhiteSpace(extraNotes)
                ? extraNotes.Trim()
                : "Đảm bảo trật tự an toàn xã hội, phòng chống cháy nổ, kiểm soát số lượng người tham dự không vượt quá quy mô được duyệt.";
            string ketQuaThamDinh = "Đã hoàn thành kiểm tra tính hợp lệ và thẩm định các điều kiện an toàn, địa điểm, bản quyền.";
            string ketQuaNgoaiLe = "Không phát sinh ngoại lệ chưa được phê duyệt";
            string lyDoTuChoi = hoSo != null && !string.IsNullOrWhiteSpace(hoSo.ly_do_tu_choi)
                ? hoSo.ly_do_tu_choi
                : (!string.IsNullOrWhiteSpace(extraNotes) ? extraNotes.Trim() : "Hồ sơ chưa đáp ứng đầy đủ điều kiện về an toàn sức chứa hoặc bản quyền trình chiếu công cộng.");
            string hieuLucTu = batDau;
            string hieuLucDen = ketThuc;
            string nguoiTiepNhan = "Cán bộ tiếp nhận và xử lý hồ sơ";
            string nguoiKiemTra = "Cán bộ thẩm định hồ sơ";
            string ngayKiemTra = now.ToString("dd/MM/yyyy HH:mm");
            string ketQuaTrungLich = "Không trùng lịch với sự kiện khác tại cùng địa điểm trong khung giờ đăng ký.";
            string ketQuaAnToan = "Đạt yêu cầu (Phương án an ninh, y tế và thoát hiểm đầy đủ, rõ ràng).";
            string danhSachQuyTac = "Quy tắc kiểm tra thời hạn nộp hồ sơ, sức chứa địa điểm, bản quyền trình chiếu và phương án an toàn (Phiên bản hiện hành).";
            string tongHopKetQua = "Đạt yêu cầu thẩm định thực tế.";
            string kienNghiXuLy = !string.IsNullOrWhiteSpace(extraNotes)
                ? extraNotes.Trim()
                : "Đủ điều kiện trình cấp có thẩm quyền xem xét chấp thuận và cấp giấy phép tổ chức sự kiện.";

            // Enrich from DB if hoSo is present
            if (hoSo != null && hoSo.ma_ho_so > 0)
            {
                TryEnrichFromDatabase(
                    hoSo.ma_ho_so,
                    hoSo.nguoi_to_chuc_id,
                    ref nguoiDaiDien,
                    ref sdtToChuc,
                    ref emailToChuc,
                    ref donViCapQuyen,
                    ref phamViQuyen,
                    ref hieuLucQuyen,
                    ref danhSachTaiLieu,
                    ref tenDonViLienNganh,
                    ref linhVucChuyenMon,
                    ref noiDungXinYKien,
                    ref hanPhanHoi,
                    ref maYeuCau,
                    ref noiDungPhanHoi,
                    ref ketLuanYKien,
                    ref lyDoNgoaiLe,
                    ref minhChung,
                    ref bienPhapBuDap,
                    ref soGiayPhep,
                    ref hieuLucTu,
                    ref hieuLucDen,
                    ref dieuKienKemTheo);
            }

            string tenCoQuanHoacDonVi = spec.Code == "VB01"
                ? donViToChuc.ToUpperInvariant()
                : (spec.Code == "VB04"
                    ? tenDonViLienNganh.ToUpperInvariant()
                    : "SỞ VĂN HÓA VÀ THỂ THAO\nTRUNG TÂM CẤP PHÉP SỰ KIỆN VIEW PARTY");

            string coQuanTiepNhan = (spec.Code == "VB01" || spec.Code == "VB04")
                ? "Cơ quan tiếp nhận và thẩm định cấp phép sự kiện View Party"
                : (spec.Code == "VB03"
                    ? tenDonViLienNganh
                    : (spec.Code == "VB05"
                        ? "Lãnh đạo có thẩm quyền phê duyệt cấp phép sự kiện View Party"
                        : donViToChuc));

            string hoTenNguoiKy = spec.Code == "VB01"
                ? nguoiDaiDien
                : (spec.Code == "VB04"
                    ? "Đại diện " + tenDonViLienNganh
                    : (spec.Code == "VB06" || spec.Code == "VB07" || spec.Code == "VB08"
                        ? "Lãnh đạo phê duyệt cấp phép"
                        : "Cán bộ thẩm định hồ sơ"));

            string soVanBan = string.Format(
                CultureInfo.InvariantCulture,
                "{0:00}/{1}/{2}-VP",
                hoSo != null && hoSo.ma_ho_so > 0 ? hoSo.ma_ho_so : 1,
                now.Year,
                spec.Code);

            return new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["TEN_CO_QUAN_HOAC_DON_VI"] = tenCoQuanHoacDonVi,
                ["SO_VAN_BAN"] = soVanBan,
                ["DIA_DANH"] = "TP. Hồ Chí Minh",
                ["NGAY"] = now.ToString("dd"),
                ["THANG"] = now.ToString("MM"),
                ["NAM"] = now.ToString("yyyy"),
                ["CO_QUAN_TIEP_NHAN"] = coQuanTiepNhan,
                ["TEN_DON_VI_TO_CHUC"] = donViToChuc,
                ["NGUOI_DAI_DIEN"] = nguoiDaiDien,
                ["SDT_TO_CHUC"] = sdtToChuc,
                ["EMAIL_TO_CHUC"] = emailToChuc,
                ["DIA_CHI_TO_CHUC"] = diaChiToChuc,
                ["MA_HO_SO"] = maHoSo,
                ["TEN_SU_KIEN"] = tenSuKien,
                ["LOAI_SU_KIEN"] = loaiSuKien,
                ["NOI_DUNG"] = noiDung,
                ["BAT_DAU"] = batDau,
                ["KET_THUC"] = ketThuc,
                ["TEN_DIA_DIEM"] = tenDiaDiem,
                ["DIA_CHI_DIA_DIEM"] = diaChiDiaDiem,
                ["QUY_MO"] = quyMo,
                ["SUC_CHUA"] = sucChua,
                ["HINH_THUC_THUONG_MAI"] = hinhThucThuongMai,
                ["LOAI_VE"] = loaiVe,
                ["SO_LUONG_VE"] = soLuongVe,
                ["GIA_VE"] = giaVe,
                ["KENH_PHAT_HANH_VE"] = kenhPhatHanh,
                ["DON_VI_CAP_QUYEN"] = donViCapQuyen,
                ["PHAM_VI_QUYEN"] = phamViQuyen,
                ["HIEU_LUC_QUYEN"] = hieuLucQuyen,
                ["NHAN_SU_AN_NINH"] = nhanSuAnNinh,
                ["NHAN_SU_Y_TE"] = nhanSuYTe,
                ["PHUONG_AN_THOAT_HIEM"] = phuongAnThoatHiem,
                ["PHUONG_AN_XU_LY_SU_CO"] = phuongAnSuCo,
                ["DANH_SACH_TAI_LIEU"] = danhSachTaiLieu,
                ["HO_TEN_NGUOI_KY"] = hoTenNguoiKy,
                ["NGAY_TIEP_NHAN"] = ngayTiepNhan,
                ["NOI_DUNG_CAN_BO_SUNG"] = noiDungCanBoSung,
                ["TAI_LIEU_CAN_BO_SUNG"] = taiLieuCanBoSung,
                ["HAN_BO_SUNG"] = hanBoSung,
                ["TEN_DON_VI_LIEN_NGANH"] = tenDonViLienNganh,
                ["LINH_VUC_CHUYEN_MON"] = linhVucChuyenMon,
                ["NOI_DUNG_XIN_Y_KIEN"] = noiDungXinYKien,
                ["HAN_PHAN_HOI"] = hanPhanHoi,
                ["MA_YEU_CAU"] = maYeuCau,
                ["NGAY_PHAN_HOI"] = ngayPhanHoi,
                ["NOI_DUNG_PHAN_HOI"] = noiDungPhanHoi,
                ["KET_LUAN_Y_KIEN"] = ketLuanYKien,
                ["KIEN_NGHI"] = kienNghi,
                ["TEP_DINH_KEM"] = tepDinhKem,
                ["NGUOI_DE_XUAT"] = nguoiDeXuat,
                ["MA_QUY_TAC"] = maQuyTac,
                ["PHIEN_BAN"] = phienBan,
                ["DIEU_KIEN_KHONG_DAT"] = dieuKienKhongDat,
                ["TONG_HOP_Y_KIEN_LIEN_NGANH"] = tongHopYKienLienNganh,
                ["LY_DO_NGOAI_LE"] = lyDoNgoaiLe,
                ["MINH_CHUNG"] = minhChung,
                ["BIEN_PHAP_BU_DAP"] = bienPhapBuDap,
                ["CAP_THAM_QUYEN"] = capThamQuyen,
                ["QUY_MO_DUOC_DUYET"] = quyMoDuocDuyet,
                ["QUYEN_TRINH_CHIEU"] = quyenTrinhChieu,
                ["DIEU_KIEN_KEM_THEO"] = dieuKienKemTheo,
                ["KET_QUA_THAM_DINH"] = ketQuaThamDinh,
                ["KET_QUA_NGOAI_LE"] = ketQuaNgoaiLe,
                ["LY_DO_TU_CHOI"] = lyDoTuChoi,
                ["SO_GIAY_PHEP"] = soGiayPhep,
                ["HIEU_LUC_TU"] = hieuLucTu,
                ["HIEU_LUC_DEN"] = hieuLucDen,
                ["NGAY_NOP"] = ngayNop,
                ["NGUOI_TIEP_NHAN"] = nguoiTiepNhan,
                ["NGUOI_KIEM_TRA"] = nguoiKiemTra,
                ["NGAY_KIEM_TRA"] = ngayKiemTra,
                ["KET_QUA_TRUNG_LICH"] = ketQuaTrungLich,
                ["KET_QUA_AN_TOAN"] = ketQuaAnToan,
                ["DANH_SACH_QUY_TAC_VA_PHIEN_BAN"] = danhSachQuyTac,
                ["TONG_HOP_KET_QUA"] = tongHopKetQua,
                ["KIEN_NGHI_XU_LY"] = kienNghiXuLy
            };
        }

        private static void TryEnrichFromDatabase(
            long maHoSo,
            long nguoiToChucId,
            ref string nguoiDaiDien,
            ref string sdtToChuc,
            ref string emailToChuc,
            ref string donViCapQuyen,
            ref string phamViQuyen,
            ref string hieuLucQuyen,
            ref string danhSachTaiLieu,
            ref string tenDonViLienNganh,
            ref string linhVucChuyenMon,
            ref string noiDungXinYKien,
            ref string hanPhanHoi,
            ref string maYeuCau,
            ref string noiDungPhanHoi,
            ref string ketLuanYKien,
            ref string lyDoNgoaiLe,
            ref string minhChung,
            ref string bienPhapBuDap,
            ref string soGiayPhep,
            ref string hieuLucTu,
            ref string hieuLucDen,
            ref string dieuKienKemTheo)
        {
            try
            {
                using (var conn = Db.OpenConnection())
                {
                    if (nguoiToChucId > 0)
                    {
                        using (var cmd = new SqlCommand("SELECT ho_ten, email, so_dien_thoai FROM dbo.TAI_KHOAN WHERE ma_tai_khoan = @Id;", conn))
                        {
                            cmd.Parameters.Add("@Id", SqlDbType.BigInt).Value = nguoiToChucId;
                            using (var r = cmd.ExecuteReader())
                            {
                                if (r.Read())
                                {
                                    if (!r.IsDBNull(0) && !string.IsNullOrWhiteSpace(r.GetString(0))) nguoiDaiDien = r.GetString(0);
                                    if (!r.IsDBNull(1) && !string.IsNullOrWhiteSpace(r.GetString(1))) emailToChuc = r.GetString(1);
                                    if (!r.IsDBNull(2) && !string.IsNullOrWhiteSpace(r.GetString(2))) sdtToChuc = r.GetString(2);
                                }
                            }
                        }
                    }

                    using (var cmd = new SqlCommand("SELECT TOP 1 don_vi_cap_quyen, pham_vi_quyen, hieu_luc_tu, hieu_luc_den FROM dbo.QUYEN_TRINH_CHIEU WHERE ma_ho_so = @MaHoSo;", conn))
                    {
                        cmd.Parameters.Add("@MaHoSo", SqlDbType.BigInt).Value = maHoSo;
                        using (var r = cmd.ExecuteReader())
                        {
                            if (r.Read())
                            {
                                if (!r.IsDBNull(0) && !string.IsNullOrWhiteSpace(r.GetString(0))) donViCapQuyen = r.GetString(0);
                                if (!r.IsDBNull(1) && !string.IsNullOrWhiteSpace(r.GetString(1))) phamViQuyen = r.GetString(1);
                                if (!r.IsDBNull(2) && !r.IsDBNull(3))
                                {
                                    hieuLucQuyen = r.GetDateTimeOffset(2).ToString("dd/MM/yyyy") + " đến " + r.GetDateTimeOffset(3).ToString("dd/MM/yyyy");
                                }
                            }
                        }
                    }

                    using (var cmd = new SqlCommand("SELECT ten_tep, loai_tai_lieu FROM dbo.TAI_LIEU WHERE ma_ho_so = @MaHoSo ORDER BY ma_tai_lieu;", conn))
                    {
                        cmd.Parameters.Add("@MaHoSo", SqlDbType.BigInt).Value = maHoSo;
                        var docs = new List<string>();
                        using (var r = cmd.ExecuteReader())
                        {
                            while (r.Read())
                            {
                                string name = r.IsDBNull(0) ? "" : r.GetString(0);
                                if (!string.IsNullOrWhiteSpace(name)) docs.Add(name);
                            }
                        }
                        if (docs.Count > 0)
                            danhSachTaiLieu = string.Join("; ", docs);
                    }

                    using (var cmd = new SqlCommand(@"
SELECT TOP 1 yc.ma_yeu_cau, dv.ten_don_vi, dv.linh_vuc, yc.noi_dung_yeu_cau, yc.han_phan_hoi, yc.noi_dung_phan_hoi, yc.ket_luan_y_kien
FROM dbo.YEU_CAU_Y_KIEN yc
INNER JOIN dbo.DON_VI_LIEN_NGANH dv ON dv.ma_don_vi = yc.ma_don_vi
WHERE yc.ma_ho_so = @MaHoSo
ORDER BY yc.ma_yeu_cau DESC;", conn))
                    {
                        cmd.Parameters.Add("@MaHoSo", SqlDbType.BigInt).Value = maHoSo;
                        using (var r = cmd.ExecuteReader())
                        {
                            if (r.Read())
                            {
                                maYeuCau = "YC-" + r.GetInt64(0).ToString("0000");
                                if (!r.IsDBNull(1)) tenDonViLienNganh = r.GetString(1);
                                if (!r.IsDBNull(2)) linhVucChuyenMon = r.GetString(2);
                                if (!r.IsDBNull(3) && !string.IsNullOrWhiteSpace(r.GetString(3))) noiDungXinYKien = r.GetString(3);
                                if (!r.IsDBNull(4)) hanPhanHoi = r.GetDateTimeOffset(4).ToString("dd/MM/yyyy HH:mm");
                                if (!r.IsDBNull(5) && !string.IsNullOrWhiteSpace(r.GetString(5))) noiDungPhanHoi = r.GetString(5);
                                if (!r.IsDBNull(6) && !string.IsNullOrWhiteSpace(r.GetString(6))) ketLuanYKien = r.GetString(6);
                            }
                        }
                    }

                    using (var cmd = new SqlCommand(@"
SELECT TOP 1 ly_do, minh_chung, bien_phap_bu_dap
FROM dbo.NGOAI_LE
WHERE ma_ho_so = @MaHoSo
ORDER BY ma_ngoai_le DESC;", conn))
                    {
                        cmd.Parameters.Add("@MaHoSo", SqlDbType.BigInt).Value = maHoSo;
                        using (var r = cmd.ExecuteReader())
                        {
                            if (r.Read())
                            {
                                if (!r.IsDBNull(0) && !string.IsNullOrWhiteSpace(r.GetString(0))) lyDoNgoaiLe = r.GetString(0);
                                if (!r.IsDBNull(1) && !string.IsNullOrWhiteSpace(r.GetString(1))) minhChung = r.GetString(1);
                                if (!r.IsDBNull(2) && !string.IsNullOrWhiteSpace(r.GetString(2))) bienPhapBuDap = r.GetString(2);
                            }
                        }
                    }

                    using (var cmd = new SqlCommand(@"
SELECT TOP 1 so_giay_phep, hieu_luc_tu, hieu_luc_den, dieu_kien_kem_theo
FROM dbo.GIAY_PHEP
WHERE ma_ho_so = @MaHoSo
ORDER BY ma_giay_phep DESC;", conn))
                    {
                        cmd.Parameters.Add("@MaHoSo", SqlDbType.BigInt).Value = maHoSo;
                        using (var r = cmd.ExecuteReader())
                        {
                            if (r.Read())
                            {
                                if (!r.IsDBNull(0) && !string.IsNullOrWhiteSpace(r.GetString(0))) soGiayPhep = r.GetString(0);
                                if (!r.IsDBNull(1)) hieuLucTu = r.GetDateTimeOffset(1).ToString("dd/MM/yyyy HH:mm");
                                if (!r.IsDBNull(2)) hieuLucDen = r.GetDateTimeOffset(2).ToString("dd/MM/yyyy HH:mm");
                                if (!r.IsDBNull(3) && !string.IsNullOrWhiteSpace(r.GetString(3))) dieuKienKemTheo = r.GetString(3);
                            }
                        }
                    }
                }
            }
            catch
            {
                // Ignore database enrichment errors when previewing offline
            }
        }

        public static string BuildDocumentText(string templateCode, HoSoCapPhep hoSo, string extraNotes = null)
        {
            var spec = FindTemplate(templateCode);
            var map = BuildPlaceholderMap(spec.Code, hoSo, extraNotes);

            // If we have the .docx template on disk, extract its paragraphs and replace tokens so preview matches 100%!
            string templatePath = ResolveTemplatePath(spec.TemplateFileName);
            if (!string.IsNullOrWhiteSpace(templatePath) && File.Exists(templatePath))
            {
                try
                {
                    using (var fs = new FileStream(templatePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (var archive = new ZipArchive(fs, ZipArchiveMode.Read))
                    {
                        var docEntry = archive.GetEntry("word/document.xml");
                        if (docEntry != null)
                        {
                            XDocument xdoc;
                            using (var stream = docEntry.Open())
                            {
                                xdoc = XDocument.Load(stream);
                            }
                            ReplacePlaceholdersInXml(xdoc, map);
                            XNamespace w = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
                            var sbFromDocx = new StringBuilder();
                            foreach (var p in xdoc.Descendants(w + "p"))
                            {
                                string line = string.Concat(p.Descendants(w + "t").Select(t => t.Value)).Trim();
                                if (!string.IsNullOrWhiteSpace(line))
                                {
                                    sbFromDocx.AppendLine(line);
                                    if (line.StartsWith("---------------", StringComparison.Ordinal) ||
                                        line.StartsWith("Kính gửi:", StringComparison.Ordinal) ||
                                        line.StartsWith("ĐƠN ", StringComparison.Ordinal) ||
                                        line.StartsWith("THÔNG BÁO", StringComparison.Ordinal) ||
                                        line.StartsWith("CÔNG VĂN", StringComparison.Ordinal) ||
                                        line.StartsWith("VĂN BẢN", StringComparison.Ordinal) ||
                                        line.StartsWith("PHIẾU ", StringComparison.Ordinal) ||
                                        line.StartsWith("GIẤY PHÉP", StringComparison.Ordinal) ||
                                        line.StartsWith("VÀ THẨM ĐỊNH", StringComparison.Ordinal))
                                    {
                                        sbFromDocx.AppendLine();
                                    }
                                }
                            }
                            return sbFromDocx.ToString().Trim();
                        }
                    }
                }
                catch
                {
                    // Fallback to text builder below if reading zip fails
                }
            }

            var sb = new StringBuilder();
            sb.AppendLine(map["TEN_CO_QUAN_HOAC_DON_VI"]);
            sb.AppendLine("Số: " + map["SO_VAN_BAN"]);
            sb.AppendLine("CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM");
            sb.AppendLine("Độc lập - Tự do - Hạnh phúc");
            sb.AppendLine("---------------");
            sb.AppendLine(string.Format("{0}, ngày {1} tháng {2} năm {3}", map["DIA_DANH"], map["NGAY"], map["THANG"], map["NAM"]));
            sb.AppendLine();
            sb.AppendLine(spec.Name.ToUpperInvariant());
            sb.AppendLine("Kính gửi: " + map["CO_QUAN_TIEP_NHAN"]);
            sb.AppendLine();
            sb.AppendLine("- Mã hồ sơ: " + map["MA_HO_SO"]);
            sb.AppendLine("- Tên sự kiện: " + map["TEN_SU_KIEN"]);
            sb.AppendLine("- Đơn vị tổ chức: " + map["TEN_DON_VI_TO_CHUC"]);
            sb.AppendLine("- Thời gian: " + map["BAT_DAU"] + " đến " + map["KET_THUC"]);
            sb.AppendLine("- Địa điểm: " + map["TEN_DIA_DIEM"] + " – " + map["DIA_CHI_DIA_DIEM"]);
            if (!string.IsNullOrWhiteSpace(extraNotes))
            {
                sb.AppendLine("- Nội dung chi tiết: " + extraNotes.Trim());
            }
            sb.AppendLine();
            sb.AppendLine("Người ký: " + map["HO_TEN_NGUOI_KY"]);
            return sb.ToString();
        }

        private static string ResolveTemplatePath(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return null;
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string[] candidates =
            {
                Path.Combine(baseDir, "Templates", fileName),
                Path.Combine(baseDir, @"..\..\Templates", fileName),
                Path.Combine(baseDir, @"..\..\..\Viewparty\Templates", fileName),
                @"D:\N4HK1\PTPMHDT\Viewparty\Viewparty\Templates\" + fileName
            };
            foreach (string path in candidates)
            {
                try
                {
                    string full = Path.GetFullPath(path);
                    if (File.Exists(full)) return full;
                }
                catch
                {
                }
            }
            return null;
        }

        private static void ReplacePlaceholdersInXml(XDocument xdoc, IDictionary<string, string> map)
        {
            if (xdoc == null || map == null) return;
            XNamespace w = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";

            // Ordered by key length descending so longer keys like TEN_DON_VI_LIEN_NGANH are replaced before shorter substrings
            var orderedPairs = map.OrderByDescending(kv => kv.Key.Length).ToList();

            foreach (var textNode in xdoc.Descendants(w + "t"))
            {
                string val = textNode.Value;
                if (string.IsNullOrEmpty(val)) continue;
                string updated = val;
                foreach (var kv in orderedPairs)
                {
                    if (updated.Contains(kv.Key))
                    {
                        updated = updated.Replace(kv.Key, kv.Value ?? string.Empty);
                    }
                }
                if (!string.Equals(val, updated, StringComparison.Ordinal))
                {
                    textNode.Value = updated;
                    textNode.SetAttributeValue(XNamespace.Xml + "space", "preserve");
                }
            }
        }

        public static void ShowPreviewDialog(IWin32Window owner, string templateCode, HoSoCapPhep hoSo, string extraNotes = null)
        {
            var spec = FindTemplate(templateCode);
            string content = BuildDocumentText(spec.Code, hoSo, extraNotes);

            var hostControl = owner as Control;
            if (hostControl != null)
            {
                var overlay = new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = Color.FromArgb(245, 248, 252),
                    Font = new Font("Arial", 10F, FontStyle.Regular)
                };

                var header = new Panel
                {
                    Dock = DockStyle.Top,
                    Height = 58,
                    BackColor = Color.FromArgb(20, 44, 65),
                    Padding = new Padding(24, 12, 24, 10)
                };
                var lblTitle = new Label
                {
                    Dock = DockStyle.Fill,
                    ForeColor = Color.White,
                    Font = new Font("Arial", 13F, FontStyle.Bold),
                    Text = spec.Code + " — " + spec.Name,
                    TextAlign = ContentAlignment.MiddleLeft
                };
                header.Controls.Add(lblTitle);

                var bodyPanel = new Panel
                {
                    Dock = DockStyle.Fill,
                    Padding = new Padding(28, 18, 28, 14),
                    BackColor = Color.FromArgb(245, 248, 252)
                };

                var txtPreview = new TextBox
                {
                    Dock = DockStyle.Fill,
                    Multiline = true,
                    ReadOnly = true,
                    ScrollBars = ScrollBars.Vertical,
                    BackColor = Color.White,
                    ForeColor = Color.FromArgb(29, 51, 72),
                    BorderStyle = BorderStyle.FixedSingle,
                    Font = new Font("Arial", 10.5F, FontStyle.Regular),
                    Text = content.Replace("\r\n", "\n").Replace("\n", Environment.NewLine)
                };
                txtPreview.SelectionStart = 0;
                txtPreview.SelectionLength = 0;
                bodyPanel.Controls.Add(txtPreview);

                var footer = new Panel
                {
                    Dock = DockStyle.Bottom,
                    Height = 60,
                    BackColor = Color.White,
                    Padding = new Padding(28, 10, 28, 10)
                };

                var btnBack = new Button
                {
                    Text = "← Quay lại",
                    Width = 130,
                    Height = 38,
                    Left = 28,
                    Top = 11,
                    BackColor = Color.FromArgb(233, 242, 244),
                    ForeColor = Color.FromArgb(29, 51, 72),
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Arial", 10F, FontStyle.Bold),
                    Cursor = Cursors.Hand
                };
                btnBack.FlatAppearance.BorderColor = Color.FromArgb(219, 228, 237);
                btnBack.Click += delegate
                {
                    hostControl.Controls.Remove(overlay);
                    overlay.Dispose();
                };

                var btnExport = new Button
                {
                    Text = "Xuất file .docx",
                    Width = 160,
                    Height = 38,
                    Left = 170,
                    Top = 11,
                    BackColor = Color.FromArgb(19, 122, 120),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Arial", 10F, FontStyle.Bold),
                    Cursor = Cursors.Hand
                };
                btnExport.FlatAppearance.BorderSize = 0;
                btnExport.Click += delegate
                {
                    ExportToDocxWithDialog(hostControl, spec.Code, hoSo, extraNotes);
                };

                footer.Controls.Add(btnBack);
                footer.Controls.Add(btnExport);

                overlay.Controls.Add(bodyPanel);
                overlay.Controls.Add(footer);
                overlay.Controls.Add(header);

                hostControl.Controls.Add(overlay);
                overlay.BringToFront();
                return;
            }
        }

        public static bool ExportToDocxWithDialog(IWin32Window owner, string templateCode, HoSoCapPhep hoSo, string extraNotes = null)
        {
            var spec = FindTemplate(templateCode);
            string caseSuffix = hoSo != null && hoSo.ma_ho_so > 0
                ? string.Format(CultureInfo.InvariantCulture, "_HS{0:0000}", hoSo.ma_ho_so)
                : string.Empty;

            using (var saveDialog = new SaveFileDialog())
            {
                saveDialog.Title = "Xuất văn bản hành chính — " + spec.Code + " (" + spec.Name + ")";
                saveDialog.Filter = "Tài liệu Word (*.docx)|*.docx";
                saveDialog.DefaultExt = "docx";
                saveDialog.FileName = spec.Code + caseSuffix + "_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".docx";

                if (saveDialog.ShowDialog(owner) != DialogResult.OK)
                    return false;

                WriteDocxFromTemplate(saveDialog.FileName, spec.Code, hoSo, extraNotes);

                MessageBox.Show(
                    owner,
                    "Đã xuất văn bản " + spec.Code + " (" + spec.Name + ") thành công:\n" + saveDialog.FileName,
                    "Xuất văn bản thành công",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return true;
            }
        }

        public static void WriteDocxFromTemplate(string filePath, string templateCode, HoSoCapPhep hoSo, string extraNotes = null)
        {
            var spec = FindTemplate(templateCode);
            string templatePath = ResolveTemplatePath(spec.TemplateFileName);
            if (!string.IsNullOrWhiteSpace(templatePath) && File.Exists(templatePath))
            {
                var map = BuildPlaceholderMap(spec.Code, hoSo, extraNotes);
                File.Copy(templatePath, filePath, true);
                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite))
                using (var archive = new ZipArchive(stream, ZipArchiveMode.Update))
                {
                    var docEntry = archive.GetEntry("word/document.xml");
                    if (docEntry != null)
                    {
                        XDocument xdoc;
                        using (var entryStream = docEntry.Open())
                        {
                            xdoc = XDocument.Load(entryStream);
                        }
                        ReplacePlaceholdersInXml(xdoc, map);
                        docEntry.Delete();
                        var newEntry = archive.CreateEntry("word/document.xml", CompressionLevel.Optimal);
                        using (var writer = new StreamWriter(newEntry.Open(), new UTF8Encoding(false)))
                        {
                            writer.Write(xdoc.ToString(SaveOptions.DisableFormatting));
                        }
                    }
                }
                return;
            }

            string text = BuildDocumentText(spec.Code, hoSo, extraNotes);
            WriteDocxFile(filePath, spec.Code + " - " + spec.Name, text);
        }

        public static void WriteDocxFile(string filePath, string title, string content)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);

            XNamespace w = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
            var body = new XElement(w + "body");

            if (!string.IsNullOrWhiteSpace(title))
            {
                body.Add(
                    new XElement(w + "p",
                        new XElement(w + "pPr",
                            new XElement(w + "jc", new XAttribute(w + "val", "center"))),
                        new XElement(w + "r",
                            new XElement(w + "rPr",
                                new XElement(w + "b"),
                                new XElement(w + "sz", new XAttribute(w + "val", "28"))),
                            new XElement(w + "t", title))));
            }

            string[] lines = (content ?? string.Empty).Replace("\r\n", "\n").Split('\n');
            foreach (string line in lines)
            {
                bool isBold = line.StartsWith("CỘNG HÒA", StringComparison.Ordinal) ||
                              line.StartsWith("Độc lập", StringComparison.Ordinal) ||
                              line.StartsWith("I.", StringComparison.Ordinal) ||
                              line.StartsWith("II.", StringComparison.Ordinal) ||
                              line.StartsWith("III.", StringComparison.Ordinal) ||
                              line.StartsWith("IV.", StringComparison.Ordinal);

                var run = new XElement(w + "r");
                if (isBold)
                    run.Add(new XElement(w + "rPr", new XElement(w + "b")));
                run.Add(new XElement(w + "t",
                    new XAttribute(XNamespace.Xml + "space", "preserve"),
                    line));
                body.Add(new XElement(w + "p", run));
            }

            body.Add(
                new XElement(w + "sectPr",
                    new XElement(w + "pgSz",
                        new XAttribute(w + "w", "11906"),
                        new XAttribute(w + "h", "16838")),
                    new XElement(w + "pgMar",
                        new XAttribute(w + "top", "1134"),
                        new XAttribute(w + "right", "1134"),
                        new XAttribute(w + "bottom", "1134"),
                        new XAttribute(w + "left", "1418"))));

            var documentXml = new XDocument(
                new XDeclaration("1.0", "UTF-8", "yes"),
                new XElement(w + "document",
                    new XAttribute(XNamespace.Xmlns + "w", w.NamespaceName),
                    body));

            const string contentTypesXml = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>
<Types xmlns=""http://schemas.openxmlformats.org/package/2006/content-types"">
  <Default Extension=""rels"" ContentType=""application/vnd.openxmlformats-package.relationships+xml""/>
  <Default Extension=""xml"" ContentType=""application/xml""/>
  <Override PartName=""/word/document.xml"" ContentType=""application/vnd.openxmlformats-officedocument.wordprocessingml.document.main+xml""/>
</Types>";

            const string relsXml = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>
<Relationships xmlns=""http://schemas.openxmlformats.org/package/2006/relationships"">
  <Relationship Id=""rId1"" Type=""http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument"" Target=""word/document.xml""/>
</Relationships>";

            using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            using (var archive = new ZipArchive(stream, ZipArchiveMode.Create))
            {
                WriteZipEntry(archive, "[Content_Types].xml", contentTypesXml);
                WriteZipEntry(archive, "_rels/.rels", relsXml);
                WriteZipEntry(archive, "word/document.xml", documentXml.ToString(SaveOptions.DisableFormatting));
            }
        }

        private static void WriteZipEntry(ZipArchive archive, string entryName, string content)
        {
            var entry = archive.CreateEntry(entryName, CompressionLevel.Optimal);
            using (var writer = new StreamWriter(entry.Open(), new UTF8Encoding(false)))
            {
                writer.Write(content);
            }
        }
    }
}
