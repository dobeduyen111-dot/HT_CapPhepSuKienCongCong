using System;

namespace Viewparty.Models
{
    // Maps directly to dbo.QUYEN_TRINH_CHIEU in 01_ViewParty_CauTruc_16Bang (1).sql.
    public sealed class QuyenTrinhChieu
    {
        public long ma_quyen { get; set; }
        public long ma_ho_so { get; set; }
        public string don_vi_cap { get; set; }
        public string so_van_ban { get; set; }
        public string loai_quyen { get; set; }
        public string nguon_tin_hieu { get; set; }
        public string noi_dung_duoc_cap { get; set; }
        public string pham_vi_dia_diem { get; set; }
        public DateTime hieu_luc_tu { get; set; }
        public DateTime hieu_luc_den { get; set; }
        public string tep_dinh_kem { get; set; }
        public bool trang_thai { get; set; }
    }
}
