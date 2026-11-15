using System;

namespace Viewparty.Models
{
    // Maps directly to dbo.NGOAI_LE in 01_ViewParty_CauTruc_16Bang (1).sql.
    public sealed class NgoaiLe
    {
        public long ma_ngoai_le { get; set; }
        public long ma_ho_so { get; set; }
        public long ma_ket_qua { get; set; }
        public long nguoi_de_xuat_id { get; set; }
        public long? nguoi_phe_duyet_id { get; set; }
        public string ly_do { get; set; }
        public string minh_chung { get; set; }
        public string bien_phap_bu_dap { get; set; }
        public short cap_tham_quyen_yeu_cau { get; set; }
        public string trang_thai { get; set; }
        public DateTimeOffset ngay_de_xuat { get; set; }
        public DateTimeOffset? ngay_quyet_dinh { get; set; }
        public string ghi_chu_quyet_dinh { get; set; }
    }
}
