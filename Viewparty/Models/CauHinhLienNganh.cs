using System;

namespace Viewparty.Models
{
    // Maps directly to dbo.CAU_HINH_LIEN_NGANH in 01_ViewParty_CauTruc_16Bang (1).sql.
    public sealed class CauHinhLienNganh
    {
        public long ma_cau_hinh { get; set; }
        public long ma_loai_su_kien { get; set; }
        public long ma_don_vi { get; set; }
        public long? quy_tac_id { get; set; }
        public bool bat_buoc { get; set; }
        public int so_ngay_phan_hoi { get; set; }
        public DateTime hieu_luc_tu { get; set; }
        public DateTime? hieu_luc_den { get; set; }
        public bool trang_thai { get; set; }
    }
}
