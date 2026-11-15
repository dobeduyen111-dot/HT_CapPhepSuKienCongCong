using System;

namespace Viewparty.Models
{
    // Maps directly to dbo.QUY_TAC in 01_ViewParty_CauTruc_16Bang (1).sql.
    public sealed class QuyTac
    {
        public long quy_tac_id { get; set; }
        public string ma_quy_tac { get; set; }
        public int phien_ban { get; set; }
        public long ma_loai_su_kien { get; set; }
        public string nhom { get; set; }
        public string muc_do { get; set; }
        public string dieu_kien_ap_dung { get; set; }
        public string dieu_kien_dat { get; set; }
        public bool cho_phep_ngoai_le { get; set; }
        public short? cap_tham_quyen { get; set; }
        public DateTime hieu_luc_tu { get; set; }
        public DateTime? hieu_luc_den { get; set; }
        public bool trang_thai { get; set; }
    }
}
