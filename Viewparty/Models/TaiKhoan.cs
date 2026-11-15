using System;

namespace Viewparty.Models
{
    // Maps directly to dbo.TAI_KHOAN in 01_ViewParty_CauTruc_16Bang (1).sql.
    public sealed class TaiKhoan
    {
        public long ma_tai_khoan { get; set; }
        public long? ma_don_vi { get; set; }
        public string ten_dang_nhap { get; set; }
        public string mat_khau_bam { get; set; }
        public string ho_ten { get; set; }
        public string email { get; set; }
        public string so_dien_thoai { get; set; }
        public string vai_tro { get; set; }
        public short? cap_tham_quyen { get; set; }
        public string trang_thai { get; set; }
        public DateTimeOffset ngay_tao { get; set; }
    }
}
