using System;
using System.Collections.Generic;
using Util.Try;

namespace PSD_Project.API.Features.Commerce.Transactions
{
    public interface ITransactionsService
    {
        Try<Transaction, Exception> CreateTransaction(TransactionDetails transactionDetails);
        Try<List<Transaction>, Exception> GetTransactions();
        Try<List<Transaction>, Exception> GetTransactionsForUser(int userId);
        Try<Transaction, Exception> GetTransaction(int transactionId);
    }
}