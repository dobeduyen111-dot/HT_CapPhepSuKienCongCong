using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Viewparty.Models;

namespace Viewparty.Data
{
    internal sealed class LicenseDecisionRepository
    {
        public IList<LicenseDecisionCase> GetDecisionCases(long approverId)
        {
            try
            {
                using (var connection = Db.OpenConnection())
                using (var command = new SqlCommand("dbo.sp_DanhSachHoSoChoQuyetDinh", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@NguoiPheDuyetId", SqlDbType.BigInt).Value = approverId;
                    using (var reader = command.ExecuteReader())
                    {
                        var cases = new List<LicenseDecisionCase>();
                        while (reader.Read())
                        {
                            cases.Add(new LicenseDecisionCase
                            {
                                CaseId = reader.GetInt64(reader.GetOrdinal("ma_ho_so")),
                                EventName = ReadString(reader, "ten_su_kien"),
                                Status = ReadString(reader, "trang_thai"),
                                SubmittedAt = ReadDateTimeOffset(reader, "ngay_nop"),
                                StartsAt = ReadDateTimeOffset(reader, "bat_dau"),
                                EndsAt = ReadDateTimeOffset(reader, "ket_thuc"),
                                Attendance = ReadNullableInt32(reader, "quy_mo"),
                                VenueName = ReadString(reader, "ten_dia_diem"),
                                VenueAddress = ReadString(reader, "dia_chi"),
                                VenueCapacity = ReadNullableInt32(reader, "suc_chua"),
                                CanApprove = reader.GetBoolean(reader.GetOrdinal("du_dieu_kien_phe_duyet")),
                                AssessedRuleCount = reader.GetInt32(reader.GetOrdinal("so_quy_tac_da_danh_gia")),
                                MandatoryFailureCount = reader.GetInt32(reader.GetOrdinal("so_khong_dat_bat_buoc")),
                                ApprovedExceptionCount = reader.GetInt32(reader.GetOrdinal("so_ngoai_le_duoc_chap_thuan")),
                                RespondedOpinionCount = reader.GetInt32(reader.GetOrdinal("so_y_kien_da_phan_hoi")),
                                LicenseNumber = ReadString(reader, "so_giay_phep"),
                                RejectionReason = ReadString(reader, "ly_do_tu_choi")
                            });
                        }
                        return cases;
                    }
                }
            }
            catch (SqlException ex) { throw ToWorkflowException(ex, "tải hồ sơ chờ quyết định"); }
        }

        public IList<IssuedLicenseItem> GetIssuedLicenses(long approverId)
        {
            try
            {
                using (var connection = Db.OpenConnection())
                using (var command = new SqlCommand("dbo.sp_DanhSachGiayPhep", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@NguoiPheDuyetId", SqlDbType.BigInt).Value = approverId;
                    using (var reader = command.ExecuteReader())
                    {
                        var licenses = new List<IssuedLicenseItem>();
                        while (reader.Read())
                        {
                            licenses.Add(new IssuedLicenseItem
                            {
                                LicenseId = reader.GetInt64(reader.GetOrdinal("ma_giay_phep")),
                                CaseId = reader.GetInt64(reader.GetOrdinal("ma_ho_so")),
                                LicenseNumber = reader.GetString(reader.GetOrdinal("so_giay_phep")),
                                IssuedAt = reader.GetDateTime(reader.GetOrdinal("ngay_cap")),
                                ValidFrom = reader.GetDateTime(reader.GetOrdinal("hieu_luc_tu")),
                                ValidUntil = reader.GetDateTime(reader.GetOrdinal("hieu_luc_den")),
                                EventName = ReadString(reader, "ten_su_kien"),
                                VenueName = ReadString(reader, "ten_dia_diem"),
                                Conditions = ReadString(reader, "dieu_kien_kem_theo")
                            });
                        }
                        return licenses;
                    }
                }
            }
            catch (SqlException ex) { throw ToWorkflowException(ex, "tải giấy phép đã cấp"); }
        }

        public ApproverReviewData GetReviewData(long approverId, long caseId)
        {
            const string sql = @"
IF NOT EXISTS (SELECT 1 FROM dbo.TAI_KHOAN WHERE ma_tai_khoan=@NguoiPheDuyetId
               AND vai_tro='NGUOI_PHE_DUYET' AND trang_thai='HOAT_DONG' AND cap_tham_quyen>0)
    THROW 52306, N'Không có thẩm quyền xem chi tiết hồ sơ.', 1;
IF NOT EXISTS (SELECT 1 FROM dbo.HO_SO_CAP_PHEP WHERE ma_ho_so=@MaHoSo)
    THROW 52316, N'Hồ sơ không tồn tại hoặc không còn hiệu lực.', 1;
SELECT d.ten_don_vi, y.trang_thai, y.ket_luan, y.noi_dung_phan_hoi,
       y.kien_nghi, y.ngay_phan_hoi
FROM dbo.YEU_CAU_Y_KIEN y
JOIN dbo.DON_VI_LIEN_NGANH d ON d.ma_don_vi=y.ma_don_vi
WHERE y.ma_ho_so=@MaHoSo
ORDER BY y.ma_yeu_cau;
SELECT loai_tai_lieu, ten_tai_lieu, duong_dan
FROM dbo.TAI_LIEU WHERE ma_ho_so=@MaHoSo ORDER BY ma_tai_lieu;
SELECT hanh_dong, tu_trang_thai, den_trang_thai, ket_qua, ghi_chu, thoi_diem
FROM dbo.LICH_SU_XU_LY WHERE ma_ho_so=@MaHoSo ORDER BY thoi_diem, ma_lich_su;";
            try
            {
                using (var connection = Db.OpenConnection())
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@MaHoSo", SqlDbType.BigInt).Value = caseId;
                    command.Parameters.Add("@NguoiPheDuyetId", SqlDbType.BigInt).Value = approverId;
                    var data = new ApproverReviewData
                    {
                        Opinions = new List<ApproverOpinionItem>(),
                        Documents = new List<ApproverDocumentItem>(),
                        History = new List<ApproverHistoryItem>()
                    };
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read()) data.Opinions.Add(new ApproverOpinionItem
                        {
                            UnitName = ReadString(reader, "ten_don_vi"), Status = ReadString(reader, "trang_thai"),
                            Conclusion = ReadString(reader, "ket_luan"), Response = ReadString(reader, "noi_dung_phan_hoi"),
                            Recommendation = ReadString(reader, "kien_nghi"), RespondedAt = ReadNullableDateTimeOffset(reader, "ngay_phan_hoi")
                        });
                        if (reader.NextResult()) while (reader.Read()) data.Documents.Add(new ApproverDocumentItem
                        {
                            Type = ReadString(reader, "loai_tai_lieu"), Name = ReadString(reader, "ten_tai_lieu"), Path = ReadString(reader, "duong_dan")
                        });
                        if (reader.NextResult()) while (reader.Read()) data.History.Add(new ApproverHistoryItem
                        {
                            Action = ReadString(reader, "hanh_dong"), FromStatus = ReadString(reader, "tu_trang_thai"),
                            ToStatus = ReadString(reader, "den_trang_thai"), Result = ReadString(reader, "ket_qua"),
                            Note = ReadString(reader, "ghi_chu"), At = reader.GetDateTimeOffset(reader.GetOrdinal("thoi_diem"))
                        });
                    }
                    return data;
                }
            }
            catch (SqlException ex) { throw ToWorkflowException(ex, "tải chi tiết hồ sơ"); }
        }

        public LicenseIssueResult Approve(long caseId, long approverId, string conditions)
        {
            try
            {
                using (var connection = Db.OpenConnection())
                using (var command = new SqlCommand("dbo.sp_PheDuyetCapGiayPhep", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@MaHoSo", SqlDbType.BigInt).Value = caseId;
                    command.Parameters.Add("@NguoiPheDuyetId", SqlDbType.BigInt).Value = approverId;
                    command.Parameters.Add("@DieuKienKemTheo", SqlDbType.NVarChar, -1).Value =
                        string.IsNullOrWhiteSpace(conditions) ? (object)DBNull.Value : conditions.Trim();
                    using (var reader = command.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        if (!reader.Read())
                            throw new LicenseDecisionWorkflowException(
                                "SQL Server không xác nhận kết quả cấp giấy phép.");
                        return new LicenseIssueResult
                        {
                            CaseId = reader.GetInt64(reader.GetOrdinal("ma_ho_so")),
                            LicenseNumber = reader.GetString(reader.GetOrdinal("so_giay_phep"))
                        };
                    }
                }
            }
            catch (SqlException ex) { throw ToWorkflowException(ex, "phê duyệt và cấp giấy phép"); }
        }

        public void Reject(long caseId, long approverId, string reason)
        {
            try
            {
                using (var connection = Db.OpenConnection())
                using (var command = new SqlCommand("dbo.sp_TuChoiCapPhep", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@MaHoSo", SqlDbType.BigInt).Value = caseId;
                    command.Parameters.Add("@NguoiPheDuyetId", SqlDbType.BigInt).Value = approverId;
                    command.Parameters.Add("@LyDo", SqlDbType.NVarChar, -1).Value = reason;
                    command.ExecuteNonQuery();
                }
            }
            catch (SqlException ex) { throw ToWorkflowException(ex, "từ chối hồ sơ"); }
        }

        private static LicenseDecisionWorkflowException ToWorkflowException(
            SqlException exception, string operation)
        {
            foreach (SqlError error in exception.Errors)
            {
                switch (error.Number)
                {
                    case 52306:
                    case 52313:
                        return new LicenseDecisionWorkflowException(
                            "Tài khoản Người phê duyệt không hoạt động hoặc không đủ thẩm quyền.");
                    case 52307:
                        return new LicenseDecisionWorkflowException(
                            "Hồ sơ chưa hoàn tất thẩm định/ngoại lệ hoặc chưa đủ ý kiến liên ngành.");
                    case 52308:
                        return new LicenseDecisionWorkflowException(
                            "Thời điểm sự kiện đã bắt đầu; không thể cấp giấy phép.");
                    case 52309:
                        return new LicenseDecisionWorkflowException(
                            "Không khóa được lịch địa điểm để cấp phép. Hãy thử lại.");
                    case 52310:
                        return new LicenseDecisionWorkflowException(
                            "Địa điểm không còn đủ điều kiện hoặc lịch đã bị trùng.");
                    case 52311:
                        return new LicenseDecisionWorkflowException(
                            "Hồ sơ đã được cấp giấy phép.");
                    case 52312:
                        return new LicenseDecisionWorkflowException("Phải nhập lý do từ chối.");
                    case 52314:
                        return new LicenseDecisionWorkflowException(
                            "Hồ sơ chưa đến bước có thể ra quyết định từ chối.");
                    case 52316:
                        return new LicenseDecisionWorkflowException(
                            "Hồ sơ đã đổi trạng thái. Hãy tải lại danh sách quyết định.");
                    case 208:
                    case 2812:
                        return new LicenseDecisionWorkflowException(
                            "Database chưa được cập nhật UC16/UC17/UC19. Hãy chạy lại File 03.", exception);
                    case 207:
                        return new LicenseDecisionWorkflowException(
                            "Database thiếu cột UC16/UC17/UC19. Hãy chạy lại File 01 trước File 03.", exception);
                }
            }
            return new LicenseDecisionWorkflowException(
                "Không thể " + operation + ". Kiểm tra schema, stored procedure và quyền truy cập database.",
                exception);
        }

        private static string ReadString(IDataRecord reader, string name)
        {
            int ordinal = reader.GetOrdinal(name);
            return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
        }

        private static int? ReadNullableInt32(IDataRecord reader, string name)
        {
            int ordinal = reader.GetOrdinal(name);
            return reader.IsDBNull(ordinal) ? (int?)null : reader.GetInt32(ordinal);
        }

        private static DateTimeOffset? ReadDateTimeOffset(IDataRecord reader, string name)
        {
            int ordinal = reader.GetOrdinal(name);
            return reader.IsDBNull(ordinal) ? (DateTimeOffset?)null : (DateTimeOffset)reader.GetValue(ordinal);
        }

        private static DateTimeOffset? ReadNullableDateTimeOffset(IDataRecord reader, string name)
        {
            int ordinal = reader.GetOrdinal(name);
            return reader.IsDBNull(ordinal) ? (DateTimeOffset?)null : (DateTimeOffset)reader.GetValue(ordinal);
        }
    }

    internal sealed class LicenseDecisionWorkflowException : Exception
    {
        public LicenseDecisionWorkflowException(string message) : base(message) { }
        public LicenseDecisionWorkflowException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
