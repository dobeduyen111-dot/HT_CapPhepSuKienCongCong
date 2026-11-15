using System;

namespace Viewparty.Models
{
    // Maps directly to dbo.YEU_CAU_Y_KIEN in 01_ViewParty_CauTruc_16Bang (1).sql.
    public sealed class YeuCauYKien
    {
        public long ma_yeu_cau { get; set; }
        public long ma_ho_so { get; set; }
        public long ma_don_vi { get; set; }
        public long ma_cau_hinh { get; set; }
        public long nguoi_gui_id { get; set; }
        public DateTimeOffset ngay_gui { get; set; }
        public DateTimeOffset han_phan_hoi { get; set; }
        public string trang_thai { get; set; }
        public long? nguoi_phan_hoi_id { get; set; }
        public string ket_luan { get; set; }
        public string noi_dung_phan_hoi { get; set; }
        public string kien_nghi { get; set; }
        public DateTimeOffset? ngay_phan_hoi { get; set; }
        public string tep_dinh_kem { get; set; }
    }
}
