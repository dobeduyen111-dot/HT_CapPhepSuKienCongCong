using System;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using BCryptEngine = BCrypt.Net.BCrypt;
using Viewparty.Data;

namespace Viewparty.Services
{
    public sealed class RegistrationService
    {
        private static readonly Regex PhonePattern = new Regex(@"^\+?[0-9]{7,19}$", RegexOptions.Compiled);
        private readonly RegistrationRepository accounts;

        public RegistrationService() : this(new RegistrationRepository()) { }

        internal RegistrationService(RegistrationRepository accounts)
        {
            this.accounts = accounts ?? throw new ArgumentNullException("accounts");
        }

        public RegistrationReceipt Register(RegistrationRequest request)
        {
            if (request == null) throw new ArgumentNullException("request");

            string fullName = (request.FullName ?? string.Empty).Trim();
            string username = (request.Username ?? string.Empty).Trim();
            string email = (request.Email ?? string.Empty).Trim().ToLowerInvariant();
            string phone = NormalizePhone(request.Phone);

            if (fullName.Length == 0)
                throw new RegistrationValidationException("Vui lòng nhập họ tên người đại diện.");
            if (fullName.Length > 150)
                throw new RegistrationValidationException("Họ tên không được vượt quá 150 ký tự.");
            if (username.Length == 0)
                throw new RegistrationValidationException("Vui lòng nhập tên đăng nhập.");
            if (username.Length > 80)
                throw new RegistrationValidationException("Tên đăng nhập không được vượt quá 80 ký tự.");
            if (email.Length == 0 || email.Length > 150 || !IsValidEmail(email))
                throw new RegistrationValidationException("Vui lòng nhập email hợp lệ (tối đa 150 ký tự).");
            if (phone.Length == 0 || phone.Length > 20 || !PhonePattern.IsMatch(phone))
                throw new RegistrationValidationException("Số điện thoại phải có từ 7 đến 19 chữ số, có thể bắt đầu bằng dấu +.");
            if (string.IsNullOrWhiteSpace(request.Password))
                throw new RegistrationValidationException("Vui lòng nhập mật khẩu.");
            if (!string.Equals(request.Password, request.ConfirmPassword, StringComparison.Ordinal))
                throw new RegistrationValidationException("Mật khẩu xác nhận không khớp.");
            if (request.Password.Length < 8)
                throw new RegistrationValidationException("Mật khẩu phải có ít nhất 8 ký tự.");
            if (Encoding.UTF8.GetByteCount(request.Password) > 72)
                throw new RegistrationValidationException("Mật khẩu không được vượt quá 72 byte UTF-8.");
            if (!request.ConsentAccepted)
                throw new RegistrationValidationException("Bạn cần đồng ý với chính sách sử dụng để đăng ký.");

            string passwordHash = BCryptEngine.HashPassword(request.Password);
            var result = accounts.CreateOrganizerAccount(username, passwordHash, fullName, email, phone);
            if (!string.Equals(result.Status, "HOAT_DONG", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException(
                    "Tài khoản đã được tạo nhưng trạng thái không cho phép đăng nhập ngay. Kiểm tra dbo.sp_DangKyNguoiToChuc.");

            return new RegistrationReceipt(result.AccountId, username);
        }

        private static bool IsValidEmail(string email)
        {
            foreach (char character in email)
                if (character > 127) return false;

            try
            {
                var address = new MailAddress(email);
                return string.Equals(address.Address, email, StringComparison.OrdinalIgnoreCase);
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private static string NormalizePhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return string.Empty;

            var normalized = new StringBuilder(phone.Length);
            foreach (char character in phone.Trim())
            {
                if (character == ' ' || character == '-' || character == '(' || character == ')' || character == '.')
                    continue;
                normalized.Append(character);
            }
            return normalized.ToString();
        }
    }

    public sealed class RegistrationRequest
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public bool ConsentAccepted { get; set; }
    }

    public sealed class RegistrationReceipt
    {
        public long AccountId { get; private set; }
        public string Username { get; private set; }

        internal RegistrationReceipt(long accountId, string username)
        {
            AccountId = accountId;
            Username = username;
        }
    }

    public sealed class RegistrationValidationException : Exception
    {
        public RegistrationValidationException(string message) : base(message) { }
    }
}
