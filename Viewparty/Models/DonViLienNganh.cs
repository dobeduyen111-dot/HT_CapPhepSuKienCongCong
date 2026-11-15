using System;

namespace Viewparty.Models
{
    // Maps directly to dbo.DON_VI_LIEN_NGANH in 01_ViewParty_CauTruc_16Bang (1).sql.
    public sealed class DonViLienNganh
    {
        public long ma_don_vi { get; set; }
        public string ten_don_vi { get; set; }
        public string linh_vuc { get; set; }
        public bool trang_thai { get; set; }
    }
}
