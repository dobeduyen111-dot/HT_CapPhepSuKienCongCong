using System;

namespace Viewparty.Models
{
    // Maps directly to dbo.TAI_LIEU in 01_ViewParty_CauTruc_16Bang (1).sql.
    public sealed class TaiLieu
    {
        public long ma_tai_lieu { get; set; }
        public long ma_ho_so { get; set; }
        public string loai_tai_lieu { get; set; }
        public string ten_tai_lieu { get; set; }
        public string duong_dan { get; set; }
        public DateTimeOffset ngay_tai_len { get; set; }
    }
}
