using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PSD_Project.API.Features.Commerce.Orders;
using PSD_Project.API.Features.Commerce.Transactions;
using PSD_Project.API.Features.Users;
using PSD_Project.Service;
using PSD_Project.Service.Http;
using Util.Collections;
using Util.Try;

namespace PSD_Project.API.Features.Commerce.Orders
{
    public class OrdersHandler : IOrdersHandler
    {
        private static readonly Dictionary<int, Order> Orders = new Dictionary<int, Order>();
        private static readonly IUsersService UsersService = Services.GetUsersService();

        private static readonly ITransactionsRepository TransactionsRepository = new TransactionsRepository();

        public Task<Try<Order, Exception>> QueueOrder(NewOrderDetails newOrderDetails)
        {
            var transaction =
                new Order(Orders.Keys.DefaultIfEmpty(0).Max() + 1, newOrderDetails.CustomerId, DateTime.Now, newOrderDetails.Cart);
            Orders[transaction.Id] = transaction;
            var result = Try.Of<Order, Exception>(transaction);
            return Task.FromResult(result);
        }

        Task<Try<List<Order>, Exception>> IOrdersHandler.GetOrders()
        {
            return Task.FromResult(Try.Of<List<Order>, Exception>(Orders.Values.ToList()));
        }

        public Task<Try<Order, Exception>> GetOrder(int id)
        {
            var result = Orders.Get(id).OrErr(() => new Exception());
            return Task.FromResult(result);
        }
        
        public async Task<Try<TransactionRecord, Exception>> HandleOrder(int unhandledTransactionId, int staffHandlerId)
        {
            var staff = await UsersService.GetUser(staffHandlerId);

            var transactionProcessAttempt = await staff
                .Bind(VerifyUserCanHandleTransaction)
                .Bind(u => PairWithUnhandledTransaction(u, unhandledTransactionId))
                .Bind(TryAddToRepository);

            if (transactionProcessAttempt.IsOk())
            {
                Orders.Remove(unhandledTransactionId);
            }

            return transactionProcessAttempt;
        }

        private Task<Try<TransactionRecord, Exception>> TryAddToRepository((int StaffId, Order Transaction) pair)
        {
            return TransactionsRepository.CreateTransaction(
                pair.Transaction.CustomerId, 
                pair.StaffId,
                DateTime.Now,
                pair.Transaction.Items.Select(i => new TransactionEntry(i.RamenId, i.Quantity)).ToList());
        }

        private Try<(int StaffId, Order Transaction), Exception> PairWithUnhandledTransaction(User u, int unhandledTransactionId)
        {
            return Orders.Get(unhandledTransactionId)
                .Map(t => (StaffId: u.Id, Transaction: t))
                .OrErr(() => new Exception());
        }

        private Try<User, Exception> VerifyUserCanHandleTransaction(User u)
        {
            return u.Check(CanHandleTransactions, _ => new Exception());
        }


        private bool CanHandleTransactions(User user)
        {
            return user.Role.Id == 1 || user.Role.Id == 2;
        }
    }
}