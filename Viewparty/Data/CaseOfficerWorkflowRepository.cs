using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Viewparty.Models;

namespace Viewparty.Data
{
    internal sealed class CaseOfficerWorkflowRepository
    {
        public IList<CaseOfficerCaseItem> GetInbox(long officerId)
        {
            return GetCases(officerId, "h.trang_thai='DA_NOP'");
        }

        public IList<CaseOfficerCaseItem> GetAssigned(long officerId, string status)
        {
            return GetCases(officerId, "h.nguoi_xu_ly_hien_tai_id=@NguoiXuLyId AND h.trang_thai=@TrangThai", status);
        }

        private IList<CaseOfficerCaseItem> GetCases(long officerId, string predicate, string status = null)
        {
            try
            {
                using (var connection = Db.OpenConnection())
                using (var command = new SqlCommand(
                    "SELECT h.ma_ho_so,h.ten_su_kien,l.ten_loai,t.ho_ten,h.trang_thai,d.ten_dia_diem,h.ngay_nop,h.quy_mo " +
                    "FROM dbo.HO_SO_CAP_PHEP h " +
                    "JOIN dbo.TAI_KHOAN t ON t.ma_tai_khoan=h.nguoi_to_chuc_id " +
                    "JOIN dbo.LOAI_SU_KIEN l ON l.ma_loai_su_kien=h.ma_loai_su_kien " +
                    "LEFT JOIN dbo.DIA_DIEM d ON d.ma_dia_diem=h.ma_dia_diem " +
                    "WHERE " + predicate + " ORDER BY h.ngay_nop,h.ma_ho_so;", connection))
                {
                    command.Parameters.Add("@NguoiXuLyId", SqlDbType.BigInt).Value = officerId;
                    if (status != null) command.Parameters.Add("@TrangThai", SqlDbType.VarChar, 30).Value = status;
                    using (var reader = command.ExecuteReader())
                    {
                        var result = new List<CaseOfficerCaseItem>();
                        while (reader.Read()) result.Add(ReadCase(reader));
                        return result;
                    }
                }
            }
            catch (SqlException ex) { throw ToException(ex, "tải danh sách hồ sơ"); }
        }

        public CaseOfficerCaseItem GetCase(long caseId, long officerId)
        {
            try
            {
                using (var connection = Db.OpenConnection())
                using (var command = new SqlCommand(
                    "SELECT h.ma_ho_so,h.ten_su_kien,l.ten_loai,t.ho_ten,h.trang_thai,d.ten_dia_diem,h.ngay_nop,h.quy_mo " +
                    "FROM dbo.HO_SO_CAP_PHEP h JOIN dbo.TAI_KHOAN t ON t.ma_tai_khoan=h.nguoi_to_chuc_id " +
                    "JOIN dbo.LOAI_SU_KIEN l ON l.ma_loai_su_kien=h.ma_loai_su_kien " +
                    "LEFT JOIN dbo.DIA_DIEM d ON d.ma_dia_diem=h.ma_dia_diem " +
                    "WHERE h.ma_ho_so=@MaHoSo AND (h.trang_thai='DA_NOP' OR h.nguoi_xu_ly_hien_tai_id=@NguoiXuLyId);", connection))
                {
                    command.Parameters.Add("@MaHoSo", SqlDbType.BigInt).Value = caseId;
                    command.Parameters.Add("@NguoiXuLyId", SqlDbType.BigInt).Value = officerId;
                    using (var reader = command.ExecuteReader(CommandBehavior.SingleRow))
                        return reader.Read() ? ReadCase(reader) : null;
                }
            }
            catch (SqlException ex) { throw ToException(ex, "tải chi tiết hồ sơ"); }
        }

        public IList<CaseOfficerDocumentItem> GetDocuments(long caseId, long officerId)
        {
            try
            {
                using (var connection = Db.OpenConnection())
                using (var command = new SqlCommand(
                    "SELECT tl.ma_tai_lieu,tl.loai_tai_lieu,tl.ten_tai_lieu,tl.duong_dan " +
                    "FROM dbo.TAI_LIEU tl JOIN dbo.HO_SO_CAP_PHEP h ON h.ma_ho_so=tl.ma_ho_so " +
                    "WHERE tl.ma_ho_so=@MaHoSo AND (h.trang_thai='DA_NOP' OR h.nguoi_xu_ly_hien_tai_id=@NguoiXuLyId) " +
                    "ORDER BY tl.ma_tai_lieu;", connection))
                {
                    command.Parameters.Add("@MaHoSo", SqlDbType.BigInt).Value = caseId;
                    command.Parameters.Add("@NguoiXuLyId", SqlDbType.BigInt).Value = officerId;
                    using (var reader = command.ExecuteReader())
                    {
                        var result = new List<CaseOfficerDocumentItem>();
                        while (reader.Read()) result.Add(new CaseOfficerDocumentItem
                        {
                            DocumentId = reader.GetInt64(0), Type = ReadString(reader, 1),
                            Name = ReadString(reader, 2), Path = ReadString(reader, 3)
                        });
                        return result;
                    }
                }
            }
            catch (SqlException ex) { throw ToException(ex, "tải tài liệu hồ sơ"); }
        }

        public IList<CaseOfficerRuleItem> GetRules(long caseId, long officerId)
        {
            try
            {
                using (var connection = Db.OpenConnection())
                using (var command = new SqlCommand(
                    "SELECT r.quy_tac_id,r.ma_quy_tac,r.phien_ban,r.nhom,r.muc_do,r.dieu_kien_ap_dung, " +
                    "k.ket_qua,k.gia_tri_thuc_te,k.ghi_chu " +
                    "FROM dbo.HO_SO_CAP_PHEP h JOIN dbo.QUY_TAC r ON r.ma_loai_su_kien=h.ma_loai_su_kien " +
                    "LEFT JOIN dbo.KET_QUA_KIEM_TRA k ON k.ma_ho_so=h.ma_ho_so AND k.quy_tac_id=r.quy_tac_id " +
                    "WHERE h.ma_ho_so=@MaHoSo AND h.nguoi_xu_ly_hien_tai_id=@NguoiXuLyId " +
                    "AND h.trang_thai='DANG_THAM_DINH' AND r.trang_thai=1 " +
                    "AND r.hieu_luc_tu<=CONVERT(DATE,h.ngay_nop) " +
                    "AND (r.hieu_luc_den IS NULL OR r.hieu_luc_den>=CONVERT(DATE,h.ngay_nop)) " +
                    "ORDER BY r.muc_do DESC,r.ma_quy_tac,r.phien_ban;", connection))
                {
                    command.Parameters.Add("@MaHoSo", SqlDbType.BigInt).Value = caseId;
                    command.Parameters.Add("@NguoiXuLyId", SqlDbType.BigInt).Value = officerId;
                    using (var reader = command.ExecuteReader())
                    {
                        var result = new List<CaseOfficerRuleItem>();
                        while (reader.Read()) result.Add(new CaseOfficerRuleItem
                        {
                            RuleId = reader.GetInt64(0), RuleCode = ReadString(reader, 1), Version = reader.GetInt32(2),
                            Group = ReadString(reader, 3), Severity = ReadString(reader, 4), Condition = ReadString(reader, 5),
                            Result = ReadString(reader, 6), ActualValue = ReadString(reader, 7), Note = ReadString(reader, 8)
                        });
                        return result;
                    }
                }
            }
            catch (SqlException ex) { throw ToException(ex, "tải quy tắc thẩm định"); }
        }

        public void Accept(long caseId, long officerId) { Execute("dbo.sp_TiepNhanHoSo", caseId, officerId, null); }

        public void CheckValidity(long caseId, long officerId, bool valid, string reason)
        {
            try
            {
                using (var connection = Db.OpenConnection())
                using (var command = new SqlCommand("dbo.sp_KiemTraHopLeHoSo", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@MaHoSo", SqlDbType.BigInt).Value = caseId;
                    command.Parameters.Add("@NguoiXuLyId", SqlDbType.BigInt).Value = officerId;
                    command.Parameters.Add("@HopLe", SqlDbType.Bit).Value = valid;
                    command.Parameters.Add("@LyDo", SqlDbType.NVarChar, -1).Value =
                        string.IsNullOrWhiteSpace(reason) ? (object)DBNull.Value : reason.Trim();
                    command.ExecuteNonQuery();
                }
            }
            catch (SqlException ex) { throw ToException(ex, "ghi kết quả kiểm tra hợp lệ"); }
        }

        public void SaveRuleResult(long caseId, long officerId, long ruleId, string result, string actualValue, string note)
        {
            try
            {
                using (var connection = Db.OpenConnection())
                using (var command = new SqlCommand("dbo.sp_GhiKetQuaThamDinh", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@MaHoSo", SqlDbType.BigInt).Value = caseId;
                    command.Parameters.Add("@NguoiXuLyId", SqlDbType.BigInt).Value = officerId;
                    command.Parameters.Add("@QuyTacId", SqlDbType.BigInt).Value = ruleId;
                    command.Parameters.Add("@KetQua", SqlDbType.VarChar, 20).Value = result;
                    command.Parameters.Add("@GiaTriThucTe", SqlDbType.NVarChar, -1).Value = string.IsNullOrWhiteSpace(actualValue) ? (object)DBNull.Value : actualValue.Trim();
                    command.Parameters.Add("@GhiChu", SqlDbType.NVarChar, -1).Value = string.IsNullOrWhiteSpace(note) ? (object)DBNull.Value : note.Trim();
                    command.ExecuteNonQuery();
                }
            }
            catch (SqlException ex) { throw ToException(ex, "lưu kết quả thẩm định"); }
        }

        public void FinishAppraisal(long caseId, long officerId)
        {
            Execute("dbo.sp_KetThucThamDinh", caseId, officerId, null);
        }

        private static void Execute(string procedure, long caseId, long officerId, object unused)
        {
            try
            {
                using (var connection = Db.OpenConnection())
                using (var command = new SqlCommand(procedure, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@MaHoSo", SqlDbType.BigInt).Value = caseId;
                    command.Parameters.Add("@NguoiXuLyId", SqlDbType.BigInt).Value = officerId;
                    command.ExecuteNonQuery();
                }
            }
            catch (SqlException ex) { throw ToException(ex, "cập nhật hồ sơ"); }
        }

        private static CaseOfficerCaseItem ReadCase(IDataRecord reader)
        {
            return new CaseOfficerCaseItem
            {
                CaseId = reader.GetInt64(0), EventName = ReadString(reader, 1), EventType = ReadString(reader, 2),
                OrganizerName = ReadString(reader, 3), Status = ReadString(reader, 4), VenueName = ReadString(reader, 5),
                SubmittedAt = reader.IsDBNull(6) ? (DateTimeOffset?)null : (DateTimeOffset)reader.GetValue(6),
                Attendance = reader.IsDBNull(7) ? (int?)null : reader.GetInt32(7)
            };
        }

        private static string ReadString(IDataRecord reader, int ordinal) { return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal); }
        private static CaseOfficerWorkflowException ToException(SqlException ex, string operation)
        {
            foreach (SqlError error in ex.Errors)
            {
                switch (error.Number)
                {
                    case 52101: return new CaseOfficerWorkflowException("Tài khoản không có quyền tiếp nhận hồ sơ.");
                    case 52102: return new CaseOfficerWorkflowException("Hồ sơ chưa được nộp hoặc đã có người tiếp nhận.");
                    case 52103: return new CaseOfficerWorkflowException("Phải nhập lý do khi yêu cầu bổ sung.");
                    case 52104: return new CaseOfficerWorkflowException("Hồ sơ chưa được phân công cho tài khoản hiện tại.");
                    case 52105: return new CaseOfficerWorkflowException("Trạng thái hồ sơ không còn phù hợp với thao tác.");
                    case 52107: return new CaseOfficerWorkflowException("Kết quả thẩm định không hợp lệ.");
                    case 52108: return new CaseOfficerWorkflowException("Giá trị thực tế phải là JSON hợp lệ.");
                    case 52109: return new CaseOfficerWorkflowException("Hồ sơ hoặc quy tắc không còn đủ điều kiện thẩm định.");
                    case 52110: return new CaseOfficerWorkflowException("Hồ sơ chưa được phân công hoặc chưa ở trạng thái thẩm định.");
                    case 52111: return new CaseOfficerWorkflowException("Chưa thẩm định hết quy tắc bắt buộc.");
                    case 208:
                    case 2812: return new CaseOfficerWorkflowException("Database chưa có stored procedure cần thiết cho workflow Case Officer.", ex);
                }
            }
            return new CaseOfficerWorkflowException("Không thể " + operation + ". Vui lòng thử lại hoặc liên hệ quản trị viên.", ex);
        }
    }

    internal sealed class CaseOfficerWorkflowException : Exception
    {
        public CaseOfficerWorkflowException(string message) : base(message) { }
        public CaseOfficerWorkflowException(string message, Exception inner) : base(message, inner) { }
    }
}
