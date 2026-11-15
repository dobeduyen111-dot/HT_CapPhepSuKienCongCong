using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Viewparty.Data
{
    /// <summary>Read-only checks for the schema contract used by the login service.</summary>
    internal sealed class DatabaseHealthRepository
    {
        private static readonly string[] RequiredTables =
        {
            "DON_VI_LIEN_NGANH", "LOAI_SU_KIEN", "DIA_DIEM", "TAI_KHOAN",
            "HO_SO_CAP_PHEP", "QUYEN_TRINH_CHIEU", "TAI_LIEU", "QUY_TAC",
            "CAU_HINH_LIEN_NGANH", "KET_QUA_KIEM_TRA", "YEU_CAU_Y_KIEN",
            "NGOAI_LE", "GIAY_PHEP", "LICH_SU_XU_LY", "NHAT_KY_CAU_HINH", "THONG_BAO"
        };

        private static readonly string[] LoginColumns =
        {
            "ma_tai_khoan", "ten_dang_nhap", "mat_khau_bam", "ho_ten",
            "vai_tro", "cap_tham_quyen", "trang_thai", "ma_don_vi"
        };

        private static readonly string[] LoginColumnTypes =
        {
            "bigint", "varchar(80)", "varchar(255)", "nvarchar(150)",
            "varchar(30)", "smallint", "varchar(25)", "bigint"
        };

        public void ValidateLoginSchema()
        {
            using (var connection = Db.OpenConnection())
            {
                var presentTables = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                using (var command = new SqlCommand(
                    "SELECT name FROM sys.tables WHERE schema_id=SCHEMA_ID('dbo');", connection))
                using (var reader = command.ExecuteReader())
                    while (reader.Read()) presentTables.Add(reader.GetString(0));

                var missing = RequiredTables.Where(name => !presentTables.Contains(name)).ToArray();
                var unexpected = presentTables.Where(name => !RequiredTables.Contains(name, StringComparer.OrdinalIgnoreCase)).ToArray();
                if (missing.Length != 0 || unexpected.Length != 0 || presentTables.Count != RequiredTables.Length)
                    throw new InvalidOperationException(
                        "Database phải có đúng 16 bảng View Party. Thiếu: " +
                        (missing.Length == 0 ? "không" : string.Join(", ", missing)) + "; ngoài schema dự kiến: " +
                        (unexpected.Length == 0 ? "không" : string.Join(", ", unexpected)) + ".");

                var parameterRows = new List<Tuple<string, string, short, bool>>();
                using (var command = new SqlCommand(
                    "SELECT p.name, TYPE_NAME(p.user_type_id), p.max_length, p.is_output " +
                    "FROM sys.parameters p WHERE p.object_id=OBJECT_ID(N'dbo.sp_LayTaiKhoanDangNhap', N'P') ORDER BY p.parameter_id;",
                    connection))
                using (var reader = command.ExecuteReader())
                    while (reader.Read()) parameterRows.Add(Tuple.Create(reader.GetString(0), reader.GetString(1), reader.GetInt16(2), reader.GetBoolean(3)));

                if (parameterRows.Count == 0)
                    throw new InvalidOperationException("Không tìm thấy dbo.sp_LayTaiKhoanDangNhap trong database đang chọn.");
                if (parameterRows.Count != 1 || parameterRows[0].Item1 != "@TenDangNhap" ||
                    !string.Equals(parameterRows[0].Item2, "varchar", StringComparison.OrdinalIgnoreCase) ||
                    parameterRows[0].Item3 != 80 || parameterRows[0].Item4)
                    throw new InvalidOperationException("Tham số dbo.sp_LayTaiKhoanDangNhap không khớp hợp đồng @TenDangNhap VARCHAR(80) INPUT.");

                var actualColumns = new List<string>();
                var actualTypes = new List<string>();
                using (var command = new SqlCommand(
                    "SELECT name, system_type_name FROM sys.dm_exec_describe_first_result_set_for_object(" +
                    "OBJECT_ID(N'dbo.sp_LayTaiKhoanDangNhap', N'P'), 0) WHERE is_hidden=0 ORDER BY column_ordinal;",
                    connection))
                using (var reader = command.ExecuteReader())
                    while (reader.Read()) { actualColumns.Add(reader.GetString(0)); actualTypes.Add(reader.GetString(1)); }

                if (!LoginColumns.SequenceEqual(actualColumns, StringComparer.OrdinalIgnoreCase) ||
                    !LoginColumnTypes.SequenceEqual(actualTypes, StringComparer.OrdinalIgnoreCase))
                    throw new InvalidOperationException("Các cột hoặc kiểu dữ liệu trả về của dbo.sp_LayTaiKhoanDangNhap không khớp hợp đồng đăng nhập.");
            }
        }
    }
}
