using System;
using System.Threading.Tasks;
using PSD_Project.Features.Commerce.Transactions;
using Util.Try;

namespace PSD_Project.Features.Commerce.Orders
{
    public interface IOrdersHandler
    { 
        Task<Try<UnhandledTransaction, Exception>> QueueOrderAsync(Order order);
    }
}