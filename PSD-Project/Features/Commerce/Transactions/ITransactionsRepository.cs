using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PSD_Project.Features.Commerce.Orders;
using Util.Try;

namespace PSD_Project.Features.Commerce.Transactions
{
    public interface ITransactionsRepository
    {
        Task<Try<TransactionRecord, Exception>> CreateTransactionAsync(
            int customerId, 
            int staffId,
            DateTime date,
            List<TransactionEntry> entries);
        Task<Try<List<TransactionRecord>, Exception>> GetTransactionsAsync();
        Task<Try<TransactionRecord, Exception>> GetTransactionAsync(int transactionId);
    }
}