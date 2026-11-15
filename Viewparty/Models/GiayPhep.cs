using System;

namespace Viewparty.Models
{
    // Maps directly to dbo.GIAY_PHEP in 01_ViewParty_CauTruc_16Bang (1).sql.
    public sealed class GiayPhep
    {
        public long ma_giay_phep { get; set; }
        public long ma_ho_so { get; set; }
        public long nguoi_phe_duyet_id { get; set; }
        public string so_giay_phep { get; set; }
        public DateTime ngay_cap { get; set; }
        public DateTime hieu_luc_tu { get; set; }
        public DateTime hieu_luc_den { get; set; }
        public string dieu_kien_kem_theo { get; set; }
    }
}
