using System.Configuration;
using PSD_Project.Services.Sql;
using PSD_Project.Services.Sql.QueryStrings;

namespace PSD_Project.Services
{
    public static class Services
    {
        public static SqlDialect GetConfiguredDialect()
        {
            switch (ConfigurationManager.AppSettings["sql-dialect"])
            {
                case "MySQL":
                    return SqlDialect.MySql;
                default:
                    return SqlDialect.SqlServer;
            }
        }

        public static QueryStringBuilder GetQueryStringBuilder(SqlDialect dialect)
        {
            switch (dialect)
            {
                case SqlDialect.MySql:
                    return new MySqlQueryStringBuilder();
                case SqlDialect.SqlServer:
                default:
                    return new SqlServerQueryStringBuilder();
            }
        }
    }
}