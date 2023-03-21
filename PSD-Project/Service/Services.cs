using System;
using System.Configuration;
using PSD_Project.API.Features.Commerce.Orders;
using PSD_Project.Service.Http;
using PSD_Project.Service.Sql;
using PSD_Project.Service.Sql.QueryStrings;

namespace PSD_Project.Service
{
    public static class Services
    {
        private static readonly IAuthService AuthService = new HttpAuthService(new Uri("http://localhost:5000/api/login"), RaamenApp.HttpClient);
        private static readonly IUsersService UsersService = new HttpUsersService(new Uri("http://localhost:5000/api/users"), RaamenApp.HttpClient);
        private static readonly IRegisterService RegisterService = new HttpRegisterService(new Uri("http://localhost:5000/api/register"), RaamenApp.HttpClient);
        private static readonly IRamenService RamenService = new HttpRamenService(new Uri("http://localhost:5000/ramen"), RaamenApp.HttpClient);
        private static readonly IOrdersService OrdersService = new OrdersService();
        
        public static IAuthService GetAuthService() => AuthService;

        public static IRegisterService GetRegisterService() => RegisterService;

        public static IUsersService GetUsersService() => UsersService;

        public static IRamenService GetRamenService() => RamenService;

        public static IOrdersService GetOrdersService() => OrdersService;

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