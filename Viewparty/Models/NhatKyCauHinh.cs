using System;

namespace Viewparty.Models
{
    // Maps directly to dbo.NHAT_KY_CAU_HINH in 01_ViewParty_CauTruc_16Bang (1).sql.
    public sealed class NhatKyCauHinh
    {
        public long ma_nhat_ky { get; set; }
        public long ma_tai_khoan { get; set; }
        public string doi_tuong { get; set; }
        public string khoa_doi_tuong { get; set; }
        public string hanh_dong { get; set; }
        public string gia_tri_truoc { get; set; }
        public string gia_tri_sau { get; set; }
        public DateTimeOffset thoi_diem { get; set; }
    }
}
