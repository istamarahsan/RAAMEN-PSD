using System.Configuration;
using PSD_Project.API.Features.Authentication;
using PSD_Project.API.Features.Commerce.Orders;
using PSD_Project.API.Features.Commerce.Transactions;
using PSD_Project.API.Features.Ramen;
using PSD_Project.API.Features.Register;
using PSD_Project.API.Features.Users;
using PSD_Project.API.Features.Users.Authorization;
using PSD_Project.API.Util.Sql;
using PSD_Project.API.Util.Sql.QueryStrings;

namespace PSD_Project.API
{
    public static class Services
    {
        private static readonly IUserRepository UserRepository = new UsersRolesRepository();
        private static readonly IRolesRepository RolesRepository = new UsersRolesRepository();
        private static readonly UsersService UsersService = new UsersService(UserRepository);
        private static readonly IRamenService RamenService = new RamenRepository();
        private static readonly IUserSessionsService UserSessionsService = new UserSessionsService();
        private static readonly IAuthenticationService AuthenticationService = new AuthenticationService(UserSessionsService, UsersService);
        private static readonly ITransactionsService TransactionsService = new TransactionsRepository();
        private static readonly IAuthorizationService AuthorizationService = new AuthorizationService(RolesRepository);
        private static readonly IOrdersService OrdersService = new OrdersService(UsersService, TransactionsService, RamenService, AuthorizationService);
        public static IAuthenticationService GetAuthenticationService() => AuthenticationService;

        public static IUsersService GetUsersService() => UsersService;

        public static IRamenService GetRamenService() => RamenService;

        public static IOrdersService GetOrdersService() => OrdersService;

        public static IUserSessionsService GetUserSessionsService() => UserSessionsService;

        public static ITransactionsService GetTransactionsService() => TransactionsService;
        
        public static IAuthorizationService GetAuthorizationService() => AuthorizationService;

        public static IRegisterService GetRegisterService() => UsersService;

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