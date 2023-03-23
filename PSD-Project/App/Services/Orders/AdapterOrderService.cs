using System.Collections.Generic;
using PSD_Project.API.Features.Commerce.Orders;
using PSD_Project.API.Features.Commerce.Transactions;
using PSD_Project.App.Common;
using Util.Try;

namespace PSD_Project.App.Services.Orders
{
    public class AdapterOrderService : IOrderService
    {
        private readonly OrdersController ordersController = new OrdersController();

        public Try<Order, OrderServiceError> PlaceOrder(int token, NewOrderDetails orderDetails)
        {
            return ordersController.WithAuthToken(token, controller =>
                controller.PlaceOrder(orderDetails)
                    .InterpretAs<Order>()
                    .MapErr(_ => OrderServiceError.InternalServiceError));
        }

        public Try<Transaction, OrderServiceError> HandleOrder(int token, int orderId)
        {
            return ordersController.WithAuthToken(token, controller =>
                controller.HandleOrder(orderId)
                    .InterpretAs<Transaction>()
                    .MapErr(_ => OrderServiceError.InternalServiceError));
        }

        public Try<List<Order>, OrderServiceError> GetOrders()
        {
            return ordersController.GetOrders()
                .InterpretAs<List<Order>>()
                .MapErr(_ => OrderServiceError.InternalServiceError);
        }
    }
}