namespace Viewparty.Models
{
    public sealed class HoSoNopResult
    {
        public bool ThanhCong { get; private set; }
        public int? MaLoi { get; private set; }
        public string LyDo { get; private set; }
        public long MaHoSo { get; private set; }
        public string TrangThai { get; private set; }

        public HoSoNopResult(bool thanhCong, int? maLoi, string lyDo, long maHoSo, string trangThai)
        {
            ThanhCong = thanhCong;
            MaLoi = maLoi;
            LyDo = lyDo;
            MaHoSo = maHoSo;
            TrangThai = trangThai;
        }
    }
}
