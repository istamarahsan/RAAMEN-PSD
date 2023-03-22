using System.Collections.Generic;
using PSD_Project.API.Features.Commerce.Orders;
using PSD_Project.API.Features.Commerce.Transactions;
using Util.Try;

namespace PSD_Project.App.Services.Orders
{
    public interface IOrderService
    {
        Try<Order, OrderServiceError> PlaceOrder(NewOrderDetails orderDetails);
        Try<Transaction, OrderServiceError> HandleOrder(int handlerId, int orderId);
        Try<List<Order>, OrderServiceError> GetOrders();
    }
}