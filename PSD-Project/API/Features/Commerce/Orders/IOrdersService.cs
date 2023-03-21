using System;
using System.Collections.Generic;
using PSD_Project.API.Features.Commerce.Transactions;
using Util.Try;

namespace PSD_Project.API.Features.Commerce.Orders
{
    public interface IOrdersService
    { 
        Try<Order, Exception> QueueOrder(NewOrderDetails newOrderDetails);
        Try<List<Order>, Exception> GetOrders();
        Try<Order, Exception> GetOrder(int id);
        Try<Transaction, Exception> HandleOrder(int unhandledTransactionId, int staffId);
    }
}