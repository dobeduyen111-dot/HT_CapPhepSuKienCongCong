using System;
using System.Collections.Generic;

namespace Viewparty.Models
{
    public sealed class HoSoDraftData
    {
        public long? ma_ho_so { get; set; }
        public long ma_loai_su_kien { get; set; }
        public long? ma_dia_diem { get; set; }
        public string ten_don_vi_to_chuc { get; set; }
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
        public QuyenTrinhChieu quyen_trinh_chieu { get; set; }
        public string existing_projection_attachment_path { get; set; }
        public IList<TaiLieu> tai_lieu { get; set; }
        public IList<TaiLieu> existing_documents_snapshot { get; set; }

        public HoSoDraftData()
        {
            tai_lieu = new List<TaiLieu>();
        }
    }

    public sealed class HoSoFileUpload
    {
        public string LoaiTaiLieu { get; set; }
        public string SourcePath { get; set; }

        public HoSoFileUpload(string loaiTaiLieu, string sourcePath)
        {
            LoaiTaiLieu = loaiTaiLieu;
            SourcePath = sourcePath;
        }
    }
}
