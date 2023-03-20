using System;
using System.Threading.Tasks;
using System.Web.Http;
using PSD_Project.Features.LogIn;
using PSD_Project.Services;
using Util.Try;

namespace PSD_Project.Features.Commerce.Transactions
{
    [RoutePrefix("api/transactions")]
    public class TransactionsController : ApiController
    {
        private readonly ITransactionsHandler transactionsHandler;
        private readonly IAuthService authService;

        public TransactionsController()
        {
            transactionsHandler = new CommerceHandler();
            authService = new LoginAuthService();
        }

        public TransactionsController(ITransactionsHandler transactionsHandler, IAuthService authService)
        {
            this.transactionsHandler = transactionsHandler;
            this.authService = authService;
        }

        [Route]
        [HttpGet]
        public async Task<IHttpActionResult> GetTransactions()
        {
            var transactions = await transactionsHandler.GetUnhandledTransactionsAsync();
            return Ok(transactions);
        }

        [Route("{id}")]
        [HttpPost]
        public async Task<IHttpActionResult> HandleTransaction(int id, [FromUri] int token)
        {
            IHttpActionResult HandleError(Exception e)
            {
                switch (e)
                {
                    case ArgumentException _ :
                        return NotFound();
                    default:
                        return InternalServerError();
                }
            }

            var auth = await authService.Authenticate(token);
            var handleTransaction = await auth.Bind(userSession => transactionsHandler.HandleTransactionAsync(id, userSession.Id));
            return handleTransaction.Match(Ok, HandleError);
        }
    }
}