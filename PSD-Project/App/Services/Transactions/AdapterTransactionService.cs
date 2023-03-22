using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http.Results;
using PSD_Project.API.Features.Commerce.Transactions;
using PSD_Project.App.Common;
using Util.Try;

namespace PSD_Project.App.Services.Transactions
{
    public class AdapterTransactionService : ITransactionService
    {
        private readonly TransactionsController transactionsController = new TransactionsController();
        
        public Try<List<Transaction>, TransactionServiceError> GetTransactions(int token)
        {
            var request = new HttpRequestMessage();
            request.Headers.Authorization = new AuthenticationHeaderValue("", token.ToString());
            transactionsController.Request = request;
            return transactionsController.GetTransactions()
                .InterpretAs<List<Transaction>>()
                .MapErr(_ => TransactionServiceError.InternalServiceError);
        }

        public Try<List<Transaction>, TransactionServiceError> GetAllTransactions(int token)
        {
            var request = new HttpRequestMessage();
            request.Headers.Authorization = new AuthenticationHeaderValue("", token.ToString());
            transactionsController.Request = request;
            return transactionsController.GetAllTransactions()
                .InterpretAs<List<Transaction>>()
                .MapErr(_ => TransactionServiceError.InternalServiceError);
        }
    }
}