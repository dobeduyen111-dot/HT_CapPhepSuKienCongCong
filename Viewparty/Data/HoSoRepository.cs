using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Viewparty.Models;

namespace Viewparty.Data
{
    internal sealed class HoSoRepository
    {
        public async Task<HoSoNopResult> SubmitAsync(long organizerId, long draftId, CancellationToken cancellationToken)
        {
            using (var connection = Db.CreateConnection())
            {
                try
                {
                    await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                    using (var command = new SqlCommand("dbo.sp_NopHoSo", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@MaHoSo", SqlDbType.BigInt).Value = draftId;
                        command.Parameters.Add("@NguoiToChucId", SqlDbType.BigInt).Value = organizerId;
                        using (var reader = await command.ExecuteReaderAsync(
                            CommandBehavior.SingleRow, cancellationToken).ConfigureAwait(false))
                        {
                            if (!await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                                throw new HoSoRepositoryException(
                                    "dbo.sp_NopHoSo không trả về kết quả xác nhận.");

                            return new HoSoNopResult(
                                reader.GetBoolean(reader.GetOrdinal("thanh_cong")),
                                reader.IsDBNull(reader.GetOrdinal("ma_loi"))
                                    ? (int?)null
                                    : reader.GetInt32(reader.GetOrdinal("ma_loi")),
                                ReadString(reader, "ly_do"),
                                reader.GetInt64(reader.GetOrdinal("ma_ho_so")),
                                ReadString(reader, "trang_thai"));
                        }
                    }
                }
                catch (SqlException ex) when (ex.Number == 1205)
                {
                    return new HoSoNopResult(false, 53018,
                        "Lịch vừa được cập nhật bởi một giao dịch khác. Hồ sơ vẫn ở trạng thái Nháp; hãy kiểm tra lại rồi thử nộp lại.",
                        draftId, "NHAP");
                }
                catch (SqlException ex)
                {
                    throw ToRepositoryException(ex, "nộp hồ sơ");
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
            }
        }

        public IList<LoaiSuKien> GetActiveEventTypes()
        {
            try
            {
                using (var connection = Db.OpenConnection())
                using (var command = new SqlCommand(
                    "SELECT ma_loai_su_kien,ten_loai,mo_ta,trang_thai FROM dbo.LOAI_SU_KIEN WHERE trang_thai=1 ORDER BY ten_loai;",
                    connection))
                using (var reader = command.ExecuteReader())
                {
                    var result = new List<LoaiSuKien>();
                    while (reader.Read())
                    {
                        result.Add(new LoaiSuKien
                        {
                            ma_loai_su_kien = reader.GetInt64(0),
                            ten_loai = reader.GetString(1),
                            mo_ta = reader.IsDBNull(2) ? null : reader.GetString(2),
                            trang_thai = reader.GetBoolean(3)
                        });
                    }
                    return result;
                }
            }
            catch (SqlException ex)
            {
                throw ToRepositoryException(ex, "tải danh mục loại sự kiện");
            }
        }

        public IList<HoSoCapPhep> GetDrafts(long organizerId)
        {
            try
            {
                using (var connection = Db.OpenConnection())
                using (var command = new SqlCommand("dbo.sp_LietKeHoSoNhapToChuc", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@NguoiToChucId", SqlDbType.BigInt).Value = organizerId;
                    using (var reader = command.ExecuteReader())
                    {
                        var result = new List<HoSoCapPhep>();
                        while (reader.Read())
                        {
                            result.Add(new HoSoCapPhep
                            {
                                ma_ho_so = reader.GetInt64(reader.GetOrdinal("ma_ho_so")),
                                ten_su_kien = ReadString(reader, "ten_su_kien"),
                                ten_don_vi_to_chuc = ReadString(reader, "ten_don_vi_to_chuc"),
                                bat_dau = ReadDateTimeOffset(reader, "bat_dau"),
                                ket_thuc = ReadDateTimeOffset(reader, "ket_thuc"),
                                quy_mo = ReadInt32(reader, "quy_mo")
                            });
                        }
                        return result;
                    }
                }
            }
            catch (SqlException ex)
            {
                throw ToRepositoryException(ex, "tra cứu hồ sơ Nháp");
            }
        }

        public HoSoDraftData GetDraft(long organizerId, long draftId)
        {
            try
            {
                using (var connection = Db.OpenConnection())
                using (var command = new SqlCommand("dbo.sp_LayHoSoNhapToChuc", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@NguoiToChucId", SqlDbType.BigInt).Value = organizerId;
                    command.Parameters.Add("@MaHoSo", SqlDbType.BigInt).Value = draftId;

                    HoSoDraftData draft;
                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                            throw new HoSoRepositoryException("Không tìm thấy hồ sơ Nháp thuộc tài khoản hiện tại.");

                        draft = new HoSoDraftData
                        {
                            ma_ho_so = ReadInt64(reader, "ma_ho_so"),
                            ma_loai_su_kien = ReadInt64(reader, "ma_loai_su_kien"),
                            ma_dia_diem = ReadNullableInt64(reader, "ma_dia_diem"),
                            ten_don_vi_to_chuc = ReadString(reader, "ten_don_vi_to_chuc"),
                            ten_su_kien = ReadString(reader, "ten_su_kien"),
                            bat_dau = ReadDateTimeOffset(reader, "bat_dau"),
                            ket_thuc = ReadDateTimeOffset(reader, "ket_thuc"),
                            quy_mo = ReadInt32(reader, "quy_mo"),
                            noi_dung = ReadString(reader, "noi_dung"),
                            hinh_thuc_thuong_mai = ReadString(reader, "hinh_thuc_thuong_mai"),
                            co_tai_tro = reader.GetBoolean(reader.GetOrdinal("co_tai_tro")),
                            thong_tin_tai_tro = ReadString(reader, "thong_tin_tai_tro"),
                            loai_ve = ReadString(reader, "loai_ve"),
                            gia_ve = ReadDecimal(reader, "gia_ve"),
                            so_luong_ve = ReadInt32(reader, "so_luong_ve"),
                            kenh_phat_hanh_ve = ReadString(reader, "kenh_phat_hanh_ve"),
                            cach_kiem_soat_ve = ReadString(reader, "cach_kiem_soat_ve"),
                            nhan_su_y_te = ReadString(reader, "nhan_su_y_te"),
                            nhan_su_an_ninh = ReadString(reader, "nhan_su_an_ninh"),
                            phuong_an_thoat_hiem = ReadString(reader, "phuong_an_thoat_hiem"),
                            phuong_an_xu_ly_su_co = ReadString(reader, "phuong_an_xu_ly_su_co")
                        };

                        if (reader.NextResult() && reader.Read())
                        {
                            draft.quyen_trinh_chieu = new QuyenTrinhChieu
                            {
                                don_vi_cap = ReadString(reader, "don_vi_cap"),
                                so_van_ban = ReadString(reader, "so_van_ban"),
                                loai_quyen = ReadString(reader, "loai_quyen"),
                                nguon_tin_hieu = ReadString(reader, "nguon_tin_hieu"),
                                noi_dung_duoc_cap = ReadString(reader, "noi_dung_duoc_cap"),
                                pham_vi_dia_diem = ReadString(reader, "pham_vi_dia_diem"),
                                hieu_luc_tu = reader.GetDateTime(reader.GetOrdinal("hieu_luc_tu")),
                                hieu_luc_den = reader.GetDateTime(reader.GetOrdinal("hieu_luc_den")),
                                tep_dinh_kem = ReadString(reader, "tep_dinh_kem")
                            };
                            draft.existing_projection_attachment_path = draft.quyen_trinh_chieu.tep_dinh_kem;
                        }

                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                draft.tai_lieu.Add(new TaiLieu
                                {
                                    ma_tai_lieu = reader.GetInt64(reader.GetOrdinal("ma_tai_lieu")),
                                    ma_ho_so = draft.ma_ho_so.Value,
                                    loai_tai_lieu = reader.GetString(reader.GetOrdinal("loai_tai_lieu")),
                                    ten_tai_lieu = reader.GetString(reader.GetOrdinal("ten_tai_lieu")),
                                    duong_dan = reader.GetString(reader.GetOrdinal("duong_dan"))
                                });
                            }
                        }
                    }
                    draft.existing_documents_snapshot = draft.tai_lieu.Select(item => new TaiLieu
                    {
                        ma_tai_lieu = item.ma_tai_lieu,
                        ma_ho_so = item.ma_ho_so,
                        loai_tai_lieu = item.loai_tai_lieu,
                        ten_tai_lieu = item.ten_tai_lieu,
                        duong_dan = item.duong_dan,
                        ngay_tai_len = item.ngay_tai_len
                    }).ToList();
                    return draft;
                }
            }
            catch (SqlException ex)
            {
                throw ToRepositoryException(ex, "tải nội dung hồ sơ Nháp");
            }
        }

        public long SaveDraft(long organizerId, HoSoDraftData draft, IList<TaiLieu> documents)
        {
            try
            {
                using (var connection = Db.OpenConnection())
                using (var command = new SqlCommand("dbo.sp_LuuHoSoNhapDayDu", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    Add(command, "@MaHoSo", SqlDbType.BigInt, (object)draft.ma_ho_so ?? DBNull.Value);
                    Add(command, "@NguoiToChucId", SqlDbType.BigInt, organizerId);
                    Add(command, "@MaLoaiSuKien", SqlDbType.BigInt, draft.ma_loai_su_kien);
                    Add(command, "@TenDonViToChuc", SqlDbType.NVarChar, 200, DbValue(draft.ten_don_vi_to_chuc));
                    Add(command, "@TenSuKien", SqlDbType.NVarChar, 200, DbValue(draft.ten_su_kien));
                    Add(command, "@BatDau", SqlDbType.DateTimeOffset, DbValue(draft.bat_dau));
                    Add(command, "@KetThuc", SqlDbType.DateTimeOffset, DbValue(draft.ket_thuc));
                    Add(command, "@QuyMo", SqlDbType.Int, DbValue(draft.quy_mo));
                    Add(command, "@NoiDung", SqlDbType.NVarChar, -1, DbValue(draft.noi_dung));
                    Add(command, "@HinhThucThuongMai", SqlDbType.VarChar, 20, DbValue(draft.hinh_thuc_thuong_mai));
                    Add(command, "@CoTaiTro", SqlDbType.Bit, draft.co_tai_tro);
                    Add(command, "@ThongTinTaiTro", SqlDbType.NVarChar, -1, DbValue(draft.thong_tin_tai_tro));
                    Add(command, "@LoaiVe", SqlDbType.VarChar, 20, DbValue(draft.loai_ve));
                    Add(command, "@GiaVe", SqlDbType.Decimal, DbValue(draft.gia_ve));
                    command.Parameters["@GiaVe"].Precision = 14;
                    command.Parameters["@GiaVe"].Scale = 2;
                    Add(command, "@SoLuongVe", SqlDbType.Int, DbValue(draft.so_luong_ve));
                    Add(command, "@KenhPhatHanhVe", SqlDbType.NVarChar, -1, DbValue(draft.kenh_phat_hanh_ve));
                    Add(command, "@CachKiemSoatVe", SqlDbType.NVarChar, -1, DbValue(draft.cach_kiem_soat_ve));
                    Add(command, "@NhanSuYTe", SqlDbType.NVarChar, -1, DbValue(draft.nhan_su_y_te));
                    Add(command, "@NhanSuAnNinh", SqlDbType.NVarChar, -1, DbValue(draft.nhan_su_an_ninh));
                    Add(command, "@PhuongAnThoatHiem", SqlDbType.NVarChar, -1, DbValue(draft.phuong_an_thoat_hiem));
                    Add(command, "@PhuongAnXuLySuCo", SqlDbType.NVarChar, -1, DbValue(draft.phuong_an_xu_ly_su_co));

                    var right = draft.quyen_trinh_chieu;
                    Add(command, "@DonViCap", SqlDbType.NVarChar, 200, DbValue(right == null ? null : right.don_vi_cap));
                    Add(command, "@SoVanBan", SqlDbType.NVarChar, 100, DbValue(right == null ? null : right.so_van_ban));
                    Add(command, "@LoaiQuyen", SqlDbType.NVarChar, 100, DbValue(right == null ? null : right.loai_quyen));
                    Add(command, "@NguonTinHieu", SqlDbType.NVarChar, 200, DbValue(right == null ? null : right.nguon_tin_hieu));
                    Add(command, "@NoiDungDuocCap", SqlDbType.NVarChar, -1, DbValue(right == null ? null : right.noi_dung_duoc_cap));
                    Add(command, "@PhamViDiaDiem", SqlDbType.NVarChar, -1, DbValue(right == null ? null : right.pham_vi_dia_diem));
                    Add(command, "@HieuLucTu", SqlDbType.Date, DbValue(right == null ? (DateTime?)null : right.hieu_luc_tu));
                    Add(command, "@HieuLucDen", SqlDbType.Date, DbValue(right == null ? (DateTime?)null : right.hieu_luc_den));
                    Add(command, "@TepQuyen", SqlDbType.NVarChar, 500, DbValue(right == null ? null : right.tep_dinh_kem));

                    var documentXml = new XElement("TaiLieu",
                        (documents ?? new List<TaiLieu>()).Select(item =>
                            new XElement("Item",
                                new XAttribute("Loai", item.loai_tai_lieu),
                                new XAttribute("Ten", item.ten_tai_lieu),
                                new XAttribute("DuongDan", item.duong_dan))));
                    Add(command, "@TaiLieuXml", SqlDbType.Xml, documentXml.ToString(SaveOptions.DisableFormatting));

                    using (var reader = command.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        if (!reader.Read())
                            throw new HoSoRepositoryException("SQL Server không trả về mã hồ sơ đã lưu.");
                        return reader.GetInt64(reader.GetOrdinal("ma_ho_so"));
                    }
                }
            }
            catch (SqlException ex)
            {
                throw ToRepositoryException(ex, "lưu hồ sơ Nháp");
            }
        }

        public IList<HoSoCapPhep> GetAllByOrganizer(long organizerId)
        {
            const string sql = @"
SELECT h.ma_ho_so, h.nguoi_to_chuc_id, h.nguoi_xu_ly_hien_tai_id, h.ma_loai_su_kien, h.ma_dia_diem,
       h.trang_thai, h.ten_don_vi_to_chuc, h.ngay_tao, h.ngay_nop, h.ten_su_kien,
       h.bat_dau, h.ket_thuc, h.quy_mo, h.noi_dung, h.hinh_thuc_thuong_mai,
       h.co_tai_tro, h.thong_tin_tai_tro, h.loai_ve, h.gia_ve, h.so_luong_ve,
       h.kenh_phat_hanh_ve, h.cach_kiem_soat_ve, h.nhan_su_y_te, h.nhan_su_an_ninh,
       h.phuong_an_thoat_hiem, h.phuong_an_xu_ly_su_co, h.ly_do_tu_choi,
       l.ten_loai, d.ten_dia_diem, d.dia_chi AS dia_chi_dia_diem, d.suc_chua AS suc_chua_dia_diem,
       t.ho_ten AS nguoi_to_chuc,
       gp.so_giay_phep,
       (SELECT TOP 1 ls.ghi_chu
        FROM dbo.LICH_SU_XU_LY ls
        WHERE ls.ma_ho_so = h.ma_ho_so AND ls.den_trang_thai = 'YEU_CAU_BO_SUNG'
        ORDER BY ls.thoi_diem DESC, ls.ma_lich_su DESC) AS ghi_chu_bo_sung
FROM dbo.HO_SO_CAP_PHEP h
JOIN dbo.LOAI_SU_KIEN l ON l.ma_loai_su_kien = h.ma_loai_su_kien
JOIN dbo.TAI_KHOAN t ON t.ma_tai_khoan = h.nguoi_to_chuc_id
LEFT JOIN dbo.DIA_DIEM d ON d.ma_dia_diem = h.ma_dia_diem
LEFT JOIN dbo.GIAY_PHEP gp ON gp.ma_ho_so = h.ma_ho_so
WHERE h.nguoi_to_chuc_id = @NguoiToChucId
ORDER BY h.ngay_tao DESC, h.ma_ho_so DESC;";
            try
            {
                using (var connection = Db.OpenConnection())
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@NguoiToChucId", SqlDbType.BigInt).Value = organizerId;
                    using (var reader = command.ExecuteReader())
                    {
                        var result = new List<HoSoCapPhep>();
                        while (reader.Read())
                        {
                            result.Add(ReadFullHoSo(reader));
                        }
                        return result;
                    }
                }
            }
            catch (SqlException ex)
            {
                throw ToRepositoryException(ex, "tải danh sách hồ sơ của Người tổ chức");
            }
        }

        public HoSoTheoDoiDetail GetTrackingDetail(long organizerId, long caseId)
        {
            try
            {
                var detail = new HoSoTheoDoiDetail();
                using (var connection = Db.OpenConnection())
                {
                    using (var command = new SqlCommand("dbo.sp_TheoDoiHoSo", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@MaHoSo", SqlDbType.BigInt).Value = caseId;
                        command.Parameters.Add("@NguoiToChucId", SqlDbType.BigInt).Value = organizerId;
                        using (var reader = command.ExecuteReader())
                        {
                            if (!reader.Read())
                                throw new HoSoRepositoryException("Không tìm thấy hồ sơ thuộc tài khoản hiện tại.");

                            if (reader.NextResult())
                            {
                                while (reader.Read())
                                {
                                    detail.LichSu.Add(new LichSuXuLy
                                    {
                                        ma_lich_su = ReadInt64(reader, "ma_lich_su"),
                                        ma_ho_so = caseId,
                                        hanh_dong = ReadString(reader, "hanh_dong"),
                                        tu_trang_thai = ReadString(reader, "tu_trang_thai"),
                                        den_trang_thai = ReadString(reader, "den_trang_thai"),
                                        ghi_chu = ReadString(reader, "ghi_chu"),
                                        thoi_diem = ReadDateTimeOffset(reader, "thoi_diem") ?? DateTimeOffset.Now
                                    });
                                }
                            }
                        }
                    }

                    const string detailSql = @"
SELECT h.ma_ho_so, h.nguoi_to_chuc_id, h.nguoi_xu_ly_hien_tai_id, h.ma_loai_su_kien, h.ma_dia_diem,
       h.trang_thai, h.ten_don_vi_to_chuc, h.ngay_tao, h.ngay_nop, h.ten_su_kien,
       h.bat_dau, h.ket_thuc, h.quy_mo, h.noi_dung, h.hinh_thuc_thuong_mai,
       h.co_tai_tro, h.thong_tin_tai_tro, h.loai_ve, h.gia_ve, h.so_luong_ve,
       h.kenh_phat_hanh_ve, h.cach_kiem_soat_ve, h.nhan_su_y_te, h.nhan_su_an_ninh,
       h.phuong_an_thoat_hiem, h.phuong_an_xu_ly_su_co, h.ly_do_tu_choi,
       l.ten_loai, d.ten_dia_diem, d.dia_chi AS dia_chi_dia_diem, d.suc_chua AS suc_chua_dia_diem,
       t.ho_ten AS nguoi_to_chuc,
       gp.so_giay_phep,
       (SELECT TOP 1 ls.ghi_chu
        FROM dbo.LICH_SU_XU_LY ls
        WHERE ls.ma_ho_so = h.ma_ho_so AND ls.den_trang_thai = 'YEU_CAU_BO_SUNG'
        ORDER BY ls.thoi_diem DESC, ls.ma_lich_su DESC) AS ghi_chu_bo_sung
FROM dbo.HO_SO_CAP_PHEP h
JOIN dbo.LOAI_SU_KIEN l ON l.ma_loai_su_kien = h.ma_loai_su_kien
JOIN dbo.TAI_KHOAN t ON t.ma_tai_khoan = h.nguoi_to_chuc_id
LEFT JOIN dbo.DIA_DIEM d ON d.ma_dia_diem = h.ma_dia_diem
LEFT JOIN dbo.GIAY_PHEP gp ON gp.ma_ho_so = h.ma_ho_so
WHERE h.ma_ho_so = @MaHoSo AND h.nguoi_to_chuc_id = @NguoiToChucId;

SELECT don_vi_cap, so_van_ban, loai_quyen, nguon_tin_hieu, noi_dung_duoc_cap,
       pham_vi_dia_diem, hieu_luc_tu, hieu_luc_den, tep_dinh_kem
FROM dbo.QUYEN_TRINH_CHIEU
WHERE ma_ho_so = @MaHoSo AND trang_thai = 1;

SELECT ma_tai_lieu, ma_ho_so, loai_tai_lieu, ten_tai_lieu, duong_dan, ngay_tai_len
FROM dbo.TAI_LIEU
WHERE ma_ho_so = @MaHoSo
ORDER BY ngay_tai_len DESC, ma_tai_lieu DESC;";

                    using (var command = new SqlCommand(detailSql, connection))
                    {
                        command.Parameters.Add("@MaHoSo", SqlDbType.BigInt).Value = caseId;
                        command.Parameters.Add("@NguoiToChucId", SqlDbType.BigInt).Value = organizerId;
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                                detail.HoSo = ReadFullHoSo(reader);

                            if (reader.NextResult() && reader.Read())
                            {
                                detail.QuyenTrinhChieu = new QuyenTrinhChieu
                                {
                                    don_vi_cap = ReadString(reader, "don_vi_cap"),
                                    so_van_ban = ReadString(reader, "so_van_ban"),
                                    loai_quyen = ReadString(reader, "loai_quyen"),
                                    nguon_tin_hieu = ReadString(reader, "nguon_tin_hieu"),
                                    noi_dung_duoc_cap = ReadString(reader, "noi_dung_duoc_cap"),
                                    pham_vi_dia_diem = ReadString(reader, "pham_vi_dia_diem"),
                                    hieu_luc_tu = reader.GetDateTime(reader.GetOrdinal("hieu_luc_tu")),
                                    hieu_luc_den = reader.GetDateTime(reader.GetOrdinal("hieu_luc_den")),
                                    tep_dinh_kem = ReadString(reader, "tep_dinh_kem")
                                };
                            }

                            if (reader.NextResult())
                            {
                                while (reader.Read())
                                {
                                    detail.TaiLieu.Add(new TaiLieu
                                    {
                                        ma_tai_lieu = ReadInt64(reader, "ma_tai_lieu"),
                                        ma_ho_so = ReadInt64(reader, "ma_ho_so"),
                                        loai_tai_lieu = ReadString(reader, "loai_tai_lieu"),
                                        ten_tai_lieu = ReadString(reader, "ten_tai_lieu"),
                                        duong_dan = ReadString(reader, "duong_dan"),
                                        ngay_tai_len = ReadDateTimeOffset(reader, "ngay_tai_len") ?? DateTimeOffset.Now
                                    });
                                }
                            }
                        }
                    }
                }
                return detail;
            }
            catch (SqlException ex)
            {
                throw ToRepositoryException(ex, "xem chi tiết và tiến độ hồ sơ");
            }
        }

        public string SupplementApplication(
            long organizerId,
            long caseId,
            string noiDung,
            string nhanSuYTe,
            string nhanSuAnNinh,
            string phuongAnThoatHiem,
            string phuongAnXuLySuCo,
            string ghiChuGiaiTrinh,
            IList<TaiLieu> newDocuments)
        {
            try
            {
                using (var connection = Db.OpenConnection())
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        using (var spCmd = new SqlCommand("dbo.sp_BoSungNoiDung", connection, transaction))
                        {
                            spCmd.CommandType = CommandType.StoredProcedure;
                            Add(spCmd, "@MaHoSo", SqlDbType.BigInt, caseId);
                            Add(spCmd, "@NguoiToChucId", SqlDbType.BigInt, organizerId);
                            Add(spCmd, "@NoiDung", SqlDbType.NVarChar, -1, DbValue(noiDung));
                            Add(spCmd, "@NhanSuYTe", SqlDbType.NVarChar, -1, DbValue(nhanSuYTe));
                            Add(spCmd, "@NhanSuAnNinh", SqlDbType.NVarChar, -1, DbValue(nhanSuAnNinh));
                            Add(spCmd, "@PhuongAnThoatHiem", SqlDbType.NVarChar, -1, DbValue(phuongAnThoatHiem));
                            Add(spCmd, "@PhuongAnXuLySuCo", SqlDbType.NVarChar, -1, DbValue(phuongAnXuLySuCo));
                            spCmd.ExecuteNonQuery();
                        }

                        if (newDocuments != null)
                        {
                            foreach (var doc in newDocuments)
                            {
                                if (doc == null) continue;
                                using (var docCmd = new SqlCommand(
                                    "INSERT dbo.TAI_LIEU(ma_ho_so, loai_tai_lieu, ten_tai_lieu, duong_dan) VALUES(@MaHoSo, @Loai, @Ten, @DuongDan);",
                                    connection, transaction))
                                {
                                    Add(docCmd, "@MaHoSo", SqlDbType.BigInt, caseId);
                                    Add(docCmd, "@Loai", SqlDbType.VarChar, 30, doc.loai_tai_lieu);
                                    Add(docCmd, "@Ten", SqlDbType.NVarChar, 250, doc.ten_tai_lieu);
                                    Add(docCmd, "@DuongDan", SqlDbType.NVarChar, 500, doc.duong_dan);
                                    docCmd.ExecuteNonQuery();
                                }
                            }
                        }

                        const string resumeSql = @"
DECLARE @DenTrangThai VARCHAR(30);
SELECT @DenTrangThai = CASE WHEN nguoi_xu_ly_hien_tai_id IS NOT NULL THEN 'DANG_KIEM_TRA' ELSE 'DA_NOP' END
FROM dbo.HO_SO_CAP_PHEP
WHERE ma_ho_so = @MaHoSo AND nguoi_to_chuc_id = @NguoiToChucId;

UPDATE dbo.HO_SO_CAP_PHEP
SET trang_thai = @DenTrangThai
WHERE ma_ho_so = @MaHoSo AND nguoi_to_chuc_id = @NguoiToChucId AND trang_thai = 'YEU_CAU_BO_SUNG';

INSERT dbo.LICH_SU_XU_LY(ma_ho_so, ma_tai_khoan, hanh_dong, tu_trang_thai, den_trang_thai, ghi_chu)
VALUES(@MaHoSo, @NguoiToChucId, N'Bổ sung và nộp lại hồ sơ', 'YEU_CAU_BO_SUNG', @DenTrangThai, @GhiChu);

SELECT @DenTrangThai AS trang_thai;";
                        string nextStatus;
                        using (var resumeCmd = new SqlCommand(resumeSql, connection, transaction))
                        {
                            Add(resumeCmd, "@MaHoSo", SqlDbType.BigInt, caseId);
                            Add(resumeCmd, "@NguoiToChucId", SqlDbType.BigInt, organizerId);
                            Add(resumeCmd, "@GhiChu", SqlDbType.NVarChar, -1, DbValue(ghiChuGiaiTrinh));
                            nextStatus = Convert.ToString(resumeCmd.ExecuteScalar()) ?? "DANG_KIEM_TRA";
                        }

                        transaction.Commit();
                        return nextStatus;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            catch (SqlException ex)
            {
                throw ToRepositoryException(ex, "bổ sung hồ sơ");
            }
        }

        public IList<ThongBao> GetNotifications(long accountId)
        {
            const string sql = @"
SELECT ma_thong_bao, nguoi_nhan_id, ma_ho_so, loai_thong_bao, trang_thai,
       noi_dung, thoi_diem_tao, thoi_diem_gui, so_lan_thu
FROM dbo.THONG_BAO
WHERE nguoi_nhan_id = @NguoiNhanId
ORDER BY thoi_diem_tao DESC, ma_thong_bao DESC;";
            try
            {
                using (var connection = Db.OpenConnection())
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@NguoiNhanId", SqlDbType.BigInt).Value = accountId;
                    using (var reader = command.ExecuteReader())
                    {
                        var list = new List<ThongBao>();
                        while (reader.Read())
                        {
                            list.Add(new ThongBao
                            {
                                ma_thong_bao = ReadInt64(reader, "ma_thong_bao"),
                                nguoi_nhan_id = ReadInt64(reader, "nguoi_nhan_id"),
                                ma_ho_so = ReadNullableInt64(reader, "ma_ho_so"),
                                loai_thong_bao = ReadString(reader, "loai_thong_bao"),
                                trang_thai = ReadString(reader, "trang_thai"),
                                noi_dung = ReadString(reader, "noi_dung"),
                                thoi_diem_tao = ReadDateTimeOffset(reader, "thoi_diem_tao") ?? DateTimeOffset.Now,
                                thoi_diem_gui = ReadDateTimeOffset(reader, "thoi_diem_gui"),
                                so_lan_thu = ReadInt32(reader, "so_lan_thu") ?? 0
                            });
                        }
                        return list;
                    }
                }
            }
            catch (SqlException ex)
            {
                throw ToRepositoryException(ex, "tải thông báo");
            }
        }

        private static HoSoCapPhep ReadFullHoSo(IDataRecord reader)
        {
            return new HoSoCapPhep
            {
                ma_ho_so = ReadInt64(reader, "ma_ho_so"),
                nguoi_to_chuc_id = ReadInt64(reader, "nguoi_to_chuc_id"),
                nguoi_xu_ly_hien_tai_id = ReadNullableInt64(reader, "nguoi_xu_ly_hien_tai_id"),
                ma_loai_su_kien = ReadInt64(reader, "ma_loai_su_kien"),
                ma_dia_diem = ReadNullableInt64(reader, "ma_dia_diem"),
                trang_thai = ReadString(reader, "trang_thai"),
                ten_don_vi_to_chuc = ReadString(reader, "ten_don_vi_to_chuc"),
                ngay_tao = ReadDateTimeOffset(reader, "ngay_tao") ?? DateTimeOffset.Now,
                ngay_nop = ReadDateTimeOffset(reader, "ngay_nop"),
                ten_su_kien = ReadString(reader, "ten_su_kien"),
                bat_dau = ReadDateTimeOffset(reader, "bat_dau"),
                ket_thuc = ReadDateTimeOffset(reader, "ket_thuc"),
                quy_mo = ReadInt32(reader, "quy_mo"),
                noi_dung = ReadString(reader, "noi_dung"),
                hinh_thuc_thuong_mai = ReadString(reader, "hinh_thuc_thuong_mai"),
                co_tai_tro = !reader.IsDBNull(reader.GetOrdinal("co_tai_tro")) && reader.GetBoolean(reader.GetOrdinal("co_tai_tro")),
                thong_tin_tai_tro = ReadString(reader, "thong_tin_tai_tro"),
                loai_ve = ReadString(reader, "loai_ve"),
                gia_ve = ReadDecimal(reader, "gia_ve"),
                so_luong_ve = ReadInt32(reader, "so_luong_ve"),
                kenh_phat_hanh_ve = ReadString(reader, "kenh_phat_hanh_ve"),
                cach_kiem_soat_ve = ReadString(reader, "cach_kiem_soat_ve"),
                nhan_su_y_te = ReadString(reader, "nhan_su_y_te"),
                nhan_su_an_ninh = ReadString(reader, "nhan_su_an_ninh"),
                phuong_an_thoat_hiem = ReadString(reader, "phuong_an_thoat_hiem"),
                phuong_an_xu_ly_su_co = ReadString(reader, "phuong_an_xu_ly_su_co"),
                ly_do_tu_choi = ReadString(reader, "ly_do_tu_choi"),
                ten_loai = ReadString(reader, "ten_loai"),
                ten_dia_diem = ReadString(reader, "ten_dia_diem"),
                dia_chi_dia_diem = ReadString(reader, "dia_chi_dia_diem"),
                suc_chua_dia_diem = ReadInt32(reader, "suc_chua_dia_diem"),
                nguoi_to_chuc = ReadString(reader, "nguoi_to_chuc"),
                so_giay_phep = ReadString(reader, "so_giay_phep"),
                ghi_chu_bo_sung = ReadString(reader, "ghi_chu_bo_sung")
            };
        }

        private static HoSoRepositoryException ToRepositoryException(SqlException exception, string operation)
        {
            foreach (SqlError error in exception.Errors)
            {
                switch (error.Number)
                {
                    case 52011:
                        return new HoSoRepositoryException("Tài khoản Người tổ chức không hoạt động hoặc không hợp lệ.");
                    case 52012:
                        return new HoSoRepositoryException("Loại sự kiện không tồn tại hoặc đã ngừng hoạt động.");
                    case 52013:
                        return new HoSoRepositoryException("Chỉ được sửa hồ sơ Nháp thuộc tài khoản hiện tại.");
                    case 52016:
                        return new HoSoRepositoryException("Thời gian kết thúc phải sau thời gian bắt đầu.");
                    case 52017:
                        return new HoSoRepositoryException("Thông tin quyền trình chiếu chưa đầy đủ hoặc thời hạn không hợp lệ.");
                    case 52027:
                        return new HoSoRepositoryException("Bạn không có quyền xem hồ sơ này.");
                    case 52106:
                        return new HoSoRepositoryException("Chỉ có thể bổ sung hồ sơ đang ở trạng thái YEU_CAU_BO_SUNG thuộc tài khoản của bạn.");
                    case 208:
                    case 2812:
                        return new HoSoRepositoryException(
                            "Database chưa được cập nhật cho UC03/UC04. Hãy áp dụng File 01 và File 03 mới nhất.", exception);
                    case 207:
                        return new HoSoRepositoryException(
                            "Database thiếu cột cần thiết cho UC03/UC04. Hãy áp dụng File 01 và File 03 mới nhất.", exception);
                }
            }
            return new HoSoRepositoryException("Không thể " + operation + ". Kiểm tra schema và stored procedure.", exception);
        }

        private static void Add(SqlCommand command, string name, SqlDbType type, object value)
        {
            command.Parameters.Add(name, type).Value = value ?? DBNull.Value;
        }

        private static void Add(SqlCommand command, string name, SqlDbType type, int size, object value)
        {
            command.Parameters.Add(name, type, size).Value = value ?? DBNull.Value;
        }

        private static object DbValue(object value) { return value ?? DBNull.Value; }
        private static string ReadString(IDataRecord reader, string name)
        {
            int ordinal = reader.GetOrdinal(name);
            return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
        }
        private static long ReadInt64(IDataRecord reader, string name) { return reader.GetInt64(reader.GetOrdinal(name)); }
        private static long? ReadNullableInt64(IDataRecord reader, string name)
        {
            int ordinal = reader.GetOrdinal(name);
            return reader.IsDBNull(ordinal) ? (long?)null : reader.GetInt64(ordinal);
        }
        private static int? ReadInt32(IDataRecord reader, string name)
        {
            int ordinal = reader.GetOrdinal(name);
            return reader.IsDBNull(ordinal) ? (int?)null : reader.GetInt32(ordinal);
        }
        private static decimal? ReadDecimal(IDataRecord reader, string name)
        {
            int ordinal = reader.GetOrdinal(name);
            return reader.IsDBNull(ordinal) ? (decimal?)null : reader.GetDecimal(ordinal);
        }
        private static DateTimeOffset? ReadDateTimeOffset(IDataRecord reader, string name)
        {
            int ordinal = reader.GetOrdinal(name);
            return reader.IsDBNull(ordinal) ? (DateTimeOffset?)null : (DateTimeOffset)reader.GetValue(ordinal);
        }
    }

    internal sealed class HoSoRepositoryException : Exception
    {
        public HoSoRepositoryException(string message) : base(message) { }
        public HoSoRepositoryException(string message, Exception innerException) : base(message, innerException) { }
    }
}
