using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Util.Try;

namespace PSD_Project.Features.Commerce.Transactions
{
    public interface ITransactionsHandler
    {
        Task<List<UnhandledTransaction>> GetUnhandledTransactionsAsync();
        Task<Try<TransactionRecord, Exception>> HandleTransactionAsync(int unhandledTransactionId, int staffId);
    }
}