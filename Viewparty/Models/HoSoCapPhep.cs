using System;

namespace Viewparty.Models
{
    // Maps directly to dbo.HO_SO_CAP_PHEP in 01_ViewParty_CauTruc_16Bang (1).sql.
    public sealed class HoSoCapPhep
    {
        public long ma_ho_so { get; set; }
        public long nguoi_to_chuc_id { get; set; }
        public long? nguoi_xu_ly_hien_tai_id { get; set; }
        public long ma_loai_su_kien { get; set; }
        public long? ma_dia_diem { get; set; }
        public string trang_thai { get; set; }
        public string ten_don_vi_to_chuc { get; set; }
        public DateTimeOffset ngay_tao { get; set; }
        public DateTimeOffset? ngay_nop { get; set; }
        public string ten_su_kien { get; set; }
        public DateTimeOffset? bat_dau { get; set; }
        public DateTimeOffset? ket_thuc { get; set; }
        public int? quy_mo { get; set; }
        public string noi_dung { get; set; }
        public string hinh_thuc_thuong_mai { get; set; }
        public bool co_tai_tro { get; set; }
        public string thong_tin_tai_tro { get; set; }
        public string loai_ve { get; set; }
        public decimal? gia_ve { get; set; }
        public int? so_luong_ve { get; set; }
        public string kenh_phat_hanh_ve { get; set; }
        public string cach_kiem_soat_ve { get; set; }
        public string nhan_su_y_te { get; set; }
        public string nhan_su_an_ninh { get; set; }
        public string phuong_an_thoat_hiem { get; set; }
        public string phuong_an_xu_ly_su_co { get; set; }
        public string ly_do_tu_choi { get; set; }

        // Joined display metadata from LOAI_SU_KIEN, DIA_DIEM, GIAY_PHEP, LICH_SU_XU_LY
        public string ten_loai { get; set; }
        public string ten_dia_diem { get; set; }
        public string dia_chi_dia_diem { get; set; }
        public int? suc_chua_dia_diem { get; set; }
        public string nguoi_to_chuc { get; set; }
        public string so_giay_phep { get; set; }
        public string ghi_chu_bo_sung { get; set; }
    }

    public sealed class HoSoTheoDoiDetail
    {
        public HoSoCapPhep HoSo { get; set; }
        public QuyenTrinhChieu QuyenTrinhChieu { get; set; }
        public System.Collections.Generic.IList<TaiLieu> TaiLieu { get; set; } =
            new System.Collections.Generic.List<TaiLieu>();
        public System.Collections.Generic.IList<LichSuXuLy> LichSu { get; set; } =
            new System.Collections.Generic.List<LichSuXuLy>();
    }
}
