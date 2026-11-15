using System;
using System.Data.SqlClient;
using Viewparty.Data;

namespace Viewparty.Services
{
    public sealed class DatabaseDiagnosticsService
    {
        private readonly DatabaseHealthRepository health = new DatabaseHealthRepository();

        public DatabaseConnectionInfo CheckConnectionAndLoginSchema()
        {
            try
            {
                var info = Db.CheckConnection();
                health.ValidateLoginSchema();
                return info;
            }
            catch (SqlException ex)
            {
                throw DatabaseConnectionException.FromSqlException(ex);
            }
        }
    }
}
