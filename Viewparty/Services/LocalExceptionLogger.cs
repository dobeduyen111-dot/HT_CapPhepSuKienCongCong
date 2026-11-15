using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Viewparty.Services
{
    /// <summary>Writes technical failures locally after removing credentials and authentication inputs.</summary>
    internal static class LocalExceptionLogger
    {
        private static readonly object Sync = new object();
        private static readonly Regex BcryptHash = new Regex(@"\$2[aby]\$\d{2}\$[./A-Za-z0-9]{53}", RegexOptions.Compiled);
        private static readonly Regex ConnectionSecret = new Regex(
            @"(?i)(password|pwd|user\s*id|uid)\s*=\s*[^;\r\n]*", RegexOptions.Compiled);

        public static string LogPath
        {
            get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ViewParty", "Logs", "login-errors.log"); }
        }

        public static void Write(Exception exception, params string[] sensitiveValues)
        {
            if (exception == null) return;
            try
            {
                var details = exception.ToString();
                if (sensitiveValues != null)
                    foreach (var value in sensitiveValues)
                        if (!string.IsNullOrEmpty(value)) details = details.Replace(value, "[REDACTED]");
                details = BcryptHash.Replace(details, "[BCRYPT_HASH_REDACTED]");
                details = ConnectionSecret.Replace(details, "$1=[REDACTED]");
                var entry = "===== " + DateTimeOffset.Now.ToString("o") + " =====\r\n" + details + "\r\n\r\n";
                lock (Sync)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(LogPath));
                    File.AppendAllText(LogPath, entry, new UTF8Encoding(false));
                }
            }
            catch
            {
                // Logging must never mask the original application failure.
            }
        }
    }
}
