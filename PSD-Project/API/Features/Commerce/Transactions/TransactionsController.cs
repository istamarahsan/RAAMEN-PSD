using System;
using System.Threading.Tasks;
using System.Web.Http;
using PSD_Project.API.Service;

namespace PSD_Project.API.Features.Commerce.Transactions
{
    [RoutePrefix("api/transactions")]
    public class TransactionsController : ApiController
    {
        private readonly ITransactionsService transactionsService;

        public TransactionsController()
        {
            transactionsService = Services.GetTransactionsService();
        }

        public TransactionsController(ITransactionsService transactionsService)
        {
            this.transactionsService = transactionsService;
        }

        [Route]
        [HttpGet]
        public IHttpActionResult GetTransactions()
        {
            var transactions = transactionsService.GetTransactions();
            return transactions.Match(Ok, HandleError);
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
                default:
                    return InternalServerError(exception);
            }
        }
    }
}