using System;

namespace Viewparty.Models
{
    // Maps directly to dbo.KET_QUA_KIEM_TRA in 01_ViewParty_CauTruc_16Bang (1).sql.
    public sealed class KetQuaKiemTra
    {
        public long ma_ket_qua { get; set; }
        public long ma_ho_so { get; set; }
        public long quy_tac_id { get; set; }
        public long nguoi_kiem_tra_id { get; set; }
        public string ket_qua { get; set; }
        public string gia_tri_thuc_te { get; set; }
        public string ghi_chu { get; set; }
        public DateTimeOffset ngay_danh_gia { get; set; }
    }
}
