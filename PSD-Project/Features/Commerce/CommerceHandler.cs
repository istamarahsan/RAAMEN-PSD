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
    public class CommerceHandler : IOrdersHandler, ITransactionsHandler
    {
        private static readonly Dictionary<int, UnhandledTransaction> UnhandledTransactions = new Dictionary<int, UnhandledTransaction>();
        private static readonly IUsersService UsersService = new UsersService();

        private static readonly ITransactionsRepository TransactionsRepository = new RawMySqlTransactionsRepository();

        public Task<Try<UnhandledTransaction, Exception>> QueueOrderAsync(Order order)
        {
            var transaction =
                new UnhandledTransaction(UnhandledTransactions.Keys.DefaultIfEmpty(0).Max() + 1, order.CustomerId, DateTime.Now, order.Cart);
            UnhandledTransactions[transaction.Id] = transaction;
            var result = Try.Of<UnhandledTransaction, Exception>(transaction);
            return Task.FromResult(result);
        }

        public Task<List<UnhandledTransaction>> GetUnhandledTransactionsAsync()
        {
            return Task.FromResult(UnhandledTransactions.Values.ToList());
        }

        public async Task<Try<TransactionRecord, Exception>> HandleTransactionAsync(int unhandledTransactionId, int staffHandlerId)
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

        private Task<Try<TransactionRecord, Exception>> TryAddToRepository((int StaffId, UnhandledTransaction Transaction) pair)
        {
            return TransactionsRepository.AddNewTransactionAsync(pair.Transaction.CustomerId, pair.StaffId, pair.Transaction.Items);
        }

        private Try<(int StaffId, UnhandledTransaction Transaction), Exception> PairWithUnhandledTransaction(User u, int unhandledTransactionId)
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