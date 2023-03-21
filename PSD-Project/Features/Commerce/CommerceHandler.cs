using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PSD_Project.Features.Commerce.Orders;
using PSD_Project.Features.Users;
using PSD_Project.Services;
using Util.Collections;
using Util.Try;

namespace PSD_Project.Features.Commerce.Transactions
{
    public class CommerceHandler : IOrdersHandler
    {
        private static readonly Dictionary<int, Order> UnhandledTransactions = new Dictionary<int, Order>();
        private static readonly IUsersService UsersService = new UsersService();

        private static readonly ITransactionsRepository TransactionsRepository = new TransactionsRepository();

        public Task<Try<Order, Exception>> QueueOrderAsync(NewOrderDetails newOrderDetails)
        {
            var transaction =
                new Order(UnhandledTransactions.Keys.DefaultIfEmpty(0).Max() + 1, newOrderDetails.CustomerId, DateTime.Now, newOrderDetails.Cart);
            UnhandledTransactions[transaction.Id] = transaction;
            var result = Try.Of<Order, Exception>(transaction);
            return Task.FromResult(result);
        }

        public Task<List<Order>> GetOrdersAsync()
        {
            return Task.FromResult(UnhandledTransactions.Values.ToList());
        }

        public async Task<Try<TransactionRecord, Exception>> HandleOrderAsync(int unhandledTransactionId, int staffHandlerId)
        {
            var staff = await UsersService.GetUserAsync(staffHandlerId);

            var transactionProcessAttempt = await staff
                .Bind(VerifyUserCanHandleTransaction)
                .Bind(u => PairWithUnhandledTransaction(u, unhandledTransactionId))
                .Bind(TryAddToRepository);

            if (transactionProcessAttempt.IsOk())
            {
                UnhandledTransactions.Remove(unhandledTransactionId);
            }

            return transactionProcessAttempt;
        }

        private Task<Try<TransactionRecord, Exception>> TryAddToRepository((int StaffId, Order Transaction) pair)
        {
            return TransactionsRepository.CreateTransactionAsync(
                pair.Transaction.CustomerId, 
                pair.StaffId,
                DateTime.Now,
                pair.Transaction.Items.Select(i => new TransactionEntry(i.RamenId, i.Quantity)).ToList());
        }

        private Try<(int StaffId, Order Transaction), Exception> PairWithUnhandledTransaction(User u, int unhandledTransactionId)
        {
            return UnhandledTransactions.Get(unhandledTransactionId)
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