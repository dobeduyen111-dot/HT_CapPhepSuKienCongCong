using System;
using System.Data.SqlClient;
using BCryptEngine = BCrypt.Net.BCrypt;
using Viewparty.Data;
using Viewparty.Models;

namespace Viewparty.Services
{
    public sealed class AuthenticationService
    {
        private readonly AccountRepository accounts = new AccountRepository();

        public AuthenticatedUser Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                throw new AuthenticationFailureException(AuthenticationFailureKind.EmptyInput, "Vui lòng nhập tên đăng nhập và mật khẩu.");

            LoginRecord record;
            try { record = accounts.FindForLogin(username.Trim()); }
            catch (SqlException ex) { throw DatabaseConnectionException.FromSqlException(ex); }

            if (record == null)
                throw new AuthenticationFailureException(AuthenticationFailureKind.AccountNotFound, "Không tìm thấy tài khoản.");
            if (!string.Equals(record.Status, "HOAT_DONG", StringComparison.OrdinalIgnoreCase))
                throw new AuthenticationFailureException(AuthenticationFailureKind.AccountDisabled,
                    "Tài khoản đang bị vô hiệu hóa hoặc chưa được kích hoạt.");

            try
            {
                if (!BCryptEngine.Verify(password, record.PasswordHash))
                    throw new AuthenticationFailureException(AuthenticationFailureKind.InvalidPassword, "Mật khẩu không đúng.");
            }
            catch (AuthenticationFailureException) { throw; }
            catch (Exception ex)
            {
                throw new AuthenticationFailureException(AuthenticationFailureKind.BCryptError,
                    "Không thể xác thực hash mật khẩu đã lưu. Lỗi xác thực BCrypt đã được ghi log.", ex);
            }

            var displayRole = DisplayRoleFor(record.RoleCode);
            if (displayRole == null)
                throw new AuthenticationFailureException(AuthenticationFailureKind.SchemaError,
                    "Tài khoản có mã vai trò không được ứng dụng hỗ trợ.");
            return new AuthenticatedUser(record.AccountId, record.Username, record.FullName,
                record.RoleCode, displayRole, record.ApprovalLevel, record.UnitId);
        }

        private static string DisplayRoleFor(string roleCode)
        {
            switch (roleCode)
            {
                case "NGUOI_TO_CHUC": return "Người tổ chức";
                case "NGUOI_XU_LY": return "Người xử lý";
                case "LIEN_NGANH": return "Đơn vị liên ngành";
                case "NGUOI_PHE_DUYET": return "Người phê duyệt";
                case "QUAN_TRI_VIEN": return "Quản trị viên";
                default: return null;
            }
        }
    }

    public enum AuthenticationFailureKind { EmptyInput, AccountNotFound, InvalidPassword, AccountDisabled, BCryptError, SchemaError }

    public sealed class AuthenticationFailureException : Exception
    {
        public AuthenticationFailureKind FailureKind { get; private set; }
        public AuthenticationFailureException(AuthenticationFailureKind kind, string message) : base(message) { FailureKind = kind; }
        public AuthenticationFailureException(AuthenticationFailureKind kind, string message, Exception innerException)
            : base(message, innerException) { FailureKind = kind; }
    }
}
