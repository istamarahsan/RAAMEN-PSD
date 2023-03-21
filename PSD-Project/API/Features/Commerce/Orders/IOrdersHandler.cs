using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PSD_Project.API.Features.Commerce.Transactions;
using Util.Try;

namespace PSD_Project.API.Features.Commerce.Orders
{
    public interface IOrdersHandler
    { 
        Task<Try<Order, Exception>> QueueOrder(NewOrderDetails newOrderDetails);
        Task<Try<List<Order>, Exception>> GetOrders();
        Task<Try<Order, Exception>> GetOrder(int id);
        Task<Try<TransactionRecord, Exception>> HandleOrder(int unhandledTransactionId, int staffId);
    }
}