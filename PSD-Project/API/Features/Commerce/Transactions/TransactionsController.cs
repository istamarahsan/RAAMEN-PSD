using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace PSD_Project.API.Features.Commerce.Transactions
{
    [RoutePrefix("api/transactions")]
    public class TransactionsController : ApiController
    {
        private readonly ITransactionsRepository transactionsRepository;

        public TransactionsController()
        {
            transactionsRepository = new TransactionsRepository();
        }

        public TransactionsController(ITransactionsRepository transactionsRepository)
        {
            this.transactionsRepository = transactionsRepository;
        }

        [Route]
        [HttpGet]
        public async Task<IHttpActionResult> GetTransactions()
        {
            var transactions = await transactionsRepository.GetTransactions();
            return transactions.Match(Ok, HandleError);
        }

        [Route("{id}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetTransaction(int transactionId)
        {
            var transaction = await transactionsRepository.GetTransaction(transactionId);
            return transaction.Match(Ok, HandleError);
        }

        [Route]
        [HttpPost]
        public async Task<IHttpActionResult> CreateTransaction([FromBody] NewTransactionDetails form)
        {
            var record = await transactionsRepository.CreateTransaction(
                form.CustomerId,
                form.StaffId,
                form.Date,
                form.Details);
            return record.Match(Ok, HandleError);
        }
        
        private IHttpActionResult HandleError(Exception e)
        {
            switch (e)
            {
                case ArgumentException _ :
                    return NotFound();
                default:
                    return InternalServerError();
            }
        }
    }
}