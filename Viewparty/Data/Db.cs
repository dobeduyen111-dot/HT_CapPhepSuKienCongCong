using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Viewparty.Data
{
    /// <summary>ADO.NET connection factory for Windows-authenticated SQL Server.</summary>
    public static class Db
    {
        private const string ConnectionName = "ViewPartyDb";
        private const string DefaultServer = @"(localdb)\MSSQLLocalDB";
        private const string DefaultDatabase = "ViewPartyDB_UC28";
        private static string resolvedConnectionString;

        public static SqlConnection CreateConnection()
        {
            if (!string.IsNullOrEmpty(resolvedConnectionString))
                return new SqlConnection(resolvedConnectionString);

            var builder = GetConfiguredBuilder();
            return new SqlConnection(builder.ConnectionString);
        }

        private static SqlConnectionStringBuilder GetConfiguredBuilder()
        {
            var setting = ConfigurationManager.ConnectionStrings[ConnectionName];
            if (setting == null || string.IsNullOrWhiteSpace(setting.ConnectionString))
                throw new DatabaseConnectionException(SqlFailureKind.Configuration,
                    "Chưa cấu hình connection string 'ViewPartyDb' trong App.config.");

            SqlConnectionStringBuilder builder;
            try { builder = new SqlConnectionStringBuilder(setting.ConnectionString); }
            catch (ArgumentException)
            {
                throw new DatabaseConnectionException(SqlFailureKind.Configuration,
                    "Connection string 'ViewPartyDb' không hợp lệ.");
            }

            if (string.IsNullOrWhiteSpace(builder.DataSource))
                builder.DataSource = DefaultServer;
            if (string.IsNullOrWhiteSpace(builder.InitialCatalog))
                builder.InitialCatalog = DefaultDatabase;

            return builder;
        }

        public static SqlConnection OpenConnection()
        {
            if (!string.IsNullOrEmpty(resolvedConnectionString))
            {
                var cached = new SqlConnection(resolvedConnectionString);
                try
                {
                    cached.Open();
                    return cached;
                }
                catch (SqlException)
                {
                    cached.Dispose();
                    resolvedConnectionString = null;
                }
            }

            var primaryBuilder = GetConfiguredBuilder();
            SqlException firstException = null;

            foreach (string candidateConnectionString in GetCandidateConnectionStrings(primaryBuilder))
            {
                var connection = new SqlConnection(candidateConnectionString);
                try
                {
                    connection.Open();
                    var finalBuilder = new SqlConnectionStringBuilder(candidateConnectionString)
                    {
                        ConnectTimeout = primaryBuilder.ConnectTimeout > 0 ? primaryBuilder.ConnectTimeout : 30
                    };
                    resolvedConnectionString = finalBuilder.ConnectionString;
                    return connection;
                }
                catch (SqlException ex)
                {
                    connection.Dispose();
                    if (firstException == null) firstException = ex;
                }
            }

            throw DatabaseConnectionException.FromSqlException(firstException);
        }

        private static IEnumerable<string> GetCandidateConnectionStrings(SqlConnectionStringBuilder primary)
        {
            var yielded = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var servers = new List<string>
            {
                primary.DataSource,
                DefaultServer,
                @".\MSSQLSERVER01",
                "localhost",
                ".",
                @".\SQLEXPRESS"
            };
            var databases = new List<string>
            {
                primary.InitialCatalog,
                "ViewPartyDB_UC28",
                "ViewPartyDB"
            };

            bool isFirst = true;
            foreach (string server in servers)
            {
                if (string.IsNullOrWhiteSpace(server)) continue;
                foreach (string db in databases)
                {
                    if (string.IsNullOrWhiteSpace(db)) continue;
                    string key = server.Trim() + "|" + db.Trim();
                    if (!yielded.Add(key)) continue;

                    var candidate = new SqlConnectionStringBuilder(primary.ConnectionString)
                    {
                        DataSource = server.Trim(),
                        InitialCatalog = db.Trim(),
                        ConnectTimeout = isFirst ? Math.Min(primary.ConnectTimeout, 8) : 3
                    };
                    isFirst = false;
                    yield return candidate.ConnectionString;
                }
            }
        }

        public static DatabaseConnectionInfo CheckConnection()
        {
            using (var connection = OpenConnection())
            using (var command = new SqlCommand(
                "SELECT DB_NAME() AS database_name, CONVERT(varchar(128), SERVERPROPERTY('ProductVersion')) AS product_version;", connection))
            using (var reader = command.ExecuteReader(CommandBehavior.SingleRow))
            {
                if (!reader.Read())
                    throw new DatabaseConnectionException(SqlFailureKind.Other, "SQL Server không trả về thông tin database đang kết nối.");
                return new DatabaseConnectionInfo
                {
                    DatabaseName = reader.GetString(reader.GetOrdinal("database_name")),
                    ProductVersion = reader.GetString(reader.GetOrdinal("product_version"))
                };
            }
        }
    }

    public sealed class DatabaseConnectionInfo
    {
        public string DatabaseName { get; set; }
        public string ProductVersion { get; set; }
    }

    public sealed class DatabaseConnectionException : Exception
    {
        public SqlFailureKind FailureKind { get; private set; }

        public DatabaseConnectionException(string safeMessage) : this(SqlFailureKind.Configuration, safeMessage) { }
        internal DatabaseConnectionException(SqlFailureKind kind, string safeMessage, Exception innerException = null)
            : base(safeMessage, innerException) { FailureKind = kind; }

        public static DatabaseConnectionException FromSqlException(SqlException exception)
        {
            if (exception == null)
                return new DatabaseConnectionException(SqlFailureKind.Other, "Không thể mở kết nối SQL Server.");
            var messages = exception.Message ?? string.Empty;
            if (exception.Number == 207 || exception.Number == 208 || exception.Number == 2812 || exception.Number == 11514 || exception.Number == 11526)
                return new DatabaseConnectionException(SqlFailureKind.SchemaError,
                    "Schema đăng nhập không khớp: thiếu bảng/cột hoặc stored procedure dbo.sp_LayTaiKhoanDangNhap.", exception);
            if (exception.Number == 4060 || exception.Number == 911 || exception.Number == 1832 || exception.Number == 5120)
                return new DatabaseConnectionException(SqlFailureKind.DatabaseNotFound,
                    "Không mở được database ViewPartyDB_UC28 / ViewPartyDB. Xác minh database đã được tạo và tài khoản Windows hiện tại có quyền truy cập.", exception);
            if (exception.Number == 18456 || exception.Number == 229 || exception.Number == 297 || exception.Number == 916 || exception.Number == 15404)
                return new DatabaseConnectionException(SqlFailureKind.AccessDenied,
                    "Windows Authentication không có quyền truy cập database. Cấp quyền cho tài khoản Windows đang chạy ứng dụng.", exception);
            if (exception.Number == 53 || exception.Number == 40 || exception.Number == -1 || exception.Number == 2 ||
                messages.IndexOf("Local Database Runtime error", StringComparison.OrdinalIgnoreCase) >= 0 ||
                messages.IndexOf("LocalDB", StringComparison.OrdinalIgnoreCase) >= 0 ||
                messages.IndexOf("Error Locating Server/Instance Specified", StringComparison.OrdinalIgnoreCase) >= 0)
                return new DatabaseConnectionException(SqlFailureKind.InstanceNotFound,
                    "Không khởi động hoặc kết nối được SQL Server / (localdb)\\MSSQLLocalDB. Xác minh SQL Server hoặc LocalDB đang chạy.", exception);
            return new DatabaseConnectionException(SqlFailureKind.Other,
                "SQL Server trả về lỗi khi mở database. Kiểm tra trạng thái SQL Server và quyền truy cập.", exception);
        }
    }

    public enum SqlFailureKind { Configuration, InstanceNotFound, AccessDenied, DatabaseNotFound, SchemaError, Other }
}
