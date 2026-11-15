using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Viewparty.Models;

namespace Viewparty.Data
{
    internal sealed class NgoaiLeWorkflowRepository
    {
        public IList<NgoaiLeEligibleResult> GetEligibleResults(long caseId, long caseOfficerId)
        {
            try
            {
                using (var connection = Db.OpenConnection())
                using (var command = new SqlCommand("dbo.sp_DanhSachKetQuaDuocDeXuatNgoaiLe", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@MaHoSo", SqlDbType.BigInt).Value = caseId;
                    command.Parameters.Add("@NguoiXuLyId", SqlDbType.BigInt).Value = caseOfficerId;
                    using (var reader = command.ExecuteReader())
                    {
                        var results = new List<NgoaiLeEligibleResult>();
                        while (reader.Read())
                        {
                            results.Add(new NgoaiLeEligibleResult
                            {
                                ResultId = reader.GetInt64(reader.GetOrdinal("ma_ket_qua")),
                                RuleCode = reader.GetString(reader.GetOrdinal("ma_quy_tac")),
                                Version = reader.GetInt32(reader.GetOrdinal("phien_ban")),
                                Group = reader.GetString(reader.GetOrdinal("nhom")),
                                ApprovalLevel = reader.GetInt16(reader.GetOrdinal("cap_tham_quyen")),
                                Note = ReadString(reader, "ghi_chu")
                            });
                        }
                        return results;
                    }
                }
            }
            catch (SqlException ex) { throw ToWorkflowException(ex, "tải kết quả đủ điều kiện ngoại lệ"); }
        }

        public long Propose(long caseId, long resultId, long caseOfficerId,
            string reason, string evidence, string mitigation)
        {
            try
            {
                using (var connection = Db.OpenConnection())
                using (var command = new SqlCommand("dbo.sp_DeXuatNgoaiLe", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@MaHoSo", SqlDbType.BigInt).Value = caseId;
                    command.Parameters.Add("@MaKetQua", SqlDbType.BigInt).Value = resultId;
                    command.Parameters.Add("@NguoiXuLyId", SqlDbType.BigInt).Value = caseOfficerId;
                    command.Parameters.Add("@LyDo", SqlDbType.NVarChar, -1).Value = reason;
                    command.Parameters.Add("@MinhChung", SqlDbType.NVarChar, -1).Value = evidence;
                    command.Parameters.Add("@BienPhapBuDap", SqlDbType.NVarChar, -1).Value = mitigation;
                    object value = command.ExecuteScalar();
                    if (value == null || value == DBNull.Value)
                        throw new NgoaiLeWorkflowException(
                            "SQL Server không xác nhận đã tạo đề xuất ngoại lệ.");
                    return Convert.ToInt64(value);
                }
            }
            catch (SqlException ex) { throw ToWorkflowException(ex, "gửi đề xuất ngoại lệ"); }
        }

        public IList<NgoaiLeDecisionItem> GetPendingDecisions(long approverId)
        {
            try
            {
                using (var connection = Db.OpenConnection())
                using (var command = new SqlCommand("dbo.sp_DanhSachNgoaiLeChoPheDuyet", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@NguoiPheDuyetId", SqlDbType.BigInt).Value = approverId;
                    using (var reader = command.ExecuteReader())
                    {
                        var items = new List<NgoaiLeDecisionItem>();
                        while (reader.Read())
                        {
                            items.Add(new NgoaiLeDecisionItem
                            {
                                ExceptionId = reader.GetInt64(reader.GetOrdinal("ma_ngoai_le")),
                                CaseId = reader.GetInt64(reader.GetOrdinal("ma_ho_so")),
                                ResultId = reader.GetInt64(reader.GetOrdinal("ma_ket_qua")),
                                ProposerId = reader.GetInt64(reader.GetOrdinal("nguoi_de_xuat_id")),
                                ProposerName = ReadString(reader, "ten_nguoi_de_xuat"),
                                EventName = ReadString(reader, "ten_su_kien"),
                                RuleCode = ReadString(reader, "ma_quy_tac"),
                                Version = reader.GetInt32(reader.GetOrdinal("phien_ban")),
                                Group = ReadString(reader, "nhom"),
                                AssessmentNote = ReadString(reader, "ghi_chu_tham_dinh"),
                                Reason = ReadString(reader, "ly_do"),
                                Evidence = ReadString(reader, "minh_chung"),
                                Mitigation = ReadString(reader, "bien_phap_bu_dap"),
                                RequiredApprovalLevel = reader.GetInt16(
                                    reader.GetOrdinal("cap_tham_quyen_yeu_cau")),
                                ProposedAt = reader.GetDateTimeOffset(reader.GetOrdinal("ngay_de_xuat"))
                            });
                        }
                        return items;
                    }
                }
            }
            catch (SqlException ex) { throw ToWorkflowException(ex, "tải đề xuất chờ duyệt"); }
        }

        public void Decide(long exceptionId, long approverId, bool approve, string note)
        {
            try
            {
                using (var connection = Db.OpenConnection())
                using (var command = new SqlCommand("dbo.sp_QuyetDinhNgoaiLe", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@MaNgoaiLe", SqlDbType.BigInt).Value = exceptionId;
                    command.Parameters.Add("@NguoiPheDuyetId", SqlDbType.BigInt).Value = approverId;
                    command.Parameters.Add("@ChapThuan", SqlDbType.Bit).Value = approve;
                    command.Parameters.Add("@GhiChu", SqlDbType.NVarChar, -1).Value = note;
                    command.ExecuteNonQuery();
                }
            }
            catch (SqlException ex) { throw ToWorkflowException(ex, "lưu quyết định ngoại lệ"); }
        }

        private static NgoaiLeWorkflowException ToWorkflowException(SqlException exception, string operation)
        {
            foreach (SqlError error in exception.Errors)
            {
                switch (error.Number)
                {
                    case 52301: return new NgoaiLeWorkflowException(
                        "Cần nhập đủ lý do, minh chứng và biện pháp bù đắp.");
                    case 52302: return new NgoaiLeWorkflowException(
                        "Chưa nhận đủ ý kiến liên ngành bắt buộc.");
                    case 52303: return new NgoaiLeWorkflowException(
                        "Kết quả không đủ điều kiện đề xuất hoặc xét ngoại lệ.");
                    case 52304: return new NgoaiLeWorkflowException(
                        "Đề xuất ngoại lệ không còn ở trạng thái chờ duyệt.");
                    case 52305: return new NgoaiLeWorkflowException(
                        "Bạn không đủ cấp thẩm quyền hoặc là người đã đề xuất ngoại lệ.");
                    case 52315: return new NgoaiLeWorkflowException(
                        "Cần chọn quyết định và nhập lý do/căn cứ.");
                    case 52316: return new NgoaiLeWorkflowException(
                        "Hồ sơ đã đổi trạng thái; hãy tải lại dữ liệu trước khi xử lý.");
                    case 208:
                    case 2812:
                        return new NgoaiLeWorkflowException(
                            "Database chưa được cập nhật UC27/UC18. Hãy chạy lại File 03.", exception);
                    case 207:
                        return new NgoaiLeWorkflowException(
                            "Database thiếu cột UC27/UC18. Hãy chạy lại File 01 trước File 03.", exception);
                }
            }
            return new NgoaiLeWorkflowException(
                "Không thể " + operation + ". Kiểm tra schema, stored procedure và quyền truy cập database.",
                exception);
        }

        private static string ReadString(IDataRecord reader, string name)
        {
            int ordinal = reader.GetOrdinal(name);
            return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
        }
    }

    internal sealed class NgoaiLeWorkflowException : Exception
    {
        public NgoaiLeWorkflowException(string message) : base(message) { }
        public NgoaiLeWorkflowException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
