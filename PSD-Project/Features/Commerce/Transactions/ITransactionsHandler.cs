using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PSD_Project.Features.Commerce.Orders;
using Util.Try;

namespace PSD_Project.Features.Commerce.Transactions
{
    public interface ITransactionsHandler
    {
        Task<List<Order>> GetUnhandledTransactionsAsync();
        Task<Try<TransactionRecord, Exception>> HandleTransactionAsync(int unhandledTransactionId, int staffId);
    }
}