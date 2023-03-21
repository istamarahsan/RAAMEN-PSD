using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PSD_Project.Features.Commerce.Transactions;
using Util.Try;

namespace PSD_Project.Features.Commerce.Orders
{
    public interface IOrdersHandler
    { 
        Task<Try<Order, Exception>> QueueOrderAsync(NewOrderDetails newOrderDetails);
        Task<List<Order>> GetOrdersAsync();
        Task<Try<TransactionRecord, Exception>> HandleOrderAsync(int unhandledTransactionId, int staffId);
    }
}