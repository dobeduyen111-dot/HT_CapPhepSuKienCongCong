using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Viewparty.Models;

namespace Viewparty.Data
{
    internal sealed class InteragencyWorkflowRepository
    {
        public IList<InteragencyCaseItem> GetAssignedCases(long caseOfficerId)
        {
            try
            {
                using (var connection = Db.OpenConnection())
                using (var command = new SqlCommand("dbo.sp_DanhSachHoSoXinYKien", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@NguoiXuLyId", SqlDbType.BigInt).Value = caseOfficerId;
                    using (var reader = command.ExecuteReader())
                    {
                        var items = new List<InteragencyCaseItem>();
                        while (reader.Read())
                        {
                            items.Add(new InteragencyCaseItem
                            {
                                CaseId = reader.GetInt64(reader.GetOrdinal("ma_ho_so")),
                                EventName = ReadString(reader, "ten_su_kien"),
                                EventType = ReadString(reader, "ten_loai"),
                                Status = ReadString(reader, "trang_thai"),
                                RequiredUnits = reader.GetInt32(reader.GetOrdinal("so_don_vi_bat_buoc")),
                                RespondedRequiredUnits = reader.GetInt32(
                                    reader.GetOrdinal("so_don_vi_da_phan_hoi"))
                            });
                        }
                        return items;
                    }
                }
            }
            catch (SqlException ex)
            {
                throw ToWorkflowException(ex, "tải hồ sơ xin ý kiến");
            }
        }

        public IList<InteragencyRequestItem> GetUnitRequests(long accountId)
        {
            try
            {
                using (var connection = Db.OpenConnection())
                using (var command = new SqlCommand("dbo.sp_DanhSachYeuCauLienNganh", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@NguoiPhanHoiId", SqlDbType.BigInt).Value = accountId;
                    using (var reader = command.ExecuteReader())
                    {
                        var items = new List<InteragencyRequestItem>();
                        while (reader.Read())
                        {
                            items.Add(new InteragencyRequestItem
                            {
                                RequestId = reader.GetInt64(reader.GetOrdinal("ma_yeu_cau")),
                                CaseId = reader.GetInt64(reader.GetOrdinal("ma_ho_so")),
                                EventName = ReadString(reader, "ten_su_kien"),
                                UnitName = ReadString(reader, "ten_don_vi"),
                                Field = ReadString(reader, "linh_vuc"),
                                Deadline = reader.GetDateTimeOffset(reader.GetOrdinal("han_phan_hoi")),
                                Status = ReadString(reader, "trang_thai"),
                                Conclusion = ReadString(reader, "ket_luan"),
                                Response = ReadString(reader, "noi_dung_phan_hoi"),
                                Recommendation = ReadString(reader, "kien_nghi")
                            });
                        }
                        return items;
                    }
                }
            }
            catch (SqlException ex)
            {
                throw ToWorkflowException(ex, "tải yêu cầu liên ngành");
            }
        }

        public int CreateRequiredRequests(long caseId, long caseOfficerId)
        {
            try
            {
                using (var connection = Db.OpenConnection())
                using (var command = new SqlCommand("dbo.sp_GuiYeuCauLienNganh", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@MaHoSo", SqlDbType.BigInt).Value = caseId;
                    command.Parameters.Add("@NguoiXuLyId", SqlDbType.BigInt).Value = caseOfficerId;
                    using (var reader = command.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        if (!reader.Read())
                            throw new InteragencyWorkflowException(
                                "SQL Server không xác nhận kết quả gửi yêu cầu liên ngành.");
                        return reader.GetInt32(reader.GetOrdinal("so_yeu_cau_moi"));
                    }
                }
            }
            catch (SqlException ex)
            {
                throw ToWorkflowException(ex, "gửi yêu cầu liên ngành");
            }
        }

        public void SubmitOpinion(
            long requestId, long accountId, string conclusion, string response, string recommendation)
        {
            try
            {
                using (var connection = Db.OpenConnection())
                using (var command = new SqlCommand("dbo.sp_PhanHoiLienNganh", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@MaYeuCau", SqlDbType.BigInt).Value = requestId;
                    command.Parameters.Add("@NguoiPhanHoiId", SqlDbType.BigInt).Value = accountId;
                    command.Parameters.Add("@KetLuan", SqlDbType.VarChar, 30).Value = conclusion;
                    command.Parameters.Add("@NoiDung", SqlDbType.NVarChar, -1).Value = response;
                    command.Parameters.Add("@KienNghi", SqlDbType.NVarChar, -1).Value =
                        string.IsNullOrWhiteSpace(recommendation) ? (object)DBNull.Value : recommendation.Trim();
                    command.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw ToWorkflowException(ex, "gửi phản hồi liên ngành");
            }
        }

        public InteragencySummaryResult Summarize(long caseId, long caseOfficerId)
        {
            try
            {
                using (var connection = Db.OpenConnection())
                using (var command = new SqlCommand("dbo.sp_TongHopThamDinh", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@MaHoSo", SqlDbType.BigInt).Value = caseId;
                    command.Parameters.Add("@NguoiXuLyId", SqlDbType.BigInt).Value = caseOfficerId;
                    using (var reader = command.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        if (!reader.Read())
                            throw new InteragencyWorkflowException(
                                "SQL Server không trả về kết quả tổng hợp thẩm định.");
                        return new InteragencySummaryResult
                        {
                            CaseId = reader.GetInt64(reader.GetOrdinal("ma_ho_so")),
                            RuleCount = reader.GetInt32(reader.GetOrdinal("so_quy_tac")),
                            MandatoryFailures = reader.GetInt32(
                                reader.GetOrdinal("so_khong_dat_bat_buoc")),
                            NonExemptMandatoryFailures = reader.GetInt32(
                                reader.GetOrdinal("so_khong_duoc_ngoai_le")),
                            Warnings = reader.GetInt32(reader.GetOrdinal("so_canh_bao")),
                            ProposedConclusion = ReadString(reader, "ket_luan_de_xuat"),
                            NextStatus = ReadString(reader, "trang_thai_tiep_theo")
                        };
                    }
                }
            }
            catch (SqlException ex)
            {
                throw ToWorkflowException(ex, "tổng hợp thẩm định");
            }
        }

        private static InteragencyWorkflowException ToWorkflowException(SqlException exception, string operation)
        {
            foreach (SqlError error in exception.Errors)
            {
                switch (error.Number)
                {
                    case 52201:
                        return new InteragencyWorkflowException(
                            "Hồ sơ chưa được phân công cho bạn hoặc chưa ở trạng thái gửi ý kiến.");
                    case 52202:
                        return new InteragencyWorkflowException(
                            "Tài khoản Người xử lý không hoạt động hoặc không hợp lệ.");
                    case 52203:
                        return new InteragencyWorkflowException(
                            "Không có cấu hình liên ngành bắt buộc còn hiệu lực cho hồ sơ.");
                    case 52204:
                        return new InteragencyWorkflowException(
                            "Kết luận hoặc nội dung phản hồi không hợp lệ.");
                    case 52205:
                        return new InteragencyWorkflowException(
                            "Yêu cầu không thuộc đơn vị của tài khoản liên ngành hiện tại.");
                    case 52206:
                        return new InteragencyWorkflowException(
                            "Yêu cầu đã được phản hồi hoặc không còn chờ xử lý.");
                    case 52207:
                        return new InteragencyWorkflowException(
                            "Hồ sơ chưa được phân công cho bạn hoặc chưa ở trạng thái tổng hợp.");
                    case 52208:
                        return new InteragencyWorkflowException(
                            "Chưa nhận đủ ý kiến liên ngành bắt buộc. Hồ sơ chưa được tổng hợp.");
                    case 52209:
                        return new InteragencyWorkflowException(
                            "Chưa có kết quả kiểm tra quy tắc để tổng hợp.");
                    case 52211:
                        return new InteragencyWorkflowException(
                            "Chưa hoàn tất đánh giá tất cả quy tắc bắt buộc.");
                    case 52212:
                        return new InteragencyWorkflowException(
                            "Cấu hình yêu cầu đơn vị bắt buộc đang ngừng hoạt động hoặc chưa có tài khoản liên ngành hoạt động.");
                    case 208:
                    case 2812:
                        return new InteragencyWorkflowException(
                            "Database chưa được cập nhật UC13–UC15. Hãy chạy lại File 01 trước File 03.", exception);
                    case 207:
                        return new InteragencyWorkflowException(
                            "Database thiếu cột UC13–UC15. Hãy chạy lại File 01 trước File 03.", exception);
                }
            }
            return new InteragencyWorkflowException(
                "Không thể " + operation + ". Kiểm tra schema, stored procedure và quyền truy cập database.",
                exception);
        }

        private static string ReadString(IDataRecord reader, string name)
        {
            int ordinal = reader.GetOrdinal(name);
            return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
        }
    }

    internal sealed class InteragencyWorkflowException : Exception
    {
        public InteragencyWorkflowException(string message) : base(message) { }
        public InteragencyWorkflowException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
