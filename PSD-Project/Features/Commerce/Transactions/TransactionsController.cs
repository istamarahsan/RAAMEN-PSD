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
        private readonly ITransactionsRepository transactionsRepository;

        public TransactionsController()
        {
            transactionsRepository = new TransactionsRepository();
        }

        public TransactionsController(ITransactionsHandler transactionsHandler)
        {
        }

        [Route]
        [HttpGet]
        public async Task<IHttpActionResult> GetTransactions()
        {
            var transactions = await transactionsRepository.GetTransactionsAsync();
            return Ok(transactions);
        }

        [Route("{id}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetTransaction(int transactionId)
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
            
            var transaction = await transactionsRepository.GetTransactionAsync(transactionId);
            return transaction.Match(Ok, HandleError);
        }

        [Route]
        [HttpPost]
        public async Task<IHttpActionResult> CreateTransaction([FromBody] NewTransactionDetails form)
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

            var record = await transactionsRepository.CreateTransactionAsync(
                form.CustomerId,
                form.StaffId,
                form.Date,
                form.Details);
            return record.Match(Ok, HandleError);
        }
    }
}