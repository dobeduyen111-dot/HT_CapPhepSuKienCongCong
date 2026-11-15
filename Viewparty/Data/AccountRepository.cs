using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Viewparty.Models;

namespace Viewparty.Data
{
    internal sealed class LoginRecord
    {
        public long AccountId { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string FullName { get; set; }
        public string RoleCode { get; set; }
        public short? ApprovalLevel { get; set; }
        public string Status { get; set; }
        public long? UnitId { get; set; }
    }

    internal sealed class AccountActivityItem
    {
        public DateTimeOffset Timestamp { get; set; }
        public string Category { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
    }

    /// <summary>Account data access for login, personal profile management, and password security.</summary>
    internal sealed class AccountRepository
    {
        public LoginRecord FindForLogin(string username)
        {
            using (var connection = Db.OpenConnection())
            using (var command = new SqlCommand("dbo.sp_LayTaiKhoanDangNhap", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@TenDangNhap", SqlDbType.VarChar, 80).Value = username;
                using (var reader = command.ExecuteReader(CommandBehavior.SingleRow))
                {
                    if (!reader.Read()) return null;
                    try
                    {
                        return new LoginRecord
                        {
                            AccountId = reader.GetInt64(reader.GetOrdinal("ma_tai_khoan")),
                            Username = reader.GetString(reader.GetOrdinal("ten_dang_nhap")),
                            PasswordHash = reader.GetString(reader.GetOrdinal("mat_khau_bam")),
                            FullName = reader.GetString(reader.GetOrdinal("ho_ten")),
                            RoleCode = reader.GetString(reader.GetOrdinal("vai_tro")),
                            ApprovalLevel = reader.IsDBNull(reader.GetOrdinal("cap_tham_quyen")) ? (short?)null : reader.GetInt16(reader.GetOrdinal("cap_tham_quyen")),
                            Status = reader.GetString(reader.GetOrdinal("trang_thai")),
                            UnitId = reader.IsDBNull(reader.GetOrdinal("ma_don_vi")) ? (long?)null : reader.GetInt64(reader.GetOrdinal("ma_don_vi"))
                        };
                    }
                    catch (Exception ex) when (ex is IndexOutOfRangeException || ex is InvalidCastException || ex is SqlException)
                    {
                        throw new DatabaseConnectionException(SqlFailureKind.SchemaError,
                            "Không thể đọc thông tin tài khoản đăng nhập từ hệ thống.", ex);
                    }
                }
            }
        }

        public TaiKhoan GetProfile(long accountId)
        {
            const string sql = @"
SELECT ma_tai_khoan, ma_don_vi, ten_dang_nhap, mat_khau_bam, ho_ten, email, so_dien_thoai, vai_tro, cap_tham_quyen, trang_thai, ngay_tao
FROM dbo.TAI_KHOAN
WHERE ma_tai_khoan = @AccountId;";

            using (var connection = Db.OpenConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@AccountId", SqlDbType.BigInt).Value = accountId;
                using (var r = command.ExecuteReader(CommandBehavior.SingleRow))
                {
                    if (!r.Read()) return null;
                    return new TaiKhoan
                    {
                        ma_tai_khoan = r.GetInt64(0),
                        ma_don_vi = r.IsDBNull(1) ? (long?)null : r.GetInt64(1),
                        ten_dang_nhap = r.GetString(2),
                        mat_khau_bam = r.GetString(3),
                        ho_ten = r.GetString(4),
                        email = r.IsDBNull(5) ? string.Empty : r.GetString(5),
                        so_dien_thoai = r.IsDBNull(6) ? string.Empty : r.GetString(6),
                        vai_tro = r.GetString(7),
                        cap_tham_quyen = r.IsDBNull(8) ? (short?)null : r.GetInt16(8),
                        trang_thai = r.GetString(9),
                        ngay_tao = r.GetDateTimeOffset(10)
                    };
                }
            }
        }

        public void UpdateProfile(long accountId, string fullName, string email, string phone)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("Vui lòng nhập họ và tên người dùng.");
            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
                throw new ArgumentException("Vui lòng nhập địa chỉ email hợp lệ.");

            const string sql = @"
UPDATE dbo.TAI_KHOAN
SET ho_ten = @HoTen,
    email = @Email,
    so_dien_thoai = @SoDienThoai
WHERE ma_tai_khoan = @AccountId;";

            using (var connection = Db.OpenConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@HoTen", SqlDbType.NVarChar, 150).Value = fullName.Trim();
                command.Parameters.Add("@Email", SqlDbType.VarChar, 150).Value = email.Trim();
                command.Parameters.Add("@SoDienThoai", SqlDbType.VarChar, 25).Value =
                    string.IsNullOrWhiteSpace(phone) ? (object)DBNull.Value : phone.Trim();
                command.Parameters.Add("@AccountId", SqlDbType.BigInt).Value = accountId;
                command.ExecuteNonQuery();
            }
        }

        public void ChangePassword(long accountId, string currentPassword, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(currentPassword))
                throw new ArgumentException("Vui lòng nhập mật khẩu hiện tại.");
            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Trim().Length < 6)
                throw new ArgumentException("Mật khẩu mới phải có tối thiểu 6 ký tự.");

            var profile = GetProfile(accountId);
            if (profile == null)
                throw new InvalidOperationException("Không tìm thấy thông tin tài khoản hiện tại.");

            bool verified = false;
            try
            {
                verified = BCrypt.Net.BCrypt.Verify(currentPassword, profile.mat_khau_bam);
            }
            catch
            {
                verified = false;
            }

            if (!verified)
                throw new InvalidOperationException("Mật khẩu hiện tại không chính xác. Vui lòng kiểm tra lại.");

            string newHash = BCrypt.Net.BCrypt.HashPassword(newPassword.Trim(), workFactor: 11);
            const string sql = "UPDATE dbo.TAI_KHOAN SET mat_khau_bam = @MatKhauBam WHERE ma_tai_khoan = @AccountId;";
            using (var connection = Db.OpenConnection())
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@MatKhauBam", SqlDbType.VarChar, 255).Value = newHash;
                command.Parameters.Add("@AccountId", SqlDbType.BigInt).Value = accountId;
                command.ExecuteNonQuery();
            }
        }

        public void ResetPasswordByUsernameAndEmail(string username, string email, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Vui lòng nhập tên đăng nhập.");
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Vui lòng nhập địa chỉ email đã đăng ký.");
            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Trim().Length < 6)
                throw new ArgumentException("Mật khẩu mới phải có ít nhất 6 ký tự.");

            const string checkSql = @"
SELECT ma_tai_khoan
FROM dbo.TAI_KHOAN
WHERE ten_dang_nhap = @Username AND LOWER(ISNULL(email, '')) = LOWER(@Email);";

            using (var connection = Db.OpenConnection())
            {
                long accountId = 0;
                using (var checkCmd = new SqlCommand(checkSql, connection))
                {
                    checkCmd.Parameters.Add("@Username", SqlDbType.VarChar, 80).Value = username.Trim();
                    checkCmd.Parameters.Add("@Email", SqlDbType.VarChar, 150).Value = email.Trim();
                    object scalar = checkCmd.ExecuteScalar();
                    if (scalar != null && scalar != DBNull.Value)
                        accountId = Convert.ToInt64(scalar);
                }

                if (accountId <= 0)
                    throw new InvalidOperationException("Tên đăng nhập hoặc địa chỉ email xác thực không khớp với hồ sơ tài khoản trên hệ thống.");

                string newHash = BCrypt.Net.BCrypt.HashPassword(newPassword.Trim(), workFactor: 11);
                using (var updateCmd = new SqlCommand("UPDATE dbo.TAI_KHOAN SET mat_khau_bam = @Hash WHERE ma_tai_khoan = @Id;", connection))
                {
                    updateCmd.Parameters.Add("@Hash", SqlDbType.VarChar, 255).Value = newHash;
                    updateCmd.Parameters.Add("@Id", SqlDbType.BigInt).Value = accountId;
                    updateCmd.ExecuteNonQuery();
                }
            }
        }

        public IList<AccountActivityItem> GetRecentActivity(long accountId)
        {
            var list = new List<AccountActivityItem>();
            const string sql = @"
SELECT TOP (12) ngay_gui AS thoi_gian, N'Thông báo hệ thống' AS phan_loai, tieu_de, noi_dung
FROM dbo.THONG_BAO
WHERE nguoi_nhan_id = @AccountId
UNION ALL
SELECT TOP (12) thoi_gian, N'Thao tác xử lý hồ sơ' AS phan_loai, hanh_dong AS tieu_de, ISNULL(ghi_chu, N'Cập nhật tiến độ hồ sơ #' + CAST(ma_ho_so AS NVARCHAR(20))) AS noi_dung
FROM dbo.LICH_SU_XU_LY
WHERE nguoi_thuc_hien_id = @AccountId
ORDER BY thoi_gian DESC;";

            try
            {
                using (var connection = Db.OpenConnection())
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@AccountId", SqlDbType.BigInt).Value = accountId;
                    using (var r = command.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            list.Add(new AccountActivityItem
                            {
                                Timestamp = r.GetDateTimeOffset(0),
                                Category = r.IsDBNull(1) ? "Hoạt động" : r.GetString(1),
                                Title = r.IsDBNull(2) ? "" : r.GetString(2),
                                Detail = r.IsDBNull(3) ? "" : r.GetString(3)
                            });
                        }
                    }
                }
            }
            catch
            {
                // Return empty if offline
            }
            return list;
        }
    }
}
