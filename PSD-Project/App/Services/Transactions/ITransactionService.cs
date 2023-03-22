using System.Collections.Generic;
using PSD_Project.API.Features.Commerce.Transactions;
using Util.Try;

namespace PSD_Project.App.Services.Transactions
{
    public interface ITransactionService
    {
        Try<List<Transaction>, TransactionServiceError> GetTransactions(int token);
        Try<List<Transaction>, TransactionServiceError> GetAllTransactions(int token);
    }
}