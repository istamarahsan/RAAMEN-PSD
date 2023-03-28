using System;
using System.Collections.Generic;
using System.Linq;
using PSD_Project.API.Features.Commerce.Transactions;
using PSD_Project.API.Features.Ramen;
using PSD_Project.API.Features.Users;
using PSD_Project.API.Features.Users.Authorization;
using Util.Collections;
using Util.Try;

namespace PSD_Project.API.Features.Commerce.Orders
{
    public class OrdersService : IOrdersService
    {
        private static readonly Dictionary<int, Order> OrdersQueue = new Dictionary<int, Order>();
        private readonly IUsersService usersService;
        private readonly ITransactionsService transactionsService;
        private readonly IRamenService ramenService;
        private readonly IAuthorizationService authorizationService;
        
        public OrdersService(IUsersService usersService, ITransactionsService transactionsService, IRamenService ramenService, IAuthorizationService authorizationService)
        {
            this.usersService = usersService;
            this.transactionsService = transactionsService;
            this.ramenService = ramenService;
            this.authorizationService = authorizationService;
        }

        public Try<Order, Exception> QueueOrder(NewOrderDetails newOrderDetails)
        {
            return VerifyUserCanPlaceOrder(newOrderDetails)
                .Bind(VerifyCartItemsExist)
                .Map(CreateOrderAndAddToQueue);
        }

        Try<List<Order>, Exception> IOrdersService.GetOrders()
        {
            return Try.Of<List<Order>, Exception>(OrdersQueue.Values.ToList());
        }

        public Try<Order, Exception> GetOrder(int id)
        {
            var result = OrdersQueue.Get(id).OrErr(() => new Exception());
            return result;
        }
        
        public Try<Transaction, Exception> HandleOrder(int unhandledTransactionId, int orderHandlerId)
        {
            var orderProcessResult = usersService.GetUser(orderHandlerId)
                .Bind(VerifyUserCanHandleOrder)
                .Bind(u => PairWithUnhandledTransaction(u, unhandledTransactionId))
                .Bind(AddToRepository);

            if (orderProcessResult.IsOk())
            {
                OrdersQueue.Remove(unhandledTransactionId);
            }

            return orderProcessResult;
        }

        private Try<Transaction, Exception> AddToRepository((int HandlerId, Order Transaction) pair)
        {
            return transactionsService.CreateTransaction(new TransactionDetails(
                pair.Transaction.CustomerId,
                pair.HandlerId,
                DateTime.Now,
                pair.Transaction.Items.Select(i => new TransactionEntry(i.RamenId, i.Quantity)).ToList()));
        }

        private Try<(int StaffId, Order Transaction), Exception> PairWithUnhandledTransaction(User u, int unhandledTransactionId)
        {
            return OrdersQueue.Get(unhandledTransactionId)
                .Map(t => (StaffId: u.Id, Transaction: t))
                .OrErr(() => new Exception());
        }

        private Order CreateOrderAndAddToQueue(NewOrderDetails orderDetails)
        {
            var nextId = OrdersQueue.Keys.DefaultIfEmpty(0).Max() + 1;
            OrdersQueue[nextId] = new Order(nextId, orderDetails.CustomerId, DateTime.Now, orderDetails.Cart);
            return OrdersQueue[nextId];
        }

        private Try<NewOrderDetails, Exception> VerifyUserCanPlaceOrder(NewOrderDetails orderDetails)
        {
            return usersService.GetUser(orderDetails.CustomerId)
                .Map(user => user.Role.Id)
                .Map(role => authorizationService.RoleHasPermission(role, Permission.PlaceOrders))
                .Bind(canPlaceOrder => canPlaceOrder.AssertTrue(() => orderDetails, () => new Exception()));
        }
        
        private Try<NewOrderDetails, Exception> VerifyCartItemsExist(NewOrderDetails orderDetails)
        {
            return orderDetails.Assert(details => CartItemsExist(details.Cart), _ => new Exception());
        }
        
        private Try<User, Exception> VerifyUserCanHandleOrder(User user)
        {
            return authorizationService.RoleHasPermission(user.Role.Id, Permission.HandleOrders)
                .AssertTrue<User, Exception>(() => user, () => new UnauthorizedAccessException());
        }
        
        private bool CartItemsExist(List<CartItem> cart)
        {
            return cart.Select(item => item.RamenId)
                .All(ramenId => ramenService.GetRamen(ramenId).IsOk());
        }
    }
}