using System.Collections.Generic;
using Viewparty.Models;

namespace Viewparty.Services
{
    /// <summary>UI access policy for the modules currently present in the client.</summary>
    public sealed class AuthorizationService
    {
        private static readonly IDictionary<string, string[]> RoleMenus = new Dictionary<string, string[]>
        {
            { "NGUOI_TO_CHUC", new[] { "Tổng quan", "Hồ sơ của tôi", "Tạo hồ sơ", "Tra cứu địa điểm", "Thông báo" } },
            { "NGUOI_XU_LY", new[] { "Tổng quan", "Tiếp nhận hồ sơ", "Kiểm tra hợp lệ", "Thẩm định hồ sơ", "Ý kiến liên ngành", "Tổng hợp / ngoại lệ" } },
            { "LIEN_NGANH", new[] { "Tổng quan", "Yêu cầu cần phản hồi", "Chi tiết hồ sơ", "Gửi ý kiến" } },
            { "NGUOI_PHE_DUYET", new[] { "Tổng quan", "Hồ sơ chờ duyệt", "Xét ngoại lệ", "Quyết định cấp phép", "Giấy phép đã cấp" } },
            { "QUAN_TRI_VIEN", new[] { "Tổng quan", "Tài khoản / vai trò", "Loại sự kiện", "Địa điểm", "Quy tắc / liên ngành", "Nhật ký cấu hình" } }
        };

        public bool CanOpen(AuthenticatedUser user, string module)
        {
            string[] allowed;
            return user != null && RoleMenus.TryGetValue(user.RoleCode, out allowed) &&
                   System.Array.IndexOf(allowed, module) >= 0;
        }

        public bool IsAuthenticatedRole(AuthenticatedUser user)
        {
            string[] allowed;
            return user != null && user.AccountId > 0 &&
                   RoleMenus.TryGetValue(user.RoleCode, out allowed) && allowed.Length > 0;
        }

        public string[] GetModules(AuthenticatedUser user)
        {
            string[] allowed;
            return user != null && RoleMenus.TryGetValue(user.RoleCode, out allowed)
                ? (string[])allowed.Clone()
                : new string[0];
        }
    }
}
