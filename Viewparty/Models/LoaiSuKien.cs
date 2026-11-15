using System;

namespace Viewparty.Models
{
    // Maps directly to dbo.LOAI_SU_KIEN in 01_ViewParty_CauTruc_16Bang (1).sql.
    public sealed class LoaiSuKien
    {
        public long ma_loai_su_kien { get; set; }
        public string ten_loai { get; set; }
        public string mo_ta { get; set; }
        public bool trang_thai { get; set; }
    }
}
