using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Util.Try;

namespace PSD_Project.API.Features.Commerce.Transactions
{
    public interface ITransactionsRepository
    {
        Task<Try<TransactionRecord, Exception>> CreateTransaction(
            int customerId, 
            int staffId,
            DateTime date,
            List<TransactionEntry> entries);
        Task<Try<List<TransactionRecord>, Exception>> GetTransactions();
        Task<Try<TransactionRecord, Exception>> GetTransaction(int transactionId);
    }
}