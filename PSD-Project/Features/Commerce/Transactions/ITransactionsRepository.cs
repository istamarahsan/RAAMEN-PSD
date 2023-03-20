using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PSD_Project.Features.Commerce.Orders;
using Util.Try;

namespace PSD_Project.Features.Commerce.Transactions
{
    public interface ITransactionsRepository
    {
        Task<Try<TransactionRecord, Exception>> AddNewTransactionAsync(int customerId, int staffId,
            List<CartItem> cart);
    }
}