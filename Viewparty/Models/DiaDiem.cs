using System;

namespace Viewparty.Models
{
    // Maps directly to dbo.DIA_DIEM in 01_ViewParty_CauTruc_16Bang (1).sql.
    public sealed class DiaDiem
    {
        public long ma_dia_diem { get; set; }
        public string ten_dia_diem { get; set; }
        public string dia_chi { get; set; }
        public int suc_chua { get; set; }
        public bool trang_thai { get; set; }
        public string canh_bao_lich { get; set; }
        public bool co_canh_bao_trung_lich { get; set; }
    }
}
