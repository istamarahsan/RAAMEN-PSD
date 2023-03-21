using System;
using System.Collections.Generic;
using Util.Try;

namespace PSD_Project.API.Features.Commerce.Transactions
{
    public interface ITransactionsService
    {
        Try<Transaction, Exception> CreateTransaction(
            int customerId, 
            int staffId,
            DateTime date,
            List<TransactionEntry> entries);
        Try<List<Transaction>, Exception> GetTransactions();
        Try<Transaction, Exception> GetTransaction(int transactionId);
    }
}