using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Viewparty.Models;

namespace Viewparty.Data
{
    internal sealed class DiaDiemRepository
    {
        public IList<DiaDiem> GetAllActive()
        {
            try
            {
                using (var connection = Db.OpenConnection())
                using (var command = new SqlCommand(
                    "SELECT ma_dia_diem, ten_dia_diem, dia_chi, suc_chua, trang_thai FROM dbo.DIA_DIEM WHERE trang_thai = 1 ORDER BY ten_dia_diem;",
                    connection))
                {
                    var items = new List<DiaDiem>();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            items.Add(new DiaDiem
                            {
                                ma_dia_diem = reader.GetInt64(0),
                                ten_dia_diem = reader.GetString(1),
                                dia_chi = reader.GetString(2),
                                suc_chua = reader.GetInt32(3),
                                trang_thai = reader.GetBoolean(4),
                                canh_bao_lich = "Chưa kiểm tra lịch",
                                co_canh_bao_trung_lich = false
                            });
                        }
                    }
                    return items;
                }
            }
            catch (SqlException ex)
            {
                throw ToRepositoryException(ex, "tải danh sách địa điểm");
            }
        }

        public IList<HoSoCapPhep> GetOrganizerDrafts(long organizerId)
        {
            try
            {
                using (var connection = Db.OpenConnection())
                using (var command = new SqlCommand("dbo.sp_LietKeHoSoNhapToChuc", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@NguoiToChucId", SqlDbType.BigInt).Value = organizerId;
                    var drafts = new List<HoSoCapPhep>();
                    using (var reader = command.ExecuteReader())
                    {
                        int id = reader.GetOrdinal("ma_ho_so");
                        int name = reader.GetOrdinal("ten_su_kien");
                        int start = reader.GetOrdinal("bat_dau");
                        int end = reader.GetOrdinal("ket_thuc");
                        int size = reader.GetOrdinal("quy_mo");
                        while (reader.Read())
                        {
                            drafts.Add(new HoSoCapPhep
                            {
                                ma_ho_so = reader.GetInt64(id),
                                ten_su_kien = reader.IsDBNull(name) ? null : reader.GetString(name),
                                bat_dau = reader.IsDBNull(start) ? (DateTimeOffset?)null : reader.GetDateTimeOffset(start),
                                ket_thuc = reader.IsDBNull(end) ? (DateTimeOffset?)null : reader.GetDateTimeOffset(end),
                                quy_mo = reader.IsDBNull(size) ? (int?)null : reader.GetInt32(size)
                            });
                        }
                    }
                    return drafts;
                }
            }
            catch (SqlException ex)
            {
                throw ToRepositoryException(ex, "tra cứu hồ sơ Nháp");
            }
        }

        public IList<DiaDiem> Search(string keyword, DateTimeOffset start, DateTimeOffset end, int audienceSize)
        {
            try
            {
                using (var connection = Db.OpenConnection())
                using (var command = new SqlCommand("dbo.sp_TraCuuDiaDiem", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@TuKhoa", SqlDbType.NVarChar, 100).Value =
                        string.IsNullOrWhiteSpace(keyword) ? (object)DBNull.Value : keyword.Trim();
                    command.Parameters.Add("@QuyMo", SqlDbType.Int).Value = audienceSize;
                    command.Parameters.Add("@BatDau", SqlDbType.DateTimeOffset).Value = start;
                    command.Parameters.Add("@KetThuc", SqlDbType.DateTimeOffset).Value = end;

                    var results = new List<DiaDiem>();
                    using (var reader = command.ExecuteReader())
                    {
                        int id = reader.GetOrdinal("ma_dia_diem");
                        int name = reader.GetOrdinal("ten_dia_diem");
                        int address = reader.GetOrdinal("dia_chi");
                        int capacity = reader.GetOrdinal("suc_chua");
                        int status = reader.GetOrdinal("trang_thai");
                        int warning = reader.GetOrdinal("canh_bao_lich");

                        while (reader.Read())
                        {
                            string scheduleWarning = reader.GetString(warning);
                            results.Add(new DiaDiem
                            {
                                ma_dia_diem = reader.GetInt64(id),
                                ten_dia_diem = reader.GetString(name),
                                dia_chi = reader.GetString(address),
                                suc_chua = reader.GetInt32(capacity),
                                trang_thai = reader.GetBoolean(status),
                                canh_bao_lich = FormatScheduleWarning(scheduleWarning),
                                co_canh_bao_trung_lich = string.Equals(
                                    scheduleWarning, "CANH_BAO_TRUNG_LICH", StringComparison.Ordinal)
                            });
                        }
                    }

                    return results;
                }
            }
            catch (SqlException ex)
            {
                throw ToRepositoryException(ex, "tra cứu địa điểm");
            }
        }

        public void AssignToDraft(long draftId, long organizerId, long venueId)
        {
            try
            {
                using (var connection = Db.OpenConnection())
                using (var command = new SqlCommand("dbo.sp_ChonDiaDiemNhap", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@MaHoSo", SqlDbType.BigInt).Value = draftId;
                    command.Parameters.Add("@NguoiToChucId", SqlDbType.BigInt).Value = organizerId;
                    command.Parameters.Add("@MaDiaDiem", SqlDbType.BigInt).Value = venueId;

                    using (var reader = command.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        if (!reader.Read())
                            throw new DiaDiemRepositoryException(
                                "SQL Server không xác nhận địa điểm đã được gán vào hồ sơ nháp.");
                    }
                }
            }
            catch (SqlException ex)
            {
                throw ToRepositoryException(ex, "chọn địa điểm");
            }
        }

        private static DiaDiemRepositoryException ToRepositoryException(SqlException exception, string operation)
        {
            foreach (SqlError error in exception.Errors)
            {
                switch (error.Number)
                {
                    case 52011:
                        return new DiaDiemRepositoryException(
                            "Tài khoản Người tổ chức không hoạt động hoặc không hợp lệ.");
                    case 52014:
                        return new DiaDiemRepositoryException(
                            "Địa điểm không tồn tại hoặc đã ngừng hoạt động. Hãy tra cứu lại.");
                    case 52015:
                        return new DiaDiemRepositoryException(
                            "Không thể chọn địa điểm: hồ sơ không thuộc tài khoản hiện tại hoặc không còn ở trạng thái Nháp.");
                    case 52025:
                        return new DiaDiemRepositoryException(
                            "Địa điểm không đủ sức chứa theo quy mô đã lưu trong hồ sơ nháp.");
                    case 52027:
                        return new DiaDiemRepositoryException(
                            "Hãy lưu quy mô khách dự kiến vào hồ sơ nháp trước khi chọn địa điểm.");
                    case 52028:
                        return new DiaDiemRepositoryException(
                            "Tài khoản Người tổ chức không hoạt động nên không thể gán địa điểm.");
                    case 208:
                    case 2812:
                        return new DiaDiemRepositoryException(
                            "Database chưa có stored procedure cần thiết cho UC28. Hãy áp dụng bản cập nhật SQL.", exception);
                }
            }

            return new DiaDiemRepositoryException(
                "Không thể " + operation + ". Kiểm tra schema, stored procedure và quyền truy cập database.", exception);
        }

        private static string FormatScheduleWarning(string code)
        {
            switch (code)
            {
                case "CANH_BAO_TRUNG_LICH":
                    return "Có thể trùng lịch - tham khảo";
                case "CHUA_TRUNG_LICH":
                    return "Chưa phát hiện trùng lịch";
                default:
                    return "Chưa kiểm tra lịch";
            }
        }
    }

    internal sealed class DiaDiemRepositoryException : Exception
    {
        public DiaDiemRepositoryException(string message) : base(message) { }
        public DiaDiemRepositoryException(string message, Exception innerException) : base(message, innerException) { }
    }
}
