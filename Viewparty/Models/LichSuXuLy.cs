using System;

namespace Viewparty.Models
{
    // Maps directly to dbo.LICH_SU_XU_LY in 01_ViewParty_CauTruc_16Bang (1).sql.
    public sealed class LichSuXuLy
    {
        public long ma_lich_su { get; set; }
        public long ma_ho_so { get; set; }
        public long? ma_tai_khoan { get; set; }
        public string hanh_dong { get; set; }
        public string tu_trang_thai { get; set; }
        public string den_trang_thai { get; set; }
        public string ket_qua { get; set; }
        public string ghi_chu { get; set; }
        public DateTimeOffset thoi_diem { get; set; }
    }
}
