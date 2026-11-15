using System;

namespace Viewparty.Models
{
    // Maps directly to dbo.THONG_BAO in 01_ViewParty_CauTruc_16Bang (1).sql.
    public sealed class ThongBao
    {
        public long ma_thong_bao { get; set; }
        public long nguoi_nhan_id { get; set; }
        public long? ma_ho_so { get; set; }
        public string loai_thong_bao { get; set; }
        public string trang_thai { get; set; }
        public string noi_dung { get; set; }
        public DateTimeOffset thoi_diem_tao { get; set; }
        public DateTimeOffset? thoi_diem_gui { get; set; }
        public int so_lan_thu { get; set; }
    }
}
