using System;
using System.Management;
using System.Threading.Tasks;
using System.Web.Http;
using PSD_Project.API.Features.Authentication;
using PSD_Project.API.Features.Users;
using PSD_Project.API.Features.Users.Authorization;
using PSD_Project.API.Util;
using Util.Try;

namespace PSD_Project.API.Features.Commerce.Transactions
{
    [RoutePrefix("api/transactions")]
    public class TransactionsController : ApiController
    {
        private readonly ITransactionsService transactionsService;
        private readonly IAuthorizationService authorizationService;
        private readonly IAuthenticationService authenticationService;
        private readonly IUsersService usersService;

        public TransactionsController()
        {
            transactionsService = Services.GetTransactionsService();
            authorizationService = Services.GetAuthorizationService();
            authenticationService = Services.GetAuthenticationService();
            usersService = Services.GetUsersService();
        }

        public TransactionsController(
            ITransactionsService transactionsService, 
            IAuthorizationService authorizationService, 
            IAuthenticationService authenticationService,
            IUsersService usersService)
        {
            this.transactionsService = transactionsService;
            this.authorizationService = authorizationService;
            this.authenticationService = authenticationService;
            this.usersService = usersService;
        }

        [Route]
        [HttpGet]
        public IHttpActionResult GetTransactions()
        {
            return Request.ExtractAuthToken()
                .Bind(authenticationService.GetSession)
                .Map(userSession => (userId: userSession.Id, roleId: userSession.Role.Id))
                .Bind(request => usersService.GetRoleOfId(request.roleId).Map(role => (request.userId, role)))
                .Bind(request => authorizationService.RoleHasPermission(request.role, Permission.ReadOwnTransactions)
                        .Assert<Exception>(true, () => new UnauthorizedAccessException())
                        .Bind(_ => transactionsService.GetTransactionsForUser(request.userId)))
                .Match(Ok, HandleError);
        }
        
        [Route("all")]
        [HttpGet]
        public IHttpActionResult GetAllTransactions()
        {
            return Request.ExtractAuthToken()
                .Bind(authenticationService.GetSession)
                .Map(userSession => (userId: userSession.Id, roleId: userSession.Role.Id))
                .Bind(request => usersService.GetRoleOfId(request.roleId).Map(role => (request.userId, role)))
                .Bind(request => authorizationService.RoleHasPermission(request.role, Permission.ReadAllTransactions)
                        .Assert<Exception>(true, () => new UnauthorizedAccessException())
                        .Bind(_ => transactionsService.GetTransactions()))
                .Match(Ok, HandleError);
        }

        [Route("{id}")]
        [HttpGet]
        public IHttpActionResult GetTransaction(int transactionId)
        {
            var transaction = transactionsService.GetTransaction(transactionId);
            return transaction.Match(Ok, HandleError);
        }

        [Route]
        [HttpPost]
        public IHttpActionResult CreateTransaction([FromBody] TransactionDetails form)
        {
            return transactionsService.CreateTransaction(form).Match(Ok, HandleError);
        }
        
        private IHttpActionResult HandleError(Exception exception)
        {
            switch (exception)
            {
                case ArgumentException _ :
                    return NotFound();
                case UnauthorizedAccessException _ :
                    return Unauthorized();
                default:
                    return InternalServerError(exception);
            }
        }
    }
}