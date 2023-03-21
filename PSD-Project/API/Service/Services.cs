using System.Configuration;
using PSD_Project.API.Features.Commerce.Orders;
using PSD_Project.API.Features.Commerce.Transactions;
using PSD_Project.API.Features.LogIn;
using PSD_Project.API.Features.Ramen;
using PSD_Project.API.Features.Register;
using PSD_Project.API.Features.Users;
using PSD_Project.API.Service.Sql;
using PSD_Project.API.Service.Sql.QueryStrings;

namespace PSD_Project.API.Service
{
    public static class Services
    {
        private static readonly IUserRepository UserRepository = new UserRepository();
        private static readonly IUsersService UsersService = new UsersService(UserRepository);
        private static readonly IRegisterService RegisterService = new RegisterService(UsersService);
        private static readonly IRamenService RamenService = new RamenRepository();
        private static readonly IOrdersService OrdersService = new OrdersService();
        private static readonly IUserSessionsService UserSessionsService = new UserSessionsService();
        private static readonly IAuthService AuthService = new AuthService(UserSessionsService, UsersService);
        private static readonly ITransactionsService TransactionsService = new TransactionsRepository();
        
        public static IAuthService GetAuthService() => AuthService;

        public static IRegisterService GetRegisterService() => RegisterService;

        public static IUsersService GetUsersService() => UsersService;

        public static IRamenService GetRamenService() => RamenService;

        public static IOrdersService GetOrdersService() => OrdersService;

        public static IUserSessionsService GetUserSessionsService() => UserSessionsService;

        public static ITransactionsService GetTransactionsService() => TransactionsService;

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