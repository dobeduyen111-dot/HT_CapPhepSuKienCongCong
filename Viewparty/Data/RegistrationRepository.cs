using System;
using System.Data;
using System.Data.SqlClient;

namespace Viewparty.Data
{
    internal sealed class RegistrationRepository
    {
        public RegistrationResult CreateOrganizerAccount(
            string username, string passwordHash, string fullName, string email, string phone)
        {
            try
            {
                using (var connection = Db.OpenConnection())
                using (var command = new SqlCommand("dbo.sp_DangKyNguoiToChuc", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@TenDangNhap", SqlDbType.VarChar, 80).Value = username;
                    command.Parameters.Add("@MatKhauBam", SqlDbType.VarChar, 255).Value = passwordHash;
                    command.Parameters.Add("@HoTen", SqlDbType.NVarChar, 150).Value = fullName;
                    command.Parameters.Add("@Email", SqlDbType.VarChar, 150).Value = email;
                    command.Parameters.Add("@SoDienThoai", SqlDbType.VarChar, 20).Value = phone;

                    using (var reader = command.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        if (!reader.Read())
                            throw new DatabaseConnectionException(SqlFailureKind.SchemaError,
                                "dbo.sp_DangKyNguoiToChuc không trả về thông tin tài khoản vừa tạo.");

                        try
                        {
                            return new RegistrationResult
                            {
                                AccountId = reader.GetInt64(reader.GetOrdinal("ma_tai_khoan")),
                                Status = reader.GetString(reader.GetOrdinal("trang_thai"))
                            };
                        }
                        catch (Exception ex) when (ex is IndexOutOfRangeException || ex is InvalidCastException)
                        {
                            throw new DatabaseConnectionException(SqlFailureKind.SchemaError,
                                "Kết quả dbo.sp_DangKyNguoiToChuc không khớp contract đăng ký.", ex);
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                var conflict = GetConflict(ex);
                if (conflict != null) throw conflict;
                throw DatabaseConnectionException.FromSqlException(ex);
            }
        }

        private static RegistrationConflictException GetConflict(SqlException exception)
        {
            foreach (SqlError error in exception.Errors)
            {
                if (error.Number == 52002 ||
                    error.Message.IndexOf("UQ_TAI_KHOAN_ten_dang_nhap", StringComparison.OrdinalIgnoreCase) >= 0)
                    return new RegistrationConflictException(RegistrationConflictField.Username,
                        "Tên đăng nhập đã được sử dụng.");

                if (error.Number == 52006 ||
                    error.Message.IndexOf("UQ_TAI_KHOAN_email", StringComparison.OrdinalIgnoreCase) >= 0)
                    return new RegistrationConflictException(RegistrationConflictField.Email,
                        "Email đã được sử dụng.");

                if (error.Number == 52007 ||
                    error.Message.IndexOf("UX_TAI_KHOAN_so_dien_thoai", StringComparison.OrdinalIgnoreCase) >= 0)
                    return new RegistrationConflictException(RegistrationConflictField.Phone,
                        "Số điện thoại đã được sử dụng.");
            }

            if (exception.Number == 2601 || exception.Number == 2627)
                return new RegistrationConflictException(RegistrationConflictField.Unknown,
                    "Tên đăng nhập, email hoặc số điện thoại đã được sử dụng.");

            return null;
        }
    }

    internal sealed class RegistrationResult
    {
        public long AccountId { get; set; }
        public string Status { get; set; }
    }

    public enum RegistrationConflictField { Username, Email, Phone, Unknown }

    public sealed class RegistrationConflictException : Exception
    {
        public RegistrationConflictField Field { get; private set; }

        public RegistrationConflictException(RegistrationConflictField field, string message)
            : base(message)
        {
            Field = field;
        }
    }
}
